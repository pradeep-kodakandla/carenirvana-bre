using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace carenirvana.bre.common.Range
{
    public interface IRange<T>
    {
        T MinValue { get; }

        T MaxValue { get; }

        IList<T> Values { get; }
    }
}
