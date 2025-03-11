using Newtonsoft.Json;

namespace carenirvana.bre.model.Impl
{
    public class RuleFunction
    {
        [JsonProperty("functionname")]
        public string Name { get; set; }

        [JsonProperty("functiondescription")]
        public string Description { get; set; }

        [JsonProperty("functioncode")]
        public string MethodBody { get; set; }

        [JsonProperty("return")]
        public string ReturnType { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("updatedBy")]
        public string UpdatedBy { get; set; }

        [JsonProperty("updatedOn")]
        public DateTime UpdatedOn { get; set; }

        [JsonProperty("ruleparams")]
        public List<RuleParams> Parameters { get; set; }
    }
}
