using System;
using System.Windows;
using DirigibleBattle.Managers;



namespace DirigibleBattle
{
    public partial class MainWindow
    {
        private PlayerManager _playerManager;
        private PrizeManager _prizeManager;
        private WindManager _windManager;
        private GameManager _gameManager;
        private UIManager _uiManager;
        private RenderManager _renderManager;
        private NetworkManager _networkManager;
        private TimeManager _timeManager;


        public MainWindow()
        {
            InitializeComponent();

            _uiManager = new UIManager(ServerButton, ClientButton, IpAddressInput, GameOverLabel, firstPlayerInfo, secondPlayerInfo);

            _playerManager = new PlayerManager();
            _prizeManager = new PrizeManager();
            _windManager = new WindManager();

            _gameManager = new GameManager(glControl, this, _uiManager, _playerManager, _prizeManager, _windManager);
            _timeManager = new TimeManager(_gameManager, _prizeManager, _windManager);

            _networkManager = new NetworkManager(_gameManager, _uiManager, _timeManager, _playerManager);
            _playerManager.SetManagers(_networkManager, _gameManager);
            _prizeManager.SetManagers(_networkManager, _gameManager);
            _windManager.SetManagers(_networkManager, _gameManager);

            _renderManager = new RenderManager(_gameManager, _networkManager);

            _uiManager.DisplayLocalIPAddress(IpAddressLabel, IpAddressInput);

        }

        private void ServerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _networkManager.StartServer();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting server: {ex.Message}");
            }
        }
        private void ClientButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _networkManager.StartClient(IpAddressInput);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting as client: {ex.Message}");
            }
        }

        private void GlControl_Render(TimeSpan obj)
        {
            _renderManager.GlControl_Render(obj);
        }
    }
}
