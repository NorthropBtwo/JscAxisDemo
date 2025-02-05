namespace JscAxisDemoWinForms
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            txbIP = new TextBox();
            cmdConnect = new Button();
            txbSend = new TextBox();
            cmdSend = new Button();
            txbReceive = new TextBox();
            cmdDisconnect = new Button();
            cmdPwc = new Button();
            lblState = new Label();
            cmdPowerQuit = new Button();
            txbGoPosition = new TextBox();
            cmdGoPosition = new Button();
            lblPosition = new Label();
            timTellPosition = new System.Windows.Forms.Timer(components);
            cmdStopMotion = new Button();
            cmdGoPositionAsync = new Button();
            cmdReference = new Button();
            lblErrorString = new Label();
            txbCustomCommand = new TextBox();
            cmdCustomCommand = new Button();
            lblCustomCommand = new Label();
            SuspendLayout();
            // 
            // txbIP
            // 
            txbIP.Location = new Point(12, 12);
            txbIP.Name = "txbIP";
            txbIP.Size = new Size(139, 27);
            txbIP.TabIndex = 0;
            txbIP.Text = "192.168.0.66";
            // 
            // cmdConnect
            // 
            cmdConnect.Location = new Point(157, 10);
            cmdConnect.Name = "cmdConnect";
            cmdConnect.Size = new Size(130, 29);
            cmdConnect.TabIndex = 1;
            cmdConnect.Text = "Connect";
            cmdConnect.UseVisualStyleBackColor = true;
            cmdConnect.Click += cmdConnect_Click;
            // 
            // txbSend
            // 
            txbSend.Location = new Point(12, 45);
            txbSend.Name = "txbSend";
            txbSend.Size = new Size(139, 27);
            txbSend.TabIndex = 2;
            // 
            // cmdSend
            // 
            cmdSend.Location = new Point(157, 45);
            cmdSend.Name = "cmdSend";
            cmdSend.Size = new Size(130, 29);
            cmdSend.TabIndex = 3;
            cmdSend.Text = "Send";
            cmdSend.UseVisualStyleBackColor = true;
            cmdSend.Click += cmdSend_Click;
            // 
            // txbReceive
            // 
            txbReceive.Location = new Point(12, 95);
            txbReceive.Multiline = true;
            txbReceive.Name = "txbReceive";
            txbReceive.ScrollBars = ScrollBars.Vertical;
            txbReceive.Size = new Size(275, 373);
            txbReceive.TabIndex = 4;
            // 
            // cmdDisconnect
            // 
            cmdDisconnect.Location = new Point(293, 10);
            cmdDisconnect.Name = "cmdDisconnect";
            cmdDisconnect.Size = new Size(130, 29);
            cmdDisconnect.TabIndex = 5;
            cmdDisconnect.Text = "Disconnect";
            cmdDisconnect.UseVisualStyleBackColor = true;
            cmdDisconnect.Click += cmdDisconnect_Click;
            // 
            // cmdPwc
            // 
            cmdPwc.Location = new Point(293, 94);
            cmdPwc.Name = "cmdPwc";
            cmdPwc.Size = new Size(137, 29);
            cmdPwc.TabIndex = 6;
            cmdPwc.Text = "PWC";
            cmdPwc.UseVisualStyleBackColor = true;
            cmdPwc.Click += cmdPwc_Click;
            // 
            // lblState
            // 
            lblState.AutoSize = true;
            lblState.Font = new Font("Segoe UI", 16.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblState.Location = new Point(481, 9);
            lblState.Name = "lblState";
            lblState.Size = new Size(185, 38);
            lblState.TabIndex = 7;
            lblState.Text = "Disconnected";
            // 
            // cmdPowerQuit
            // 
            cmdPowerQuit.Location = new Point(436, 94);
            cmdPowerQuit.Name = "cmdPowerQuit";
            cmdPowerQuit.Size = new Size(137, 29);
            cmdPowerQuit.TabIndex = 8;
            cmdPowerQuit.Text = "PQ";
            cmdPowerQuit.UseVisualStyleBackColor = true;
            cmdPowerQuit.Click += cmdPowerQuit_Click;
            // 
            // txbGoPosition
            // 
            txbGoPosition.Location = new Point(294, 199);
            txbGoPosition.Name = "txbGoPosition";
            txbGoPosition.Size = new Size(129, 27);
            txbGoPosition.TabIndex = 9;
            txbGoPosition.Text = "0";
            txbGoPosition.TextAlign = HorizontalAlignment.Right;
            // 
            // cmdGoPosition
            // 
            cmdGoPosition.Location = new Point(436, 198);
            cmdGoPosition.Name = "cmdGoPosition";
            cmdGoPosition.Size = new Size(137, 29);
            cmdGoPosition.TabIndex = 10;
            cmdGoPosition.Text = "Go Position";
            cmdGoPosition.UseVisualStyleBackColor = true;
            cmdGoPosition.Click += cmdGoPosition_Click;
            // 
            // lblPosition
            // 
            lblPosition.AutoSize = true;
            lblPosition.Font = new Font("Segoe UI", 16.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblPosition.Location = new Point(293, 273);
            lblPosition.Name = "lblPosition";
            lblPosition.Size = new Size(129, 38);
            lblPosition.TabIndex = 11;
            lblPosition.Text = "Position: ";
            // 
            // timTellPosition
            // 
            timTellPosition.Enabled = true;
            timTellPosition.Interval = 2000;
            timTellPosition.Tick += timTellPosition_Tick;
            // 
            // cmdStopMotion
            // 
            cmdStopMotion.Location = new Point(293, 129);
            cmdStopMotion.Name = "cmdStopMotion";
            cmdStopMotion.Size = new Size(280, 29);
            cmdStopMotion.TabIndex = 12;
            cmdStopMotion.Text = "Stop Motion";
            cmdStopMotion.UseVisualStyleBackColor = true;
            cmdStopMotion.Click += cmdStopMotion_Click;
            // 
            // cmdGoPositionAsync
            // 
            cmdGoPositionAsync.Location = new Point(436, 233);
            cmdGoPositionAsync.Name = "cmdGoPositionAsync";
            cmdGoPositionAsync.Size = new Size(137, 29);
            cmdGoPositionAsync.TabIndex = 13;
            cmdGoPositionAsync.Text = "Go Position Async";
            cmdGoPositionAsync.UseVisualStyleBackColor = true;
            cmdGoPositionAsync.Click += cmdGoPositionAsync_Click;
            // 
            // cmdReference
            // 
            cmdReference.Location = new Point(294, 164);
            cmdReference.Name = "cmdReference";
            cmdReference.Size = new Size(280, 29);
            cmdReference.TabIndex = 14;
            cmdReference.Text = "Reference";
            cmdReference.UseVisualStyleBackColor = true;
            cmdReference.Click += cmdReference_Click;
            // 
            // lblErrorString
            // 
            lblErrorString.AutoSize = true;
            lblErrorString.Location = new Point(471, 54);
            lblErrorString.Name = "lblErrorString";
            lblErrorString.Size = new Size(50, 20);
            lblErrorString.TabIndex = 15;
            lblErrorString.Text = "label1";
            // 
            // txbCustomCommand
            // 
            txbCustomCommand.Location = new Point(293, 323);
            txbCustomCommand.Name = "txbCustomCommand";
            txbCustomCommand.Size = new Size(129, 27);
            txbCustomCommand.TabIndex = 16;
            txbCustomCommand.Text = "VER";
            txbCustomCommand.TextAlign = HorizontalAlignment.Right;
            // 
            // cmdCustomCommand
            // 
            cmdCustomCommand.Location = new Point(436, 322);
            cmdCustomCommand.Name = "cmdCustomCommand";
            cmdCustomCommand.Size = new Size(137, 29);
            cmdCustomCommand.TabIndex = 17;
            cmdCustomCommand.Text = "Send Command";
            cmdCustomCommand.UseVisualStyleBackColor = true;
            cmdCustomCommand.Click += cmdCustomCommand_Click;
            // 
            // lblCustomCommand
            // 
            lblCustomCommand.AutoSize = true;
            lblCustomCommand.Location = new Point(293, 353);
            lblCustomCommand.Name = "lblCustomCommand";
            lblCustomCommand.Size = new Size(50, 20);
            lblCustomCommand.TabIndex = 18;
            lblCustomCommand.Text = "label1";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 480);
            Controls.Add(lblCustomCommand);
            Controls.Add(cmdCustomCommand);
            Controls.Add(txbCustomCommand);
            Controls.Add(lblErrorString);
            Controls.Add(cmdReference);
            Controls.Add(cmdGoPositionAsync);
            Controls.Add(cmdStopMotion);
            Controls.Add(lblPosition);
            Controls.Add(cmdGoPosition);
            Controls.Add(txbGoPosition);
            Controls.Add(cmdPowerQuit);
            Controls.Add(lblState);
            Controls.Add(cmdPwc);
            Controls.Add(cmdDisconnect);
            Controls.Add(txbReceive);
            Controls.Add(cmdSend);
            Controls.Add(txbSend);
            Controls.Add(cmdConnect);
            Controls.Add(txbIP);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox txbIP;
        private Button cmdConnect;
        private TextBox txbSend;
        private Button cmdSend;
        private TextBox txbReceive;
        private Button cmdDisconnect;
        private Button cmdPwc;
        private Label lblState;
        private Button cmdPowerQuit;
        private TextBox txbGoPosition;
        private Button cmdGoPosition;
        private Label lblPosition;
        private System.Windows.Forms.Timer timTellPosition;
        private Button cmdStopMotion;
        private Button cmdGoPositionAsync;
        private Button cmdReference;
        private Label lblErrorString;
        private TextBox txbCustomCommand;
        private Button cmdCustomCommand;
        private Label lblCustomCommand;
    }
}
