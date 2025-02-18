using Newtonsoft.Json;

namespace carenirvanabre.engine.Models
{
    public class Rule
    {
        [JsonProperty("rulename")]
        public string RuleName { get; set; }

        [JsonProperty("batchrule")]
        public string BatchRule { get; set; }

        [JsonProperty("errormessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        [JsonProperty("expression")]
        public string Expression { get; set; }

        [JsonProperty("successevent")]
        public string SuccessEvent { get; set; }
    }
}
