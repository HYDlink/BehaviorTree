using System;

namespace BehaviorTree
{
    public enum State
    {
        Success,
        Fail,
        Running
    }

    public class TreeNode
    {
        public TreeNode(string nodeName, string nodeType)
        {
            NodeName = nodeName;
            NodeType = nodeType;
        }

        public string NodeType;
        public string NodeName;
        public virtual State Tick() => State.Success;
    }

    public class TaskNode : TreeNode
    {
        public Action Task;
        public Func<bool> SuccessPred;

        public TaskNode(Action task, Func<bool> successPred, string nodeName)
            : base(nodeName, "TaskNode")
        {
            Task = task;
            SuccessPred = successPred;
        }

        public override State Tick()
        {
            if (SuccessPred())
            {
                return State.Success;
            }

            Task();
            return State.Running;
        }
    }

    public class ConditionNode : TreeNode
    {
        private Func<bool> Pred;

        public ConditionNode(Func<bool> pred, string nodeName) : base(nodeName, "ConditionNode")
        {
            Pred = pred;
        }

        public override State Tick() => Pred() ? State.Success : State.Fail;
    }
}