using KnotBackgroundService.Models;
using KnotBackgroundService.Services;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;

namespace KnotBackgroundService
{
    public partial class KnotService : ServiceBase
    {

        private string _COMPORT = ConfigurationManager.AppSettings["COMPORT"] ?? "";
        private string _fusionConnectionString = ConfigurationManager.AppSettings["FusionConnectionString"] ?? "";
        private string _knotConnectionString = ConfigurationManager.AppSettings["KnotConnectionString"] ?? "";

        const string START = "#START#";
        const string END = "#END#";
        const int KNOT_COUNT = 6;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private SerialPort ComPort = new SerialPort(); //Initialise ComPort Variable as SerialPort
        private readonly SqlTableDependency<DataStep> _dependency;
        private FusionDataService fusionDataService;
        private KnotDataService KnotDataService;
        private ExportService exportService;
        private DataKnot currentKnotData;
        private string currentBarcodeModelStepUID;
        private int currentSequence;
        private string currentSerialPortReceived;
        private bool isFocusKnotData;
        public KnotService()
        {
            InitializeComponent();
            ComPort.DataReceived += new SerialDataReceivedEventHandler(onComport_DataReceived);
            var mapper = new ModelToTableMapper<DataStep>();
            mapper.AddMapping(model => model.DataStepUID, "DataStepUID");
            mapper.AddMapping(model => model.StepName, "StepName");
            _dependency = new SqlTableDependency<DataStep>(_fusionConnectionString,
                                                           "DataStep",
                                                           mapper: mapper);
            _dependency.OnChanged += _dependency_OnChanged;
            _dependency.OnError += _dependency_OnError;
            _dependency.Start();
            isFocusKnotData = false;
            fusionDataService = new FusionDataService(_fusionConnectionString);
            KnotDataService = new KnotDataService(_knotConnectionString);
            exportService = new ExportService();
        }

        private void _dependency_OnChanged(object sender, RecordChangedEventArgs<DataStep> e)
        {
            if (e.ChangeType != ChangeType.None)
            {
                Logger.Debug(JsonConvert.SerializeObject(e.Entity));
                switch (e.ChangeType)
                {
                    case ChangeType.Insert:
                        switch (e.Entity.StepName)
                        {
                            case "BARCODE MODEL":
                                currentBarcodeModelStepUID = e.Entity.DataStepUID;
                                currentKnotData = new DataKnot
                                {
                                    CycleID = e.Entity.CycleID,
                                    DataStepUID = e.Entity.DataStepUID,
                                    TransDateTime = DateTime.Now
                                };
                                currentSequence = 1;
                                break;
                            case "PY MODEL 1 START":
                            case "MODEL 1":
                                currentKnotData.Model = "MODEL 1";
                                break;
                            case "PY MODEL 2 START":
                            case "MODEL 2":
                                currentKnotData.Model = "MODEL 2";
                                break;
                            case "PY MODEL 3 START":
                            case "MODEL 3":
                                currentKnotData.Model = "MODEL 3";
                                break;
                            case "IAI START PB":
                                isFocusKnotData = true;
                                break;
                            case "AMBIENT SENSOR CONFIRMED":
                                currentKnotData.AmbientSensor = e.Entity.OutcomeType;
                                break;
                            case "ADAPATER CORD CONFIRMED":
                                currentKnotData.AdaptorCord = e.Entity.OutcomeType;
                                break;
                            case "LH FOAM JIG PUSH FWD CONFIRMED":
                                currentKnotData.LHFoam = e.Entity.OutcomeType;
                                break;
                            case "RH FOAM JIG  DOWN END":
                                currentKnotData.RHFoam = e.Entity.OutcomeType;
                                break;
                            case "UPPER FOAM JIG BWD CONFIRMED":
                                currentKnotData.Upperfoam = e.Entity.OutcomeType;
                                break;
                            case "WAITING IAI":
                                isFocusKnotData = false;
                                if (currentSequence != KNOT_COUNT)
                                {
                                    Logger.Error($"BARCODE: {currentKnotData.Barcode} | Knot data not complete");
                                }
                                else
                                {
                                    if(!string.IsNullOrEmpty(currentBarcodeModelStepUID))
                                        currentKnotData.Barcode = fusionDataService.GetDataStepSubResult(currentBarcodeModelStepUID);
                                    //Insert data to Knot table
                                    KnotDataService.InsertKnotData(currentKnotData);
                                    //reset seq
                                    currentSequence = 0;
                                    currentBarcodeModelStepUID = "";
                                }
                                break;
                            default:
                                Logger.Debug($"GotStepChange (NotFocus) : {e.Entity.StepName}");
                                break;
                        }
                        break;
                }
            }
        }

        private void _dependency_OnError(object sender, TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs e)
        {
            Logger.Error(e.Error, "_dependency_OnError");
        }

        private void onComport_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (isFocusKnotData)
            {
                try
                {
                    string receivedData = ComPort.ReadExisting();
                    Logger.Debug(receivedData);
                    Logger.Debug(NormalizeLineBreaks(receivedData));
                    currentSerialPortReceived += NormalizeLineBreaks(receivedData);
                    if (currentSerialPortReceived.Contains(START) && currentSerialPortReceived.Contains(END))
                    {
                        currentKnotData.SetTorque(currentSequence, currentSerialPortReceived);
                        currentSequence++;
                        currentSerialPortReceived = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "onComport_DataReceived");
                }
            }
        }

        private string NormalizeLineBreaks(string input)
        {
            StringBuilder builder = new StringBuilder((int)(input.Length * 1.1));
            foreach (char c in input)
            {
                switch (c)
                {
                    case '\r':
                        builder.Append(START);
                        break;
                    case '\n':
                        builder.Append(END);
                        break;
                    default:
                        builder.Append(c);
                        break;
                }
            }
            return builder.ToString();
        }

        protected override void OnStart(string[] args)
        {
            Logger.Info("onStart");
            //string[] ports = SerialPort.GetPortNames();
            //if (ports.Contains("_COMPORT"))
            //{
            //    ComPort.PortName = _COMPORT;
            //    ComPort.BaudRate = 9800;
            //    ComPort.Parity = Parity.None;
            //    ComPort.DataBits = 8;
            //    ComPort.StopBits = StopBits.One;
            //    try
            //    {
            //        //Open Port
            //        ComPort.Open();
            //        Logger.Info("comport connected");
            //    }
            //    catch (UnauthorizedAccessException e) { Logger.Error(e); }
            //    catch (System.IO.IOException e) { Logger.Error(e); }
            //    catch (ArgumentException e) { Logger.Error(e); }
            //}
            //else
            //{
            //    Logger.Error($"Comport {_COMPORT} invalid please change setting");
            //    throw new Exception($"Comport {_COMPORT} invalid please change setting");
            //}
            exportService.SetScheduler();
        }

        protected override void OnStop()
        {
            Logger.Info("onStop");
        }

        public void OnDebug()
        {
            OnStart(null);
        }
    }
}
