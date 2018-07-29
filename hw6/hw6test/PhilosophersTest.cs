using System;
using System.Threading;
using System.Threading.Tasks;
using Hw6;
using NUnit.Framework;

namespace Hw6Test
{
    public class PhilosophersTest
    {
        private const int Spoons = 5;
        private const int Iterations = 100; 

        private readonly LockGroup _lockGroup = new LockGroup(Spoons);

        private void Philosopher(int id)
        {
            var spoons = new[] { id, (id + 1) % Spoons };
            for (var it = 0; it < Iterations; ++it)
            {
                _lockGroup.Accquire(spoons);
                Thread.Sleep(100);
                _lockGroup.Release(spoons);
                Thread.Sleep(200);
            }
        }

        [Test]
        public void TestPhilosophers()
        {
            var tasks = new Task[Spoons];
            for (var i = 0; i < Spoons; ++i)
            {
                var i1 = i;
                tasks[i] = Task.Factory.StartNew(() => Philosopher(i1), TaskCreationOptions.LongRunning);
            }
            foreach (var task in tasks)
            {
                task.Wait();
            }
        }
    }
}