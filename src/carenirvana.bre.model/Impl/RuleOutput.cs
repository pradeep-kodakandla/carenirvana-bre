namespace carenirvana.bre.model.Impl
{
    public class RuleOutput
    {
        public int RunId { get; set; }

        public int UniqueId { get; set; }

        public int RuleId { get; set; }

        public string RuleName { get; set; }

        public DateTime RunDtTm { get; set; }

        public bool Result { get; set; }

        public string OutputMessage { get; set; }
    }
}
