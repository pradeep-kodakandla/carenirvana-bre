using Newtonsoft.Json;

namespace carenirvanabre.engine.Models
{
    public class RuleNode
    {
        [JsonProperty("rulesetname")]
        public string RulesetName { get; set; }

        [JsonProperty("rules")]
        public List<Rule> Rules { get; set; }
    }
}
