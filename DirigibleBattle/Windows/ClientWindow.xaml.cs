using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DirigibleBattle.Windows
{
    public partial class ClientWindow : Window
    {
        private Socket _clientSocket;
        private string _serverIP;

        public ClientWindow()
        {
            InitializeComponent();
        }

        private async void ConnectToServer_Click(object sender, RoutedEventArgs e)
        {
            _serverIP = ServerIP.Text;
            await ConnectToServerAsync();
        }

        private async Task ConnectToServerAsync()
        {
            try
            {
                _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                await _clientSocket.ConnectAsync(new IPEndPoint(IPAddress.Parse(_serverIP), 8080));

                Log("Connected to server.");
                await StartReceivingDataAsync();
            }
            catch (Exception ex)
            {
                Log($"Connection error: {ex.Message}");
            }
        }

        private async Task StartReceivingDataAsync()
        {
            byte[] buffer = new byte[1024];

            while (_clientSocket.Connected)
            {
                try
                {
                    int bytesReceived = await _clientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                    if (bytesReceived == 0)
                    {
                        Log("Connection closed by server.");
                        break;
                    }

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                    if (message == "START_GAME")
                    {
                        Log("Game started by server.");
                        break;
                    }
                    else
                    {
                        // Обработка состояния игры (получение и обновление)
                    }
                }
                catch (SocketException ex)
                {
                    Log($"Socket error: {ex.Message}");
                    break;
                }
            }
        }

        private void Log(string message)
        {
            ClientLog.Items.Add($"{DateTime.Now:HH:mm:ss} - {message}");
        }
    }
}
