using CsvHelper;
using IDEReport.Models;
using IDEReport.Services;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IDEReport
{
    public partial class Report : Form
    {
        private KnotDataService _operationService;
        private string _connectionString;
        private bool _isConnectToServer;
        public Report()
        {
            InitializeComponent();
            _connectionString = ConfigurationManager.AppSettings.Get("KnotConnectionString");
            _operationService = new KnotDataService(_connectionString);
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            _isConnectToServer = _operationService.IsValidConnection();
            if (_isConnectToServer)
            {
                var resultList = _operationService.Search(dtStart.Value, dtEnd.Value, txtBarcode.Text, txtModelName.Text);
                var bindingList = new BindingList<DataKnotViewModel>(resultList);
                var source = new BindingSource(bindingList, null);
                gwReportResult.DataSource = source;
                gwReportResult.AutoResizeColumns();
            }
            else
            {
                _isConnectToServer = _operationService.IsValidConnection();
                MessageBox.Show("Please setup database and try again");
            }

        }

        private void Report_Load(object sender, EventArgs e)
        {
            dtStart.Value = DateTime.Now.Date;
            dtEnd.Value = DateTime.Now.Date.AddDays(1);
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            if (_isConnectToServer)
            {
                var resultList = _operationService.Search(dtStart.Value, dtEnd.Value, txtBarcode.Text, txtModelName.Text);
                if (resultList.Count() > 0)
                {
                    var browseFolder = folderBrowserDialog1.ShowDialog();
                    var path = Path.Combine(folderBrowserDialog1.SelectedPath, "Export" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
                    //create a new ExcelPackage
                    using (ExcelPackage excelPackage = new ExcelPackage())
                    {
                        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                        worksheet.Cells["A1"].LoadFromCollection(resultList, true);
                        worksheet.Cells.AutoFitColumns();
                        FileInfo fi = new FileInfo(path);
                        excelPackage.SaveAs(fi);
                    }

                    MessageBox.Show("Exported : " + path + "  total records : " + resultList.Count());
                }
                else
                {
                    MessageBox.Show("Don't have data to export (0 record)");
                }
            }
            else
            {
                _isConnectToServer = _operationService.IsValidConnection();
                MessageBox.Show("Please setup database and try again");
            }
        }

        private void btnCSV_Click(object sender, EventArgs e)
        {
            if (_isConnectToServer)
            {
                var resultList = _operationService.Search(dtStart.Value, dtEnd.Value, txtBarcode.Text, txtModelName.Text);
                if (resultList.Count() > 0)
                {
                    var browseFolder = folderBrowserDialog1.ShowDialog();
                    var path = Path.Combine(folderBrowserDialog1.SelectedPath, "Export" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv");
                    //create a new ExcelPackage
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

                    MessageBox.Show("Exported : " + path + "  total records : " + resultList.Count());
                }
                else
                {
                    MessageBox.Show("Don't have data to export (0 record)");
                }
            }
            else
            {
                _isConnectToServer = _operationService.IsValidConnection();
                MessageBox.Show("Please setup database and try again");
            }
        }
    }
}
