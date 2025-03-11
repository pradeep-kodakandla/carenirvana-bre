using System.Collections.Concurrent;

namespace carenirvana.bre.common.DataStructure
{
    public class BlockingCollectionPartitioner<T> : Partitioner<T>
    {
        private readonly BlockingCollection<T> _collection;

        internal BlockingCollectionPartitioner(BlockingCollection<T> collection)
        {
            _collection = collection;
        }

        public override bool SupportsDynamicPartitions => true;

        public override IList<IEnumerator<T>> GetPartitions(int partitionCount)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(partitionCount, 1);
            var dynamicPartitions = GetDynamicPartitions();
            return Enumerable.Range(0, partitionCount).Select(_ => dynamicPartitions.GetEnumerator()).ToArray();
        }

        public override IEnumerable<T> GetDynamicPartitions()
        {
            return _collection.GetConsumingEnumerable();
        }
    }
}
