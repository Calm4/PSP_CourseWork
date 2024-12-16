using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpConnectionLibrary
{
    public class Server : ITcpNetworkConnection, IDisposable
    {
        public event Action<object> OnGetNetworkData;
        public event Action<string> OnConnectionLost;

        private bool isConnected;

        public Socket ServerSocket { get; private set; }
        private Socket _clientSocket;

        public Server(int port = 8000)
        {
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ipAddress = new IPEndPoint(IPAddress.Any, port);

            ServerSocket.Bind(ipAddress);

            ServerSocket.Listen(1);
        }


        public async Task Start()
        {
            Console.WriteLine("Waiting for a connection...");
            _clientSocket = await Task.Run(() => ServerSocket.Accept());

            if (_clientSocket != null)
            {
                isConnected = true;
            }

            var localEndPoint = _clientSocket.LocalEndPoint as IPEndPoint;
            var remoteEndPoint = _clientSocket.RemoteEndPoint as IPEndPoint;

            if (localEndPoint != null)
            {
                Console.WriteLine($"Server IP: {localEndPoint.Address}, Port: {localEndPoint.Port}");
            }
            if (remoteEndPoint != null)
            {
                Console.WriteLine($"Client IP: {remoteEndPoint.Address}, Port: {remoteEndPoint.Port}");
            }
        }


        public async Task UpdateNetworkData<T>(T obj)
        {
            try
            {
                var requestTexts = ReadDataFromClient();

                if (string.IsNullOrWhiteSpace(requestTexts))
                {
                    Console.WriteLine("Received empty or null data: ");
                    return;
                }

                try
                {
                    var request = JsonConvert.DeserializeObject<T>(requestTexts);
                    OnGetNetworkData?.Invoke(request);

                    Console.WriteLine("Request TEXT:" + requestTexts);

                    var dataText = JsonConvert.SerializeObject(obj);
                    byte[] data = Encoding.UTF8.GetBytes(dataText);

                    await Task.Run(() =>
                    {
                        _clientSocket.Send(data);
                    });

                }
                catch (JsonException jsonEx)
                {
                    LogError($"JSON error: {jsonEx.Message} + JSON text: ({requestTexts})");
                }

            }
            catch (Exception ex)
            {
                if (isConnected)
                {
                    OnConnectionLost?.Invoke("Потеряно соединение с клиентом");
                    Dispose();
                    LogError($"General error: {ex.Message}");
                    isConnected = false;
                }
            }

        }


        private string ReadDataFromClient()
        {
            var buffer = new byte[65535];
            int bytesRead = _clientSocket.Receive(buffer);

            if (bytesRead == 0)
                return string.Empty;

            string rawData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            var jsonMessages = rawData.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var jsonMessage in jsonMessages)
            {
                if (IsValidJson(jsonMessage))
                {
                    return jsonMessage;
                }
            }

            return string.Empty;
        }



        private bool IsValidJson(string str)
        {
            try
            {
                JsonConvert.DeserializeObject(str);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }

        private void LogError(string message)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public void Dispose()
        {
            _clientSocket?.Close();
            ServerSocket.Close();
            ServerSocket.Dispose();
        }

        public void UnsubscribeActions()
        {
            OnGetNetworkData = null;
        }
    }
}
