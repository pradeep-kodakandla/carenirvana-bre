using Newtonsoft.Json;

namespace carenirvanabre.engine.Models
{
    public class RuleVariable
    {
        [JsonProperty("variablename")]
        public string VariableName { get; set; }

        [JsonProperty("variablevalue")]
        public string VariableValue { get; set; }
    }
}
