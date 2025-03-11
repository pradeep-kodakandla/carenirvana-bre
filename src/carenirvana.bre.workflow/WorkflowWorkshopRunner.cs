using carenirvana.bre.common.DataStructure;
using carenirvana.bre.model;
using carenirvana.bre.model.Impl;

namespace carenirvana.bre.workflow
{
    public class WorkflowWorkshopRunner(WorkItemProcessor workItemProcessor)
    {
        private readonly WorkItemProcessor _workItemProcessor = workItemProcessor ?? throw new ArgumentNullException(nameof(workItemProcessor));

        public void Run()
        {
            _workItemProcessor.ProcessWorkItemsInParallel();
        }
    }
}
