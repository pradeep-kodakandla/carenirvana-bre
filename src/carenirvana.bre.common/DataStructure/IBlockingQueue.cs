using System.Collections.Concurrent;

namespace carenirvana.bre.common.DataStructure
{
    public interface IBlockingQueue<T>
    {
        int Count { get; }

        bool IsAddingCompleted { get; }

        void Enqueue(T item);

        void CompleteAdding();

        Partitioner<T> GetConsumingPartitioner();

        IEnumerable<T> GetConsumingEnumerable();

        bool TryTake(out T item);
    }
}
