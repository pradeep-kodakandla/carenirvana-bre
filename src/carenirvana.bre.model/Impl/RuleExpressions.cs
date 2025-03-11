using Newtonsoft.Json;

namespace carenirvana.bre.model.Impl
{
    public class RuleExpressions
    {
        [JsonProperty("expression")]
        public string Expression { get; set; }

        [JsonProperty("logiccondition")]
        public string LogicCondition { get; set; }
    }
}
