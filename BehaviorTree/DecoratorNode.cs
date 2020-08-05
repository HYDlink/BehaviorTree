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
        public IfNode(Func<bool> pred, TreeNode child_, string nodeName) :
            base(new SequenceNode(new List<TreeNode> { new ConditionNode(pred), child_ }))
        { }
    }

    public class LoopNode : DecoratorNode
    {
        protected Func<bool> loopCond;

        public LoopNode(Func<bool> loopCond_, TreeNode child_) : base(child_)
        {
            loopCond = loopCond_;
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