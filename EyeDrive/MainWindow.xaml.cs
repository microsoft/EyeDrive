using Microsoft.HandsFree.GazePointer;
using Microsoft.HandsFree.Win32;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Threading;
using WPFMediaKit.DirectShow.Controls;

namespace EyeDrive
{
    public partial class MainWindow
    {
        const int WM_SYSCOMMAND = 0x0112;
        const int SC_MONITORPOWER = 0xF170;
        const int SC_SCREENSAVE = 0xF140;

        readonly TraceSource _trace = new TraceSource("Main", SourceLevels.Information);
        IDrive.IDrive _driveInterface = new Object() as IDrive.IDrive;
        private GazePointer _egm;

        private readonly DispatcherTimer _eyesOffStopTimer;
        private DispatcherTimer _gazeToolbarVisiblityTimer;
        private readonly GazeClickParameters _quickClickParams;
        private readonly GazeClickParameters _delayedClickParams;

        private readonly Stopwatch _windowTimerStopwatch = new Stopwatch();

        private VideoCaptureElement _frontView;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
            Activated += MainWindow_Activated;
            Deactivated += MainWindow_Deactivated;

            SystemEvents.SessionSwitch += (s, e) =>
            {
                _trace.TraceEvent(TraceEventType.Information, 0, "Session switching occuring");
                MainWindow_Deactivated(s, e);
            };

            _eyesOffStopTimer = new DispatcherTimer(
                TimeSpan.FromMilliseconds(250),
                DispatcherPriority.Normal,
                (sender, args) =>
                {
                    _trace.TraceEvent(TraceEventType.Information, 0, "Eyes Off, stopped");
                    Stop();
                    _eyesOffStopTimer.Stop();
                },
                Dispatcher);

            _quickClickParams = new GazeClickParameters
            {
                MouseDownDelay = 99,
                MouseUpDelay = 100,
                RepeatMouseDownDelay = uint.MaxValue
            };

            _delayedClickParams = new GazeClickParameters
            {
                MouseDownDelay = 100,
                MouseUpDelay = 1000,
                RepeatMouseDownDelay = uint.MaxValue
            };

            VisualStateManager.GoToElementState(Canvas, DriveStates.Stopped.ToString(), true);
        }

        #region Event Handlers

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var source = (HwndSource)PresentationSource.FromVisual(this);
            source.AddHook(WndProc);
        }

        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Handle messages...
            switch (msg)
            {
                case WM_SYSCOMMAND:
                    switch (wParam.ToInt32())
                    {
                        case SC_MONITORPOWER:
                            if (lParam.ToInt32() == 2)
                            {
                                _trace.TraceEvent(TraceEventType.Information, 0, "Monitor turning off");
                                MainWindow_Deactivated(this, EventArgs.Empty);
                            }
                            break;

                        case SC_SCREENSAVE:
                            _trace.TraceEvent(TraceEventType.Information, 0, "Screen saver coming up");
                            MainWindow_Deactivated(this, EventArgs.Empty);
                            break;
                    }
                    break;
            }

            return IntPtr.Zero;
        }

        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
            _trace.TraceEvent(TraceEventType.Information, 0, "MainWindow Deactivated");
            Stop();
            MainWindowActive = false;
        }

        private void MainWindow_Activated(object sender, EventArgs e)
        {
            _trace.TraceEvent(TraceEventType.Information, 0, "MainWindow Activated");
            MainWindowActive = true;
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            _driveInterface?.Dispose();

            _windowTimerStopwatch?.Stop();

            _gazeToolbarVisiblityTimer?.Stop();

            _eyesOffStopTimer?.Stop();

            GazePointer.DetachAll();

            Application.Current.Shutdown();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _egm = GazePointer.Attach(this, null, GetGazeClickParameters);

            EyesOffStopsDrivingEnabled = true;

            _egm.EyesOn += (s, ea) =>
            {
                _trace.TraceInformation("Eyes On again");
                _eyesOffStopTimer.Stop();
            };

            IsEnabled = false;

            Dispatcher.BeginInvoke((Action)(() =>
            {
                //_driveInterface = new ChairDuino.ChairDuino();
                //_driveInterface = new StealthDrive.Stealth();
                _driveInterface = new NullDrive();

                //ChairduinoDisconnected = false;

                _driveInterface.ConnectionOpened += (o, e1) =>
                {
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        ChairduinoDisconnected = false;
                    }));
                };

                _driveInterface.ConnectionClosed += (o, e2) =>
                {
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        ChairduinoDisconnected = true;
                    }));
                };

                _driveInterface.CommunicationsError += (o, ce) =>
                {
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        ChairduinoDisconnected = true;
                        //MessageBox.Show(ce.Message);
                    }));
                };

                _driveInterface.Initialize();

                IsEnabled = true;

                DrivingButtonsEnabled = true;

            }));

            _windowTimerStopwatch.Start();

            // Set the maximum number of cameras in the settings dialog
            var count = MultimediaUtil.VideoInputDevices.Length;

            // Start the camera feeds
            StartAllCameras();

            // Add watcher for GazeToolBar Visibility
            AddGazeToolbarVisiblityCheck();
        }
        #endregion

        #region Gaze Toolbar Visibility Check
        private IntPtr _gazeTaskbarRestoreWindow;

        private void AddGazeToolbarVisiblityCheck()
        {
            _gazeToolbarVisiblityTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 250),
                IsEnabled = true
            };
            _gazeToolbarVisiblityTimer.Tick += GazeToolbarVisiblityTimerOnTick;
            _gazeToolbarVisiblityTimer.Start();
        }

        private void GazeToolbarVisiblityTimerOnTick(object sender, EventArgs eventArgs)
        {
            if (_gazeTaskbarRestoreWindow == IntPtr.Zero)
            {
                _gazeTaskbarRestoreWindow = User32.FindWindowByCaption(IntPtr.Zero, "TaskbarWindow");
            }

            if (_gazeTaskbarRestoreWindow != IntPtr.Zero)
            {
                var style = User32.GetWindowLong(_gazeTaskbarRestoreWindow, User32.GWL_STYLE);

                if (style != 0)
                {
                    GazeToolbarVisible = (style & User32.WS_VISIBLE) == User32.WS_VISIBLE;
                }
                else // error state, most likely due to window not existing
                {
                    _gazeTaskbarRestoreWindow = IntPtr.Zero;
                }
            }
            else
            {
                GazeToolbarVisible = false;
            }
        }

        #endregion

        private bool GazeToolbarVisible;

        private bool ChairduinoDisconnected = true;

        public bool MainWindowActive { get; set; }

        public bool DrivingEnabled => !ChairduinoDisconnected && !GazeToolbarVisible && MainWindowActive;

        private bool _eyesOffStopsDriving;

        private bool EyesOffStopsDrivingEnabled
        {
            set
            {
                //Prevent multiple subscriptions to the EyesOff event
                if (_eyesOffStopsDriving != value)
                {
                    _eyesOffStopsDriving = value;
                    if (value)
                    {
                        _egm.EyesOff += EyesOffForceStop;
                        _egm.EyesOff -= EyesOffAllowed;

                    }
                    else
                    {
                        _eyesOffStopsDriving = value;
                        _egm.EyesOff -= EyesOffForceStop;
                        _egm.EyesOff += EyesOffAllowed;
                    }
                }
            }
        }

        private bool DrivingButtonsEnabled
        {
            set
            {
                ForwardButton.IsEnabled = value;
                RightButton.IsEnabled = value;
                LeftButton.IsEnabled = value;
                ReverseButton.IsEnabled = value;
                ForwardLeftButton.IsEnabled = value;
                ForwardRightButton.IsEnabled = value;
            }
        }

        public void EyesOffForceStop(object s, EventArgs ea)
        {
            if (!_eyesOffStopTimer.IsEnabled)
            {
                _trace.TraceEvent(TraceEventType.Information, 0, "Eyes Off, stopping");
                _eyesOffStopTimer.Start();
            }
        }

        public void EyesOffAllowed(object s, EventArgs ea)
        {
            _trace.TraceEvent(TraceEventType.Information, 0, "Eyes Off, no response");
        }

        private GazeClickParameters GetGazeClickParameters(FrameworkElement element)
        {
            if (element == EyeGazeOnButton ||
                element == SettingsButton)
            {
                return _delayedClickParams;
            }

            return _quickClickParams;
        }

        enum DriveStates
        {
            Stopped,
            Forward,
            ForwardLeft,
            ForwardRight,
            Reverse,
            ReverseLeft,
            ReverseRight,
            Right,
            LeftLeft
        }

        DriveStates _driveState = DriveStates.Stopped;

        private void SetState(DriveStates state, Func<bool> func, string traceMessage)
        {
            if (_driveState != state && DrivingEnabled &&
                (state == DriveStates.Stopped || GazePointer.IsHandsFreeInvoked))
            {
                _driveState = state;

                if (func())
                {
                    _trace.TraceInformation(traceMessage);
                }

                VisualStateManager.GoToElementState(Canvas, state.ToString(), true);
            }
        }

        private void Stop()
        {
            if (_driveInterface != null)
            {
                SetState(DriveStates.Stopped, _driveInterface.Stop, "Stop");
            }
        }

        private void Forward_Click(object sender, EventArgs e)
        {
            SetState(DriveStates.Forward, _driveInterface.Forward, "Forward");
        }

        private void ForwardLeft_Click(object sender, EventArgs e)
        {
            SetState(DriveStates.ForwardLeft, _driveInterface.ForwardLeft, "Forward Left");
        }

        private void ForwardRight_Click(object sender, EventArgs e)
        {
            SetState(DriveStates.ForwardRight, _driveInterface.ForwardRight, "Forward Right");
        }

        private void ReverseLeft_Click(object sender, EventArgs e)
        {
            SetState(DriveStates.ReverseLeft, _driveInterface.ReverseLeft, "Reverse Left");
        }

        private void ReverseRight_Click(object sender, EventArgs e)
        {
            SetState(DriveStates.ReverseRight, _driveInterface.ReverseRight, "Reverse Right");
        }

        private void Reverse_Click(object sender, EventArgs e)
        {
            SetState(DriveStates.Reverse, _driveInterface.Reverse, "Reverse");
        }

        private void Left_Click(object sender, EventArgs e)
        {
            SetState(DriveStates.LeftLeft, _driveInterface.Left, "Left");
        }

        private void Right_Click(object sender, EventArgs e)
        {
            SetState(DriveStates.Right, _driveInterface.Right, "Right");
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Stop();

            // Launch settings window
            var settingsWindow = new Window()
            {
                Owner = this
            };
            settingsWindow.Loaded += SettingsWindow_Loaded;
            settingsWindow.Unloaded += SettingsWindow_Unloaded;
            settingsWindow.ShowDialog();
        }

        private void EyeGazeOnButton_OnClick(object sender, EventArgs e)
        {
            Stop();

            DrivingButtonsEnabled = true;
            EyesOffStopsDrivingEnabled = true;

            // Button state needs to be reset
            EyeGazeOnButton.Visibility = Visibility.Collapsed;
            ForwardButton.Visibility = Visibility.Visible;
            ForwardLeftButton.Visibility = Visibility.Visible;
            ForwardRightButton.Visibility = Visibility.Visible;
            RightButton.Visibility = Visibility.Visible;
            LeftButton.Visibility = Visibility.Visible;
            ReverseButton.Visibility = Visibility.Visible;
            Grid.SetRow(StopButton, 5);
        }

        private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            GazePointer.Attach(sender as Window, null, null, null, true);
            StopAllCameras();
        }

        private void SettingsWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            StartAllCameras();
        }

        void ExitApplicationAction(object o)
        {
            _trace.TraceInformation("Exit to Desktop");
            Stop();

            var app = (App)Application.Current;
            app.Shutdown();
        }

        static void CalibrateAction(object o)
        {
            GazePointer.LaunchRecalibration();
        }

        public void StartAllCameras()
        {
            int cameraIndex = 0;

            StartCamera(ref _frontView, 480, 240, 15, cameraIndex);
        }

        private void StartCamera(ref VideoCaptureElement camera, int width, int height, int fps, int deviceIndex)
        {
            if (deviceIndex >= MultimediaUtil.VideoInputDevices.Length || deviceIndex < 0)
            {
                // Invalid device index should be ignored
                return;
            }

            // Initialize the element
            camera = new VideoCaptureElement
            {
                DesiredPixelWidth = width,
                DesiredPixelHeight = height,
                FPS = fps,
                VideoCaptureDevice = MultimediaUtil.VideoInputDevices[deviceIndex]
            };

            camera.BeginInit();
            camera.EndInit();

            // Add the control to layout
            camera.Width = CameraCanvas.Width;
            camera.Height = CameraCanvas.Height;
            CameraCanvas.Children.Add(camera);

            // start the camera stream
            camera.Play();
        }

        public void StopAllCameras()
        {
            StopCamera(ref _frontView);
        }

        public void StopCamera(ref VideoCaptureElement camera)
        {
            if (camera == null)
            {
                return;
            }

            camera.Stop();
            CameraCanvas.Children.Remove(camera);
            camera = null;
        }
    }
}
