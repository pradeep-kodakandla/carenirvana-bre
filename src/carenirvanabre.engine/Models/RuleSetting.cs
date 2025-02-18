using Newtonsoft.Json;
using System.Text.Json;

namespace carenirvanabre.engine.Models
{
    public class RuleSetting
    {
        [JsonProperty("rulevariable")]
        public List<RuleVariable> RuleVariable { get; set; }

        [JsonProperty("ruledatacategory")]
        public List<RuleDataCategory> RuleDataCategory { get; set; }

        [JsonProperty("rulemodel")]
        public List<RuleModel> RuleModel { get; set; }

        [JsonProperty("rulenode")]
        public List<RuleNode> RuleNode { get; set; }
    }
}
