namespace SerialPortConnector
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnConnect = new System.Windows.Forms.Button();
            this.cmbBaudeRate = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtSend = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.rdHex = new System.Windows.Forms.RadioButton();
            this.rdText = new System.Windows.Forms.RadioButton();
            this.cmbStopBits = new System.Windows.Forms.ComboBox();
            this.cmbDataBits = new System.Windows.Forms.ComboBox();
            this.cmbParity = new System.Windows.Forms.ComboBox();
            this.cmbPortName = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rttbDebug = new System.Windows.Forms.RichTextBox();
            this.rtxtDataArea = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(502, 5);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 1;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // cmbBaudeRate
            // 
            this.cmbBaudeRate.FormattingEnabled = true;
            this.cmbBaudeRate.Items.AddRange(new object[] {
            "9600",
            "19200",
            "38400",
            "57600",
            "115200"});
            this.cmbBaudeRate.Location = new System.Drawing.Point(139, 5);
            this.cmbBaudeRate.Name = "cmbBaudeRate";
            this.cmbBaudeRate.Size = new System.Drawing.Size(79, 21);
            this.cmbBaudeRate.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.rdHex);
            this.panel1.Controls.Add(this.rdText);
            this.panel1.Controls.Add(this.cmbStopBits);
            this.panel1.Controls.Add(this.cmbDataBits);
            this.panel1.Controls.Add(this.cmbParity);
            this.panel1.Controls.Add(this.cmbPortName);
            this.panel1.Controls.Add(this.cmbBaudeRate);
            this.panel1.Controls.Add(this.btnConnect);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1044, 94);
            this.panel1.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtSend);
            this.groupBox1.Controls.Add(this.btnSend);
            this.groupBox1.Location = new System.Drawing.Point(64, 32);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(401, 54);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Data";
            // 
            // txtSend
            // 
            this.txtSend.Location = new System.Drawing.Point(6, 19);
            this.txtSend.Name = "txtSend";
            this.txtSend.Size = new System.Drawing.Size(289, 20);
            this.txtSend.TabIndex = 5;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(301, 17);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 4;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // rdHex
            // 
            this.rdHex.AutoSize = true;
            this.rdHex.Location = new System.Drawing.Point(12, 69);
            this.rdHex.Name = "rdHex";
            this.rdHex.Size = new System.Drawing.Size(44, 17);
            this.rdHex.TabIndex = 9;
            this.rdHex.TabStop = true;
            this.rdHex.Text = "Hex";
            this.rdHex.UseVisualStyleBackColor = true;
            // 
            // rdText
            // 
            this.rdText.AutoSize = true;
            this.rdText.Location = new System.Drawing.Point(10, 46);
            this.rdText.Name = "rdText";
            this.rdText.Size = new System.Drawing.Size(46, 17);
            this.rdText.TabIndex = 8;
            this.rdText.TabStop = true;
            this.rdText.Text = "Text";
            this.rdText.UseVisualStyleBackColor = true;
            // 
            // cmbStopBits
            // 
            this.cmbStopBits.FormattingEnabled = true;
            this.cmbStopBits.Items.AddRange(new object[] {
            "1"});
            this.cmbStopBits.Location = new System.Drawing.Point(405, 5);
            this.cmbStopBits.Name = "cmbStopBits";
            this.cmbStopBits.Size = new System.Drawing.Size(91, 21);
            this.cmbStopBits.TabIndex = 7;
            // 
            // cmbDataBits
            // 
            this.cmbDataBits.FormattingEnabled = true;
            this.cmbDataBits.Items.AddRange(new object[] {
            "5",
            "6",
            "7",
            "8"});
            this.cmbDataBits.Location = new System.Drawing.Point(224, 5);
            this.cmbDataBits.Name = "cmbDataBits";
            this.cmbDataBits.Size = new System.Drawing.Size(48, 21);
            this.cmbDataBits.TabIndex = 6;
            // 
            // cmbParity
            // 
            this.cmbParity.FormattingEnabled = true;
            this.cmbParity.Items.AddRange(new object[] {
            "None",
            "Odd",
            "Even",
            "Mark",
            "Space"});
            this.cmbParity.Location = new System.Drawing.Point(278, 5);
            this.cmbParity.Name = "cmbParity";
            this.cmbParity.Size = new System.Drawing.Size(121, 21);
            this.cmbParity.TabIndex = 5;
            // 
            // cmbPortName
            // 
            this.cmbPortName.FormattingEnabled = true;
            this.cmbPortName.Location = new System.Drawing.Point(12, 5);
            this.cmbPortName.Name = "cmbPortName";
            this.cmbPortName.Size = new System.Drawing.Size(121, 21);
            this.cmbPortName.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rttbDebug);
            this.panel2.Controls.Add(this.rtxtDataArea);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 94);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1044, 356);
            this.panel2.TabIndex = 4;
            // 
            // rttbDebug
            // 
            this.rttbDebug.Location = new System.Drawing.Point(638, 3);
            this.rttbDebug.Name = "rttbDebug";
            this.rttbDebug.Size = new System.Drawing.Size(403, 350);
            this.rttbDebug.TabIndex = 1;
            this.rttbDebug.Text = "";
            // 
            // rtxtDataArea
            // 
            this.rtxtDataArea.Location = new System.Drawing.Point(0, 0);
            this.rtxtDataArea.Name = "rtxtDataArea";
            this.rtxtDataArea.Size = new System.Drawing.Size(632, 356);
            this.rtxtDataArea.TabIndex = 0;
            this.rtxtDataArea.Text = "";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(485, 36);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 11;
            this.button1.Text = "Connect";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1044, 450);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Serialport";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.ComboBox cmbBaudeRate;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox cmbPortName;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.ComboBox cmbParity;
        private System.Windows.Forms.ComboBox cmbStopBits;
        private System.Windows.Forms.ComboBox cmbDataBits;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton rdHex;
        private System.Windows.Forms.RadioButton rdText;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtSend;
        private System.Windows.Forms.RichTextBox rtxtDataArea;
        private System.Windows.Forms.RichTextBox rttbDebug;
        private System.Windows.Forms.Button button1;
    }
}

