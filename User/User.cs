using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace floorgame
{
    public class User
    {
        public int TrackingId { get; private set; }
        public Point Position { get; set; }
        public Brush Color { get; private set; }
        public Ellipse Marker { get; private set; }

        public User(int trackingId, Brush color)
        {
            TrackingId = trackingId;
            Color = color;
            Marker = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = color,
                Stroke = Brushes.Black
            };
        }

        public void UpdateMarkerPosition(Canvas canvas)
        {
            Canvas.SetLeft(Marker, Position.X - Marker.Width / 2);
            Canvas.SetTop(Marker, Position.Y - Marker.Height / 2);

            if (!canvas.Children.Contains(Marker))
            {
                canvas.Children.Add(Marker);
            }
        }

        public void RemoveMarker(Canvas canvas)
        {
            if (canvas.Children.Contains(Marker))
            {
                canvas.Children.Remove(Marker);
            }
        }
    }

}
