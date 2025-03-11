
using System.Collections.Concurrent;

namespace carenirvana.bre.model.Impl
{
    public class WorkflowItem(ConcurrentDictionary<string, IInputObject> inputData, int id) : IWorkflowItem
    {
        private readonly ConcurrentDictionary<string, IInputObject> _inputData = inputData;
        private readonly List<RuleOutput> _outputRules = [];

        public int Id { get; } = id;

        public IList<RuleOutput> Outputs => _outputRules;

        public IInputObject Input(string inputObjectName)
        {
            return _inputData[inputObjectName];
        }

        public void AddOutput(RuleOutput output)
        {
            _outputRules.Add(output);
        }
    }
}
