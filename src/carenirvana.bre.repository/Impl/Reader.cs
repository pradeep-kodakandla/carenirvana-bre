using System.Collections.Concurrent;
using carenirvana.bre.common.DataStructure;
using carenirvana.bre.common.Range;
using carenirvana.bre.model;
using carenirvana.bre.model.Impl;

namespace carenirvana.bre.repository.Impl
{
    public class Reader(
                List<RuleDataCategory> ruleDataCategories, 
                List<RuleModel> ruleModels, 
                IRuleDataRepository ruleDataRepository,
                IBlockingQueue<IWorkflowItem> workItems,
                string inputTableName,
                string uniqueIdColumnName) : IReader
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<int, IInputObject>> allInputs = new();
        private readonly List<RuleDataCategory> ruleDataCategories = ruleDataCategories;
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
            Parallel.ForEach(range.Values, item =>
            {
                workItems.Enqueue(GetWorkItem(item));
            });
        }

        private IWorkflowItem GetWorkItem(int id)
        {
            ConcurrentDictionary<string, IInputObject> inputData = new();
            foreach (var item in allInputs.Keys)
            {
                var inputObject = allInputs[item].Where(x => x.Key == id).FirstOrDefault().Value;
                inputData.TryAdd(item, inputObject);
            }
            return new WorkflowItem(inputData, id);
        }

        private async Task LoadData(IRange<int> range)
        {
            foreach (var ruleDataCategory in ruleDataCategories)
            {
                PrepareInputData(range, ruleDataCategory.CategoryName);
            }
        }

        private void PrepareInputData(IRange<int> range, string categoryName)
        {
            allInputs.TryAdd(categoryName, ReadDataFromRepository(
                categoryName,
                "carenirvana.bre.engine.runtime",
                "carenirvana.bre.engine.inputdata",
                range));
        }

        private string BuildQuery(string categoryName, IRange<int> range)
        {
            var attributes = string.Join(",", ruleModels
                .Where(x => x.CategoryName.Equals(categoryName))
                .Select(x => x.DataFieldName));
            return $"select {uniqueIdColumnName},{attributes} from {inputTableName} where {uniqueIdColumnName} between {range.MinValue} and {range.MaxValue}";
        }

        private ConcurrentDictionary<int, IInputObject> ReadDataFromRepository(
                string categoryName,
                string assemblyName,
                string nameSpace,
                IRange<int> range)
        {
            return ruleDataRepository.GetInputDataAsync(
                BuildQuery(categoryName, range),
                assemblyName,
                nameSpace,
                categoryName);
        }
    }
}