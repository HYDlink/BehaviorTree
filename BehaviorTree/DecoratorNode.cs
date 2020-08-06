using System;
using System.Collections.Generic;

namespace BehaviorTree
{
    public class DecoratorNode : TreeNode
    {
        protected TreeNode child;

        public DecoratorNode(TreeNode child_)
        {
            child = child_;
        }

        public override State Tick()
        {
            return child.Tick();
        }
    }

    public class IfNode : DecoratorNode
    {
        public IfNode(Func<bool> pred, TreeNode child_)
            : base(new SequenceNode(new List<TreeNode> {new ConditionNode(pred), child_}))
        {
        }
        
        public IfNode(Func<bool> pred, Action action)
        : this(pred, new TaskNode(action))
        {
        }
    }

    /// <summary>
    /// 循环节点，每次进入后执行一次，如果执行状态不是成功，那么返回执行
    /// 然后检查 loopCond 是否为真，如果为真返回 Running 状态，否则返回 Success
    /// </summary>
    public class LoopNode : DecoratorNode
    {
        protected Func<bool> loopCond;

        public LoopNode(Func<bool> loopCond_, TreeNode child_) : base(child_)
        {
            loopCond = loopCond_;
        }

        public LoopNode(Func<bool> loopCond_, Action action)
            : this(loopCond_, new TaskNode(action))
        {
        }

        public override State Tick()
        {
            var state = child.Tick();
            if (state != State.Success)
            {
                return state;
            }

            if (loopCond.Invoke())
            {
                return State.Running;
            }

            return State.Success;
        }
    }
}