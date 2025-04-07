using carenirvana.bre.common.DataStructure;
using carenirvana.bre.model;
using System.Collections.Concurrent;

namespace carenirvana.bre.repository.Impl
{
    public class PostgresBulkWriter(
        IBlockingQueue<IWorkflowItem> outputQueue,
        IRuleDataRepository ruleDataRepository,
        string destTableName,
        int batchCount,
        CancellationTokenSource cancelTokenSource) : IBulkWriter
    {
        private readonly IBlockingQueue<IWorkflowItem> _outputQueue = outputQueue ?? throw new ArgumentNullException(nameof(outputQueue));
        private readonly ConcurrentQueue<IWorkflowItem> _bcpOutputItems = new();
        private readonly IRuleDataRepository _ruleDataRepository = ruleDataRepository ?? throw new ArgumentNullException(nameof(ruleDataRepository));
        private readonly int _batchCount = batchCount;
        private readonly string _destTableName = destTableName ?? throw new ArgumentNullException(nameof(destTableName));
        private readonly CancellationTokenSource _cancelTokenSource = cancelTokenSource ?? throw new ArgumentNullException(nameof(cancelTokenSource));
        private readonly object _lock = new();

        public void Write()
        {
            PrepareOutputDataToWrite();
        }

        private void PrepareOutputDataToWrite()
        {
            foreach (var workItem in _outputQueue.GetConsumingEnumerable())
            {
                try
                {
                    EnqueueWorkItem(workItem);
                    if (IsBatchReady())
                    {
                        lock (_lock)
                        {
                            BulkInsertBatch();
                        }
                    }
                }
                catch (Exception ex)
                {
                    HandleProcessingError(workItem, ex);
                }
            }

            // Write the last batch
            BulkInsertBatch();
        }

        private void EnqueueWorkItem(IWorkflowItem workItem)
        {
            _bcpOutputItems.Enqueue(workItem);
        }

        private bool IsBatchReady()
        {
            return _bcpOutputItems.Count == _batchCount;
        }

        private void BulkInsertBatch()
        {   
            if (!_bcpOutputItems.IsEmpty)
            {
                Console.WriteLine($"Writing batch count of ... {_batchCount}");
                _ruleDataRepository.BulkInsert(_bcpOutputItems, _destTableName);
                ClearBatch();
            }
        }

        private void ClearBatch()
        {
            while (_bcpOutputItems.TryDequeue(out _)) { }
        }

        private void HandleProcessingError(IWorkflowItem workItem, Exception ex)
        {
            Console.WriteLine($"Error processing work item {workItem.Id}: {ex.Message}");
        }
    }
}
