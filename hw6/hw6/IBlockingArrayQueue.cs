namespace Hw6
{
    public interface IBlockingArrayQueue<T>
    {
        void Enqueue(T element);
        bool TryEnqueue(T element);

        T Dequeue();
        (bool succeeded, T value) TryDequeue();

        void Clear();
    }
}