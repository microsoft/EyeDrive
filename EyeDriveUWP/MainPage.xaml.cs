using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace EyeDriveUWP
{
    public sealed partial class MainPage : Page
    {
        private IDrive.IDrive driveInterface = new NullDrive();

        public MainPage()
        {
            InitializeComponent();
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
