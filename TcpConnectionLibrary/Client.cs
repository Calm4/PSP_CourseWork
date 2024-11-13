using Newtonsoft.Json;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpConnectionLibrary
{
    public class Client : ITcpConnectionHandler, IDisposable
    {
        public event Action<object> OnGetData;

        private Socket _socket;
        private string _address;
        private int _port;

        public Client(string ipAddress, int port = 8000)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _address = ipAddress;
            _port = port;
        }

        public async Task Connect()
        {
            try
            {
                await Task.Run(() =>
                {
                    _socket.Connect(_address, _port);
                    Console.WriteLine("Connected to server");
                });
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

                // Отправляем JSON-запрос, соответствующий типу T
                var requestObj = default(T);
                var requestJson = JsonConvert.SerializeObject(requestObj);
                var requestData = Encoding.UTF8.GetBytes(requestJson);

                await Task.Run(() =>
                {
                    _socket.Send(requestData);
                    Console.WriteLine($"Request sent to server: {requestJson}");
                });

                // Получаем ответ от сервера
                byte[] buffer = new byte[1024];
                int bytesReceived = await Task.Run(() => _socket.Receive(buffer));

                if (bytesReceived == 0)
                {
                    Console.WriteLine("No data received");
                    return;
                }

                var resultText = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                Console.WriteLine($"Received data: {resultText}");

                var result = JsonConvert.DeserializeObject<T>(resultText);
                OnGetData?.Invoke(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetData: {ex.Message}");
            }
        }



        public async Task UpdateData<T>(T obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            Console.WriteLine($"Sending JSON data: {json}");

            var data = Encoding.UTF8.GetBytes(json);

            // Отправка данных серверу
            await Task.Run(() =>
            {
                _socket.Send(data);
                Console.WriteLine("Data sent to server");
            });

            // Получение ответа от сервера
            var buffer = new byte[1024];
            int bytesReceived = await Task.Run(() => _socket.Receive(buffer));
            var resultText = Encoding.UTF8.GetString(buffer, 0, bytesReceived);

            var result = JsonConvert.DeserializeObject<T>(resultText);
            OnGetData?.Invoke(result);
        }

        public void Dispose()
        {
            _socket.Close();
            _socket.Dispose();
        }

        public void ClearAllListeners()
        {
            OnGetData = null;
        }
    }
}