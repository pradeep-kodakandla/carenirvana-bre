using carenirvana.bre.common.DataStructure;
using System.Collections.Concurrent;

namespace carenirvana.bre.common.Extensions
{
    public static class BlockingCollectionExtensions
    {
        public static Partitioner<T> GetConsumingPartitioner<T>(this BlockingCollection<T> collection)
        {
            return new BlockingCollectionPartitioner<T>(collection);
        }
    }
}
