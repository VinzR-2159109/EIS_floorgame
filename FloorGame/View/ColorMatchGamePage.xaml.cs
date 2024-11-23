using FloorGame.Model.Calibration;
using FloorGame.Model.Games;
using FloorGame.Model.Player;
using Microsoft.Kinect;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FloorGame.View;

public partial class ColorMatchGamePage : Page
{
    private ColorMatchGame _model;
    private Dictionary<int, (Rectangle tile, TextBlock timer)> _tiles;
    private Dictionary<ColorMatchGame.Tile.TileState, Color> _tileStateColors;
    private Dictionary<ColorMatchGame.HandState, Color> _playerStateColors;

    public ColorMatchGamePage(KinectSensor kinectSensor, CalibrationClass.CalibrationData calibrationData)
    {
        InitializeComponent();
        playerArea.Init(kinectSensor, calibrationData);

        _model = new ColorMatchGame(kinectSensor, calibrationData, playerArea.PlayerHandler, (playerArea.PlayerWalkCanvas.Width, playerArea.PlayerWalkCanvas.Height));
        _tiles = new Dictionary<int, (Rectangle tile, TextBlock timer)>();

        boardCanvas.Width = playerArea.PlayerWalkCanvas.Width;
        boardCanvas.Height = playerArea.PlayerWalkCanvas.Height;

        _tileStateColors = new Dictionary<ColorMatchGame.Tile.TileState, Color>()
        {
            { ColorMatchGame.Tile.TileState.BLOCKED, Color.FromRgb(255, 0, 0) },
            { ColorMatchGame.Tile.TileState.Left, Color.FromRgb(255, 255, 0)},
            { ColorMatchGame.Tile.TileState.Right, Color.FromRgb(0, 0, 255)},
            { ColorMatchGame.Tile.TileState.FREE, Color.FromRgb(255, 255, 255)},
        };

        _playerStateColors = new Dictionary<ColorMatchGame.HandState, Color>()
        {
            { ColorMatchGame.HandState.Left,  Color.FromRgb(255, 255, 0)},
            { ColorMatchGame.HandState.Right, Color.FromRgb(0, 0, 255)},
            { ColorMatchGame.HandState.Both,  Color.FromRgb(255, 255, 255)},
            { ColorMatchGame.HandState.None,  Color.FromRgb(255, 255, 255)},
        };

        GenerateBoard(boardCanvas, _model.TileAmount);

        _model.OnTilesChanged += UpdateTiles;
        _model.OnTileChanged += UpdateTile;
        _model.OnPlayerChanged += UpdatePlayer;
    }

    private void UpdateTiles(ColorMatchGame.Tile[,] tiles)
    {
        foreach (ColorMatchGame.Tile tile in tiles)
            UpdateTile(tile);
    }

    private void UpdateTile(ColorMatchGame.Tile tile)
    {
        var visualTile = _tiles[tile.TilePosition.x + tile.TilePosition.y * _model.TileAmount];
        visualTile.timer.Text = tile.TimerCount.ToString();
        visualTile.tile.Fill = new SolidColorBrush(_tileStateColors[tile.State]);
    }

    private void UpdatePlayer(Player player, ColorMatchGame.HandState handState)
    {
        player.FillColor = _playerStateColors[handState];
    }

    private void GenerateBoard(Canvas canvas, int tileCount)
    {
        canvas.Children.Clear();

        double tileWidth = canvas.Width / tileCount;
        double tileHeight = canvas.Height / tileCount;

        double boardWidth = tileWidth * tileCount;
        double boardHeight = tileHeight * tileCount;
        double offsetX = (canvas.Width - boardWidth) / 2;
        double offsetY = (canvas.Height - boardHeight) / 2;

        for (int row = 0; row < tileCount; row++)
        {
            for (int col = 0; col < tileCount; col++)
            {
                Rectangle rectangle = new Rectangle
                {
                    Width = tileWidth,
                    Height = tileHeight,
                    Fill = new SolidColorBrush(_tileStateColors[ColorMatchGame.Tile.TileState.FREE]),
                    Stroke = Brushes.Black
                };

                Canvas.SetLeft(rectangle, offsetX + col * tileWidth);
                Canvas.SetTop(rectangle, offsetY + row * tileHeight);
                canvas.Children.Add(rectangle);

                TextBlock textBlock = new TextBlock
                {
                    Text = "",
                    Foreground = Brushes.Black,
                    FontWeight = FontWeights.Bold,
                    FontSize = Math.Min(tileHeight, tileWidth) / 3,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Center,
                    Width = tileWidth,
                    Height = tileHeight           };
                textBlock.Padding = new Thickness(0, (tileHeight - textBlock.FontSize * 1.3) / 2, 0, 0);

                Canvas.SetLeft(textBlock, offsetX + col * tileWidth);
                Canvas.SetTop(textBlock, offsetY + row * tileHeight);
                canvas.Children.Add(textBlock);

                _tiles[col + row * tileCount] = (rectangle, textBlock);
            }
        }
    }
}
