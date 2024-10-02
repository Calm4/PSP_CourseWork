using Network;
using System;
using System.Windows;

namespace DirigibleBattle
{
    public partial class ClientWindow : Window
    {
        private GameClient _gameClient;

        public ClientWindow()
        {
            InitializeComponent();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            string ip = IpTextBox.Text;
            int port = int.Parse(PortTextBox.Text);

            _gameClient = new GameClient();
            _gameClient.ConnectToServer(ip, port);

            MessageBox.Show($"Connected to server at {ip}:{port}", "Connection Status", MessageBoxButton.OK, MessageBoxImage.Information);

            // После подключения можно запустить игру
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _gameClient?.Disconnect();
        }
    }
}
