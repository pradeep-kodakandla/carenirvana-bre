namespace carenirvana.bre.model
{
    public interface IBatchInfo
    {
        int BatchNum { get; set; }

        int StartId { get; set; }

        int EndId { get; set; }

        string Status { get; set; }

        DateTime StartTime { get; set; }

        DateTime EndTime { get; set; }

        int BatchSize { get; set; }
    }
}
