using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpConnectionLibrary
{
    public class Server : ITcpConnectionHandler, IDisposable
    {
        public event Action<object> OnGetData;

        public Socket ServerSocket { get; private set; }
        private Socket _clientSocket;
        private int _port;

        public Server(int port = 8000)
        {
            _port = port;
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ServerSocket.Bind(new IPEndPoint(IPAddress.Any, _port));
            ServerSocket.Listen(10);
        }

        public async Task Start()
        {

            Console.WriteLine("Waiting for a connection...");
            _clientSocket = await Task.Run(() => ServerSocket.Accept());


            Console.WriteLine("Client connected");
        }

        public async Task UpdateData<T>(T obj)
        {
            await Task.Run(() =>
            {
                try
                {

                    // Читаем данные в цикле
                    var requestText = ReadDataFromClient();
                    Console.WriteLine("REQUEST TEXT:" + requestText);

                    // Проверяем JSON перед десериализацией
                    if (string.IsNullOrWhiteSpace(requestText))
                    {
                        Console.WriteLine("Received empty or null data");
                        return;
                    }

                    var request = JsonConvert.DeserializeObject<T>(requestText);
                    Console.WriteLine("REQUEST: " + request);

                    // Отправляем ответ клиенту
                    var dataText = JsonConvert.SerializeObject(obj);
                    byte[] data = Encoding.UTF8.GetBytes(dataText);
                    _clientSocket.Send(data);

                    OnGetData?.Invoke(request);

                }
                catch (JsonException jsonEx)
                {
                    LogError($"1111JSON error: {jsonEx.Message}");
                }
                catch (Exception ex)
                {
                    LogError($"222General error: {ex.Message}");
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
                    // Добавляем полученные данные в список байтов
                    for (int i = 0; i < bytesRead; i++)
                    {
                        data.Add(buffer[i]);
                    }
                    // Если меньше данных, чем размер буфера, завершение чтения
                    if (bytesRead < buffer.Length)
                        break;
                }
                catch
                {
                    Dispose();
                }
            }

            return Encoding.UTF8.GetString(data.ToArray());
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

        public void ClearAllListeners()
        {
            OnGetData = null;
        }
    }
}