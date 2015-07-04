using System.Linq;
using System.Windows;
using Microsoft.Kinect;

namespace ElBruno.Rocket.Ui
{
    public partial class MainWindow
    {
        private KinectSensor _sensor;
        private Rocket _rocket;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindowLoaded;
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            InitRocket();
            InitKinectSensor();
        }

        private void InitRocket()
        {
            _rocket = new Rocket(@"vid_0a81", @"pid_ff01");
            _rocket.Connect();
        }

        private void InitKinectSensor()
        {
            // validate
            if (KinectSensor.KinectSensors.Count == 0) return;

            // init Kinect
            var parameters = new TransformSmoothParameters
                                 {
                                     Smoothing = 0.75f,
                                     Correction = 0.1f,
                                     Prediction = 0.0f,
                                     JitterRadius = 0.05f,
                                     MaxDeviationRadius = 0.08f
                                 };

            _sensor = KinectSensor.KinectSensors[0];
            _sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            _sensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
            _sensor.SkeletonStream.Enable(parameters);
            _sensor.Start();
            colorViewer.Kinect = _sensor;
            skeletonViewer.Kinect = _sensor;
            _sensor.SkeletonFrameReady += SensorSkeletonFrameReady;
        }

        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = null;
            using (var frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    skeletons = new Skeleton[frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(skeletons);
                }
            }

            if (skeletons == null) return;

            foreach (var kinectRocketGestures in (
                from skeleton in skeletons
                where skeleton.TrackingState == SkeletonTrackingState.Tracked
                let headJoint = skeleton.Joints[JointType.Head]
                where headJoint.TrackingState != JointTrackingState.NotTracked
                select skeleton).Select(skeleton => new KinectRocketGestures(skeleton, _rocket)))
            {
                var gesture = kinectRocketGestures.ValidateGestures();
                textBlockInformation.Text = gesture;
            }
        }
    }
}
