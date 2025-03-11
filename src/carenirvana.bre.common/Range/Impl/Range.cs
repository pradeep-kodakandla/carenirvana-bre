namespace carenirvana.bre.common.Range.Impl
{
    public class Range<T> : IRange<T>
    {
        public Range(T[] values)
        {
            Values = [.. values];
            MinValue = Values[0];
            MaxValue = Values[Values.Count - 1];
        }

        public static Range<T> Empty { get; } = new Range<T>([default]);

        public T MinValue { get; }

        public T MaxValue { get; }

        public IList<T> Values { get; }

    }
}
