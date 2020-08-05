using System.ComponentModel.DataAnnotations.Schema;
using NUnit.Framework;
using System.Threading.Tasks;

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
            Assert.AreEqual(state1, State.Success);
            Assert.AreEqual(num1, 1);

            int num2 = 0;
            var task2 = new TaskNode(() => num2++) { SuccessPred = () => num2 >= 2 };
            var state2 = task2.Tick();
            Assert.AreEqual(state2, State.Running);
            Assert.AreEqual(num2, 1);

            state2 = task2.Tick();
            Assert.AreEqual(state2, State.Success);
            Assert.AreEqual(num2, 2);
        }

        [Test]
        public void IfTest()
        {
            int num1 = 0;
            var if1 = new IfNode(() => num1 > 1, new TaskNode(() => num1 = -1));
            var state = if1.Tick();
            Assert.AreEqual(state, State.Fail);
            Assert.AreEqual(num1 , 0);
            num1 = 2;state = if1.Tick();
            Assert.AreEqual(state, State.Success);
            Assert.AreEqual(num1 , -1);
        }

        [Test]
        public void LoopTest()
        {
            int num1 = 0;
            const int endNum = 4;
            var loop1 = new LoopNode(() => num1 < endNum, new TaskNode(() => num1++));
            var state = State.Running;
            for (int tmp = 0; state == State.Running ; state = loop1.Tick(), tmp++)
            {
                Assert.AreEqual(tmp, num1);
            }
            Assert.AreEqual(num1, endNum);
            Assert.AreEqual(state, State.Success);
        }

        [Test]
        public void SequenceTest()
        {
            
        }
    }
}