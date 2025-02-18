using carenirvanabre.engine.Interfaces;
using carenirvanabre.engine.Models;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace carenirvanabre
{
    public class RuleEngine : IRuleEngine
    {
        private readonly string breJson = string.Empty;
        private readonly RuleSetting? ruleSetting = null;

        public RuleEngine(string breJson)
        {
            this.breJson = breJson;

            //load rule settings...
            ruleSetting = JsonSerializer.Deserialize<RuleSetting>(breJson);

            //build rule expressions

        }

        public void ExecuteBatchRules(string ruleSet, params object[] inputs)
        {
            throw new NotImplementedException();
        }

        public void ExecuteBatchRulesAsync(string ruleSet, params object[] inputs)
        {
            throw new NotImplementedException();
        }

        public void ExecuteRule(string ruleName, params object[] inputs)
        {
            throw new NotImplementedException();
        }

        public void ExecuteRuleAsync(string ruleName, params object[] inputs)
        {
            throw new NotImplementedException();
        }
    }
}
