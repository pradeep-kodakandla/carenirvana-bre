using Newtonsoft.Json;

namespace carenirvana.bre.model.Impl
{
    public class RuleNode
    {
        [JsonProperty("rulesetname")]
        public string RulesetName { get; set; }

        [JsonProperty("rules")]
        public List<Rule> Rules { get; set; }
    }
}
