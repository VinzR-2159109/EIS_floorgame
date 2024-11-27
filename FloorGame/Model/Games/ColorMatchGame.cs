using FloorGame.Model.Calibration;
using FloorGame.Model.Player;
using Microsoft.Kinect;
using System.Windows.Media;
using System.Windows.Threading;

namespace FloorGame.Model.Games;

public class ColorMatchGame
{
    public class Tile
    {
        private Random random;

        public enum TileState { BLOCKED, Left, Right, FREE }
        public TileState State;

        public int TimerCount { get; private set; }
        public (int x, int y) TilePosition {  get; private set; }

        public Tile(int startDelay, (int x, int y) tilePosition)
        {
            TilePosition = tilePosition;
            State = TileState.FREE;
            random = new Random(Guid.NewGuid().GetHashCode());
        }

        public void Update()
        {
            TimerCount--;
            if (TimerCount < 0)
            {
                int timerCount = random.Next(6, 20);
                int nextStateNumber = random.Next(0, 100);
                
                if      (nextStateNumber < 10)  SetState(TileState.BLOCKED, timerCount);
                else if (nextStateNumber < 50)  SetState(TileState.Left, timerCount);
                else if (nextStateNumber < 80)  SetState(TileState.Right, timerCount);
                else if (nextStateNumber < 100) SetState(TileState.FREE, timerCount);

                return;
            }

            SetState(State, TimerCount);
        }

        public void SetState(TileState state, int timerCount)
        {
            State = state;
            TimerCount = timerCount;
        }
    }
    public enum HandState { Left, Right, Both, None }

    private Tile[,] _tiles;
    private (double width, double height) _playerAreaSize;
    private PlayerHandler _playerHandler;
    private DispatcherTimer _updatePlayersTimer;
    private DispatcherTimer _updateTilesTimer;

    public int TileAmount => 8;
    public event Action<Tile[,]>? OnTilesChanged;
    public event Action<Tile>? OnTileChanged;
    public event Action<Player.Player, HandState>? OnPlayerChanged;
    public event Action<Player.Player[], Player.Player>? OnPlayerLost;

    public ColorMatchGame(KinectSensor kinectSensor, CalibrationClass.CalibrationData calibrationData, PlayerHandler playerHandler, (double, double) playerAreaSize)
    {
        _playerHandler = playerHandler;
        _playerAreaSize = playerAreaSize;
        _updatePlayersTimer = new DispatcherTimer();
        _updatePlayersTimer.Interval = TimeSpan.FromMilliseconds(32);
        _updatePlayersTimer.Tick += (object? sender, EventArgs e) => UpdatePlayers();
        _updateTilesTimer = new DispatcherTimer();
        _updateTilesTimer.Interval = TimeSpan.FromSeconds(1);
        _updateTilesTimer.Tick += (object? sender, EventArgs e) => UpdateTiles();

        foreach (var player in _playerHandler.Players) player.Lives = 9;

        Random random = new Random();
        _tiles = new Tile[TileAmount, TileAmount];
        for (int x = 0; x < TileAmount; x++)
        {
            for (int y = 0; y < TileAmount; y++)
            {
                Tile newTile = new Tile(random.Next(5000, 100000), (x, y));
                _tiles[x, y] = newTile;
            }
        }

        _updatePlayersTimer.Start();
        _updateTilesTimer.Start();
    }

    private void UpdatePlayers()
    {
        Player.Player[] players = _playerHandler.Players;

        foreach (var player in players)
        {
            if (player.Skeleton == null) continue;
            HandState handState = GetHandState(player.Skeleton);

            // TODO: Fix this of by 2 error
            (int x, int y) tilePosition = ((int)(player.ImagePosition.X / _playerAreaSize.width * TileAmount), (int)(player.ImagePosition.Y / _playerAreaSize.height * TileAmount));
            if (tilePosition.x < 0 || tilePosition.x >= TileAmount || tilePosition.y < 0 || tilePosition.y >= TileAmount) continue;
            Tile tile = _tiles[tilePosition.x, tilePosition.y];

            switch (tile.State)
            {
                case Tile.TileState.BLOCKED:
                    HitTile(player, tile);
                    break;
                case Tile.TileState.Left:
                    if (handState != HandState.Left) HitTile(player, tile);
                    break;
                case Tile.TileState.Right:
                    if (handState != HandState.Right) HitTile(player, tile);
                    break;
            }

            OnPlayerChanged?.Invoke(player, handState);
        }
    }

    private void UpdateTiles()
    {
        foreach (Tile tile in _tiles) tile.Update();
        OnTilesChanged?.Invoke(_tiles);
    }

    private void HitTile(Player.Player player, Tile tile)
    {
        tile.SetState(Tile.TileState.FREE, 20);
        player.Lives--;

        if (player.Lives == 0) OnPlayerLost?.Invoke(_playerHandler.Players, player);

        OnTileChanged?.Invoke(tile);
    }

    private HandState GetHandState(Skeleton skeleton)
    {
        bool leftHandUp = false;
        bool rightHandUp = false;

        if (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.ShoulderLeft].Position.Y)
            leftHandUp = true;
        if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.ShoulderRight].Position.Y)
            rightHandUp = true;

        if (leftHandUp && rightHandUp) return HandState.Both;
        if (!leftHandUp && !rightHandUp) return HandState.None;
        if (leftHandUp && !rightHandUp) return HandState.Left;
        if (!leftHandUp && rightHandUp) return HandState.Right;

        return HandState.None;
    }
}
