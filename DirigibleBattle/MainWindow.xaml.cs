using System;
using System.Collections.Generic;
using System.Windows;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenTK.Wpf;
using System.Drawing;
using System.Windows.Threading;
using AmmunitionLibrary;
using GameLibrary;
using GameLibrary.DirigibleDecorators;
using GameLibrary.Dirigible;
using PrizesLibrary.Factories;
using PrizesLibrary.Prizes;
using TcpConnectionLibrary;
using System.Threading.Tasks;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using DirigibleBattle;
using DirigibleBattle.Managers;



namespace DirigibleBattle
{
    public partial class MainWindow
    {
        private GameManager _gameManager;
        private UIManager _uiManager;
        private RenderManager _renderManager;
        private NetworkManager _networkManager;
        private TimeManager _timeManager;


        public MainWindow()
        {
            InitializeComponent();

            _uiManager = new UIManager(ServerButton, ClientButton, IpAddressInput, GameOverLabel, firstPlayerInfo, secondPlayerInfo);
            _gameManager = new GameManager(glControl,_uiManager);

            _renderManager = new RenderManager(_gameManager);
            _timeManager = new TimeManager(_gameManager);
            _networkManager = new NetworkManager(_gameManager, _uiManager, _timeManager);


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
