using Newtonsoft.Json;

namespace carenirvana.bre.model.Impl
{
    public class Rule
    {
        [JsonProperty("rulename")]
        public string? RuleName { get; set; }

        [JsonProperty("batchrule")]
        public string? BatchRule { get; set; }

        [JsonProperty("errormessage")]
        public string? ErrorMessage { get; set; }

        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        [JsonProperty("successevent")]
        public string? SuccessEvent { get; set; }

        [JsonProperty("expressions")]
        public List<RuleExpressions>? Expressions { get; set; }

        [JsonProperty("ruleparams")]
        public List<RuleParams>? Parameters { get; set; }
    }
}
