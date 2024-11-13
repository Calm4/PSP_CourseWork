using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpConnectionLibrary
{
    public class Server : ITcpConnectionHandler, IDisposable
    {
        public event Action<object> OnGetData;

        private Socket _listenerSocket;
        private Socket _clientSocket;
        private int _port;

        public Server(int port = 8000)
        {
            _port = port;
            _listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listenerSocket.Bind(new IPEndPoint(IPAddress.Any, _port));
            _listenerSocket.Listen(10);
        }

        public async Task Start()
        {
            Console.WriteLine("Waiting for a connection...");
            _clientSocket = await Task.Run(() => _listenerSocket.Accept());
            Console.WriteLine("Client connected");
        }



        public async Task UpdateData<T>(T obj)
        {
            await Task.Run(() =>
            {
                Console.WriteLine("Start receiving data");

                var buffer = new byte[1024];
                int bytesReceived = _clientSocket.Receive(buffer);
                var requestText = Encoding.UTF8.GetString(buffer, 0, bytesReceived);

                Console.WriteLine($"Received raw data: '{requestText}'");

                try
                {
                    var request = JsonConvert.DeserializeObject<T>(requestText);
                    Console.WriteLine("Request received");

                    // Отправляем ответ клиенту
                    var dataText = JsonConvert.SerializeObject(obj);
                    byte[] data = Encoding.UTF8.GetBytes(dataText);
                    _clientSocket.Send(data);

                    Console.WriteLine("Data sent to client");
                    OnGetData?.Invoke(request);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Deserialization error: {ex.Message}");
                }
            });
        }



        public void Dispose()
        {
            _clientSocket?.Close();
            _listenerSocket.Close();
            _listenerSocket.Dispose();
        }

        public void ClearAllListeners()
        {
            OnGetData = null;
        }
    }
}

namespace GameLibrary
{
    public class NetworkData
    {
        public float BalloonPositionX;
        public float BalloonPositionY;
        public bool IsServerData;
        public bool IsClientData;
    }
}