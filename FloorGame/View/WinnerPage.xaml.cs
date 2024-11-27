using System.Windows;
using System.Windows.Controls;

namespace FloorGame.View
{
    public partial class WinnerScreen : Page
    {
        public WinnerScreen(string winnerName)
        {
            InitializeComponent();
            WinnerNameText.Text = $"{winnerName} Wins!";
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
