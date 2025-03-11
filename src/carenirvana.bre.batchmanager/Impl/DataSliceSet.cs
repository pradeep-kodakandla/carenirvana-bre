using carenirvana.bre.common.Range;
using carenirvana.bre.common.Range.Impl;

namespace carenirvana.bre.batchmanager.Impl
{
    public class DataSliceSet<T> : IDataSliceSet<T>
    {
        private readonly object _lock = new();
        private readonly int batchSize;
        private readonly T[] items;
        private int index;

        public DataSliceSet(IList<T> allIds, int batchSize = 10000)
        {
            this.items = new T[allIds.Count];
            this.batchSize = batchSize;
            allIds.CopyTo(items, 0);
        }

        public bool HasMore
        {
            get
            {
                return index <= items.Length - 1;
            }
        }

        public IRange<T> Next()
        {
            lock (_lock)
            {
                int batchSizeToReturn = GetBatchSizeToReturn();
                if (batchSizeToReturn == 0)
                {
                    return Range<T>.Empty;
                }

                var range = GetRange(batchSizeToReturn);
                AdvancePositionOfNextAvailableItem(batchSizeToReturn);
                return range;
            }
        }

        private void AdvancePositionOfNextAvailableItem(int batchSizeToReturn)
        {
            index += batchSizeToReturn;
        }

        private int GetBatchSizeToReturn()
        {
            if (!HasMore)
            {
                return 0;
            }

            int itemsRemaining = items.Length - index;
            if (itemsRemaining >= batchSize)
            {
                return batchSize;
            }

            return itemsRemaining;
        }

        private IRange<T> GetRange(int numberOfItems)
        {
            var slicedItems = items.Skip(index).Take(numberOfItems).ToArray();
            return new Range<T>(slicedItems);
        }
    }
}
