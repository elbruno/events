using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Windows.Forms;

namespace ElBruno.Arduino.LieDetector
{
    public partial class Form1 : Form
    {
        private SerialPort _serialPort;
        private string _dataReceived;
        private bool _connected;
        private const int NumberOfPointsInChart = 500;
        private const int NumberOfPointsAfterRemoval = 480;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            toolStripTextBoxArduinoComPort.Text = Properties.Settings.Default.ArduinoComPortName;
        }

        private void DrawChart(int threshold, int sensorValue, int temp)
        {
            // Simulate adding new data points
            chartLieDetector.Series["threshold"].Points.AddY(threshold);
            chartLieDetector.Series["sensorValue"].Points.AddY(sensorValue);
            chartLieDetector.Series["temp"].Points.AddY(temp);

            // Adjust Y & X axis scale
            chartLieDetector.ResetAutoValues();

            // Keep a constant number of points by removing them from the left
            while (chartLieDetector.Series["threshold"].Points.Count > NumberOfPointsInChart)
            {
                // Remove data points on the left side
                while (chartLieDetector.Series["threshold"].Points.Count > NumberOfPointsAfterRemoval)
                {
                    chartLieDetector.Series["threshold"].Points.RemoveAt(0);
                    chartLieDetector.Series["sensorValue"].Points.RemoveAt(0);
                    chartLieDetector.Series["temp"].Points.RemoveAt(0);
                }
            }

            // Invalidate chart
            chartLieDetector.Invalidate();

            toolStripStatusLabel1.Text = string.Format("threshold={0} - value={1} - dif={2}", threshold, sensorValue, temp);
        }

        public Boolean Connected
        {
            get { return _connected; }
            set
            {
                _connected = value;
                if (_connected)
                {
                    toolStripStatusLabelArduinoConnection.Text = @"Connected" + Properties.Settings.Default.ArduinoComPortName;
                }
                else
                {
                    toolStripStatusLabelArduinoConnection.Text = @"Disconnected" + Properties.Settings.Default.ArduinoComPortName;
                }

            }
        }


        void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                _dataReceived = _serialPort.ReadLine();
                if (!string.IsNullOrWhiteSpace(_dataReceived))
                {
                    ProcessSensorsData(_dataReceived);
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.ToString());
            }
        }

        private delegate void delegateProcessSensorsData(string dataReceived);

        private void ProcessSensorsData(string dataReceived)
        {
            if (chartLieDetector.InvokeRequired)
            {
                try
                {
                    chartLieDetector.Invoke(new delegateProcessSensorsData(ProcessSensorsData), _dataReceived);
                }
                catch (Exception ex)
                {
                    Debug.Write(ex.ToString());
                }
            }
            else
            {
                dataReceived = dataReceived.Trim();
                var sensors = dataReceived.Split(',');
                if (sensors.Length <= 1) return;
                int threshold;
                int sensorValue;
                int.TryParse(sensors[0], out threshold);
                int.TryParse(sensors[1], out sensorValue);
                var temp = threshold - sensorValue;
                DrawChart(threshold, sensorValue, temp);
            }
        }

        private void toolStripMenuItemConnectToArduino_Click(object sender, EventArgs e)
        {
            if (!Connected)
            {
                _serialPort = new SerialPort(Properties.Settings.Default.ArduinoComPortName, 9600);
                _serialPort.Open();
                _serialPort.DataReceived += _serialPort_DataReceived;
                Connected = true;
                toolStripMenuItemConnectToArduino.Text = @"Disconnect from Arduino";
            }
            else
            {
                toolStripMenuItemConnectToArduino.Text = "Connect to Arduino";
                _serialPort.Close();
                _serialPort.DataReceived -= _serialPort_DataReceived;
                Connected = false;
            }
        }
    }
}
