using Newtonsoft.Json;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpConnectionLibrary
{
    public class Client : ITcpNetworkConnection, IDisposable
    {
        public event Action<object> OnGetNetworkData;

        public Socket ClientSocket { get; private set; }
        private string _serverIP;
        private int _port;

        public Client(string serverIP, int port = 8000)
        {
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverIP = serverIP;
            Console.WriteLine("Connected to IP: " + _serverIP);
            _port = port;
        }

        public async Task Connect()
        {
            try
            {
                await Task.Run(() =>
                {
                    ClientSocket.Connect(_serverIP, _port);
                    Console.WriteLine("Connected to server");
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Connect: {ex.Message}");
            }
        }

        public async Task GetNetworkData<T>()
        {
            try
            {
                Console.WriteLine("Start getting data");

                var requestObj = default(T);
                var requestJson = JsonConvert.SerializeObject(requestObj);
                var requestData = Encoding.UTF8.GetBytes(requestJson);

                await Task.Run(() =>
                {
                    ClientSocket.Send(requestData);
                    //Console.WriteLine($"Request sent to server: {requestJson}");
                });

                byte[] buffer = new byte[65535];
                int bytesReceived = await Task.Run(() => ClientSocket.Receive(buffer));

                if (bytesReceived == 0)
                {
                    Console.WriteLine("No data received");
                    return;
                }

                var resultText = Encoding.UTF8.GetString(buffer, 0, bytesReceived);

                var result = JsonConvert.DeserializeObject<T>(resultText);
                OnGetNetworkData?.Invoke(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetData: {ex.Message}");
            }
        }

        public async Task UpdateNetworkData<T>(T obj)
        {
            var json = JsonConvert.SerializeObject(obj);

            //Console.WriteLine($"Sending JSON data: {json}");

            var data = Encoding.UTF8.GetBytes(json);

            await Task.Run(() =>
            {
                ClientSocket.Send(data);
            });

            if (ClientSocket != null && ClientSocket.Connected)
            {
                try
                {
                    var buffer = new byte[65535];
                    int bytesReceived = await Task.Run(() => ClientSocket.Receive(buffer));
                    var resultText = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                    var result = JsonConvert.DeserializeObject<T>(resultText);

                    OnGetNetworkData?.Invoke(result);
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"Socket error: {ex.Message}");
                }
            }
            else
            {
                Dispose();
                Console.WriteLine("Socket is not connected or is null.");
            }
        }

        public void Dispose()
        {
            ClientSocket.Close();
            ClientSocket.Dispose();
        }
        public void UnsubscribeActions()
        {
            OnGetNetworkData = null;
        }
    }
}