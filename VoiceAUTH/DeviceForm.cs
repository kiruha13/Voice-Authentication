using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace VoiceAUTH
{
    public partial class DeviceForm : Form
    {

        private string receivedLogin;
        private bool receivedParam;

        public readonly MMDevice[] AudioDevices = new MMDeviceEnumerator()
            .EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active)
            .ToArray();

        public DeviceForm(string login, bool param)
        {
            InitializeComponent();
            receivedLogin = login;
            receivedParam = param;
            foreach (MMDevice device in AudioDevices)
            {
                string deviceType = device.DataFlow == DataFlow.Capture ? "INPUT" : "OUTPUT";
                string deviceLabel = $"{deviceType}: {device.FriendlyName}";
                listBox1.Items.Add(deviceLabel);
            }

            listBox1.SelectedIndex = 0;
        }

        private WasapiCapture GetSelectedDevice()
        {
            MMDevice selectedDevice = AudioDevices[listBox1.SelectedIndex];
            return selectedDevice.DataFlow == DataFlow.Render
                ? new WasapiLoopbackCapture(selectedDevice)
                : new WasapiCapture(selectedDevice, true, 10);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            WasapiCapture captureDevice = GetSelectedDevice();
            new MainForm(captureDevice, receivedLogin, receivedParam).ShowDialog();
        }
    }
}
