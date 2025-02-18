using Newtonsoft.Json;

namespace carenirvanabre.engine.Models
{
    public class RuleModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("categoryname")]
        public string CategoryName { get; set; }

        [JsonProperty("fieldname")]
        public string FieldName { get; set; }

        [JsonProperty("datafieldname")]
        public string DataFieldName { get; set; }

        [JsonProperty("isderived")]
        public string IsDerived { get; set; }

        [JsonProperty("formula")]
        public string Formula { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("updatedBy")]
        public string UpdatedBy { get; set; }

        [JsonProperty("updatedOn")]
        public DateTime UpdatedOn { get; set; }
    }
}
