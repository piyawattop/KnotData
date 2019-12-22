using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnotBackgroundService.Models
{
    public class DataStepSub
    {
        public string DataStepSubUID { get; set; }

        public string DataStepUID { get; set; }

        public DateTime EntryTime { get; set; }

        public string OutcomeType { get; set; }

        public string ResultData { get; set; }

        public int ExecutionOrder { get; set; }

    }
}
