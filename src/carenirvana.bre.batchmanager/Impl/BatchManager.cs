using carenirvana.bre.repository;

namespace carenirvana.bre.batchmanager.Impl
{
    public class BatchManager( 
            IDataSliceSet<int> dataSliceSet, 
            IReader reader, 
            CancellationTokenSource cancelWaitingTask) : IBatchManager
    {
        private readonly IDataSliceSet<int> dataSliceSet = dataSliceSet;
        private readonly IReader reader = reader;
        private readonly CancellationTokenSource cancelWaitingTask = cancelWaitingTask;

        public bool HasMore => dataSliceSet.HasMore;

        public async Task Run()
        {
            while (dataSliceSet.HasMore)
            {
                if (cancelWaitingTask.Token.IsCancellationRequested)
                {
                    break;
                }
                await ReadNextBatch();
            }
        }

        private async Task ReadNextBatch()
        {
            var range = dataSliceSet.Next();
            await reader.QueueInputItemsAsync(range);
        }
    }
}
