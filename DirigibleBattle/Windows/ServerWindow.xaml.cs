using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DirigibleBattle.Windows
{
    public partial class ServerWindow : Window
    {
        private Socket _serverSocket;
        private List<Socket> _clientSockets = new List<Socket>();
        private bool _isGameStarted = false;

        public ServerWindow()
        {
            InitializeComponent();
            DisplayServerIP();
        }

        private void StartServer_Click(object sender, RoutedEventArgs e)
        {
            StartServer();
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            if (_clientSockets.Count > 0)
            {
                _isGameStarted = true;
                StartGameForClients();
            }
            else
            {
                Log("At least one client must be connected to start the game.");
            }
        }

        private void DisplayServerIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ServerIPText.Text = $"Server IP: {ip}";
                    break;
                }
            }
        }

        private async void StartServer()
        {
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 8080));
            _serverSocket.Listen(5);

            Log("Server started, waiting for clients...");
            await AcceptClientsAsync();
        }

        private async Task AcceptClientsAsync()
        {
            while (!_isGameStarted)
            {
                try
                {
                    var clientSocket = await _serverSocket.AcceptAsync();
                    _clientSockets.Add(clientSocket);

                    Dispatcher.Invoke(() =>
                    {
                        Log($"Client connected: {clientSocket.RemoteEndPoint}");
                        StartGameButton.Visibility = Visibility.Visible;
                    });

                    // Start handling client data
                    _ = HandleClientDataAsync(clientSocket);
                }
                catch (SocketException ex)
                {
                    Log($"Socket error: {ex.Message}");
                }
            }
        }

        private async Task HandleClientDataAsync(Socket clientSocket)
        {
            byte[] buffer = new byte[1024];
            while (clientSocket.Connected)
            {
                int bytesRead = await clientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                if (bytesRead == 0) break;

                string clientMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Dispatcher.Invoke(() => Log($"Message from client: {clientMessage}"));
            }

            clientSocket.Close();
            _clientSockets.Remove(clientSocket);
        }

        private void StartGameForClients()
        {
            // Открытие окна игры для сервера
            var gameWindow = new MainWindow(isServer: true, serverSocket: _serverSocket, clientSockets: _clientSockets);
            gameWindow.Show();

            byte[] startSignal = Encoding.UTF8.GetBytes("START_GAME");
            foreach (var clientSocket in _clientSockets)
            {
                clientSocket.Send(startSignal);
            }

            this.Close();
        }

        private void Log(string message)
        {
            ServerLog.Items.Add($"{DateTime.Now:HH:mm:ss} - {message}");
        }
    }
}
