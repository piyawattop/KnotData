using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnotBackgroundService.Models
{
    public class DataStep
    {
        public string DataStepUID { get; set; }

        public string CycleID { get; set; }

        public string StepID { get; set; }

        public int StepRevisionNo { get; set; }

        public string AssetID { get; set; }

        public int AssetRevisionNo { get; set; }

        public string StepType { get; set; }

        public string StepName { get; set; }

        public DateTime? StepStartTime { get; set; }

        public DateTime? StepEndTime { get; set; }

        public bool Completed { get; set; }

        public string OutcomeType { get; set; }

        public string UserID { get; set; }

        public string SupervisorID { get; set; }

        public int ExecutionOrder { get; set; }

        public byte[] DataStepBIN { get; set; }

    }


}
