﻿using Microsoft.Kinect;
using System.Windows.Controls;
using FloorGame.Model.Calibration;
using System.Windows;
using FloorGame.Model.SensorInput.Gestures;
using FloorGame.Model.Player;
using System.Diagnostics;

namespace FloorGame.View;

public partial class MainMenuPage : Page
{
    private RightWaveGestureDetector _rightWaveGestureDetector;
    private LeftWaveGestureDetector _leftWaveGestureDetector;

    public event Action? OnStartPongGame;
    public event Action? OnStartColorMatchGame;
    public event Action? Quit;

    public MainMenuPage(KinectSensor kinectSensor, CalibrationClass.CalibrationData calibrationData)
    {
        InitializeComponent();

        _leftWaveGestureDetector = new LeftWaveGestureDetector(kinectSensor);
        _rightWaveGestureDetector = new RightWaveGestureDetector(kinectSensor);
        playerArea.Init(kinectSensor, calibrationData);
    }

    public void Start()
    {
        _leftWaveGestureDetector.OnGestureDetected += OnWave;
        _rightWaveGestureDetector.OnGestureDetected += OnWave;
    }
    public void Stop()
    {
        _leftWaveGestureDetector.OnGestureDetected -= OnWave;
        _rightWaveGestureDetector.OnGestureDetected -= OnWave;
    }

    private void OnWave(Skeleton skeleton)
    {
        Debug.WriteLine("Wave");
        Player? player = playerArea.PlayerHandler.GetPlayer(skeleton.TrackingId);
        if (player == null) return;

        if (IsPongGameOverlap(player.ImagePosition)) OnStartPongGame?.Invoke();
        if (IsColorMatchGameOverlap(player.ImagePosition)) OnStartColorMatchGame?.Invoke();
    }

    private bool IsPongGameOverlap(Point point)
    {
        Point pongGamePosition = Game_1.TransformToVisual(playerArea.PlayerWalkCanvas).Transform(new Point(0, 0));
        Rect pongGameRect = new Rect(pongGamePosition.X, pongGamePosition.Y, Game_1.Width, Game_1.Height);

        if (pongGameRect.Contains(point)) return true;
        return false;
    }

    private bool IsColorMatchGameOverlap(Point point)
    {
        Point colorMatchGamePosition = Game_2.TransformToVisual(playerArea.PlayerWalkCanvas).Transform(new Point(0, 0));
        Rect colorMatchGameRect = new Rect(colorMatchGamePosition.X, colorMatchGamePosition.Y, Game_1.Width, Game_1.Height);

        if (colorMatchGameRect.Contains(point)) return true;
        return false;
    }
}
