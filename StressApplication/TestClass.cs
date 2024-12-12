using GameLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpConnectionLibrary;

namespace StressApplication
{
    internal class TestClass
    {
        private static NetworkData _clientResult;
        private static NetworkData _serverResult;

        private static Stopwatch _clientStopwatch;
        private static Stopwatch _serverStopwatch;

        private const int IterationsNumber = 1000;

        private static double[] clientsTimes;
        private static double[] serversTimes;

        static async Task Main(string[] args)
        {
            clientsTimes = new double[IterationsNumber];
            serversTimes = new double[IterationsNumber];

            for (int i = 0; i < IterationsNumber; i++)
            {
                await TestDataSending(i);
            }

            Console.WriteLine($"Среднее количество микросекунд для клиентов: {clientsTimes.Average()}");
            Console.WriteLine($"Среднее количество микросекунд для серверов: {serversTimes.Average()}");
        }

        private static async Task TestDataSending(int iterationNumber)
        {
            var server = new Server();
            var client = new Client("localhost");
            client.OnGetNetworkData += Client_OnGetData;
            server.OnGetNetworkData += Server_OnGetData;

            NetworkData clientNetworkData = new NetworkData()
            {
                Health = 1,
                Armor = 1,
                Ammo = 1,
                Fuel = 1,
                Speed = 1
            };
            NetworkData serverNetworkData = new NetworkData()
            {
                Health = 2,
                Armor = 2,
                Ammo = 2,
                Fuel = 2,
                Speed = 2
            };

            _clientResult = null;
            _serverResult = null;

            _serverStopwatch = new Stopwatch();
            _serverStopwatch.Start();
            await server.Start();
            server.UpdateNetworkData(serverNetworkData);

            _clientStopwatch = new Stopwatch();
            _clientStopwatch.Start();
            await client.Connect();
            client.UpdateNetworkData(clientNetworkData);



            while (_clientResult == null || _serverResult == null)
            {

            }

            var clientTime = _clientStopwatch.ElapsedTicks * (1000000 / (double)Stopwatch.Frequency);
            clientsTimes[iterationNumber] = clientTime;

            Console.WriteLine($"Результат, полученный клиентом от сервера: Здоровье: {_clientResult.Health} | Броня: {_clientResult.Armor} | Пули: {_clientResult.Ammo} | Топливо {_clientResult.Fuel} | Скорость {_clientResult.Speed}");
            Console.WriteLine($"Время, потраченное клиентом на отправку и получение данных от сервера: {clientTime} микросекунд");

            var serverTime = _serverStopwatch.ElapsedTicks * (1000000 / (double)Stopwatch.Frequency);
            serversTimes[iterationNumber] = serverTime;

            Console.WriteLine($"Результат, полученный клиентом от сервера: Здоровье: {_serverResult.Health} | Броня: {_serverResult.Armor} | Пули: {_serverResult.Ammo} | Топливо {_serverResult.Fuel} | Скорость {_serverResult.Speed}");
            Console.WriteLine($"Время, потраченное сервером на отправку и получение данных от клиента: {serverTime} микросекунд");

            server.UnsubscribeActions();
            client.UnsubscribeActions();

            server.Dispose();
            client.Dispose();
        }

        private static void Server_OnGetData(object obj)
        {
            _serverResult = (NetworkData)obj;
            _serverStopwatch?.Stop();
        }

        private static void Client_OnGetData(object obj)
        {
            _clientResult = (NetworkData)obj;
            _clientStopwatch?.Stop();
        }
    }
}
