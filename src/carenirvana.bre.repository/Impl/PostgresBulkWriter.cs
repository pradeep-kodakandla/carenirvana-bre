using carenirvana.bre.common.DataStructure;
using carenirvana.bre.model;
using System.Collections.Concurrent;

namespace carenirvana.bre.repository.Impl
{
    public class PostgresBulkWriter(
            IBlockingQueue<IWorkflowItem> outputQueue,
            IRuleDataRepository ruleDataRepository,
            string destTableName,
            CancellationTokenSource cancelTokenSource) : IBulkWriter
    {
        private readonly IBlockingQueue<IWorkflowItem> _outputQueue = outputQueue ?? throw new ArgumentNullException(nameof(outputQueue));
        private readonly ConcurrentQueue<IWorkflowItem> bcpOutputItems = new();
        private readonly IRuleDataRepository _ruleDataRepository = ruleDataRepository;
        private readonly string _destTableName = destTableName;
        private readonly CancellationTokenSource cancelTokenSource = cancelTokenSource;

        public void Write()
        {
            PrepareOutputDataToWrite();
        }

        private void PrepareOutputDataToWrite()
        {
            Parallel.ForEach(
                _outputQueue.GetConsumingPartitioner(),
                new ParallelOptions { MaxDegreeOfParallelism = 5 },
                workItem =>
                {
                    try
                    {
                        _ruleDataRepository.BulkInsert(workItem, _destTableName);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing work item {workItem.Id}: {ex.Message}");
                    }
                });
        }
    }
}
