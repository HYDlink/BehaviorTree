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
        public string NodeName { get; set; }
        public virtual State Tick() => State.Success;
    }

    public class TaskNode : TreeNode
    {
        public Func<bool> SuccessPred { get; set; }
        private Action _task;
        public TaskNode(Action task) { _task = task; }

        public override State Tick()
        {
            _task();

            if (SuccessPred == null || SuccessPred())
            {
                return State.Success;
            }

            return State.Running;
        }
    }

    public class ConditionNode : TreeNode
    {
        private Func<bool> _pred;

        public ConditionNode(Func<bool> pred)
        {
            _pred = pred;
        }

        public override State Tick() => _pred() ? State.Success : State.Fail;
    }
}