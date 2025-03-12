using System.Collections.Concurrent;
using carenirvana.bre.common.DataStructure;
using carenirvana.bre.common.Range;
using carenirvana.bre.model;
using carenirvana.bre.model.Impl;
using carenirvana.bre.utility;

namespace carenirvana.bre.repository.Impl
{
    public class Reader( 
                List<RuleModel> ruleModels, 
                IRuleDataRepository ruleDataRepository,
                IBlockingQueue<IWorkflowItem> workItems,
                string inputTableName,
                string uniqueIdColumnName) : IReader
    {
        private ConcurrentDictionary<int, IInputObject> allInputs = new();
        private readonly List<RuleModel> ruleModels = ruleModels;
        private readonly IRuleDataRepository ruleDataRepository = ruleDataRepository;
        private readonly IBlockingQueue<IWorkflowItem> workItems = workItems;
        private readonly string inputTableName = inputTableName;
        private readonly string uniqueIdColumnName = uniqueIdColumnName;

        public async Task QueueInputItemsAsync(IRange<int> range)
        {
            await LoadData(range);
            QueueItems(range);
        }

        public async Task QueueInputItemsAsync(IRange<int> range, IBatchInfo batchInfo)
        {
            await LoadData(range);
        }

        private void QueueItems(IRange<int> range)
        {
            Parallel.ForEach(range.Values, new ParallelOptions { MaxDegreeOfParallelism = 2 }, item =>
            {
                workItems.Enqueue(GetWorkItem(item));
            });
        }

        private IWorkflowItem GetWorkItem(int id)
        {
            return new WorkflowItem(allInputs[id], id);
        }

        private async Task LoadData(IRange<int> range)
        {
            PrepareInputData(range);
        }

        private void PrepareInputData(IRange<int> range)
        {
            allInputs = ReadDataFromRepository(
                ConstantsUtility.RunTimeNameSpace,
                ConstantsUtility.RunTimeInputDataTypeName,
                range);
        }

        private string BuildQuery(IRange<int> range)
        {
            var attributes = string.Join(",", ruleModels
                .Select(x => x.DataFieldName));
            return $"select {uniqueIdColumnName},{attributes} from {inputTableName} where {uniqueIdColumnName} between {range.MinValue} and {range.MaxValue}";
        }

        private ConcurrentDictionary<int, IInputObject> ReadDataFromRepository(
                string assemblyName,
                string nameSpace,
                IRange<int> range)
        {
            return ruleDataRepository.GetInputDataAsync(
                BuildQuery(range),
                assemblyName,
                nameSpace,
                inputTableName);
        }
    }
}