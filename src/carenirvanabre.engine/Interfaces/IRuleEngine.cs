namespace carenirvanabre.engine.Interfaces
{
    public interface IRuleEngine
    {
        void ExecuteBatchRulesAsync(string workflowName, params object[] inputs);

        void ExecuteBatchRules(string workflowName, params object[] inputs);

        void ExecuteRuleAsync(string workflowName, params object[] inputs);

        void ExecuteRule(string ruleName, params object[] inputs);
    }
}
