using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DirigibleBattle
{
    public partial class ServerWindow : Window
    {
        public ServerWindow()
        {
            InitializeComponent();
        }

        // Обработчик события изменения текста в IpTextBox
        private void IpTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Ваш код для обработки изменения текста
            string ipAddress = IpTextBox.Text;

            // Например, можно выполнить валидацию IP-адреса или другие действия
            // Если хотите, чтобы кнопка "Start Server" была доступна только при валидном IP
            // StartServerButton.IsEnabled = IsValidIpAddress(ipAddress);
        }

        // Метод для валидации IP-адреса (если необходимо)
        private bool IsValidIpAddress(string ipAddress)
        {
            // Простой пример валидации (можно улучшить)
            var parts = ipAddress.Split('.');
            return parts.Length == 4 && parts.All(part => int.TryParse(part, out int n) && n >= 0 && n <= 255);
        }

        private void StartServerButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для запуска сервера
        }
    }
}
