
using Newtonsoft.Json;

namespace carenirvanabre.engine.Models
{
    public class RuleDataCategory
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("categoryname")]
        public string CategoryName { get; set; }

        [JsonProperty("documentType")]
        public string DocumentType { get; set; }

        [JsonProperty("activeFlag")]
        public bool ActiveFlag { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("documentTemplate")]
        public string DocumentTemplate { get; set; }

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
