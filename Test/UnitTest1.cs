using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using NUnit.Framework;
using System.Threading.Tasks;
using static NUnit.Framework.Assert;

namespace BehaviorTree.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TaskTest()
        {
            int num1 = 0;
            var task1 = new TaskNode(() => num1++);

            var state1 = task1.Tick();
            AreEqual(state1, State.Success);
            AreEqual(num1, 1);

            int num2 = 0;
            var task2 = new TaskNode(() => num2++) {SuccessPred = () => num2 >= 2};
            var state2 = task2.Tick();
            AreEqual(state2, State.Running);
            AreEqual(num2, 1);

            state2 = task2.Tick();
            AreEqual(state2, State.Success);
            AreEqual(num2, 2);
        }

        [Test]
        public void IfTest()
        {
            int num1 = 0;
            var if1 = new IfNode(() => num1 > 1, new TaskNode(() => num1 = -1));
            var state = if1.Tick();
            AreEqual(state, State.Fail);
            AreEqual(num1, 0);
            num1 = 2;
            state = if1.Tick();
            AreEqual(state, State.Success);
            AreEqual(num1, -1);
        }

        [Test]
        public void LoopTest()
        {
            int num1 = 0;
            const int endNum = 4;
            var loop1 = new LoopNode(() => num1 < endNum, new TaskNode(() => num1++));
            var state = State.Running;
            for (int tmp = 0; state == State.Running; state = loop1.Tick(), tmp++)
            {
                AreEqual(tmp, num1);
            }

            AreEqual(num1, endNum);
            AreEqual(state, State.Success);
        }

        [Test]
        public void SequenceTest()
        {
            int num1 = 0, num2 = 10;
            var seq1 = new SequenceNode(new List<TreeNode> {new TaskNode(() => num1++), new TaskNode(() => num2++)});
            var state = seq1.Tick();
            AreEqual(state, State.Success);
            AreEqual(num1, 1);
            AreEqual(num2, 11);

            num1 = -2;
            var seq2 = new SequenceNode(new List<TreeNode>
            {
                new TaskNode(() => num1++),
                new IfNode(() => num1 >= 0,
                    new LoopNode(() => num1 > -10,
                        new TaskNode(() => num1--)))
            });
            var state2 = seq2.Tick();
            AreEqual(state2, State.Fail);
            AreEqual(num1, -1);
            state2 = seq2.Tick();
            AreEqual(state2, State.Running);
            AreEqual(num1, -1);

            while (state2 == State.Running)
            {
                state2 = seq2.Tick();
            }

            AreEqual(state2, State.Success);
            AreEqual(num1, -10);
        }


        [Test]
        public void SelectorTest()
        {
            int num1 = 0, num2 = 1;
            State state;
            var seq1 = new SelectorNode(new List<TreeNode>
            {
                new IfNode(() => num1 > 0, new TaskNode(() => num1++)),
                new IfNode(() => num2 > 0, new TaskNode(() => num2++)),
            });
            state = seq1.Tick();
            AreEqual(state, State.Success);
            AreEqual(num1, 0);
            AreEqual(num2, 2);
            
            num1 = 1;
            num2 = 1;
            state = seq1.Tick();
            AreEqual(state, State.Success);
            AreEqual(num1, 2);
            AreEqual(num2, 1);
            
            num1 = 0;
            num2 = 1;
            state = seq1.Tick();
            AreEqual(state, State.Success);
            AreEqual(num1, 0);
            AreEqual(num2, 2);
            
            num1 = 0;
            num2 = 0;
            state = seq1.Tick();
            AreEqual(state, State.Fail);
            AreEqual(num1, 0);
            AreEqual(num2, 0);
        }

        [Test]
        public void SelectorTest2()
        {
            int num1, num2;
            num1 = 0;
            var seq = new SequenceNode(new List<TreeNode>
            {
                new ConditionNode(() => num1 > 0),
                new LoopNode(() => num1 < 10, () => num1++),
            });

            State state;
            state = seq.Tick();
            while (state == State.Running)
            {
                state = seq.Tick();
            }
            AreEqual(state, State.Success);
            AreEqual(num1, 10);
        }

        [Test]
        public void ParallelTest()
        {
            int num1 = 0, num2 = 0;
            var par1 = new ParallelNode(new List<TreeNode> {new TaskNode(() => num1++), new TaskNode(() => num2++)});
            var state = par1.Tick();
            AreEqual(state, State.Success);
            AreEqual(num1, 1);
            AreEqual(num2, 1);

            num1 = num2 = 0;
            var par2 = new ParallelNode(new List<TreeNode>
            {
                new LoopNode(() => num1 < 5, () => num1++),
                new LoopNode(() => num2 < 10, () => num2++),
                new ConditionNode(() => num1 < 0)
            }) {policy = ParallelNode.Policy.FailedWhenAllFailed};

            state = par2.Tick();
            while (state == State.Running)
            {
                state = par2.Tick();
            }
            AreEqual(state, State.Success);
            AreEqual(num1, 5);
            AreEqual(num2, 10);
            
            var par3 = new ParallelNode(new List<TreeNode>
            {
                new ConditionNode(() => true),
                new ConditionNode(() => true),
                new ConditionNode(() => false),
            });
            par3.policy = ParallelNode.Policy.FailedWhenAllFailed;
            AreEqual(par3.Tick(), State.Success);
            
        }
    }
}