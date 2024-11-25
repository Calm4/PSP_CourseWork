using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GameLibrary.Dirigible;
using System.Windows.Threading;
using System.Windows.Controls;

namespace DirigibleBattle.Managers
{
    public class UIManager
    {
        private Label _firstPlayerInfo;
        private Label _secondPlayerInfo;

        private Button _serverButton;
        private Button _clientButton;
        private TextBox _ipAddressInput;

        private Label _gameOverLabel;

        private GameManager _gameManager;

        public void SetGameMaangers(GameManager gameManager)
        {
            _gameManager = gameManager;
        }


        public UIManager(Button serverButton, Button clientButton, TextBox ipAddressInput, Label gameOverLabel, Label firstPlayerInfo, Label secondPlayerInfo)
        {
            _serverButton = serverButton;
            _clientButton = clientButton;
            _ipAddressInput = ipAddressInput;
            _gameOverLabel = gameOverLabel;
            _firstPlayerInfo = firstPlayerInfo;
            _secondPlayerInfo = secondPlayerInfo;
            _gameOverLabel.Visibility = Visibility.Hidden;

        }
        public void DisplayLocalIPAddress(Label labelIp, TextBox textBoxIp)
        {
            string localIp = GetLocalIPAddress();
            labelIp.Content = $"IP Address: {localIp}";
            textBoxIp.Text = localIp;
        }

        public string GetLocalIPAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                var address = host.AddressList.FirstOrDefault(addr => addr.AddressFamily == AddressFamily.InterNetwork);
                return address?.ToString() ?? "IP not found";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting local IP address: {ex.Message}");
                return "IP not found";
            }
        }

        public void GameStateCheck(MainWindow mainWindow)
        {

            if (_gameManager.FirstPlayer.GetCollider().IntersectsWith(_gameManager.SecondPlayer.GetCollider()))
            {
                EndGame("НИЧЬЯ", _gameOverLabel, mainWindow);
            }
            else if (ColliderManager.mountineCollider.IntersectsWith(_gameManager.FirstPlayer.GetCollider()))
            {
                EndGame("ПОБЕДИЛ ИГРОК НА [СИНЕМ] ДИРИЖАБЛЕ\nИГРОК НА [КРАСНОМ] ВРЕЗАЛСЯ В ГОРУ", _gameOverLabel, mainWindow);
            }
            else if (_gameManager.FirstPlayer.Health <= 0)
            {
                EndGame("ПОБЕДИЛ ИГРОК НА [СИНЕМ] ДИРИЖАБЛЕ", _gameOverLabel, mainWindow);
            }
            else if (ColliderManager.mountineCollider.IntersectsWith(_gameManager.SecondPlayer.GetCollider()))
            {
                EndGame("ПОБЕДИЛ ИГРОК НА [КРАСНОМ] ДИРИЖАБЛЕ\nИГРОК НА [СИНЕМ] ВРЕЗАЛСЯ В ГОРУ", _gameOverLabel, mainWindow);
            }
            else if (_gameManager.SecondPlayer.Health <= 0)
            {
                EndGame("ПОБЕДИЛ ИГРОК НА [КРАСНОМ] ДИРИЖАБЛЕ", _gameOverLabel, mainWindow);
            }
        }

        private void EndGame(string resultMessage, Label gameOverLabel, MainWindow mainWindow)
        {
            //gameOverLabel.Content = resultMessage;
            gameOverLabel.Visibility = Visibility.Visible;
            mainWindow.Close();
            MessageBox.Show(resultMessage);
        }

        public void UpdatePlayerStats()
        {
            _firstPlayerInfo.Content = $"HP:{_gameManager.FirstPlayer.Health}/200\nArmor:{_gameManager.FirstPlayer.Armor}/50\n" +
                         $"Ammo:{_gameManager.FirstPlayer.Ammo}/30\nSpeed:{_gameManager.FirstPlayer.Speed * 10f:F1}x/1.5x\n" +
                         $"Fuel:{_gameManager.FirstPlayer.Fuel}/3000\nPrizes:{_gameManager.FirstPlayer.NumberOfPrizesReceived}/15\n";

            _secondPlayerInfo.Content = $"HP:{_gameManager.SecondPlayer.Health}/200\nArmor:{_gameManager.SecondPlayer.Armor}/50\n" +
                                        $"Ammo:{_gameManager.SecondPlayer.Ammo}/30\nSpeed:{_gameManager.SecondPlayer.Speed * 10f:F1}x/1.5x\n" +
                                        $"Fuel:{_gameManager.SecondPlayer.Fuel}/3000\nPrizes:{_gameManager.SecondPlayer.NumberOfPrizesReceived}/15\n";
        }

        public void HideRoleSelection()
        {
            try
            {
                Console.WriteLine("Hiding Role Selection UI elements");
                if (_ipAddressInput == null || _serverButton == null || _clientButton == null)
                {
                    Console.WriteLine("UI elements are null");
                    return;
                }

                _ipAddressInput.Visibility = Visibility.Collapsed;
                _serverButton.Visibility = Visibility.Collapsed;
                _clientButton.Visibility = Visibility.Collapsed;
                _gameOverLabel.Visibility = Visibility.Collapsed;
                Console.WriteLine("Role Selection UI elements are hidden");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HideRoleSelection: {ex.Message}");
            }
        }
    }
}
