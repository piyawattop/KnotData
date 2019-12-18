using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnotBackgroundService.Models
{
    public class DataCycle
    {
        public string CycleID { get; set; }

        public string StationID { get; set; }

        public int StationRevisionNo { get; set; }

        public DateTime? CycleStartTime { get; set; }

        public DateTime? CycleEndTime { get; set; }

        public string TicketDATA { get; set; }

        public string TicketSEQ { get; set; }

        public string TicketUID { get; set; }

        public string TicketALL { get; set; }

        public string DataCycleXML { get; set; }

        public int? TicketALLCheckSum { get; set; }

    }

}
