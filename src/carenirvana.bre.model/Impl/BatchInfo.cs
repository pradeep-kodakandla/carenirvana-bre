using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace carenirvana.bre.model.Impl
{
    public class BatchInfo : IBatchInfo
    {
        public int BatchNum { get; set; }

        public int StartId { get; set; }

        public int EndId { get; set; }

        public string Status { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int BatchSize { get; set; }
    }
}
