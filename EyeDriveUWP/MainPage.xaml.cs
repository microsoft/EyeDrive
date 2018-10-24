using EyeDrive;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace EyeDriveUWP
{
    public sealed partial class MainPage : Page
    {
        private MediaCapture mediaCapture;
        private DeviceInformation deviceInformation;
        private CaptureElement captureElement;

        private IDrive.IDrive driveInterface = new NullDrive();

        public MainPage()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await InitializeCameraAsync();
            base.OnNavigatedTo(e);
        }

        private async void Application_Resuming(object sender, object o)
        {
            await InitializeCameraAsync();
        }

        private async Task InitializeCameraAsync()
        {
            if (mediaCapture == null)
            {
                DeviceInformationCollection cameraDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
                deviceInformation = cameraDevices.First();

                mediaCapture = new MediaCapture();
                await mediaCapture.InitializeAsync(new MediaCaptureInitializationSettings { VideoDeviceId = deviceInformation.Id });
                VideoFeed.Source = mediaCapture;
                captureElement = VideoFeed;
                await captureElement.Source.StartPreviewAsync();
            }
        }

        private void ForwardLeftButton_Click(object sender, RoutedEventArgs e)
        {
            driveInterface.ForwardLeft();
        }

        private void ForwardRightButton_Click(object sender, RoutedEventArgs e)
        {
            driveInterface.ForwardRight();
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            driveInterface.Forward();
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            driveInterface.Left();
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            driveInterface.Right();
        }

        private void ReverseButton_Click(object sender, RoutedEventArgs e)
        {
            driveInterface.Reverse();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            driveInterface.Stop();
        }

        private void EyeGazeOnButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
