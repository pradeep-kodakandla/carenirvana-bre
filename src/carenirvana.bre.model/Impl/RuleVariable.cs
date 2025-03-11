using Newtonsoft.Json;

namespace carenirvana.bre.model.Impl
{
    public class RuleVariable
    {
        [JsonProperty("variablename")]
        public string? VariableName { get; set; }

        [JsonProperty("variablevalue")]
        public string? VariableValue { get; set; }
    }
}
