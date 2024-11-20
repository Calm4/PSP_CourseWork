using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpConnectionLibrary
{
    public class Client : ITcpConnectionHandler, IDisposable
    {
        public event Action<object> OnGetData;

        private readonly TcpClient _client;
        private NetworkStream _networkStream;

        private readonly string _address;
        private readonly int _port;

        private bool _disposed = false;

        public Client(string ipAddress, int port = 8000)
        {
            _client = new TcpClient();
            _address = ipAddress;
            _port = port;
        }

        public async Task Connect()
        {
            try
            {
                await _client.ConnectAsync(_address, _port);
                _networkStream = _client.GetStream();
                Console.WriteLine("Connected to server");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Connect: {ex.Message}");
            }
        }

        public async Task GetData<T>()
        {
            try
            {
                Console.WriteLine("Start getting data");

                var requestObj = default(T);
                var requestJson = JsonConvert.SerializeObject(requestObj);
                var requestData = Encoding.UTF8.GetBytes(requestJson + "\n");

                await _networkStream.WriteAsync(requestData, 0, requestData.Length);

                var responseText = await ReadDataFromServer();
                var result = JsonConvert.DeserializeObject<T>(responseText);

                OnGetData?.Invoke(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetData: {ex.Message}");
            }
        }

        public async Task UpdateData<T>(T obj)
        {
            try
            {
                var json = JsonConvert.SerializeObject(obj);
                var data = Encoding.UTF8.GetBytes(json + "\n");

                await _networkStream.WriteAsync(data, 0, data.Length);

                var responseText = await ReadDataFromServer();
                var result = JsonConvert.DeserializeObject<T>(responseText);

                OnGetData?.Invoke(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateData: {ex.Message}");
            }
        }

        private async Task<string> ReadDataFromServer()
        {
            if (_networkStream == null || !_client.Connected)
                throw new InvalidOperationException("NetworkStream is not available or client is not connected.");

            using (var reader = new StreamReader(_networkStream, Encoding.UTF8, false, 1024, true))
            {
                return await reader.ReadLineAsync();
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            _networkStream?.Dispose();
            _client?.Close();
            _disposed = true;
        }

        public void ClearAllListeners()
        {
            OnGetData = null;
        }
    }
}
