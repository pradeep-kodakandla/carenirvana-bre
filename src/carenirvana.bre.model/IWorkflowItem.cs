using carenirvana.bre.model;
using carenirvana.bre.model.Impl;

namespace carenirvana.bre.model
{
    public interface IWorkflowItem
    {
        int Id { get; }

        void AddOutput(RuleOutput output);

        IList<RuleOutput> Outputs { get; }

        IInputObject Input { get; }
    }
}
