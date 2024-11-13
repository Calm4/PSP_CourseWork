using AmmunitionLibrary;
using GameLibrary;
using GameLibrary.Dirigible;
using GameLibrary.DirigibleDecorators;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenTK.Wpf;
using PrizesLibrary.Prizes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;


namespace DirigibleBattle.Managers
{
    public class GameManager
    {
        public AbstractDirigible FirstPlayer;
        public AbstractDirigible SecondPlayer;

        public List<Bullet> FirstPlayerAmmo;
        public List<Bullet> SecondPlayerAmmo;

        public List<Prize> PrizeList;

        public int NumberOfFirstPlayerPrizes = 0;
        public int NumberOfSecondPlayerPrizes = 0;

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

        public async void GameTimer_Tick(NetworkManager networkManager, object sender, EventArgs e)
        {
            // Проверка состояния игры
            _uiManager.GameStateCheck();

            // Обработка повреждений игроков
            CheckPlayerDamage(FirstPlayerAmmo, ref SecondPlayer);
            CheckPlayerDamage(SecondPlayerAmmo, ref FirstPlayer);

            // Применение призов
            ApplyPrize(PrizeList, ref FirstPlayer, ref NumberOfFirstPlayerPrizes);
            ApplyPrize(PrizeList, ref SecondPlayer, ref NumberOfSecondPlayerPrizes);

            // Управление стрельбой игроками
            PlayerShootControl(CurrentPlayerFire, FirstPlayerAmmo, ref FirstPlayer);
            PlayerShootControl(CurrentPlayerFire, SecondPlayerAmmo, ref SecondPlayer);

            // Управление движением игроков
            networkManager.CurrentPlayer.Idle();
            networkManager. CurrentPlayer.Control(CurrentPlayerInput, TextureManager.firstDirigibleTextureLeft, TextureManager.firstDirigibleTextureRight, ColliderManager.screenBorderCollider);

            // Обновление данных сетевого игрока
            _ = networkManager.UpdateNetworkDataAsync();

            // Обновление интерфейса
            _uiManager.UpdatePlayerStats();

            // Замедление частоты обновлений для предотвращения лагов
            await Task.Delay(40);
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
        public void ApplyPrize(List<Prize> prizeList, ref AbstractDirigible player, ref int prizeCounter)
        {
            for (int i = 0; i < prizeList.Count; i++)
            {
                Prize prize = prizeList[i];

                if (player.GetCollider().IntersectsWith(prize.GetCollider()) && prizeCounter < 15)
                {
                    if (prize.GetType().Equals(typeof(AmmoPrize)))
                    {
                        int ammoBoostCount = random.Next(2, 6);
                        player = new AmmoBoostDecorator(player, ammoBoostCount);
                        prizeCounter++;
                    }
                    if (prize.GetType().Equals(typeof(ArmorPrize)))
                    {
                        int armorBoostCount = random.Next(10, 31);
                        player = new ArmorBoostDecorator(player, armorBoostCount);
                        prizeCounter++;
                    }
                    if (prize.GetType().Equals(typeof(FuelPrize)))
                    {
                        int fuelBoostCount = random.Next(250, 751);
                        player = new FuelBoostDecorator(player, fuelBoostCount);
                        prizeCounter++;
                    }
                    if (prize.GetType().Equals(typeof(HealthPrize)))
                    {
                        int healthBoostCount = random.Next(10, 31);
                        player = new HealthBoostDecorator(player, healthBoostCount);
                        prizeCounter++;
                    }
                    if (prize.GetType().Equals(typeof(SpeedBoostPrize)))
                    {
                        float speedBoostCount = (float)(random.NextDouble() * 0.02 + 0.005);
                        player = new SpeedBoostDecorator(player, speedBoostCount);
                        prizeCounter++;
                    }
                    prizeList.Remove(prize);
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
