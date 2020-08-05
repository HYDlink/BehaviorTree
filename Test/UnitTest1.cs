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

        }
    }
}