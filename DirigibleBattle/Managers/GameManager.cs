﻿using AmmunitionLibrary;
using GameLibrary;
using GameLibrary.Dirigible;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Wpf;
using PrizesLibrary.Prizes;
using System;
using System.Collections.Generic;


namespace DirigibleBattle.Managers
{
    public class GameManager
    {
        public AbstractDirigible FirstPlayer;
        public AbstractDirigible SecondPlayer;

        public List<Prize> PrizeList;

        private UIManager _uiManager;
        private PlayerManager _playerManager;
        private PrizeManager _prizeManager;
        private WindManager _windManager;
        private MainWindow _mainWindow;

        public GameManager(GLWpfControl glControl,MainWindow mainWindow, UIManager uiManager, PlayerManager playerManager, PrizeManager prizeManager, WindManager windManager)
        {
            _playerManager = playerManager;
            _prizeManager = prizeManager;
            _windManager = windManager;

            _mainWindow = mainWindow;

            _uiManager = uiManager;
           

            GameSettings(glControl);

            TextureManager.SetupTexture();
            ColliderManager.SetupColliders();

            SetupGameObjects();
        }

        

        public async void GameTimer_Tick(NetworkManager networkManager, object sender, EventArgs e)
        {
            // Проверка состояния игры
            _uiManager.GameStateCheck(_mainWindow);

            // Обработка повреждений игроков
            _playerManager.CheckPlayerDamage(networkManager._firstPlayerBulletList, ref SecondPlayer);
            _playerManager.CheckPlayerDamage(networkManager._secondPlayerBulletList, ref FirstPlayer);

            // Применение призов
            _prizeManager.ApplyPrize(PrizeList, ref FirstPlayer);
            _prizeManager.ApplyPrize(PrizeList, ref SecondPlayer);

            _playerManager.PlayerShoot();
            _uiManager.UpdatePlayerStats();

            // Управление движением игроков
            networkManager.CurrentPlayer.Idle();

            if (networkManager.CurrentPlayer == FirstPlayer)
            {
                _playerManager.PlayerControl(TextureManager.firstDirigibleTextureLeft, TextureManager.firstDirigibleTextureRight);
                _playerManager.UpdatePlayerTexture();
            }
            else
            {
                _playerManager.PlayerControl(TextureManager.secondDirigibleTextureLeft, TextureManager.secondDirigibleTextureLeft);
                _playerManager.UpdatePlayerTexture();
            }

            _ = networkManager.UpdateNetworkData();

        }

        

        private void GameSettings(GLWpfControl glControl)
        {
            var settings = new GLWpfControlSettings { MajorVersion = 3, MinorVersion = 6 };
            glControl.Start(settings);
            glControl.InvalidateVisual();
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        private void SetupGameObjects()
        {
            FirstPlayer = new BasicDirigible(new Vector2(-0.6f, -0.4f), TextureManager.firstDirigibleTextureRight);
            SecondPlayer = new BasicDirigible(new Vector2(0.5f, 0f), TextureManager.secondDirigibleTextureLeft);

            PrizeList = new List<Prize>();
        }

        public Bullet CreateNewAmmo(BulletData bulletData)
        {
            Bullet bullet = null;
            switch (bulletData.BulletType)
            {
                case 0:
                    bullet = new CommonBullet(new Vector2(bulletData.PositionX, bulletData.PositionY), TextureManager.commonBulletTexture, bulletData.IsLeft);
                    break;
                case 1:
                    bullet = new FastBullet(new Vector2(bulletData.PositionX, bulletData.PositionY), TextureManager.fastBulletTexture, bulletData.IsLeft);
                    break;
                case 2:
                    bullet = new HeavyBullet(new Vector2(bulletData.PositionX, bulletData.PositionY), TextureManager.heavyBulletTexture, bulletData.IsLeft);

                    break;
            }

            Console.WriteLine($"Bullet created with damage: {bullet.Damage}");


            return bullet;
        }
        
    }
}
