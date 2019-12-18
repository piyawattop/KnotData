using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnotBackgroundService.Models
{
    public class DataKnot
    {
        public string DataKnotUID { get; set; }
        public string Barcode { get; set; }
        public List<DataTorque> dataTorques { get; set; }
    }
    public class DataTorque
    {
        public int Seq { get; set; }
        public string Torque { get; set; }
        public string TorqueAngle { get; set; }
    }
}
