using System;
using System.Threading;

namespace Hw6
{
    public class LockFreeBlockingArrayQueue<T> : IBlockingArrayQueue<T>
    {
        private readonly T[] _elements;
        private int _readIndex, _writeIndex, _maxReadIndex;

        public LockFreeBlockingArrayQueue(int maxSize)
        {
            _elements = new T[maxSize + 1];
            _readIndex = 0;
            _writeIndex = 0;
            _maxReadIndex = 0;
        }
       
        public void Enqueue(T element)
        {
            while (!TryEnqueue(element))
            {                
            }
        }

        public bool TryEnqueue(T element)
        {
            int writeIndex, newWriteIndex;
            do
            {
                writeIndex = _writeIndex;
                var readIndex = _readIndex;

                newWriteIndex = (writeIndex + 1) % _elements.Length;
                if (newWriteIndex == readIndex)
                {
                    return false;
                }
            } while (Interlocked.CompareExchange(ref _writeIndex, newWriteIndex, writeIndex) != writeIndex);

            _elements[writeIndex] = element;

            while (Interlocked.CompareExchange(ref _maxReadIndex, newWriteIndex, writeIndex) != writeIndex)
            {
                Thread.Yield();
            }

            return true;
        }

        public (bool succeeded, T value) TryDequeue()
        {
            int readIndex, newReadIndex;

            readIndex = _readIndex;
            var maximumReadIndex = _maxReadIndex;

            newReadIndex = (readIndex + 1) % _elements.Length;
            if (readIndex != maximumReadIndex)
            {
                var data = _elements[readIndex];
                if (Interlocked.CompareExchange(ref _readIndex, newReadIndex, readIndex) == readIndex)
                {
                    return (true, data);
                }
            }
            return (false, default(T));
        }

        public T Dequeue()
        {
            while (true)
            {
                var (succeeded, value) = TryDequeue();
                if (succeeded)
                    return value;
            }
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }
    }
}