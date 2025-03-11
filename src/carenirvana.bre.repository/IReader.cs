using System.Collections.Concurrent;
using carenirvana.bre.common.Range;
using carenirvana.bre.model;

namespace carenirvana.bre.repository
{
    public interface IReader
    {
        Task QueueInputItemsAsync(IRange<int> range);

        Task QueueInputItemsAsync(IRange<int> range, IBatchInfo batchInfo);
    }
}
