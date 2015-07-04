using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Json;
using System.Text;
using WuaRpi04.Annotations;

namespace WuaRpi04
{
    public sealed partial class MainPage : INotifyPropertyChanged
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void ButtonGetTemperature_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            string city = TextBoxCountry.Text;
            string country = TextBoxCity.Text;

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"http://api.openweathermap.org/data/2.5/weather?q={city},{country}");
                var client = new HttpClient();
                var response = client.SendAsync(request).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    var bytes = Encoding.Unicode.GetBytes(result);
                    using (var stream = new MemoryStream(bytes))
                    {
                        var serializer = new DataContractJsonSerializer(typeof(Weather));
                        var weather = (Weather)serializer.ReadObject(stream);
                        TextBoxResult.Text = $"Temperature: {(weather.main.temp - 273.15f):F2} °C for {country}, {city}";
                    }
                }
                else
                {
                    TextBoxResult.Text = "Error";
                }
            }
            catch (Exception exception)
            {
                StatusInformation3 = exception.ToString();
            }
        }


        #region Properties and Property Changed
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _statusInformation;
        public string StatusInformation
        {
            get { return _statusInformation; }
            set
            {
                if (value == _statusInformation) return;
                _statusInformation = value;
                OnPropertyChanged();
            }
        }

        private string _statusInformation2;
        public string StatusInformation2
        {
            get { return _statusInformation2; }
            set
            {
                if (value == _statusInformation2) return;
                _statusInformation2 = value;
                OnPropertyChanged();
            }
        }

        private string _statusInformation3;
        public string StatusInformation3
        {
            get { return _statusInformation3; }
            set
            {
                if (value == _statusInformation3) return;
                _statusInformation3 = value;
                OnPropertyChanged();
            }
        }
        #endregion
    }

    public class Temperature
    {
        public double temp { get; set; }
    }

    public class Weather
    {
        public Temperature main { get; set; }
    }
}
