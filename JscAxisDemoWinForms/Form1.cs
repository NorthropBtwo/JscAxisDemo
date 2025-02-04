using System.Net;

namespace JscAxisDemoWinForms
{
    public partial class Form1 : Form
    {


        JScAxis? axis;

        public Form1()
        {
            InitializeComponent();
        }

        private void cmdConnect_Click(object sender, EventArgs e)
        {
            if (axis == null)
            {
                JScAxis axis = new JScAxis();
                axis.OnReceive += Received;
                axis.OnStateChanged += StateChanged;
                axis.Connect(IPAddress.Parse(txbIP.Text));
                this.axis = axis;
                UpdateConnectionButtons();
            }
        }

        private void UpdateConnectionButtons()
        {
            cmdConnect.Enabled = (axis == null);
            cmdDisconnect.Enabled = (axis != null);
        }

        private void cmdSend_Click(object sender, EventArgs e)
        {
            axis?.Send(txbSend.Text);
        }

        private void Received(string message)
        {
            if (message.StartsWith("TP"))
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
            }
            else
            {
                lblState.Text = "State: " + state.ToString();
                if (state == JScAxis.State.ERROR)
                {
                    lblErrorString.Text = await Task.Run(() => axis?.TellErrorString());
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
                UpdateConnectionButtons();
                lblState.Text = "Disconnected";
            }
        }

        private void cmdPwc_Click(object sender, EventArgs e)
        {
            axis?.PowerContinuous();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateConnectionButtons();
            lblErrorString.Text = "";
        }

        private void cmdPowerQuit_Click(object sender, EventArgs e)
        {
            axis?.PowerQuit();
        }

        private void timTellPosition_Tick(object sender, EventArgs e)
        {
            lblPosition.Text = "Position: " + axis?.TellPosition();
        }

        private void cmdStopMotion_Click(object sender, EventArgs e)
        {
            axis?.StopMoition();
        }

        private async void cmdGoPosition_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txbGoPosition.Text, out int position))
            {
                cmdGoPosition.Enabled = false;
                cmdGoPositionAsync.Enabled = false;
                await Task.Run(() => axis?.GoPosition(position));
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
                await (axis?.GoPositionAsync(position) ?? Task.CompletedTask);
                cmdGoPosition.Enabled = true;
                cmdGoPositionAsync.Enabled = true;
            }
        }

        private async void cmdReference_Click(object sender, EventArgs e)
        {
            cmdReference.Enabled = false;
            try
            {
                await Task.Run(() => axis?.Reference());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during Reference: {ex.Message}");
            }
            cmdReference.Enabled = true;
        }

        private void cmdCustomCommand_Click(object sender, EventArgs e)
        {
            lblCustomCommand.Text = axis?.SendCommand(txbCustomCommand.Text);
        }
    }
}
