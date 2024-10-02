using System.Windows;

namespace DirigibleBattle
{
    public partial class ConnectionWindow : Window
    {
        public ConnectionWindow()
        {
            InitializeComponent();
        }

        private void ServerButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика запуска сервера
            ServerWindow serverWindow = new ServerWindow();
            serverWindow.Show();
            this.Close(); // Закрываем текущее окно после выбора
        }

        private void ClientButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика запуска клиента
            ClientWindow clientWindow = new ClientWindow();
            clientWindow.Show();
            this.Close(); // Закрываем текущее окно после выбора
        }

        private void GameButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика запуска клиента
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close(); // Закрываем текущее окно после выбора
        }

        private void ClientButton_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
