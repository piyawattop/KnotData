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
            Logger.Debug("Init Service");
            InitializeComponent();
            try
            {
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
                Logger.Debug("Init Completed");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Init KnotService");
                Logger.Info(ex.Message);
                Logger.Info(ex.StackTrace);
                throw;
            }

        }

        private void _dependency_OnChanged(object sender, RecordChangedEventArgs<DataStep> e)
        {
            if (e.ChangeType != ChangeType.None)
            {
                switch (e.ChangeType)
                {
                    case ChangeType.Insert:
                        switch (e.Entity.StepName)
                        {
                            case "BARCODE MODEL":
                                Logger.Info($"Step ==> {e.Entity.StepName} # {JsonConvert.SerializeObject(e.Entity)}");
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
                                Logger.Info($"Step ==> {e.Entity.StepName} # {JsonConvert.SerializeObject(e.Entity)}");
                                currentKnotData.Model = "MODEL 1";
                                break;
                            case "PY MODEL 2 START":
                            case "MODEL 2":
                                Logger.Info($"Step ==> {e.Entity.StepName} # {JsonConvert.SerializeObject(e.Entity)}");
                                currentKnotData.Model = "MODEL 2";
                                break;
                            case "PY MODEL 3 START":
                            case "MODEL 3":
                                Logger.Info($"Step ==> {e.Entity.StepName} # {JsonConvert.SerializeObject(e.Entity)}");
                                currentKnotData.Model = "MODEL 3";
                                break;
                            case "IAI START PB":
                                Logger.Info($"Step ==> {e.Entity.StepName} # {JsonConvert.SerializeObject(e.Entity)}");
                                isFocusKnotData = true;
                                break;
                            case "AMBIENT SENSOR CONFIRMED":
                                Logger.Info($"Step ==> {e.Entity.StepName} # {JsonConvert.SerializeObject(e.Entity)}");
                                currentKnotData.AmbientSensor = e.Entity.OutcomeType;
                                break;
                            case "ADAPATER CORD CONFIRMED":
                                Logger.Info($"Step ==> {e.Entity.StepName} # {JsonConvert.SerializeObject(e.Entity)}");
                                currentKnotData.AdaptorCord = e.Entity.OutcomeType;
                                break;
                            case "LH FOAM JIG PUSH FWD CONFIRMED":
                                Logger.Info($"Step ==> {e.Entity.StepName} # {JsonConvert.SerializeObject(e.Entity)}");
                                currentKnotData.LHFoam = e.Entity.OutcomeType;
                                break;
                            case "RH FOAM JIG  DOWN END":
                                Logger.Info($"Step ==> {e.Entity.StepName} # {JsonConvert.SerializeObject(e.Entity)}");
                                currentKnotData.RHFoam = e.Entity.OutcomeType;
                                break;
                            case "UPPER FOAM JIG BWD CONFIRMED":
                                Logger.Info($"Step ==> {e.Entity.StepName} # {JsonConvert.SerializeObject(e.Entity)}");
                                currentKnotData.Upperfoam = e.Entity.OutcomeType;
                                break;
                            case "########### JUMP END ###########":
                                Logger.Info($"Step ==> {e.Entity.StepName} # {JsonConvert.SerializeObject(e.Entity)}");
                                isFocusKnotData = false;
                                if (!string.IsNullOrEmpty(currentBarcodeModelStepUID))
                                    currentKnotData.Barcode = fusionDataService.GetDataStepSubResult(currentBarcodeModelStepUID);
                                if (string.IsNullOrEmpty(currentKnotData.Barcode))
                                {
                                    Logger.Info($"**BARCODE NOT FOUND**");
                                }
                                else
                                {
                                    Logger.Info($"**BARCODE** {currentKnotData.Barcode}");
                                }
                                Logger.Error($"TOTAL TORQUE COUNT: {currentSequence - 1}");
                                if ((currentSequence - 1) != KNOT_COUNT)
                                {
                                    Logger.Error($"BARCODE: {currentKnotData.Barcode} | Knot data not complete");
                                }
                                //Insert data to Knot table
                                KnotDataService.InsertKnotData(currentKnotData);
                                //reset seq
                                currentSequence = 0;
                                currentBarcodeModelStepUID = "";
                                break;
                            default:
                                Logger.Info($"GotStepChange (NotFocus) : {e.Entity.StepName}");
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
                    Logger.Debug($"ORIGINAL # ==> {receivedData}");
                    currentSerialPortReceived += NormalizeLineBreaks(receivedData);
                    Logger.Debug($"RESULT # ==> {currentSerialPortReceived}");
                    if (currentSerialPortReceived.Contains(START) && currentSerialPortReceived.Contains(END))
                    {
                        Logger.Info($"TOTAL TORQUE # ==> {currentSerialPortReceived}");
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
            string[] ports = SerialPort.GetPortNames();
            if (ports.Contains(_COMPORT))
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
