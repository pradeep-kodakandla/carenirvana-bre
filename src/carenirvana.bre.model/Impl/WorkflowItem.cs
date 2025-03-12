
using System.Collections.Concurrent;

namespace carenirvana.bre.model.Impl
{
    public class WorkflowItem(IInputObject inputData, int id) : IWorkflowItem
    {
        private readonly IInputObject _inputData = inputData;
        private readonly List<RuleOutput> _outputRules = [];

        public int Id { get; } = id;

        public IList<RuleOutput> Outputs => _outputRules;

        public IInputObject Input => _inputData;

        public void AddOutput(RuleOutput output)
        {
            _outputRules.Add(output);
        }
    }
}
