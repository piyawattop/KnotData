using CsvHelper;
using KnotBackgroundService.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KnotBackgroundService.Services
{
    public class ExportService
    {
        private Timer Schedular;
        private string _knotConnectionString = ConfigurationManager.AppSettings["KnotConnectionString"] ?? "";
        private string _exportPath = ConfigurationManager.AppSettings["ExportPath"] ?? "";
        private string _mode = ConfigurationManager.AppSettings["Mode"] ?? "";
        private string _exportFileType = ConfigurationManager.AppSettings["ExportFileType"] ?? "";
        private string _scheduledTime = ConfigurationManager.AppSettings["ScheduledTime"] ?? "";
        private string _intervalMinutes = ConfigurationManager.AppSettings["IntervalMinutes"] ?? "";
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly KnotDataService knotDataService;

        public ExportService()
        {
            knotDataService = new KnotDataService(_knotConnectionString);
        }

        public void SetScheduler()
        {
            try
            {
                Schedular = new Timer(new TimerCallback(SchedularCallback));
                Logger.Debug($"Export scheduled Mode: {_mode}");

                //Set the Default Time.
                DateTime scheduledTime = DateTime.MinValue;

                if (_mode == "DAILY")
                {
                    //Get the Scheduled Time from AppSettings.
                    scheduledTime = DateTime.Parse(_scheduledTime);
                    if (DateTime.Now > scheduledTime)
                    {
                        //If Scheduled Time is passed set Schedule for the next day.
                        scheduledTime = scheduledTime.AddDays(1);
                    }
                }

                if (_mode.ToUpper() == "INTERVAL")
                {
                    //Get the Interval in Minutes from AppSettings.
                    int intervalMinutes = Convert.ToInt32(_intervalMinutes);

                    //Set the Scheduled Time by adding the Interval to Current Time.
                    scheduledTime = DateTime.Now.AddMinutes(intervalMinutes);
                    if (DateTime.Now > scheduledTime)
                    {
                        //If Scheduled Time is passed set Schedule for the next Interval.
                        scheduledTime = scheduledTime.AddMinutes(intervalMinutes);
                    }
                }

                TimeSpan timeSpan = scheduledTime.Subtract(DateTime.Now);
                string schedule = string.Format("{0} day(s) {1} hour(s) {2} minute(s) {3} seconds(s)", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

                Logger.Debug($"Export scheduled to run after: {schedule}");

                //Get the difference in Minutes between the Scheduled and Current Time.
                int dueTime = Convert.ToInt32(timeSpan.TotalMilliseconds);

                //Change the Timer's Due Time.
                Schedular.Change(dueTime, Timeout.Infinite);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void SchedularCallback(object e)
        {
            var path = Path.Combine(_exportPath, "Export" + DateTime.Now.ToString("yyyyMMddHHmmss") + "." + _exportFileType);
            var resultList = knotDataService.GetDataToExport(path);
            if (resultList.Count() > 0)
            {
                try
                {
                    if (_exportFileType.Equals("excel"))
                    {
                        //create a new ExcelPackage
                        using (ExcelPackage excelPackage = new ExcelPackage())
                        {
                            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                            worksheet.Cells["A1"].LoadFromCollection(resultList, true);
                            worksheet.Cells.AutoFitColumns();
                            FileInfo fi = new FileInfo(path);
                            excelPackage.SaveAs(fi);
                        }
                    }
                    else
                    {
                        using (var writer = new StreamWriter(path))
                        using (var csvWriter = new CsvWriter(writer))
                        {
                            csvWriter.Configuration.Delimiter = ",";
                            csvWriter.WriteField("Date");
                            csvWriter.WriteField("Barcode");
                            csvWriter.WriteField("Model");
                            csvWriter.WriteField("Ambient Sensor");
                            csvWriter.WriteField("Upperfoam");
                            csvWriter.WriteField("LH Foam");
                            csvWriter.WriteField("RH Foam");
                            csvWriter.WriteField("Adaptor Cord");
                            csvWriter.WriteField("Torque#1");
                            csvWriter.WriteField("Angle#1");
                            csvWriter.WriteField("Code#1");
                            csvWriter.WriteField("Torque#2");
                            csvWriter.WriteField("Angle#2");
                            csvWriter.WriteField("Code#2");
                            csvWriter.WriteField("Torque#3");
                            csvWriter.WriteField("Angle#3");
                            csvWriter.WriteField("Code#3");
                            csvWriter.WriteField("Torque#4");
                            csvWriter.WriteField("Angle#4");
                            csvWriter.WriteField("Code#4");
                            csvWriter.WriteField("Torque#5");
                            csvWriter.WriteField("Angle#5");
                            csvWriter.WriteField("Code#5");
                            csvWriter.WriteField("Torque#6");
                            csvWriter.WriteField("Angle#6");
                            csvWriter.WriteField("Code#6");

                            csvWriter.NextRecord();

                            foreach (var item in resultList)
                            {
                                csvWriter.WriteField(item.TransDateTime);
                                csvWriter.WriteField(item.Barcode);
                                csvWriter.WriteField(item.Model);
                                csvWriter.WriteField(item.AmbientSensor);
                                csvWriter.WriteField(item.Upperfoam);
                                csvWriter.WriteField(item.LHFoam);
                                csvWriter.WriteField(item.RHFoam);
                                csvWriter.WriteField(item.AdaptorCord);
                                csvWriter.WriteField(item.Torque1);
                                csvWriter.WriteField(item.Angle1);
                                csvWriter.WriteField(item.Code1);
                                csvWriter.WriteField(item.Torque2);
                                csvWriter.WriteField(item.Angle2);
                                csvWriter.WriteField(item.Code2);
                                csvWriter.WriteField(item.Torque3);
                                csvWriter.WriteField(item.Angle3);
                                csvWriter.WriteField(item.Code3);
                                csvWriter.WriteField(item.Torque4);
                                csvWriter.WriteField(item.Angle4);
                                csvWriter.WriteField(item.Code4);
                                csvWriter.WriteField(item.Torque5);
                                csvWriter.WriteField(item.Angle5);
                                csvWriter.WriteField(item.Code5);
                                csvWriter.WriteField(item.Torque6);
                                csvWriter.WriteField(item.Angle6);
                                csvWriter.WriteField(item.Code6);
                                csvWriter.NextRecord();
                            }
                            writer.Flush();
                        }
                    }
                    Logger.Info($"Exported to : {path}");
                }
                catch (Exception ex)
                {
                    knotDataService.UpdateErrorExportFile(path);
                    Logger.Error(ex);
                }
            }
            this.SetScheduler();
        }

    }
}
