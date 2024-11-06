using System.Windows;

namespace DirigibleBattle.Windows
{
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();
        }

        private void JoinAsServer_Click(object sender, RoutedEventArgs e)
        {
            var serverWindow = new ServerWindow();
            serverWindow.Show();
            this.Close();
        }

        private void JoinAsClient_Click(object sender, RoutedEventArgs e)
        {
            var clientWindow = new ClientWindow();
            clientWindow.Show();
            this.Close();
        }
    }
}
