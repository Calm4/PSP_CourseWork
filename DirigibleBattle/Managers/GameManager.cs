﻿using AmmunitionLibrary;
using GameLibrary;
using GameLibrary.Dirigible;
using GameLibrary.DirigibleDecorators;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenTK.Wpf;
using PrizesLibrary.Factories;
using PrizesLibrary.Prizes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;


namespace DirigibleBattle.Managers
{
    public class GameManager
    {
        public AbstractDirigible FirstPlayer;
        public AbstractDirigible SecondPlayer;

        public List<Bullet> FirstPlayerAmmo;
        public List<Bullet> SecondPlayerAmmo;

        public List<Prize> PrizeList;

        private KeyboardState keyboardState;

        private readonly Random random;

        private bool wasFirstPlayerFirePressed = false;
        private bool wasSecondPlayerFirePressed = false;

        private bool isFirstPlayerWindLeft = false;
        private bool isSecondPlayerWindLeft = false;

        private UIManager _uiManager;

        public readonly List<Key> CurrentPlayerInput = new List<Key>()
            {
                Key.W,
                Key.S,
                Key.A,
                Key.D,
            };


        public readonly List<Key> CurrentPlayerFire = new List<Key>()
        {
            Key.Z,
            Key.X,
            Key.C,
        };
        public readonly List<Key> CurrentPlayerFire2 = new List<Key>()
        {
            Key.B,
            Key.N,
            Key.M,
        };

        public GameManager(GLWpfControl glControl, UIManager uiManager)
        {
            _uiManager = uiManager;
            _uiManager.SetGameManager(this);

            random = new Random();
            GameSettings(glControl);

            TextureManager.SetupTexture();
            ColliderManager.SetupColliders();

            SetupGameObjects();
        }

        int currentPlayerTicks = 50;

        public async void GameTimer_Tick(NetworkManager networkManager, object sender, EventArgs e)
        {
            currentPlayerTicks++;

            // Проверка состояния игры
            _uiManager.GameStateCheck();

            // Обработка повреждений игроков
            CheckPlayerDamage(FirstPlayerAmmo, ref SecondPlayer);
            CheckPlayerDamage(SecondPlayerAmmo, ref FirstPlayer);

            // Применение призов
            ApplyPrize(networkManager, PrizeList, ref FirstPlayer);
            ApplyPrize(networkManager, PrizeList, ref SecondPlayer);

            // Управление стрельбой игроками
            PlayerShootControl(CurrentPlayerFire, FirstPlayerAmmo, ref FirstPlayer);
            PlayerShootControl(CurrentPlayerFire2, SecondPlayerAmmo, ref SecondPlayer);

            // Управление движением игроков
            networkManager.CurrentPlayer.Idle();
            networkManager.CurrentPlayer.Control(CurrentPlayerInput, TextureManager.firstDirigibleTextureLeft, TextureManager.firstDirigibleTextureRight, ColliderManager.screenBorderCollider);

            // Обновление данных сетевого игрока
            _ = networkManager.UpdateNetworkData();

            // Обновление интерфейса
            _uiManager.UpdatePlayerStats();

        }

        public void PrizeTimer_Tick(NetworkManager networkManager, object sender, EventArgs e)
        {
            if (networkManager.CurrentPrizeList.Count < 3 && (FirstPlayer.NumberOfPrizesReceived < 15 || SecondPlayer.NumberOfPrizesReceived < 15))
            {
                Prize newPrize = networkManager.PrizeFactory.AddNewPrize();
                networkManager.CurrentPrize = newPrize;
                networkManager.CurrentPrizeList.Add(networkManager.CurrentPrize);
            }
            for (int i = 0; i < PrizeList.Count; i++)
            {
                if (FirstPlayer.NumberOfPrizesReceived >= 15 && SecondPlayer.NumberOfPrizesReceived >= 15)
                {
                    networkManager.CurrentPrizeList.RemoveAt(PrizeList.Count - 1);
                }
            }
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
            FirstPlayerAmmo = new List<Bullet>();
            SecondPlayerAmmo = new List<Bullet>();
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

            return bullet;
        }

        public void PlayerShootControl(List<OpenTK.Input.Key> keys, List<Bullet> bulletsList, ref AbstractDirigible player)
        {
            keyboardState = OpenTK.Input.Keyboard.GetState();

            bool playerFireCommon = keyboardState.IsKeyDown(keys[0]);
            bool playerFireFast = keyboardState.IsKeyDown(keys[1]);
            bool playerFireHeavy = keyboardState.IsKeyDown(keys[2]);

            bool wasPlayerFirePressed = (player == FirstPlayer) ? wasFirstPlayerFirePressed : wasSecondPlayerFirePressed;

            if (!wasPlayerFirePressed && (playerFireCommon || playerFireFast || playerFireHeavy))
            {
                if (player.Ammo > 0)
                {
                    if (playerFireCommon)
                    {
                        bulletsList.Add(new CommonBullet(player.GetGunPosition() - new Vector2(0f, -0.05f), TextureManager.commonBulletTexture, player.DirigibleID == TextureManager.firstDirigibleTextureRight));
                    }
                    if (playerFireFast)
                    {
                        bulletsList.Add(new FastBullet(player.GetGunPosition() - new Vector2(0f, -0.05f), TextureManager.fastBulletTexture, player.DirigibleID == TextureManager.firstDirigibleTextureRight));
                    }
                    if (playerFireHeavy)
                    {
                        bulletsList.Add(new HeavyBullet(player.GetGunPosition() - new Vector2(0f, -0.05f), TextureManager.heavyBulletTexture, player.DirigibleID == TextureManager.firstDirigibleTextureRight));
                    }
                    player.Ammo--;
                }
                if (player == FirstPlayer)
                {
                    wasFirstPlayerFirePressed = true;
                }
                else
                {
                    wasSecondPlayerFirePressed = true;
                }
            }
            else if (wasPlayerFirePressed && !(playerFireCommon || playerFireFast || playerFireHeavy))
            {
                if (player == FirstPlayer)
                {
                    wasFirstPlayerFirePressed = false;
                }
                else
                {
                    wasSecondPlayerFirePressed = false;
                }
            }
        }
        public void ApplyPrize(NetworkManager networkManager, List<Prize> prizeList, ref AbstractDirigible player)
        {
            for (int i = 0; i < prizeList.Count; i++)
            {
                networkManager.CurrentPrize = prizeList[i];


                if (player.GetCollider().IntersectsWith(networkManager.CurrentPrize.GetCollider()) && player.NumberOfPrizesReceived < 15)
                {
                    if (networkManager.CurrentPrize.GetType().Equals(typeof(AmmoPrize)))
                    {
                        int ammoBoostCount = random.Next(2, 6);
                        player = new AmmoBoostDecorator(player, ammoBoostCount);
                        player.NumberOfPrizesReceived++;
                    }
                    if (networkManager.CurrentPrize.GetType().Equals(typeof(ArmorPrize)))
                    {
                        int armorBoostCount = random.Next(10, 31);
                        player = new ArmorBoostDecorator(player, armorBoostCount);
                        player.NumberOfPrizesReceived++;
                    }
                    if (networkManager.CurrentPrize.GetType().Equals(typeof(FuelPrize)))
                    {
                        int fuelBoostCount = random.Next(250, 751);
                        player = new FuelBoostDecorator(player, fuelBoostCount);
                        player.NumberOfPrizesReceived++;
                    }
                    if (networkManager.CurrentPrize.GetType().Equals(typeof(HealthPrize)))
                    {
                        int healthBoostCount = random.Next(10, 31);
                        player = new HealthBoostDecorator(player, healthBoostCount);
                        player.NumberOfPrizesReceived++;
                    }
                    if (networkManager.CurrentPrize.GetType().Equals(typeof(SpeedBoostPrize)))
                    {
                        float speedBoostCount = (float)(random.NextDouble() * 0.02 + 0.005);
                        player = new SpeedBoostDecorator(player, speedBoostCount);
                        player.NumberOfPrizesReceived++;
                    }
                    prizeList.Remove(networkManager.CurrentPrize);
                    i--;
                }
            }
        }

        public void CheckPlayerDamage(List<Bullet> bulletList, ref AbstractDirigible player)
        {
            for (int i = bulletList.Count - 1; i >= 0; i--)
            {
                bulletList[i].Fire();

                if (player.GetCollider().IntersectsWith(bulletList[i].GetCollider()))
                {
                    player.GetDamage(bulletList[i].Damage);
                    bulletList.RemoveAt(i);
                    continue;
                }
                if (!bulletList[i].GetCollider().IntersectsWith(ColliderManager.screenBorderCollider))
                {
                    bulletList.RemoveAt(i);
                }
            }
        }

        public void WindDirection()
        {
            if ((FirstPlayer.GetCollider().X <= ColliderManager.screenBorderCollider.X) && !isFirstPlayerWindLeft)
            {
                FirstPlayer.ChangeWindDirection(false);
            }
            else if ((FirstPlayer.GetCollider().X + FirstPlayer.GetCollider().Width >= ColliderManager.screenBorderCollider.X + ColliderManager.screenBorderCollider.Width - 0.04f) && !isFirstPlayerWindLeft)
            {
                FirstPlayer.ChangeWindDirection(false);
            }
            else
                FirstPlayer.ChangeWindDirection(true);
            if ((SecondPlayer.GetCollider().X <= ColliderManager.screenBorderCollider.X) && !isSecondPlayerWindLeft)
            {
                SecondPlayer.ChangeWindDirection(false);
            }
            else if ((SecondPlayer.GetCollider().X + SecondPlayer.GetCollider().Width >= ColliderManager.screenBorderCollider.X + ColliderManager.screenBorderCollider.Width - 0.04f) && !isSecondPlayerWindLeft) // || 
            {
                SecondPlayer.ChangeWindDirection(false);
            }
            else
                SecondPlayer.ChangeWindDirection(true);
        }
    }
}
