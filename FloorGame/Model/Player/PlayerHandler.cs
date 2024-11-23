using FloorGame.Model.Calibration;
using FloorGame.Model.SensorInput;
using Microsoft.Kinect;
using System.Windows;
using System.Windows.Media;

namespace FloorGame.Model.Player;

public class Player
{
    public Point ImagePosition { get; private set; }
    public SkeletonPoint SkeletonPosition { get; private set; }
    public Skeleton? Skeleton {  get; private set; }

    public Color PlayerColor { get; private set; }
    public Color FillColor { get; set; }

    public int Lives {  get; set; }

    public Player(Color color)
    {
        PlayerColor = color;
        FillColor = Color.FromRgb(0, 0, 0);
        Lives = 30;
    }

    public void SetPosition(Point imagePosition, SkeletonPoint skeletonPosition, Skeleton skeleton)
    {
        this.ImagePosition = imagePosition;
        this.SkeletonPosition = skeletonPosition;
        this.Skeleton = skeleton;
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
        if (!_players.ContainsKey(skeleton.TrackingId)) 
        {
            Random rand = new Random();
            byte r = (byte)rand.Next(0, 256);
            byte g = (byte)rand.Next(0, 256);
            byte b = (byte)rand.Next(0, 256);

            AddPlayer(new Player(Color.FromRgb(r, g, b)), skeleton.TrackingId); 
        }
        _players[skeleton.TrackingId].SetPosition(_calibrationClass.kinectToProjectionPoint(skeleton.Joints[JointType.Spine].Position), skeleton.Joints[JointType.Spine].Position, skeleton);
    }
}
