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
            await Task.Run(() =>
            {
                try
                {
                    var requestTexts = ReadDataFromClient();

                    if (string.IsNullOrWhiteSpace(requestTexts))
                    {
                        Console.WriteLine("Received empty or null data");
                        OnGetNetworkData?.Invoke(null);
                        return;
                    }

                    try
                    {
                        var request = JsonConvert.DeserializeObject<T>(requestTexts);

                        Console.WriteLine("Request TEXT:" + requestTexts);

                        var dataText = JsonConvert.SerializeObject(obj);
                        byte[] data = Encoding.UTF8.GetBytes(dataText);
                        _clientSocket.Send(data);

                        OnGetNetworkData?.Invoke(request);
                    }
                    catch (JsonException jsonEx)
                    {
                        LogError($"1 JSON error: {jsonEx.Message} + ({requestTexts})");
                    }

                }
                catch (Exception ex)
                {
                    LogError($"2 General error: {ex.Message}");
                }
            });
        }


        private string ReadDataFromClient()
        {
            var buffer = new byte[65535];
            var data = new List<byte>();

            while (true)
            {
                try
                {
                    int bytesRead = _clientSocket.Receive(buffer);
                    if (bytesRead == 0)
                        break;

                    for (int i = 0; i < bytesRead; i++)
                    {
                        data.Add(buffer[i]);
                    }

                    if (bytesRead < buffer.Length)
                        break;
                }
                catch (Exception ex)
                {
                    LogError($"Error while reading data: {ex.Message}");
                    Dispose();
                    return string.Empty;
                }
            }

            string rawData = Encoding.UTF8.GetString(data.ToArray());

            // Проверяем, является ли строка валидным JSON
            if (string.IsNullOrWhiteSpace(rawData) || !IsValidJson(rawData))
            {
                LogError($"Invalid or empty JSON received: {rawData}");
                return string.Empty;
            }

            return rawData;
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
