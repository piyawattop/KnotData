using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SerialPortConnector
{
    public partial class Form1 : Form
    {
        private SerialPort ComPort = new SerialPort(); //Initialise ComPort Variable as SerialPort
        public Form1()
        {
            InitializeComponent();
            updatePorts();
            cmbBaudeRate.SelectedIndex = 0;
            cmbDataBits.SelectedIndex = 3;
            cmbParity.SelectedIndex = 0;
            cmbStopBits.SelectedIndex = 0;
            cmbPortName.SelectedIndex = 0;
            rdText.Checked = true;
            ComPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(comport_DataReceived);
            groupBox1.Enabled = false;

        }
        private void updatePorts()
        {
            //Retrieve the list of all COM  ports on your computer
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                cmbPortName.Items.Add(port);
            }
        }

        private void comport_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                string receivedData = NormalizeLineBreaks(ComPort.ReadExisting());
                string debug = receivedData.Replace("\r", "\\r")
                                         .Replace("\n", "\\n");
                rttbDebug.AppendText(debug + "\n");

                rtxtDataArea.ForeColor = Color.Green; 
                rtxtDataArea.AppendText(receivedData + "\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
       private string NormalizeLineBreaks(string input)
        {
            // Allow 10% as a rough guess of how much the string may grow.
            // If we're wrong we'll either waste space or have extra copies -
            // it will still work
            StringBuilder builder = new StringBuilder((int)(input.Length * 1.1));

            bool lastWasCR = false;

            foreach (char c in input)
            {
                if (lastWasCR)
                {
                    lastWasCR = false;
                    if (c == '\n')
                    {
                        continue; // Already written \r\n
                    }
                }
                switch (c)
                {
                    case '\r':
                        builder.Append("\r\n");
                        lastWasCR = true;
                        break;
                    case '\n':
                        builder.Append("\r\n");
                        break;
                    default:
                        builder.Append(c);
                        break;
                }
            }
            return builder.ToString();
        }
        private void connect()
        {
            bool error = false;

            //check if all settings have been selected

            if (cmbPortName.SelectedIndex != -1 & cmbBaudeRate.SelectedIndex != -1 & cmbParity.SelectedIndex != -1 &
                cmbDataBits.SelectedIndex != -1 & cmbStopBits.SelectedIndex != -1)
            {  //if yes than set the port's settings
                ComPort.PortName = cmbPortName.Text;
                ComPort.BaudRate = int.Parse(cmbBaudeRate.Text);   //convert Text to integer
                ComPort.Parity = (Parity)Enum.Parse(typeof(Parity), cmbParity.Text);   //convert text to parity
                ComPort.DataBits = int.Parse(cmbDataBits.Text);  //convert text to stop bits
                ComPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), cmbStopBits.Text); //convert text to stopbits

                try //always try to use this try and catch method to open your port.
                //if there is an error yuor program will not display a message instead of freezing.
                {
                    //Open Port
                    ComPort.Open();
                }
                catch (UnauthorizedAccessException) { error = true; }
                catch (System.IO.IOException) { error = true; }
                catch (ArgumentException) { error = true; }

                if (error) MessageBox.Show(this, "Could not open the COM port. Most likely it is already in use, has been removed, or is unavailable.", "COM Port unavailable",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                MessageBox.Show("Please select all the COM Serial Port Settings", "Serial Port Interface", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            //if the port is open, change the connect button to disconnect, enable the send button
            //and disable the groupBox to prevent changing configuration of an open port.
            if (ComPort.IsOpen)
            {
                btnConnect.Text = "Disconnect";
                btnSend.Enabled = true;
                if (!rdText.Checked & !rdHex.Checked)  //if no data mode is selected, then select text mode by default
                {
                    rdText.Checked = true;
                }
                groupBox1.Enabled = true;

            }
        }
        //Call this function to close the port.
        private void disconnect()
        {
            ComPort.Close();
            btnConnect.Text = "Connect";
            btnSend.Enabled = false;
            groupBox1.Enabled = false;
        }

        private void sendData()
        {
            bool error = false;
            if (rdText.Checked == true)  //if text mode is selected, send data as text
            {
                // send the user's text straight out the port
                ComPort.Write("Your Text...");

                //show in the terminal window
                rtxtDataArea.ForeColor = Color.Green;  //write text data in green
                rtxtDataArea.AppendText(txtSend.Text + "\n");
                txtSend.Clear();
            }
            else    //if HEx mode is selected, send data in hexadecimal
            {
                try
                {
                    //convert the user's string of hex digits (example: E1 FF 1B) to a byte array

                    byte[] data = HexStringToByteArray(txtSend.Text);

                    //send the binary data out the port
                    ComPort.Write(new byte[] { 0x0A, 0x11, 0xFF }, 0, 3);

                    //show the hex digits on in the terminal window
                    rtxtDataArea.ForeColor = Color.Blue;
                    rtxtDataArea.AppendText(txtSend.Text.ToUpper() + "\n");
                    txtSend.Clear();   //clear screen after sending data
                }
                catch (FormatException) { error = true; }
                //inform the user if the hex string was not properly formatted
                catch (ArgumentException) { error = true; }

                if (error) MessageBox.Show(this, "Not properly formatted hex string: " + txtSend.Text + "\n", "Format Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
        //convert a string of hex digits (example: E1 FF 1B) to a byte array.
        //The string containing the hex digits (with or without spaces)
        //Returns an array of bytes </returns>

        private byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

        //Converts an array of bytes into a formatted string of hex digits (example: E1 FF 1B)
        //The array of bytes to be translated into a string of hex didgits.
        //Returns a well formatted string of hex digits with spacing.

        private string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
            return sb.ToString().ToUpper();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (ComPort.IsOpen)
            {
                disconnect();
            }
            else
            {
                connect();
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            sendData();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ComPort.IsOpen) ComPort.Close();   //close the port if open when existing the application

        }

        private void button1_Click(object sender, EventArgs e)
        {

            
            var currentSerialPortReceived = "#START#0093 01 01 03 23/12/19 17:30:14  00.055  0250.7                  Rt #END#";
            var obj = new DataTorque(currentSerialPortReceived);
        }

    }
    public class DataTorque
    {
        public string originalString { get; set; }
        public DataTorque(string inputString)
        {
            this.originalString = inputString;
            try
            {
                var knotDataArr = inputString.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if(knotDataArr.Length == 9)
                {
                    rgdn = knotDataArr[0];
                    sp = knotDataArr[1];
                    cy = knotDataArr[2];
                    ph = knotDataArr[3];
                    date = knotDataArr[4];
                    time = knotDataArr[5];
                    double doubleDataTemp;
                    double.TryParse(knotDataArr[6], out doubleDataTemp);
                    torque = doubleDataTemp;
                    double.TryParse(knotDataArr[7], out doubleDataTemp);
                    angle = doubleDataTemp;
                    standbyChar = knotDataArr[8];
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public string rgdn { get; set; }
        public string sp { get; set; }
        public string cy { get; set; }
        public string ph { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public double torque{ get; set; }
        public double angle{ get; set; }
        public double torqueRate{ get; set; }
        public string standbyChar { get; set; }

        public int Seq { get; set; }
        public string Torque { get; set; }
        public string TorqueAngle { get; set; }
    }
}
