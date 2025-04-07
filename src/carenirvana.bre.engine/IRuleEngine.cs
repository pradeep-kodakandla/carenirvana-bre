using carenirvana.bre.model.Impl;

namespace carenirvana.bre.engine.Interfaces
{
    public interface IRuleEngine
    {
        void ExecuteRuleAsync(string workflowName, params object[] inputs);

        RuleOutput ExecuteRule(string ruleName, params object[] inputs);

        void ExecuteRulesAsync(string ruleSet);

        void ExecuteRules(string ruleSet);
    }
}
