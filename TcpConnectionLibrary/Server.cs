using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private int _port;

        public Server(int port = 8000)
        {
            _port = port;
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ipAddress = new IPEndPoint(IPAddress.Any, _port);
            ServerSocket.Bind(ipAddress);

            // Получение локального адреса, к которому привязан сервер
            var localEndPoint = ServerSocket.LocalEndPoint as IPEndPoint;
            if (localEndPoint != null)
            {
                Console.WriteLine($"Server started on IP: {localEndPoint.Address}, Port: {localEndPoint.Port}");
            }

            ServerSocket.Listen(10);
        }


        public async Task Start()
        {
            Console.WriteLine("Waiting for a connection...");
            _clientSocket = await Task.Run(() => ServerSocket.Accept());

            // Получение локального IP-адреса и порта
            var localEndPoint = _clientSocket.LocalEndPoint as IPEndPoint;
            var remoteEndPoint = _clientSocket.RemoteEndPoint as IPEndPoint;

            if (localEndPoint != null)
            {
                Console.WriteLine($"Server is bound to IP: {localEndPoint.Address}, Port: {localEndPoint.Port}");
            }
            if (remoteEndPoint != null)
            {
                Console.WriteLine($"Client connected from IP: {remoteEndPoint.Address}, Port: {remoteEndPoint.Port}");
            }
        }


        public async Task UpdateNetworkData<T>(T obj)
        {
            await Task.Run(() =>
            {
                try
                {
                    // Читаем данные в цикле
                    var requestTexts = ReadDataFromClient();

                   
                        if (string.IsNullOrWhiteSpace(requestTexts))
                        {
                            Console.WriteLine("Received empty or null data");
                            return;
                        }

                        try
                        {
                            var request = JsonConvert.DeserializeObject<T>(requestTexts);
                            Console.WriteLine("REQUEST: " + request);

                            // Отправляем ответ клиенту
                            var dataText = JsonConvert.SerializeObject(obj);
                            byte[] data = Encoding.UTF8.GetBytes(dataText);
                            _clientSocket.Send(data);

                            OnGetNetworkData?.Invoke(request);
                        }
                        catch (JsonException jsonEx)
                        {
                            LogError($"1 JSON error: {jsonEx.Message}");
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
            var buffer = new byte[1024];
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
                catch
                {
                    Dispose();
                }
            }

            string rawData = Encoding.UTF8.GetString(data.ToArray());

            // Теперь используется строка в методе Split
            // Проверка на валидный JSON и обработка строки
            if (string.IsNullOrWhiteSpace(rawData) || !IsValidJson(rawData))
            {
                LogError("Invalid or empty JSON received.");
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
            Console.WriteLine("ERROR: " + message);
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
