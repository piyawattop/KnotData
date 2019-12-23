using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnotBackgroundService.Models
{
    public class DataKnot
    {
        public DataKnot()
        {
            TransDateTime = DateTime.Now;
            AmbientSensor = "None";
            Upperfoam = "None";
            LHFoam = "None";
            RHFoam = "None";
            AdaptorCord = "None";
        }

        [Description("Date")]
        public DateTime? TransDateTime { get; set; }
        public string Barcode { get; set; }
        public string Model { get; set; }
        [Description("Ambient Sensor")]
        public string AmbientSensor { get; set; }
        [Description("Upper foam")]
        public string Upperfoam { get; set; }
        [Description("LH Foam")]
        public string LHFoam { get; set; }
        [Description("RH Foam")]
        public string RHFoam { get; set; }
        [Description("Adaptor Cord")]
        public string AdaptorCord { get; set; }
        [Description("Torque#1")]
        public double Torque1 { get; set; }
        [Description("Angle#1")]
        public double Angle1 { get; set; }
        [Description("Code#1")]
        public string Code1 { get; set; }
        [Description("Torque#2")]
        public double Torque2 { get; set; }
        [Description("Angle#2")]
        public double Angle2 { get; set; }
        [Description("Code#2")]
        public string Code2 { get; set; }
        [Description("Torque#3")]
        public double Torque3 { get; set; }
        [Description("Angle#3")]
        public double Angle3 { get; set; }
        [Description("Code#3")]
        public string Code3 { get; set; }
        [Description("Torque#4")]
        public double Torque4 { get; set; }
        [Description("Angle#4")]
        public double Angle4 { get; set; }
        [Description("Code#4")]
        public string Code4 { get; set; }
        [Description("Torque#5")]
        public double Torque5 { get; set; }
        [Description("Angle#5")]
        public double Angle5 { get; set; }
        [Description("Code#5")]
        public string Code5 { get; set; }
        [Description("Torque#6")]
        public double Torque6 { get; set; }
        [Description("Angle#6")]
        public double Angle6 { get; set; }
        [Description("Code#6")]
        public string Code6 { get; set; }

        [Key]
        public string DataKnotUID { get; set; }
        public string CycleID { get; set; }
        public string DataStepUID { get; set; }
        public string ExportedToFile { get; set; }

        public void SetTorque(int seq, string inputString)
        {
            var knotDataArr = inputString.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            double doubleDataTemp;
            if (knotDataArr.Length == 10)
            {
                switch (seq)
                {
                    case 1:
                        double.TryParse(knotDataArr[6], out doubleDataTemp);
                        Torque1 = doubleDataTemp;
                        double.TryParse(knotDataArr[7], out doubleDataTemp);
                        Angle1 = doubleDataTemp;
                        Code1 = knotDataArr[8];
                        break;
                    case 2:
                        double.TryParse(knotDataArr[6], out doubleDataTemp);
                        Torque2 = doubleDataTemp;
                        double.TryParse(knotDataArr[7], out doubleDataTemp);
                        Angle2 = doubleDataTemp;
                        Code2 = knotDataArr[8];
                        break;
                    case 3:
                        double.TryParse(knotDataArr[6], out doubleDataTemp);
                        Torque3 = doubleDataTemp;
                        double.TryParse(knotDataArr[7], out doubleDataTemp);
                        Angle3 = doubleDataTemp;
                        Code3 = knotDataArr[8];
                        break;
                    case 4:
                        double.TryParse(knotDataArr[6], out doubleDataTemp);
                        Torque4 = doubleDataTemp;
                        double.TryParse(knotDataArr[7], out doubleDataTemp);
                        Angle4 = doubleDataTemp;
                        Code4 = knotDataArr[8];
                        break;
                    case 5:
                        double.TryParse(knotDataArr[6], out doubleDataTemp);
                        Torque5 = doubleDataTemp;
                        double.TryParse(knotDataArr[7], out doubleDataTemp);
                        Angle5 = doubleDataTemp;
                        Code5 = knotDataArr[8];
                        break;
                    case 6:
                        double.TryParse(knotDataArr[6], out doubleDataTemp);
                        Torque6 = doubleDataTemp;
                        double.TryParse(knotDataArr[7], out doubleDataTemp);
                        Angle6 = doubleDataTemp;
                        Code6 = knotDataArr[8];
                        break;
                    default:
                        break;
                }
            }
        }

        //public List<DataTorque> dataTorques { get; set; }
    }

    public class DataKnotViewModel
    {
        [Description("Date")]
        public DateTime? TransDateTime { get; set; }
        public string Barcode { get; set; }
        public string Model { get; set; }
        [Description("Ambient Sensor")]
        public string AmbientSensor { get; set; }
        [Description("Upper foam")]
        public string Upperfoam { get; set; }
        [Description("LH Foam")]
        public string LHFoam { get; set; }
        [Description("RH Foam")]
        public string RHFoam { get; set; }
        [Description("Adaptor Cord")]
        public string AdaptorCord { get; set; }
        [Description("Torque#1")]
        public double Torque1 { get; set; }
        [Description("Angle#1")]
        public double Angle1 { get; set; }
        [Description("Code#1")]
        public string Code1 { get; set; }
        [Description("Torque#2")]
        public double Torque2 { get; set; }
        [Description("Angle#2")]
        public double Angle2 { get; set; }
        [Description("Code#2")]
        public string Code2 { get; set; }
        [Description("Torque#3")]
        public double Torque3 { get; set; }
        [Description("Angle#3")]
        public double Angle3 { get; set; }
        [Description("Code#3")]
        public string Code3 { get; set; }
        [Description("Torque#4")]
        public double Torque4 { get; set; }
        [Description("Angle#4")]
        public double Angle4 { get; set; }
        [Description("Code#4")]
        public string Code4 { get; set; }
        [Description("Torque#5")]
        public double Torque5 { get; set; }
        [Description("Angle#5")]
        public double Angle5 { get; set; }
        [Description("Code#5")]
        public string Code5 { get; set; }
        [Description("Torque#6")]
        public double Torque6 { get; set; }
        [Description("Angle#6")]
        public double Angle6 { get; set; }
        [Description("Code#6")]
        public string Code6 { get; set; }
    }
    //public class DataTorque
    //{
    //    public string DataKnotUID { get; set; }
    //    public string OriginalString { get; set; }
    //    public DataTorque(string inputString)
    //    {
    //        this.OriginalString = inputString;
    //        try
    //        {
    //            var knotDataArr = inputString.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
    //            if (knotDataArr.Length == 9)
    //            {
    //                rgdn = knotDataArr[0];
    //                sp = knotDataArr[1];
    //                cy = knotDataArr[2];
    //                ph = knotDataArr[3];
    //                date = knotDataArr[4];
    //                time = knotDataArr[5];
    //                double doubleDataTemp;
    //                double.TryParse(knotDataArr[6], out doubleDataTemp);
    //                torque = doubleDataTemp;
    //                double.TryParse(knotDataArr[7], out doubleDataTemp);
    //                angle = doubleDataTemp;
    //                standbyChar = knotDataArr[8];
    //            }
    //        }
    //        catch (Exception)
    //        {
    //            throw;
    //        }
    //    }
    //    public string rgdn { get; set; }
    //    public string sp { get; set; }
    //    public string cy { get; set; }
    //    public string ph { get; set; }
    //    public string date { get; set; }
    //    public string time { get; set; }
    //    public double torque { get; set; }
    //    public double angle { get; set; }
    //    public double torqueRate { get; set; }
    //    public string standbyChar { get; set; }
    //}
}
