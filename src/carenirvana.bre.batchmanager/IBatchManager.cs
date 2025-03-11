using carenirvana.bre.common.Range;

namespace carenirvana.bre.batchmanager
{
    public interface IBatchManager
    {
        bool HasMore { get; }

        Task Run();
    }
}
