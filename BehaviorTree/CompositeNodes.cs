using System;
using System.Collections.Generic;

namespace BehaviorTree
{
    public abstract class CompositeNode : TreeNode
    {
        protected List<TreeNode> Children;

        public CompositeNode(List<TreeNode> children)
        {
            Children = children;
        }
    }

    public class SelectorNode : CompositeNode
    {
        private int runningIndex = 0;

        public SelectorNode(List<TreeNode> children) : base(children)
        {
        }

        private void Reset() => runningIndex = 0;

        /// <summary>
        /// 找到第一个可以执行的结点，对其执行后返回
        /// </summary>
        /// <returns>如果找到可以执行的结点，那么返回该节点的状态，否则返回 State.Fail</returns>
        public override State Tick()
        {
            if (Children.Count == 0)
            {
                return State.Success;
            }

            for (int i = runningIndex; i < Children.Count; i++)
            {
                var state = Children[i].Tick();
                if (State.Success == state)
                {
                    Reset();
                    return State.Success;
                }
                if (state == State.Running)
                {
                    runningIndex = i;
                    return State.Running;
                }
            }

            Reset();
            return State.Fail;
        }
    }

    public class SequenceNode : CompositeNode
    {
        private int runningIndex = 0;

        public SequenceNode(List<TreeNode> children) : base(children)
        {
        }

        private void Reset() => runningIndex = 0;

        /// <summary>
        /// 从正在执行的结点开始（如果没有则测第一个结点开始），顺序执行所有结点，直到遇到执行不成功的结点，返回这个结点的状态，否则返回成功
        /// </summary> 
        /// <returns>如果找到可以执行的结点，那么返回该节点的状态，否则返回 State.Fail</returns>
        public override State Tick()
        {
            if (Children.Count == 0)
            {
                return State.Success;
            }

            for (int i = runningIndex; i < Children.Count; i++)
            {
                var state = Children[i].Tick();
                if (State.Fail == state)
                {
                    Reset();
                    return State.Fail;
                }

                if (state == State.Running)
                {
                    runningIndex = i;
                    return State.Running;
                }
            }

            Reset();
            return State.Success;
        }
    }

    public class ParallelNode : CompositeNode
    {
        public enum Policy
        {
            FailedWhenOneFailed,
            FailedWhenAllFailed,
        }

        private List<TreeNode> RunningNodes = new List<TreeNode>();
        private int successCnt = 0;
        private int failCnt = 0;

        public Policy policy { get; set; }= Policy.FailedWhenOneFailed;

        public ParallelNode(List<TreeNode> children) : base(children)
        {
        }

        private void Reset()
        {
            successCnt = failCnt = 0;
            RunningNodes.Clear();
        }

        /// <summary>
        /// 对所有结点执行并返回
        /// </summary>
        /// <returns>如果找到可以执行的结点，那么返回该节点的状态，否则返回 State.Fail</returns>
        public override State Tick()
        {
            if (Children.Count == 0)
            {
                return State.Success;
            }

            var toRunNodes = RunningNodes.Count > 0 ? new List<TreeNode>(RunningNodes) : Children;
            Reset();

            foreach (var child in toRunNodes)
            {
                var state = child.Tick();
                if (State.Success == state)
                {
                    successCnt++;
                }

                if (State.Fail == state)
                {
                    failCnt++;
                }

                if (State.Running == state)
                {
                    RunningNodes.Add(child);
                }
            }

            if (RunningNodes.Count > 0)
            {
                return State.Running;
            }

            if ((Policy.FailedWhenOneFailed == policy && failCnt > 0)
                || (Policy.FailedWhenAllFailed == policy && successCnt == 0))
            {
                return State.Fail;
            }

            return State.Success;
        }
    }
}