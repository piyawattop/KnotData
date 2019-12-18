using KnotBackgroundService.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;

namespace KnotBackgroundService
{
    public partial class KnotService : ServiceBase
    {

        private string _COMPORT = ConfigurationManager.AppSettings["COMPORT"] ?? "";
        private string _connectionString = ConfigurationManager.AppSettings["DBConnectionString"] ?? "";


        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private SerialPort ComPort = new SerialPort(); //Initialise ComPort Variable as SerialPort
        private readonly SqlTableDependency<DataStep> _dependency;

        private DataKnot currentKnotData;
        private string currentSerialPortReceived;
        private bool isFocusKnotData;
        public KnotService()
        {
            InitializeComponent();
            ComPort.DataReceived += new SerialDataReceivedEventHandler(onComport_DataReceived);
            var mapper = new ModelToTableMapper<DataStep>();
            mapper.AddMapping(model => model.StepName, "StepName");
            _dependency = new SqlTableDependency<DataStep>(_connectionString,
                                                           "DataStep",
                                                           mapper: mapper);
            _dependency.OnChanged += _dependency_OnChanged;
            _dependency.OnError += _dependency_OnError;
            _dependency.Start();
            isFocusKnotData = false;
        }

        private void _dependency_OnChanged(object sender, RecordChangedEventArgs<DataStep> e)
        {

            if (e.ChangeType != ChangeType.None)
            {
                switch (e.ChangeType)
                {
                    case ChangeType.Insert:
                        //Check stepname
                        
                        switch (e.Entity.StepName)
                        {
                            case "BARCODE MODEL":
                                //get barcode from DataStepSub table
                                currentKnotData = new DataKnot
                                {
                                    Barcode = ""
                                };
                                break;
                            case "IAI START PB":
                                isFocusKnotData = true;
                                break;
                            case "WAITING IAI":
                                isFocusKnotData = false;
                                break;
                            default:
                                Logger.Info(e.Entity.StepName);
                                break;
                        }
                        break;

                }
            }
        }

        private void _dependency_OnError(object sender, ErrorEventArgs e)
        {
            throw e.Error;
        }

        private void onComport_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (isFocusKnotData)
            {
                try
                {
                    string receivedData = ComPort.ReadExisting();  //read all available data in the receiving buffer
                    currentSerialPortReceived += receivedData;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "onComport_DataReceived");
                }
            }
        }
        protected override void OnStart(string[] args)
        {
            Logger.Info("onStart");
            string[] ports = SerialPort.GetPortNames();
            if (ports.Contains("_COMPORT"))
            {
                ComPort.PortName = _COMPORT;
                ComPort.BaudRate = 9800;
                ComPort.Parity = Parity.None;
                ComPort.DataBits = 8;
                ComPort.StopBits = StopBits.One;
                try
                {
                    //Open Port
                    ComPort.Open();
                    Logger.Info("comport connected");
                }
                catch (UnauthorizedAccessException e) { Logger.Error(e); }
                catch (System.IO.IOException e) { Logger.Error(e); }
                catch (ArgumentException e) { Logger.Error(e); }
            }
            else
            {
                Logger.Error($"Comport {_COMPORT} invalid please change setting");
                throw new Exception($"Comport {_COMPORT} invalid please change setting");
            }
        }

        protected override void OnStop()
        {
            Logger.Info("onStop");
        }



    }
}
