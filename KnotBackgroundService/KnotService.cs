using KnotBackgroundService.Models;
using KnotBackgroundService.Services;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
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
        private bool isStartCompleted;
        private Timer timer;
        public KnotService()
        {
            Logger.Info("onInit Service");
            InitializeComponent();
            isStartCompleted = false;
            try
            {
                var mapper = new ModelToTableMapper<DataStep>();
                mapper.AddMapping(model => model.DataStepUID, "DataStepUID");
                mapper.AddMapping(model => model.StepName, "StepName");
                _dependency = new SqlTableDependency<DataStep>(_fusionConnectionString,
                                                               "DataStep",
                                                               mapper: mapper);
                _dependency.OnChanged += _dependency_OnChanged;
                _dependency.OnError += _dependency_OnError;
                _dependency.Start();
                
                Logger.Info("onInit Completed");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Init KnotService");
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace);
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
                            case "READY ON":
                                Logger.Info($"Step ==> {e.Entity.StepName} # {e.Entity.CycleID}");
                                if (currentKnotData == null)
                                {
                                    currentKnotData = new DataKnot
                                    {
                                        CycleID = e.Entity.CycleID,
                                        DataStepUID = e.Entity.DataStepUID,
                                        TransDateTime = DateTime.Now
                                    };
                                }
                                break;
                            case "BARCODE MODEL":
                                Logger.Info($"Step ==> {e.Entity.StepName} # {JsonConvert.SerializeObject(e.Entity)}");
                                currentBarcodeModelStepUID = e.Entity.DataStepUID;
                                if (currentKnotData == null)
                                {
                                    currentKnotData = new DataKnot
                                    {
                                        CycleID = e.Entity.CycleID,
                                        DataStepUID = e.Entity.DataStepUID,
                                        TransDateTime = DateTime.Now
                                    };
                                }
                                else
                                {
                                    currentKnotData.CycleID = e.Entity.CycleID;
                                    currentKnotData.DataStepUID = e.Entity.DataStepUID;
                                }
                                currentSequence = 1;
                                break;
                            case "JUMP MODEL GC7":
                                if (e.Entity.CycleID == currentKnotData.CycleID)
                                {
                                    Logger.Info($"Step ==> {e.Entity.StepName} # {JsonConvert.SerializeObject(e.Entity)}");
                                    if (currentKnotData == null)
                                    {
                                        currentKnotData = new DataKnot
                                        {
                                            CycleID = e.Entity.CycleID,
                                            TransDateTime = DateTime.Now
                                        };
                                    }
                                    currentKnotData.Model = "MODEL GC7";
                                }
                                break;
                            case "JUMP MODEL HM7":
                                if (e.Entity.CycleID == currentKnotData.CycleID)
                                {
                                    Logger.Info($"Step ==> {e.Entity.StepName} # {JsonConvert.SerializeObject(e.Entity)}");
                                    if (currentKnotData == null)
                                    {
                                        currentKnotData = new DataKnot
                                        {
                                            CycleID = e.Entity.CycleID,
                                            TransDateTime = DateTime.Now
                                        };
                                    }
                                    currentKnotData.Model = "MODEL HM7";
                                }
                                break;
                            case "JUMP MODEL HN6":
                                if (e.Entity.CycleID == currentKnotData.CycleID)
                                {
                                    Logger.Info($"Step ==> {e.Entity.StepName} # {JsonConvert.SerializeObject(e.Entity)}");
                                    if (currentKnotData == null)
                                    {
                                        currentKnotData = new DataKnot
                                        {
                                            CycleID = e.Entity.CycleID,
                                            TransDateTime = DateTime.Now
                                        };
                                    }
                                    currentKnotData.Model = "MODEL HN6";
                                }
                                break;
                            case "IAI START PB":
                                if (e.Entity.CycleID == currentKnotData.CycleID)
                                {
                                    Logger.Info($"Step ==> {e.Entity.StepName} # {JsonConvert.SerializeObject(e.Entity)}");
                                    isFocusKnotData = true;
                                }
                                break;
                            case "AMBIENT SENSOR CONFIRMED":
                                if (e.Entity.CycleID == currentKnotData.CycleID)
                                {
                                    Logger.Info($"Step ==> {e.Entity.StepName} # {e.Entity.OutcomeType}");
                                    if (currentKnotData == null)
                                    {
                                        currentKnotData = new DataKnot
                                        {
                                            TransDateTime = DateTime.Now
                                        };
                                    }
                                    currentKnotData.AmbientSensor = e.Entity.OutcomeType;
                                }
                                break;
                            case "ADAPATER CORD CONFIRMED":
                                if (e.Entity.CycleID == currentKnotData.CycleID)
                                {
                                    Logger.Info($"Step ==> {e.Entity.StepName} # {e.Entity.OutcomeType}");
                                    if (currentKnotData == null)
                                    {
                                        currentKnotData = new DataKnot
                                        {
                                            TransDateTime = DateTime.Now
                                        };
                                    }
                                    currentKnotData.AdaptorCord = e.Entity.OutcomeType;
                                }
                                break;
                            case "LH FOAM JIG PUSH FWD CONFIRMED":
                                if (e.Entity.CycleID == currentKnotData.CycleID)
                                {
                                    Logger.Info($"Step ==> {e.Entity.StepName} # {e.Entity.OutcomeType}");
                                    if (currentKnotData == null)
                                    {
                                        currentKnotData = new DataKnot
                                        {
                                            TransDateTime = DateTime.Now
                                        };
                                    }
                                    currentKnotData.LHFoam = e.Entity.OutcomeType;
                                }
                                break;
                            case "RH FOAM JIG FWD":
                                if (e.Entity.CycleID == currentKnotData.CycleID)
                                {
                                    Logger.Info($"Step ==> {e.Entity.StepName} # {e.Entity.OutcomeType}");
                                    if (currentKnotData == null)
                                    {
                                        currentKnotData = new DataKnot
                                        {
                                            TransDateTime = DateTime.Now
                                        };
                                    }
                                    currentKnotData.RHFoam = e.Entity.OutcomeType;
                                }
                                break;
                            case "UPPER FOAM JIG FWD":
                                if (e.Entity.CycleID == currentKnotData.CycleID)
                                {
                                    Logger.Info($"Step ==> {e.Entity.StepName} # {e.Entity.OutcomeType}");
                                    if (currentKnotData == null)
                                    {
                                        currentKnotData = new DataKnot
                                        {
                                            TransDateTime = DateTime.Now
                                        };
                                    }
                                    currentKnotData.Upperfoam = e.Entity.OutcomeType;
                                }
                                break;
                            case "END":
                                if (e.Entity.CycleID == currentKnotData.CycleID)
                                {
                                    Logger.Info($"Step ==> {e.Entity.StepName} # {JsonConvert.SerializeObject(e.Entity)}");
                                    isFocusKnotData = false;
                                    GetStepData();
                                    if (string.IsNullOrEmpty(currentKnotData.Barcode))
                                    {
                                        Logger.Info($"**BARCODE NOT FOUND**");
                                    }
                                    else
                                    {
                                        Logger.Info($"**BARCODE** {currentKnotData.Barcode}");
                                        Logger.Info($"TOTAL TORQUE COUNT: {currentSequence - 1}");
                                        if ((currentSequence - 1) != KNOT_COUNT)
                                        {
                                            Logger.Error($"BARCODE: {currentKnotData.Barcode} | Knot data not complete");
                                        }
                                        //Insert data to Knot table
                                        Logger.Info($"INSERT BARCODE {currentKnotData.Barcode} # {JsonConvert.SerializeObject(currentKnotData)}");
                                        KnotDataService.InsertKnotData(currentKnotData);
                                    }
                                    //reset seq
                                    currentKnotData = null;
                                    currentSequence = 0;
                                    currentBarcodeModelStepUID = "";
                                }
                                break;
                            default:
                                Logger.Debug($"GotStepChange (NotFocus) : {e.Entity.StepName}");
                                break;
                        }
                        break;
                    case ChangeType.Update:
                        switch (e.Entity.StepName)
                        {
                            case "AMBIENT SENSOR CONFIRMED":
                                if (e.Entity.CycleID == currentKnotData.CycleID)
                                {
                                    Logger.Info($"Step ==> {e.Entity.StepName} # {e.Entity.OutcomeType}");
                                    if (currentKnotData == null)
                                    {
                                        currentKnotData = new DataKnot
                                        {
                                            TransDateTime = DateTime.Now
                                        };
                                    }
                                    currentKnotData.AmbientSensor = e.Entity.OutcomeType;
                                }
                                break;
                            case "ADAPATER CORD CONFIRMED":
                                if (e.Entity.CycleID == currentKnotData.CycleID)
                                {
                                    Logger.Info($"Step ==> {e.Entity.StepName} # {e.Entity.OutcomeType}");
                                    if (currentKnotData == null)
                                    {
                                        currentKnotData = new DataKnot
                                        {
                                            TransDateTime = DateTime.Now
                                        };
                                    }
                                    currentKnotData.AdaptorCord = e.Entity.OutcomeType;
                                }
                                break;
                            case "LH FOAM JIG PUSH FWD CONFIRMED":
                                if (e.Entity.CycleID == currentKnotData.CycleID)
                                {
                                    Logger.Info($"Step ==> {e.Entity.StepName} # {e.Entity.OutcomeType}");
                                    if (currentKnotData == null)
                                    {
                                        currentKnotData = new DataKnot
                                        {
                                            TransDateTime = DateTime.Now
                                        };
                                    }
                                    currentKnotData.LHFoam = e.Entity.OutcomeType;
                                }
                                break;
                            case "RH FOAM JIG FWD":
                                if (e.Entity.CycleID == currentKnotData.CycleID)
                                {
                                    Logger.Info($"Step ==> {e.Entity.StepName} # {e.Entity.OutcomeType}");
                                    if (currentKnotData == null)
                                    {
                                        currentKnotData = new DataKnot
                                        {
                                            TransDateTime = DateTime.Now
                                        };
                                    }
                                    currentKnotData.RHFoam = e.Entity.OutcomeType;
                                }
                                break;
                            case "UPPER FOAM JIG FWD":
                                if (e.Entity.CycleID == currentKnotData.CycleID)
                                {
                                    Logger.Info($"Step ==> {e.Entity.StepName} # {e.Entity.OutcomeType}");
                                    if (currentKnotData == null)
                                    {
                                        currentKnotData = new DataKnot
                                        {
                                            TransDateTime = DateTime.Now
                                        };
                                    }
                                    currentKnotData.Upperfoam = e.Entity.OutcomeType;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                }
            }
        }

        private void GetStepData()
        {
            if (!string.IsNullOrEmpty(currentBarcodeModelStepUID))
            {
                currentKnotData.Barcode = fusionDataService.GetDataStepSubResult(currentBarcodeModelStepUID);
                Logger.Info($"GetBarcode({currentBarcodeModelStepUID}) from DB: {currentKnotData.Barcode}");
            }

            if (string.IsNullOrEmpty(currentKnotData.Model))
            {
                currentKnotData.Model = fusionDataService.GetModel(currentKnotData.CycleID);
                Logger.Info($"GetModel from DB: {currentKnotData.Model}");
            }


            if (currentKnotData.AdaptorCord == "None" || string.IsNullOrEmpty(currentKnotData.AdaptorCord))
            {
                currentKnotData.AdaptorCord = fusionDataService.GetStepOutComeType(currentKnotData.CycleID, "ADAPATER CORD CONFIRMED");
                Logger.Info($"Get AdaptorCord from DB: {currentKnotData.AdaptorCord}");
            }
            if (currentKnotData.AmbientSensor == "None" || string.IsNullOrEmpty(currentKnotData.AmbientSensor))
            {
                currentKnotData.AmbientSensor = fusionDataService.GetStepOutComeType(currentKnotData.CycleID, "AMBIENT SENSOR CONFIRMED");
                Logger.Info($"Get AmbientSensor from DB: {currentKnotData.AmbientSensor}");
            }
            if (currentKnotData.LHFoam == "None" || string.IsNullOrEmpty(currentKnotData.AmbientSensor))
            {
                currentKnotData.LHFoam = fusionDataService.GetStepOutComeType(currentKnotData.CycleID, "LH FOAM JIG PUSH FWD CONFIRMED");
                Logger.Info($"Get LHFoam from DB: {currentKnotData.LHFoam}");
            }
            if (currentKnotData.RHFoam == "None" || string.IsNullOrEmpty(currentKnotData.RHFoam))
            {
                currentKnotData.RHFoam = fusionDataService.GetStepOutComeType(currentKnotData.CycleID, "RH FOAM JIG FWD");
                Logger.Info($"Get AmbientSensor from DB: {currentKnotData.RHFoam}");
            }
            if (currentKnotData.Upperfoam == "None" || string.IsNullOrEmpty(currentKnotData.Upperfoam))
            {
                currentKnotData.Upperfoam = fusionDataService.GetStepOutComeType(currentKnotData.CycleID, "UPPER FOAM JIG FWD");
                Logger.Info($"Get AmbientSensor from DB: {currentKnotData.Upperfoam}");
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

            try
            {
                isFocusKnotData = false;
                fusionDataService = new FusionDataService(_fusionConnectionString);
                KnotDataService = new KnotDataService(_knotConnectionString);
                exportService = new ExportService();
                ComPort.DataReceived += new SerialDataReceivedEventHandler(onComport_DataReceived);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "onStart KnotService");
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace);
                throw;
            }
            timer = new Timer();
            timer.Interval = 10000; // 60 seconds
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            if (!isStartCompleted)
            {
                Logger.Info("onTimer");
                try
                {
                    ConnectComport();
                    exportService.SetScheduler();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "onTimer");
                    Logger.Error(ex.Message);
                    Logger.Error(ex.StackTrace);
                }
            }
        }
        private void ConnectComport()
        {
            bool retryConnect = true;
            int retryCount = 0;
            if (!string.IsNullOrWhiteSpace(_COMPORT))
            {
                string[] ports = SerialPort.GetPortNames();
                if (ports.Contains(_COMPORT))
                {
                    ComPort.PortName = _COMPORT;
                    ComPort.BaudRate = 9800;
                    ComPort.Parity = Parity.None;
                    ComPort.DataBits = 8;
                    ComPort.StopBits = StopBits.One;
                    while (retryConnect)
                    {
                        try
                        {
                            //Open Port
                            ComPort.Open();
                            retryConnect = false;
                            Logger.Info("comport connected");
                            isStartCompleted = true;
                            timer.Stop();
                        }
                        catch (UnauthorizedAccessException ex) { Logger.Error(ex); System.Threading.Thread.Sleep(2000); }
                        catch (System.IO.IOException ex) { Logger.Error(ex); System.Threading.Thread.Sleep(2000); }
                        catch (ArgumentException ex) { Logger.Error(ex); System.Threading.Thread.Sleep(2000); }
                        retryCount++;
                        if (retryCount > 10)
                        {
                            Logger.Error($"Cannot connect to comport {_COMPORT} please check cable or setting file");
                            throw new Exception($"Cannot connect to comport {_COMPORT} please check cable or setting file");
                        }
                    }
                }
                else
                {
                    Logger.Error($"Comport {_COMPORT} invalid please change setting");
                    throw new Exception($"Comport {_COMPORT} invalid please change setting");
                }
            }
            else
            {
                Logger.Error($"Comport {_COMPORT} not set");
            }
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
