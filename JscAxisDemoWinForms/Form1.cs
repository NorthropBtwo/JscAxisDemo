using System.Net;
using JscAxisLib;

namespace JscAxisDemoWinForms
{
    public partial class Form1 : Form
    {

        JScAxis? axis;
        CancellationTokenSource ctsDisconnected = new CancellationTokenSource();

        public Form1()
        {
            InitializeComponent();
        }

        private void cmdConnect_Click(object sender, EventArgs e)
        {
            if (axis == null)
            {
                JScAxis axis = new JScAxis();
                ctsDisconnected = new CancellationTokenSource();
                axis.OnReceive += Received;
                axis.OnStateChanged += StateChanged;
                axis.Connect(IPAddress.Parse(txbIP.Text));
                this.axis = axis;
                UpdateButtonEnableStates();
            }
        }

        private void UpdateButtonEnableStates()
        {
            bool connected = axis != null;
            cmdConnect.Enabled = !connected;
            cmdDisconnect.Enabled = connected;
            cmdSend.Enabled = connected;
            cmdPwc.Enabled = connected;
            cmdPowerQuit.Enabled = connected;
            cmdStopMotion.Enabled = connected;
            cmdGoPosition.Enabled = connected;
            cmdGoPositionAsync.Enabled = connected;
            cmdReference.Enabled = connected;
            cmdCustomCommand.Enabled = connected;
            cmdForceCalibration.Enabled = connected;
            cmdClearForceCalibration.Enabled = connected;
            cmdLimitIForce.Enabled = connected;
        }

        private void cmdSend_Click(object sender, EventArgs e)
        {
            try
            {
                axis?.Send(txbSend.Text);
            }
            catch (OperationCanceledException) { }
        }

        private void Received(string message)
        {
            if (message.StartsWith("TP")) // ignore TellPosition messages as thex would flood the textbox
            {
                return;
            }

            if (this.InvokeRequired)
            {
                BeginInvoke(new Action(() => { Received(message); }));
            }
            else
            {
                txbReceive.AppendText(Environment.NewLine + "{" + message + "}");
            }
        }

        private async void StateChanged(JScAxis.State state)
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new Action(() => { StateChanged(state); }));
                if (state == JScAxis.State.DISCONNECTED)
                {
                    ctsDisconnected.Cancel();
                }
            }
            else
            {
                lblState.Text = "State: " + state.ToString();
                if (state == JScAxis.State.ERROR)
                {
                    try
                    {
                        lblErrorString.Text = await Task.Run(() => axis?.TellErrorString(ctsDisconnected.Token));
                    }
                    catch (OperationCanceledException) { }
                }
                else if (state == JScAxis.State.DISCONNECTED) //connection was lost. This will not be called if the user clicks disconnect because this function is removed from the event before disconnecting
                {
                    if (axis != null)
                    {
                        axis.OnReceive -= Received;
                        axis.OnStateChanged -= StateChanged;
                        axis?.Disconnect();
                        axis = null;
                    }
                    lblErrorString.Text = "Connection lost";
                    UpdateButtonEnableStates();
                }
                else
                {
                    lblErrorString.Text = "";
                }
            }
        }

        private void cmdDisconnect_Click(object sender, EventArgs e)
        {
            if (axis != null)
            {
                axis.OnReceive -= Received;
                axis.OnStateChanged -= StateChanged;
                axis?.Disconnect();
                axis = null;
                UpdateButtonEnableStates();
                lblState.Text = "Disconnected";
            }
        }

        private void cmdPwc_Click(object sender, EventArgs e)
        {
            try
            {
                axis?.PowerContinuous(ctsDisconnected.Token);
            }
            catch (OperationCanceledException) { }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateButtonEnableStates();
            lblErrorString.Text = "";
            lblCustomCommand.Text = "";
        }

        private void cmdPowerQuit_Click(object sender, EventArgs e)
        {
            try
            {
                axis?.PowerQuit(ctsDisconnected.Token);
            }
            catch (OperationCanceledException) { }
        }

        private void timTellPosition_Tick(object sender, EventArgs e)
        {
            try
            {
                lblPosition.Text = "Position: " + axis?.TellPosition(ctsDisconnected.Token);
            }
            catch (OperationCanceledException) { }
        }

        private void cmdStopMotion_Click(object sender, EventArgs e)
        {
            try
            {
                axis?.StopMoition(ctsDisconnected.Token);
            }
            catch (OperationCanceledException) { }
        }

        private async void cmdGoPosition_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txbGoPosition.Text, out int position))
            {
                cmdGoPosition.Enabled = false;
                cmdGoPositionAsync.Enabled = false;
                try
                {
                    await Task.Run(() => axis?.GoPosition(position, ctsDisconnected.Token));
                }
                catch (OperationCanceledException) { }
                cmdGoPosition.Enabled = true;
                cmdGoPositionAsync.Enabled = true;
            }
        }

        private async void cmdGoPositionAsync_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txbGoPosition.Text, out int position))
            {
                cmdGoPosition.Enabled = false;
                cmdGoPositionAsync.Enabled = false;
                try
                {
                    await (axis?.GoPositionAsync(position, ctsDisconnected.Token) ?? Task.CompletedTask);
                }
                catch (OperationCanceledException) { }
                cmdGoPosition.Enabled = true;
                cmdGoPositionAsync.Enabled = true;
            }
        }

        private async void cmdReference_Click(object sender, EventArgs e)
        {
            cmdReference.Enabled = false;
            try
            {
                await Task.Run(() => axis?.Reference(ctsDisconnected.Token));
            }
            catch (OperationCanceledException ex)
            {
                MessageBox.Show($"Error during Reference: {ex.Message}");
            }
            cmdReference.Enabled = true;
        }

        private void cmdCustomCommand_Click(object sender, EventArgs e)
        {
            try
            {
                lblCustomCommand.Text = axis?.SendCommand(txbCustomCommand.Text, ctsDisconnected.Token);
            }
            catch (OperationCanceledException) { }
        }

        private async void cmdForceCalibration_Click(object sender, EventArgs e)
        {
            cmdForceCalibration.Enabled = false;
            if (int.TryParse(txbForceCalibrationFrom.Text, out int startPosition) && int.TryParse(txbForceCalibrationTo.Text, out int endPosition))
            {
                try
                {
                    await Task.Run(() => axis?.ForceCalibration(startPosition, endPosition, ctsDisconnected.Token));
                }
                catch (OperationCanceledException) { }
            }
            else
            {
                MessageBox.Show("Invalid input");
            }
            cmdForceCalibration.Enabled = true;
        }

        private async void cmdClearForceCalibration_Click(object sender, EventArgs e)
        {
            cmdClearForceCalibration.Enabled = false;
            try
            {
                await Task.Run(() => axis?.ForceCalibration(0, 0, ctsDisconnected.Token));
            }
            catch (OperationCanceledException) { }
            cmdClearForceCalibration.Enabled = true;
        }

        private void cmdLimitIForce_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txbLimitIForce.Text, out int limitIForceValue))
            {
                try
                {
                    axis?.SendCommand("LIF" + limitIForceValue, ctsDisconnected.Token);
                }
                catch (OperationCanceledException) { }
            }
            else
            {
                MessageBox.Show("Invalid input");
            }
        }
    }
}
