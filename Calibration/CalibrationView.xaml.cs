using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace floorgame.Calibration
{
    /// <summary>
    /// Interaction logic for CalibrationView.xaml
    /// </summary>
    public partial class CalibrationView : UserControl
    {
        public Canvas PlayingAreaCanvas => playingAreaCanvas;
        public TextBlock GameStateText => gameStateText;

        public CalibrationView()
        {
            InitializeComponent();
        }

        public void UpdateWindowTitle(string message)
        {
            GameStateText.Text = message;
        }

        public void DrawCalibrationMarker(Point point)
        {
            PlayingAreaCanvas.Children.Clear();
            Ellipse marker = new Ellipse
            {
                Width = 70,
                Height = 70,
                Fill = Brushes.Yellow
            };
            Canvas.SetLeft(marker, point.X - marker.Width / 2);
            Canvas.SetTop(marker, point.Y - marker.Height / 2);
            PlayingAreaCanvas.Children.Add(marker);
        }

        public void ClearMarkers()
        {
            PlayingAreaCanvas.Children.Clear();
        }
    }
}
