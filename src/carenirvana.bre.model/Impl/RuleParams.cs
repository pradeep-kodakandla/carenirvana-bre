using Newtonsoft.Json;

namespace carenirvana.bre.model.Impl
{
    public class RuleParams
    {
        [JsonProperty("paramname")]
        public string? ParameterName { get; set; }

        [JsonProperty("fieldname")]
        public string? FieldName { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }
    }
}
