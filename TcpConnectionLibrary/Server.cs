using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpConnectionLibrary
{
    public class Server : ITcpConnectionHandler, IDisposable
    {
        public event Action<object> OnGetData;

        private readonly TcpListener _listener;
        private TcpClient _client;
        private NetworkStream _networkStream;

        private bool _disposed = false;

        private readonly int _port;

        public Server(int port = 8000)
        {
            _port = port;
            _listener = new TcpListener(IPAddress.Any, _port);
        }

        public async Task Start()
        {
            _listener.Start();
            Console.WriteLine("Waiting for a connection...");

            _client = await _listener.AcceptTcpClientAsync();
            _networkStream = _client.GetStream();
            Console.WriteLine("Client connected");
        }

        public async Task UpdateData<T>(T obj)
        {
            try
            {
                var requestText = await ReadDataFromClient();
                if (string.IsNullOrWhiteSpace(requestText))
                {
                    Console.WriteLine("Received empty or null data");
                    return;
                }

                var request = JsonConvert.DeserializeObject<T>(requestText);
                Console.WriteLine("REQUEST: " + request);

                var dataText = JsonConvert.SerializeObject(obj);
                var data = Encoding.UTF8.GetBytes(dataText + "\n");
                await _networkStream.WriteAsync(data, 0, data.Length);

                OnGetData?.Invoke(request);
            }
            catch (JsonException jsonEx)
            {
                LogError($"JSON error: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                LogError($"General error: {ex.Message}");
            }
        }

        private async Task<string> ReadDataFromClient()
        {
            if (_networkStream == null || _client == null || !_client.Connected)
                throw new InvalidOperationException("NetworkStream is not available or client is not connected.");

            using (var reader = new StreamReader(_networkStream, Encoding.UTF8, false, 1024, true))
            {
                return await reader.ReadLineAsync();
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
            if (_disposed) return;

            _networkStream?.Dispose();
            _client?.Close();
            _listener.Stop();
            _disposed = true;
        }

        public void ClearAllListeners()
        {
            OnGetData = null;
        }
    }
}
