using FloorGame.Model.Calibration;
using FloorGame.Model.SensorInput;
using Microsoft.Kinect;
using System.Windows;

namespace FloorGame.Model.Player;

public class Player
{
    public Point ImagePosition { get; private set; }
    public SkeletonPoint SkeletonPosition { get; private set; }

    public Player(int trackingID)
    {

    }

    public void SetPosition(Point imagePosition, SkeletonPoint skeletonPosition)
    {
        this.ImagePosition = imagePosition;
        this.SkeletonPosition = skeletonPosition;
    }
}

public class PlayerHandler
{
    private Dictionary<int, Player> _players = new Dictionary<int, Player>();
    private SkeletonTracker _skeletonTracker;
    private CalibrationClass _calibrationClass;

    public PlayerHandler(KinectSensor kinectSensor, CalibrationClass.CalibrationData calibrationData)
    {
        _calibrationClass = new CalibrationClass(kinectSensor, calibrationData);

        _skeletonTracker = new SkeletonTracker(kinectSensor);
        _skeletonTracker.OnSkeletonUpdate += UpdatePlayerPosition;
    }

    public Player[] Players => _players.Values.ToArray();

    public Player? GetPlayer(int trackingID)
    {
        if (_players.ContainsKey(trackingID)) return _players[trackingID];
        return null;
    }

    public void AddPlayer(Player player, int trackingID)
    {
        if (_players.ContainsKey(trackingID)) return;
        _players[trackingID] = player;
    }

    private void UpdatePlayerPosition(Skeleton skeleton)
    {
        if (skeleton == null) return;
        if (!_players.ContainsKey(skeleton.TrackingId)) AddPlayer(new Player(skeleton.TrackingId), skeleton.TrackingId);
        _players[skeleton.TrackingId].SetPosition(_calibrationClass.kinectToProjectionPoint(skeleton.Joints[JointType.Spine].Position), skeleton.Joints[JointType.Spine].Position);
    }
}
