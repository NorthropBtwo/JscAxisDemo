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
                axis.OnStateChanged += StateChaned;
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

        private void StateChaned(JScAxis.State state)
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new Action(() => { StateChaned(state); }));
            }
            else
            {
                lblState.Text = "State: " + state.ToString();
            }
        }

        private void cmdDisconnect_Click(object sender, EventArgs e)
        {
            if (axis != null)
            {
                axis.OnReceive -= Received;
                axis.OnStateChanged -= StateChaned;
                axis?.Disconnect();
                axis = null;
                UpdateConnectionButtons();
            }
        }

        private void cmdPwc_Click(object sender, EventArgs e)
        {
            axis?.PowerContinuous();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateConnectionButtons();
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
                await(axis?.GoPositionAsync(position) ?? Task.CompletedTask);
                cmdGoPosition.Enabled = true;
                cmdGoPositionAsync.Enabled = true;
            }
        }
    }
}
