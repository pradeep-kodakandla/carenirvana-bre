using carenirvana.bre.common.Extensions;
using System.Collections.Concurrent;

namespace carenirvana.bre.common.DataStructure
{
    public class BlockingQueue<T> : IBlockingQueue<T>
    {
        private readonly BlockingCollection<T> _queue;

        public BlockingQueue()
        {
            _queue = [];
        }

        public BlockingQueue(int boundedCapacity)
        {
            _queue = new BlockingCollection<T>(boundedCapacity);
        }

        public int Count => _queue.Count;

        public bool IsAddingCompleted => _queue.IsAddingCompleted;

        public void Enqueue(T item)
        {
            _queue.Add(item);
        }

        public void CompleteAdding()
        {
            _queue.CompleteAdding();
        }

        public Partitioner<T> GetConsumingPartitioner()
        {
            return _queue.GetConsumingPartitioner();
        }

        public IEnumerable<T> GetConsumingEnumerable()
        {
            return _queue.GetConsumingEnumerable();
        }

        public bool TryTake(out T item)
        {
            return _queue.TryTake(out item);
        }
    }
}
