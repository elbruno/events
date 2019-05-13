using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace ChicagoCodeCampMarvelUwpApp01
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            //InitModel();
            await CameraPreview.StartAsync();
            CameraPreview.CameraHelper.FrameArrived += CameraHelper_FrameArrived;
        }

        private async void CameraHelper_FrameArrived(object sender, Microsoft.Toolkit.Uwp.Helpers.FrameEventArgs e)
        {
            // demo code goes here
        }

    }
}
