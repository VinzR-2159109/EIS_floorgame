using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Kinect;
using System.Windows;

namespace floorgame
{
    public class UserHandler
    {
        private Dictionary<int, User> users = new Dictionary<int, User>();
        private static readonly Brush[] availableColors = { Brushes.Red, Brushes.Green, Brushes.Blue, Brushes.Yellow, Brushes.Purple, Brushes.Orange };

        public User GetOrCreateUser(int trackingId)
        {
            if (!users.ContainsKey(trackingId))
            {
                users[trackingId] = new User(trackingId, GetUserColor(trackingId));
            }
            return users[trackingId];
        }

        private Brush GetUserColor(int trackingId)
        {
            int colorIndex = users.Count % availableColors.Length;
            return availableColors[colorIndex];
        }

        public void UpdateUserPosition(int trackingId, Point projectedPoint, Canvas playingAreaCanvas)
        {
            User user = GetOrCreateUser(trackingId);
            user.Position = projectedPoint;
            user.UpdateMarkerPosition(playingAreaCanvas);
        }
    }
}
