using carenirvana.bre.common.Range;

namespace carenirvana.bre.batchmanager
{
    public interface IDataSliceSet<T>
    {
        bool HasMore { get; }

        IRange<T> Next();
    }
}
