using carenirvana.bre.batchmanager;
using carenirvana.bre.batchmanager.Impl;
using carenirvana.bre.codebuilder;
using carenirvana.bre.common.DataStructure;
using carenirvana.bre.common.ObjectFactory;
using carenirvana.bre.common.ObjectFactory.Impl;
using carenirvana.bre.dataaccess;
using carenirvana.bre.model;
using carenirvana.bre.model.Impl;
using carenirvana.bre.repository;
using carenirvana.bre.repository.Impl;
using carenirvana.bre.workflow;
using Newtonsoft.Json;
using System;

namespace carenirvana.bre.engine
{
    public class RuleEngineRunner(IAbstractDataLayer dataLayer, string inputTable, string uniqueIdColumnName)
    {
        private readonly IBlockingQueue<IWorkflowItem> inputQueue = new BlockingQueue<IWorkflowItem>();
        private readonly IBlockingQueue<IWorkflowItem> outputQueue = new BlockingQueue<IWorkflowItem>();
        private readonly IObjectFactory objectFactory = new ObjectFactory();
        private readonly IAbstractDataLayer dataLayer = dataLayer;
        private readonly string inputTable = inputTable;
        private readonly string uniqueIdColumnName = uniqueIdColumnName;
        private readonly CancellationTokenSource tokenSource = new();
        private RuleSetting _ruleSetting;
        private IReader reader;
        private IBatchManager batchManager;
        private WorkflowAssemblyCacher workflowAssemblyCacher;
        private WorkflowWorkshopRunner workflowWorkshopRunner;
        private RuleInvoker ruleInvoker;
        private WorkItemProcessor workItemProcessor;
        private IDataSliceSet<int> dataSliceSet;
        private IBulkWriter bulkWriter;

        private Task batchManagerTask;
        private Task inputQueueCompleteTask;
        private Task workShopTask;

        public void Init(string breJson)
        {
            _ruleSetting = JsonConvert.DeserializeObject<RuleSetting>(breJson) ?? 
                        throw new InvalidOperationException("Invalid JSON for RuleSetting");

            // Build rule assembly methods
            BuildRulesAssemblyAndMethods();

            CreateAssemblyCacher();
        }

        public void Run()
        {
            CreateReader();
            CreateUniqueSet();
            CreateBatchManager();
            CreateWorkflowWorkshopRunner();
            CreateBulkWriter();
            StartTasks();
        }

        public void RunARule()
        {
            ruleInvoker.InvokeRuleMethod(string.Empty, null);
        }

        private void CreateBulkWriter()
        {
            bulkWriter = new PostgresBulkWriter(
                        outputQueue, 
                        new RuleDataRepository(dataLayer, objectFactory), 
                        "ruleoutput", 
                        tokenSource);
        }

        private void CreateUniqueSet()
        {
            var repository = new RuleDataRepository(dataLayer, objectFactory);
            var uniqueIds = repository.GetUniqueIds(inputTable, uniqueIdColumnName);
            dataSliceSet = new DataSliceSet<int>(uniqueIds, 1000);
        }

        private void CreateBatchManager()
        {
            batchManager = new BatchManager(dataSliceSet, reader, tokenSource);
        }

        private void BuildRulesAssemblyAndMethods()
        {
            var buildAll = new BuildAll();
            buildAll.Build(_ruleSetting, inputTable);
        }

        private void CreateReader()
        {
            reader = new Reader(
                        _ruleSetting.RuleModel,
                        new RuleDataRepository(dataLayer, objectFactory),
                        inputQueue,
                        inputTable,
                        uniqueIdColumnName);
        }

        private void CreateAssemblyCacher()
        {
            workflowAssemblyCacher = new WorkflowAssemblyCacher(objectFactory);
        }

        private void CreateWorkflowWorkshopRunner()
        {
            ruleInvoker = new RuleInvoker(workflowAssemblyCacher, _ruleSetting, new Random().Next(1, int.MaxValue));
            workItemProcessor = new WorkItemProcessor(inputQueue, outputQueue, ruleInvoker);
            workflowWorkshopRunner = new WorkflowWorkshopRunner(workItemProcessor);
        }

        private void StartTasks()
        {
            batchManagerTask = Task.Run(async() => await batchManager.Run());
            inputQueueCompleteTask = batchManagerTask.ContinueWith(t => inputQueue.CompleteAdding(), tokenSource.Token);
            workShopTask = Task.Run(()=> workflowWorkshopRunner.Run(), tokenSource.Token);

            var outputDataCompleteTask = 
                workShopTask.ContinueWith(t=> outputQueue.CompleteAdding(), tokenSource.Token);
            var bulkWriterTask = new Task(()=>bulkWriter.Write(), tokenSource.Token);
            bulkWriterTask.Start();

            Task.WaitAll([inputQueueCompleteTask, outputDataCompleteTask, bulkWriterTask], tokenSource.Token);
        }
    }
}
