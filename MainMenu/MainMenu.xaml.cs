using floorgame.Calibration;
using Microsoft.Kinect;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace floorgame
{
    public partial class MainMenu : UserControl
    {
        public event EventHandler StartPongGame;

        private CalibrationClass calibration;
        private UserHandler userHandler;
        private TrackUserPosition userTracker;
        private Timer positionTimer;

        private const int RequiredTimeInSeconds = 3;
        private int timeElapsed = 0;
        private bool isUserInTargetArea = false;

        public MainMenu(CalibrationClass calibration, UserHandler userHandler, TrackUserPosition userTracker)
        {
            this.calibration = calibration;
            this.userHandler = userHandler;
            this.userTracker = userTracker;

            InitializeComponent();
            userTracker.SkeletonUpdated += UpdateUserPosition;

            positionTimer = new Timer(1000);
            positionTimer.Elapsed += OnTimerElapsed;
        }

        private void UpdateUserPosition(Skeleton trackedSkeleton)
        {
            if (trackedSkeleton != null)
            {
                int trackingId = trackedSkeleton.TrackingId;
                Point projectedPoint = calibration.kinectToProjectionPoint(trackedSkeleton.Position);
                userHandler.UpdateUserPosition(trackingId, projectedPoint, MenuPlayAreaCanvas);
                Point game1Position = Game_1.TransformToVisual(MenuPlayAreaCanvas).Transform(new Point(0, 0));
                Rect targetArea = new Rect(game1Position.X, game1Position.Y, Game_1.Width, Game_1.Height);



                if (targetArea.Contains(projectedPoint))
                {
                    if (!isUserInTargetArea)
                    {
                        isUserInTargetArea = true;
                        timeElapsed = 0;
                        positionTimer.Start();
                    }
                }
                else
                {
                    isUserInTargetArea = false;
                    positionTimer.Stop();
                }
            }
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            timeElapsed++;

            if (timeElapsed >= RequiredTimeInSeconds)
            {
                positionTimer.Stop();
                Application.Current.Dispatcher.Invoke(_StartPongGame);
            }
        }

        private void _StartPongGame()
        {
            StartPongGame?.Invoke(this, EventArgs.Empty);
        }
    }
}
