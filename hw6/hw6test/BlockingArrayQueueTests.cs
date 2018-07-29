using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hw6;
using NUnit.Framework;

namespace Hw6Test
{
    public abstract class BlockingArrayQueueTestBase
    {
        private IBlockingArrayQueue<long> _queue;
        private long _acc;
        private bool _sequential;
        private bool _settersFinished;

        internal const int Limit = 1_000;//_000;
        private const long Sum = Limit * ((long) Limit - 1) / 2;
        private const int Repeat = 1;

        protected abstract IBlockingArrayQueue<long> CreateQueue();

        private void Setter()
        {
            long next;
            while ((next = Interlocked.Increment(ref _acc)) < Limit)
            {
                _queue.Enqueue(next);
            }
        }

        private long Getter()
        {
            var sum = 0L;
            var prev = long.MinValue;
            while (true)
            {
                var (succeeded, value) = _queue.TryDequeue();
                if (succeeded)
                {
                    if (_sequential)
                    {
                        Assert.Less(prev, value);
                    }
                    sum += value;
                    prev = value;
                }
                else if (_settersFinished)
                {
                    break;
                }
            }
            return sum;
        }

        private void RunSetters(int count)
        {
            var tasks = new Task[count];
            for (var i = 0; i < count; ++i)
            {
                tasks[i] = Task.Factory.StartNew(Setter, TaskCreationOptions.LongRunning);
            }
            new TaskFactory().ContinueWhenAll(tasks, _ => _settersFinished = true);
        }

        private long RunGettersAndWait(int count)
        {
            var tasks = new Task<long>[count];
            for (var i = 0; i < count; i++)
            {
                tasks[i] = Task.Factory.StartNew(Getter, TaskCreationOptions.LongRunning);
            }
            return tasks.Aggregate(0L, (sum, task) => sum + task.Result);
        }

        [SetUp]
        public void SetUp()
        {
            _acc = 0;
            _sequential = true;
            _settersFinished = false;
            _queue = CreateQueue();
        }

        [Test]
        public void TestSignleThreaded()
        {
            Assert.True(_queue.TryEnqueue(1));
            Assert.True(_queue.TryEnqueue(2));

            var (success, value) = _queue.TryDequeue();
            Assert.True(success && value == 1);

            (success, value) = _queue.TryDequeue();
            Assert.True(success && value == 2);

            (success, _) = _queue.TryDequeue();
            Assert.False(success);
        }

        [Test, Repeat(Repeat)]
        public void TestOneSetterAndOneGetter()
        {
            RunSetters(1);
            Assert.AreEqual(Sum, RunGettersAndWait(1));
        }

        [Test, Repeat(Repeat)]
        public void TestOneSettersAndSeveralGetters()
        {
            RunSetters(1);
            Assert.AreEqual(Sum, RunGettersAndWait(2));
        }

        [Test, Repeat(Repeat)]
        public void TestSeveralSettersAndSeveralGetters()
        {
            _sequential = false;
            RunSetters(3);
            Assert.AreEqual(Sum, RunGettersAndWait(3));
        }

        [Test, Repeat(Repeat)]
        public void TestManySettersAndManyGetters()
        {
            _sequential = false;
            RunSetters(10);
            Assert.AreEqual(Sum, RunGettersAndWait(10));
        }
    }

    [TestFixture]
    public class LockingBlockingArrayQueueTest : BlockingArrayQueueTestBase
    {
        protected override IBlockingArrayQueue<long> CreateQueue()
        {
            return new LockingBlockingArrayQueue<long>(Limit);
        }
    }

    [TestFixture]
    public class LockFreeBlockingArrayQueueTest : BlockingArrayQueueTestBase
    {
        protected override IBlockingArrayQueue<long> CreateQueue()
        {
            return new LockFreeBlockingArrayQueue<long>(Limit);
        }
    }
}
