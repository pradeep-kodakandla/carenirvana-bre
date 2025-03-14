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
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace carenirvana.bre.engine
{
    public class RuleEngineRunner
    {
        private readonly IBlockingQueue<IWorkflowItem> _inputQueue;
        private readonly IBlockingQueue<IWorkflowItem> _outputQueue;
        private readonly IObjectFactory _objectFactory;
        private readonly IAbstractDataLayer _dataLayer;
        private readonly string _inputTable;
        private readonly string _uniqueIdColumnName;
        private readonly CancellationTokenSource _tokenSource;
        private RuleSetting _ruleSetting;
        private IReader _reader;
        private IBatchManager _batchManager;
        private WorkflowAssemblyCacher _workflowAssemblyCacher;
        private WorkflowWorkshopRunner _workflowWorkshopRunner;
        private RuleInvoker _ruleInvoker;
        private WorkItemProcessor _workItemProcessor;
        private IDataSliceSet<int> _dataSliceSet;
        private IBulkWriter _bulkWriter;

        private Task _batchManagerTask;
        private Task _inputQueueCompleteTask;
        private Task _workShopTask;

        public RuleEngineRunner(IAbstractDataLayer dataLayer, string inputTable, string uniqueIdColumnName)
        {
            _inputQueue = new BlockingQueue<IWorkflowItem>();
            _outputQueue = new BlockingQueue<IWorkflowItem>();
            _objectFactory = new ObjectFactory();
            _dataLayer = dataLayer ?? throw new ArgumentNullException(nameof(dataLayer));
            _inputTable = inputTable ?? throw new ArgumentNullException(nameof(inputTable));
            _uniqueIdColumnName = uniqueIdColumnName ?? throw new ArgumentNullException(nameof(uniqueIdColumnName));
            _tokenSource = new CancellationTokenSource();
        }

        public void Init(string breJson)
        {
            _ruleSetting = JsonConvert.DeserializeObject<RuleSetting>(breJson) ??
                        throw new InvalidOperationException("Invalid JSON for RuleSetting");

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
            _ruleInvoker.InvokeRuleMethod(string.Empty, null);
        }

        private void CreateBulkWriter()
        {
            _bulkWriter = new PostgresBulkWriter(
                        _outputQueue,
                        new RuleDataRepository(_dataLayer, _objectFactory),
                        "ruleoutput",
                        5000,
                        _tokenSource);
        }

        private void CreateUniqueSet()
        {
            var repository = new RuleDataRepository(_dataLayer, _objectFactory);
            var uniqueIds = repository.GetUniqueIds(_inputTable, _uniqueIdColumnName);
            _dataSliceSet = new DataSliceSet<int>(uniqueIds, 1000);
        }

        private void CreateBatchManager()
        {
            _batchManager = new BatchManager(_dataSliceSet, _reader, _tokenSource);
        }

        private void BuildRulesAssemblyAndMethods()
        {
            var buildAll = new BuildAll();
            buildAll.Build(_ruleSetting, _inputTable);
        }

        private void CreateReader()
        {
            _reader = new Reader(
                        _ruleSetting.RuleModel,
                        new RuleDataRepository(_dataLayer, _objectFactory),
                        _inputQueue,
                        _inputTable,
                        _uniqueIdColumnName);
        }

        private void CreateAssemblyCacher()
        {
            _workflowAssemblyCacher = new WorkflowAssemblyCacher(_objectFactory);
        }

        private void CreateWorkflowWorkshopRunner()
        {
            _ruleInvoker = new RuleInvoker(_workflowAssemblyCacher, _ruleSetting, new Random().Next(1, int.MaxValue));
            _workItemProcessor = new WorkItemProcessor(_inputQueue, _outputQueue, _ruleInvoker);
            _workflowWorkshopRunner = new WorkflowWorkshopRunner(_workItemProcessor);
        }

        private void StartTasks()
        {
            _batchManagerTask = Task.Run(async ()=> await _batchManager.Run());
            _inputQueueCompleteTask = _batchManagerTask.ContinueWith(t => _inputQueue.CompleteAdding(), _tokenSource.Token);
            _workShopTask = Task.Run(() => _workflowWorkshopRunner.Run(), _tokenSource.Token);

            var outputDataCompleteTask =
                _workShopTask.ContinueWith(t => _outputQueue.CompleteAdding(), _tokenSource.Token);
            var bulkWriterTask = new Task(() => _bulkWriter.Write(), _tokenSource.Token);
            bulkWriterTask.Start();

            Task.WaitAll([_inputQueueCompleteTask, outputDataCompleteTask, bulkWriterTask], _tokenSource.Token);
        }
    }
}
