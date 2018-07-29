using System;
using System.Threading;

namespace Hw6
{
    public class LockingBlockingArrayQueue<T> : IBlockingArrayQueue<T>
    {
        private readonly T[] _elements;
        private int _head, _tail;

        private readonly object _lock;

        public LockingBlockingArrayQueue(int maxSize)
        {
            if (maxSize <= 0)
            {
                throw new ArgumentException($"{nameof(maxSize)} must be positive");
            }

            _elements = new T[maxSize + 1];
            _head = _tail = 0;

            _lock = new object();            
        }

        private bool DoTryEnqueue(T element)
        {
            if ((_head + 1) % _elements.Length == _tail)
            {
                return false;
            }

            _elements[_head++] = element;
            _head %= _elements.Length;
            Monitor.PulseAll(_lock);
            return true;
        }

        private (bool succeeded, T value) DoTryDequeue()
        {
            if (_head == _tail)
            {
                return (false, default(T));
            }

            var toReturn = _elements[_tail++];
            _tail %= _elements.Length;
            Monitor.PulseAll(_lock);
            return (true, toReturn);
        }

        public void Enqueue(T element)
        {
            lock (_lock)
            {
                while (!DoTryEnqueue(element))
                {
                    Monitor.Wait(_lock);
                }
            }
        }

        public bool TryEnqueue(T element)
        {
            lock (_lock)
            {
                return DoTryEnqueue(element);
            }
        }

        public T Dequeue()
        {
            lock (_lock)
            {
                while (true)
                {
                    var (succeeded, value) = DoTryDequeue();
                    if (succeeded)
                        return value;
                    Monitor.Wait(_lock);
                }
            }
        }

        public (bool succeeded, T value) TryDequeue()
        {
            lock (_lock)
            {
                return DoTryDequeue();
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _head = _tail = 0;
                Monitor.PulseAll(_lock);
            }
        }
    }
}
