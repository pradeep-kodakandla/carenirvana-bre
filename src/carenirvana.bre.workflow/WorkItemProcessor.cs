using carenirvana.bre.common.DataStructure;
using carenirvana.bre.model;

namespace carenirvana.bre.workflow
{
    public class WorkItemProcessor(IBlockingQueue<IWorkflowItem> inputQueue, IBlockingQueue<IWorkflowItem> outputQueue, RuleInvoker ruleInvoker)
    {
        private readonly IBlockingQueue<IWorkflowItem> _inputQueue = inputQueue ?? throw new ArgumentNullException(nameof(inputQueue));
        private readonly IBlockingQueue<IWorkflowItem> _outputQueue = outputQueue ?? throw new ArgumentNullException(nameof(outputQueue));
        private readonly RuleInvoker _ruleInvoker = ruleInvoker ?? throw new ArgumentNullException(nameof(ruleInvoker));

        public void ProcessWorkItemsInParallel()
        {
            Parallel.ForEach(
                _inputQueue.GetConsumingPartitioner(),
                new ParallelOptions { MaxDegreeOfParallelism = 4 },
                workItem =>
                {
                    try
                    {
                        _ruleInvoker.InvokeRuleMethods(workItem);
                        _outputQueue.Enqueue(workItem);
                    }
                    catch (Exception ex)
                    {
                        HandleProcessingError(workItem, ex);
                    }
                });
        }

        private void HandleProcessingError(IWorkflowItem workItem, Exception ex)
        {
            // Log or handle the error as needed
            Console.WriteLine($"Error processing work item {workItem.Id}: {ex.Message}");
        }
    }
}
