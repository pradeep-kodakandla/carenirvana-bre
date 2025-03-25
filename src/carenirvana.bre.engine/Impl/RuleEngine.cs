using carenirvana.bre.dataaccess;
using carenirvana.bre.dataaccess.Impl.Postgres;
using carenirvana.bre.engine;
using carenirvana.bre.engine.Interfaces;

namespace carenirvana.bre
{
    public class RuleEngine : IRuleEngine
    {
        private readonly string breJson;
        private RuleEngineRunner? _engineRunner;

        public RuleEngine(string breJson)
        {
            try
            {
                this.breJson = breJson;
                Init();
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error initializing RuleEngine: {ex.Message}");
                throw;
            }
        }

        public void ExecuteRule(string ruleName, params object[] inputs)
        {
        }

        public void ExecuteRuleAsync(string ruleName, params object[] inputs)
        {
        }

        public void ExecuteRules(string ruleSet)
        {
            _engineRunner.Run();
        }

        public void ExecuteRulesAsync(string ruleSet)
        {
        }

        private void Init()
        {
            _engineRunner = new RuleEngineRunner("ruleinput", "uniqueid");
            _engineRunner.Init(breJson);
        }
    }
}