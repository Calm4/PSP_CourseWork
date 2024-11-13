using GameLibrary;
using GameLibrary.Dirigible;
using OpenTK;
using PrizesLibrary.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TcpConnectionLibrary;

namespace DirigibleBattle.Managers
{
    public class NetworkManager
    {
        public AbstractDirigible CurrentPlayer { get; private set; }
        public AbstractDirigible NetworkPlayer { get; private set; }

        private ITcpConnectionHandler _handler;

        private Client _client;
        private Server _server;

        private GameManager _gameManager;
        private UIManager _uiManager;
        private TimeManager _timeManager;

        public NetworkManager(GameManager gameManager, UIManager uiManager, TimeManager timeManager)
        {
            _gameManager = gameManager;
            _uiManager = uiManager;
            _timeManager = timeManager;
        }

        public void SetNetworkStartData(ITcpConnectionHandler handler, bool isServer, int seed)
        {
            try
            {
                Console.WriteLine("Setting network start data");

                if (isServer)
                {
                    Console.WriteLine("Initializing as server");
                    if (_gameManager.FirstPlayer == null) throw new NullReferenceException("firstPlayer is null");
                    if (_gameManager.SecondPlayer == null) throw new NullReferenceException("secondPlayer is null");

                    CurrentPlayer = _gameManager.FirstPlayer;
                    NetworkPlayer = _gameManager.SecondPlayer;
                }
                else
                {
                    Console.WriteLine("Initializing as client");
                    if (_gameManager.FirstPlayer == null) throw new NullReferenceException("firstPlayer is null");
                    if (_gameManager.SecondPlayer == null) throw new NullReferenceException("secondPlayer is null");

                    CurrentPlayer = _gameManager.SecondPlayer;
                    NetworkPlayer = _gameManager.FirstPlayer;
                }

                _handler = handler;
                _handler.OnGetData += OnGetNetworkData;
                Console.WriteLine("Network start data set successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SetNetworkStartData: {ex.Message}");
            }
        }

        private void OnGetNetworkData(object obj)
        {
            try
            {
                ConnectionData networkData = (ConnectionData)obj;

                NetworkPlayer.PositionCenter = new Vector2(networkData.BalloonPositionX, networkData.BalloonPositionY);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnGetNetworkData: {ex.Message}");
            }
        }

        public async Task UpdateNetworkDataAsync()
        {
            try
            {
                var networkData = new ConnectionData
                {
                    BalloonPositionX = CurrentPlayer.PositionCenter.X,
                    BalloonPositionY = CurrentPlayer.PositionCenter.Y
                };

                if (_handler is Server)
                {
                    // Отправляем данные клиенту
                    await _handler.UpdateData(networkData);
                }
                else if (_handler is Client)
                {
                    // Отправляем данные серверу
                    await _handler.UpdateData(networkData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateNetworkDataAsync: {ex.Message}");
            }
        }

        public async void StartServer()
        {
            _server = new Server();
            int seed = new Random().Next();

            _server.OnGetData += (_) => StartGame(seed, _server, true);

            await _server.Start();
            Console.WriteLine("Server started. Client connected.");

            await _server.UpdateData<int>(seed);
        }
        public async void StartClient(TextBox ipAddressInput)
        {
            _client = new Client(ipAddressInput.Text);

            _client.OnGetData += (obj) =>
            {
                Console.WriteLine("Event OnGetData triggered");
                StartGame((int)obj, _client, false);
            };

            await _client.Connect();
            Console.WriteLine("Client connected successfully.");

            await _client.GetData<int>();
        }

        private void StartGame(int seed, ITcpConnectionHandler handler, bool isServer)
        {
            try
            {
                Console.WriteLine("StartGame is called");
                handler.ClearAllListeners(); // Очищаем подписки на события

                Console.WriteLine("Calling SetNetworkStartData");
                SetNetworkStartData(handler, isServer, seed);
                Console.WriteLine("SetNetworkStartData completed");

                // Проверяем, вызывается ли метод HideRoleSelection
                Console.WriteLine("Calling HideRoleSelection");
                if (Application.Current != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _uiManager.HideRoleSelection();
                    });
                }
                else
                {
                    Console.WriteLine("Application.Current is null. Cannot access Dispatcher.");
                }

                Console.WriteLine("HideRoleSelection completed");

                Console.WriteLine("Role Selection is Hided");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _timeManager.LaunchTimers(this);
                    Console.WriteLine("Timers started in UI thread");
                });
                Console.WriteLine("Timers started");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in StartGame: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
