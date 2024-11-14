using AmmunitionLibrary;
using GameLibrary;
using GameLibrary.Dirigible;
using OpenTK;
using PrizesLibrary.Factories;
using PrizesLibrary.Prizes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TcpConnectionLibrary;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace DirigibleBattle.Managers
{
    public class NetworkManager
    {
        private AbstractDirigible _firstPlayer;
        private AbstractDirigible _secondPlayer;

        public AbstractDirigible CurrentPlayer { get; set; }
        public AbstractDirigible NetworkPlayer { get; set; }
        public Prize CurrentPrize { get; set; }

        public List<Prize> CurrentPrizeList;

        private ITcpConnectionHandler _handler;

        private Client _client;
        private Server _server;

        private NetworkData _currentNetworkData = new NetworkData();
        private BulletData _bulletData;

        private List<Bullet> _firstAmmos;

        private List<Bullet> _secondAmmos;

        public PrizeFactory PrizeFactory { get; set; }

        private bool _wasAmmoChanged;

        private GameManager _gameManager;
        private UIManager _uiManager;
        private TimeManager _timeManager;

        private Random random;

        public NetworkManager(GameManager gameManager, UIManager uiManager, TimeManager timeManager)
        {
            _gameManager = gameManager;
            _uiManager = uiManager;
            _timeManager = timeManager;
        }

        public void SetNetworkStartData(ITcpConnectionHandler networkHandler, bool isLeftPlayer, int seed)
        {
            _handler = networkHandler;
            _firstPlayer = _gameManager.FirstPlayer;
            _secondPlayer = _gameManager.SecondPlayer;

            if (isLeftPlayer)
            {
                CurrentPlayer = _firstPlayer;
                NetworkPlayer = _secondPlayer;
            }
            else
            {
                CurrentPlayer = _secondPlayer;
                NetworkPlayer = _firstPlayer;
            }
            CurrentPrizeList = _gameManager.PrizeList;

            random = new Random(seed);
            PrizeFactory = new PrizeFactory(random);



            _firstPlayer.SetRandom(random);
            _secondPlayer.SetRandom(random);

            _handler.OnGetData += OnGetNetworkData;
        }

        private void OnGetNetworkData(object obj)
        {
            try
            {
                NetworkData networkData = (NetworkData)obj;

                NetworkPlayer.PositionCenter = new Vector2(networkData.PositionX, networkData.PositionY);

                NetworkPlayer.Health = networkData.Health;
                NetworkPlayer.Armor = networkData.Armor;
                NetworkPlayer.Fuel = networkData.Fuel;
                NetworkPlayer.Ammo = networkData.Ammo;
                NetworkPlayer.Speed = networkData.Speed;
                NetworkPlayer.NumberOfPrizesReceived = networkData.NumberOfPrizesReceived;


                var bulletData = networkData.BulletData;


                if (bulletData == null)
                {
                    Console.WriteLine("Bullet data is NULL!");
                    return;
                }

                if (CurrentPlayer == _firstPlayer)
                {
                    _secondAmmos.Add(_gameManager.CreateNewAmmo(bulletData));
                }
                else
                {
                    _firstAmmos.Add(_gameManager.CreateNewAmmo(bulletData));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnGetNetworkData: {ex.Message}");
            }
        }

        public async Task UpdateNetworkData()
        {
            try
            {
                var positionCenter = CurrentPlayer.PositionCenter;
                _currentNetworkData.PositionX = positionCenter.X;
                _currentNetworkData.PositionY = positionCenter.Y;

                _currentNetworkData.BulletData = _bulletData;

                _currentNetworkData.Health = CurrentPlayer.Health;
                _currentNetworkData.Armor = CurrentPlayer.Armor;
                _currentNetworkData.Fuel = CurrentPlayer.Fuel;
                _currentNetworkData.Ammo = CurrentPlayer.Ammo;
                _currentNetworkData.Speed = CurrentPlayer.Speed;
                _currentNetworkData.NumberOfPrizesReceived = CurrentPlayer.NumberOfPrizesReceived;

                _currentNetworkData.WasAmmoChanged = _wasAmmoChanged;

                await _handler.UpdateData(_currentNetworkData);

                _bulletData = null;
                _wasAmmoChanged = false;
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
