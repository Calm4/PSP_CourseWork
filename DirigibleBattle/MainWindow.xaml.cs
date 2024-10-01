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



namespace DirigibleBattle
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Объект первого игрока
        /// </summary>
        AbstractDirigible firstPlayer;
        /// <summary>
        /// Объект второго игрока
        /// </summary>
        AbstractDirigible secondPlayer;

        /// <summary>
        /// Амуниция превого игрока
        /// </summary>
        List<Bullet> firstPlayerAmmo;
        /// <summary>
        /// Амуниция второго игрока
        /// </summary>
        List<Bullet> secondPlayerAmmo;

        /// <summary>
        /// Проверка на нажат ли выстрел у первого игрока
        /// </summary>
        bool wasFirstPlayerFirePressed = false;
        /// <summary>
        /// Проверка на нажат ли выстрел у второго игрока
        /// </summary>
        bool wasSecondPlayerFirePressed = false;

        /// <summary>
        /// Игровой таймер
        /// </summary>
        DispatcherTimer gameTimer;
        /// <summary>
        /// Таймер призов
        /// </summary>
        DispatcherTimer prizeTimer;
        /// <summary>
        /// Таймер ветра
        /// </summary>
        DispatcherTimer windTimer;

        /// <summary>
        /// Фабрика призов
        /// </summary>
        readonly PrizeFactory prizeFactory = new PrizeFactory();
        /// <summary>
        /// Список призов
        /// </summary>
        readonly List<Prize> prizeList = new List<Prize>();
        /// <summary>
        /// Генератор для рандомных значений
        /// </summary>
        readonly Random random = new Random();

        /// <summary>
        /// Коллайдер горного массива
        /// </summary>
        RectangleF mountineCollider;
        /// <summary>
        /// Коллайдер игрового окна
        /// </summary>
        RectangleF screenBorderCollider;
        /// <summary>
        /// Клавиатурные состояния
        /// </summary>
        KeyboardState keyboardState;

        /// <summary>
        /// Текстура заднего фона
        /// </summary>
        int backGroundTexture;
        /// <summary>
        /// Текстура горного массива
        /// </summary>
        int mountainRange;

        /// <summary>
        /// Количество призов у первого игрока
        /// </summary>
        private int numberOfFirstPlayerPrizes = 0;
        /// <summary>
        /// Количество призов у второго игрока
        /// </summary>
        private int numberOfSecondPlayerPrizes = 0;

        /// <summary>
        /// Текстура простой пули
        /// </summary>
        int commonBulletTexture;
        /// <summary>
        /// Текстура быстрой пули
        /// </summary>
        int fastBulletTexture;
        /// <summary>
        /// Текстура тяжелой пули
        /// </summary>
        int heavyBulletTexture;
        /// <summary>
        /// Текстура первого дирижабля смотрящего вправо
        /// </summary>
        int firstDirigibleTextureRight;
        /// <summary>
        /// Текстура первого дирижабля смотрящего влево
        /// </summary>
        int firstDirigibleTextureLeft;
        /// <summary>
        /// Текстура второго дирижабля смотрящего вправо
        /// </summary>
        int secondDirigibleTextureRight;
        /// <summary>
        /// Текстура второго дирижабля смотрящего влево
        /// </summary>
        int secondDirigibleTextureLeft;

        /// <summary>
        /// Проверка состояния ветра первого игрока
        /// </summary>
        readonly private bool isFirstPlayerWindLeft = false; // true - ветер дует налево, false - направо

        /// <summary>
        /// Проверка состояния ветра второго игрока
        /// </summary>
        readonly private bool isSecondPlayerWindLeft = false;

        /// <summary>
        /// Время действия ветра в тиках
        /// </summary>
        private int windCounter = 0;
        /// <summary>
        /// Скорость ветра
        /// </summary>
        private float windSpeedPlayer = 0.0f;

        /// <summary>
        /// Тики ветров
        /// </summary>
        private int windTimerTicks = 50;

        /// <summary>
        /// Список кнопок для первого игрока
        /// </summary>
        readonly List<OpenTK.Input.Key> firstPlayerInput = new List<OpenTK.Input.Key>()
            {
                OpenTK.Input.Key.W,
                OpenTK.Input.Key.S,
                OpenTK.Input.Key.A,
                OpenTK.Input.Key.D,
            };
        /// <summary>
        /// Список кнопок для второго игрока
        /// </summary>
        readonly List<OpenTK.Input.Key> secondPlayerInput = new List<OpenTK.Input.Key>()
            {
                OpenTK.Input.Key.Up,
                OpenTK.Input.Key.Down,
                OpenTK.Input.Key.Left,
                OpenTK.Input.Key.Right,
            };
        /// <summary>
        /// Список кнопок для стрельбы первого игрока
        /// </summary>
        readonly List<OpenTK.Input.Key> firstPlayerFire = new List<OpenTK.Input.Key>()
        {
            OpenTK.Input.Key.Z,
            OpenTK.Input.Key.X,
            OpenTK.Input.Key.C,
        };
        /// <summary>
        /// Список кнопок для стрельбы второго игрока
        /// </summary>
        readonly List<OpenTK.Input.Key> secondPlayerFire = new List<OpenTK.Input.Key>()
        {
            OpenTK.Input.Key.Insert,
            OpenTK.Input.Key.PageUp,
            OpenTK.Input.Key.PageDown,
        };

        /// <summary>
        /// Конструктор окна MainWindow
        /// </summary>
        public MainWindow()
        {

            InitializeComponent();
            GameSettings();
            AddTexture();
            AddObjects();
            StartTimer();
        }

        /// <summary>
        /// Задает настройки запускаемого проекта
        /// </summary>
        private void GameSettings()
        {
            var settings = new GLWpfControlSettings { MajorVersion = 3, MinorVersion = 6 };
            glControl.Start(settings);
            glControl.InvalidateVisual();
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }
        /// <summary>
        /// Добавление на текстуры картинки
        /// </summary>
        private void AddTexture()
        {
            firstDirigibleTextureRight = CreateTexture.LoadTexture("dirigible_red_right_side.png");
            firstDirigibleTextureLeft = CreateTexture.LoadTexture("dirigible_red_left_side.png");
            secondDirigibleTextureRight = CreateTexture.LoadTexture("dirigible_blue_right_side.png");
            secondDirigibleTextureLeft = CreateTexture.LoadTexture("dirigible_blue_left_side.png");
            commonBulletTexture = CreateTexture.LoadTexture("CommonBullet.png");
            fastBulletTexture = CreateTexture.LoadTexture("FastBullet.png");
            heavyBulletTexture = CreateTexture.LoadTexture("HeavyBullet.png");
            backGroundTexture = CreateTexture.LoadTexture("clouds2.png");
            mountainRange = CreateTexture.LoadTexture("mountine2.png");
        }
        /// <summary>
        /// Инициализация объектов
        /// </summary>
        private void AddObjects()
        {
            firstPlayer = new BasicDirigible(new Vector2(-0.6f, -0.4f), firstDirigibleTextureRight);
            secondPlayer = new BasicDirigible(new Vector2(0.5f, 0f), secondDirigibleTextureLeft);
            firstPlayerAmmo = new List<Bullet>();
            secondPlayerAmmo = new List<Bullet>();
            screenBorderCollider = new RectangleF(0f, 0.125f, 1.025f, 0.875f);
            mountineCollider = new RectangleF(0.0f, -0.1f, 1.0f, 0.185f);
        }
        /// <summary>
        /// Запуск таймеров
        /// </summary>
        private void StartTimer()
        {
            gameTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(8.0) };
            gameTimer.Tick += GameTimer_Tick;

            prizeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(8.0) };
            prizeTimer.Tick += PrizeTimer_Tick;

            windTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(8.0) };
            windTimer.Tick += WindTimer_Tick;

            gameTimer.Start();
            prizeTimer.Start();
            windTimer.Start();
        }

        /// <summary>
        /// Таймер ветра
        /// </summary>
        private void WindTimer_Tick(object sender, EventArgs e)
        {
            if (windCounter <= windTimerTicks)
            {
                windSpeedPlayer = (float)(random.NextDouble() * (0.005f - 0.0001f) + 0.0001f);

                firstPlayer.ChangeDirectionWithWind(new Vector2(windSpeedPlayer, 0.0f));
                firstPlayer.ChangeWindDirection(true);
                secondPlayer.ChangeDirectionWithWind(new Vector2(windSpeedPlayer, 0.0f));
                secondPlayer.ChangeWindDirection(true);
                windCounter++;
            }
            else if (windCounter >= (windTimerTicks + 1) && windCounter <= windTimerTicks * 2)
            {
                windSpeedPlayer = (float)(random.NextDouble() * (0.005f - 0.0001f) + 0.0001f);

                firstPlayer.ChangeDirectionWithWind(new Vector2(-windSpeedPlayer, 0.0f));
                firstPlayer.ChangeWindDirection(true);
                secondPlayer.ChangeDirectionWithWind(new Vector2(-windSpeedPlayer, 0.0f));
                secondPlayer.ChangeWindDirection(true);
                windCounter++;
            }
            else
            {
                windCounter = 0;
                windTimerTicks = random.Next(100, 301);
            }
            WindDirection();
        }
        /// <summary>
        /// Таймер призов
        /// </summary>
        private void PrizeTimer_Tick(object sender, EventArgs e)
        {
            if (prizeList.Count < 3 && (numberOfFirstPlayerPrizes < 15 || numberOfSecondPlayerPrizes < 15))
            {
                prizeList.Add(prizeFactory.AddNewPrize());
            }
            for (int i = 0; i < prizeList.Count; i++)
            {
                if (numberOfFirstPlayerPrizes >= 15 && numberOfSecondPlayerPrizes >= 15)
                {
                    prizeList.RemoveAt(prizeList.Count - 1);
                }
            }
        }
        /// <summary>
        /// Игровой таймер
        /// </summary>
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            // что бы игра не была бесконечной, игрок может подобрать только до 15 призов
            GameStateCheck();

            CheckPlayerDamage(firstPlayerAmmo, ref secondPlayer);
            CheckPlayerDamage(secondPlayerAmmo, ref firstPlayer);

            ApplyPrize(prizeList, ref firstPlayer, ref numberOfFirstPlayerPrizes);
            ApplyPrize(prizeList, ref secondPlayer, ref numberOfSecondPlayerPrizes);


            PlayerShootControl(firstPlayerFire, firstPlayerAmmo, ref firstPlayer);
            PlayerShootControl(secondPlayerFire, secondPlayerAmmo, ref secondPlayer);

            firstPlayer.Idle();
            secondPlayer.Idle();

            firstPlayer.Control(firstPlayerInput, firstDirigibleTextureLeft, firstDirigibleTextureRight, screenBorderCollider);
            secondPlayer.Control(secondPlayerInput, secondDirigibleTextureLeft, secondDirigibleTextureRight, screenBorderCollider);

            firstPlayerInfo.Content = $"HP:{firstPlayer.Health}/200\nArmor:{firstPlayer.Armor}/50\n" +
                             $"Ammo:{firstPlayer.Ammo}/30\nSpeed:{firstPlayer.Speed * 10f:F1}x/1.5x\n" +
                             $"Fuel:{firstPlayer.Fuel}/3000\nPrizes:{numberOfFirstPlayerPrizes}/15\n";

            secondPlayerInfo.Content = $"HP:{secondPlayer.Health}/200\nArmor:{secondPlayer.Armor}/50\n" +
                                        $"Ammo:{secondPlayer.Ammo}/30\nSpeed:{secondPlayer.Speed * 10f:F1}x/1.5x\n" +
                                        $"Fuel:{secondPlayer.Fuel}/3000\nPrizes:{numberOfSecondPlayerPrizes}/15\n";

        }
        /// <summary>
        /// Направление ветра
        /// </summary>
        private void WindDirection()
        {
            if ((firstPlayer.GetCollider().X <= screenBorderCollider.X) && !isFirstPlayerWindLeft)
            {
                firstPlayer.ChangeWindDirection(false);
            }
            else if ((firstPlayer.GetCollider().X + firstPlayer.GetCollider().Width >= screenBorderCollider.X + screenBorderCollider.Width - 0.04f) && !isFirstPlayerWindLeft)
            {
                firstPlayer.ChangeWindDirection(false);
            }
            else
                firstPlayer.ChangeWindDirection(true);
            if ((secondPlayer.GetCollider().X <= screenBorderCollider.X) && !isSecondPlayerWindLeft)
            {
                secondPlayer.ChangeWindDirection(false);
            }
            else if ((secondPlayer.GetCollider().X + secondPlayer.GetCollider().Width >= screenBorderCollider.X + screenBorderCollider.Width - 0.04f) && !isSecondPlayerWindLeft) // || 
            {
                secondPlayer.ChangeWindDirection(false);
            }
            else
                secondPlayer.ChangeWindDirection(true);
        }
        /// <summary>
        /// Применение призов
        /// </summary>
        /// <param name="prizeList">Список призов</param>
        /// <param name="player">Игрок</param>
        /// <param name="prizeCounter">Количество призов</param>
        private void ApplyPrize(List<Prize> prizeList, ref AbstractDirigible player, ref int prizeCounter)
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

        /// <summary>
        /// Стрельба игрока
        /// </summary>
        /// <param name="keys">Набор кнопок для стрельбы</param>
        /// <param name="bulletsList">Список пуль</param>
        /// <param name="player">Игрок</param>
        private void PlayerShootControl(List<OpenTK.Input.Key> keys, List<Bullet> bulletsList, ref AbstractDirigible player)
        {
            keyboardState = OpenTK.Input.Keyboard.GetState();

            bool playerFireCommon = keyboardState.IsKeyDown(keys[0]);
            bool playerFireFast = keyboardState.IsKeyDown(keys[1]);
            bool playerFireHeavy = keyboardState.IsKeyDown(keys[2]);

            bool wasPlayerFirePressed = (player == firstPlayer) ? wasFirstPlayerFirePressed : wasSecondPlayerFirePressed;

            if (!wasPlayerFirePressed && (playerFireCommon || playerFireFast || playerFireHeavy))
            {
                if (player.Ammo > 0)
                {
                    if (playerFireCommon)
                    {
                        bulletsList.Add(new CommonBullet(player.GetGunPosition() - new Vector2(0f, -0.05f), commonBulletTexture, player.DirigibleID == firstDirigibleTextureRight));
                    }
                    if (playerFireFast)
                    {
                        bulletsList.Add(new FastBullet(player.GetGunPosition() - new Vector2(0f, -0.05f), fastBulletTexture, player.DirigibleID == firstDirigibleTextureRight));
                    }
                    if (playerFireHeavy)
                    {
                        bulletsList.Add(new HeavyBullet(player.GetGunPosition() - new Vector2(0f, -0.05f), heavyBulletTexture, player.DirigibleID == firstDirigibleTextureRight));
                    }
                    player.Ammo--;
                }
                if (player == firstPlayer)
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
                if (player == firstPlayer)
                {
                    wasFirstPlayerFirePressed = false;
                }
                else
                {
                    wasSecondPlayerFirePressed = false;
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
                if (!bulletList[i].GetCollider().IntersectsWith(screenBorderCollider))
                {
                    bulletList.RemoveAt(i);
                }
            }
        }
        /// <summary>
        /// Проверка состояния игры
        /// </summary>
        private void GameStateCheck()
        {

            if (firstPlayer.GetCollider().IntersectsWith(secondPlayer.GetCollider()))
            {
                gameTimer.Stop();

                MessageBox.Show("НИЧЬЯ", "ИГРА ОКОНЧЕНА", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }

            if (mountineCollider.IntersectsWith(firstPlayer.GetCollider()))
            {
                gameTimer.Stop();
                MessageBox.Show("ПОБЕДИЛ ИГРОК НА [СИНЕМ] ДИРИЖАБЛЕ\n\tИГРОК НА [КРАСНОМ] ДИРИЖАБЛЕ ВРЕЗАЛСЯ В ГОРУ", "ИГРА ОКОНЧЕНА",
                                        MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            if (firstPlayer.Health <= 0)
            {
                gameTimer.Stop();
                MessageBox.Show("\tПОБЕДИЛ ИГРОК НА [СИНЕМ] ДИРИЖАБЛЕ", "ИГРА ОКОНЧЕНА",
                                        MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            if (mountineCollider.IntersectsWith(secondPlayer.GetCollider()))
            {
                gameTimer.Stop();
                MessageBox.Show("ПОБЕДИЛ ИГРОК НА [КРАСНОМ] ДИРИЖАБЛЕ\n\tИГРОК НА [СИНЕМ] ДИРИЖАБЛЕ ВРЕЗАЛСЯ В ГОРУ", "ИГРА ОКОНЧЕНА",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            if (secondPlayer.Health <= 0)
            {
                gameTimer.Stop();
                MessageBox.Show("\tПОБЕДИЛ ИГРОК НА [КРАСНОМ] ДИРИЖАБЛЕ", "ИГРА ОКОНЧЕНА",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
        }
        /// <summary>
        /// Рендер при помощи GLControl
        /// </summary>
        /// <param name="obj">Объект</param>
        private void GlControl_Render(TimeSpan obj)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            ObjectRenderer.RenderObjects(backGroundTexture, new Vector2[4] {
                new Vector2(-1.0f, -1.0f),
                new Vector2(1.0f, -1.0f),
                new Vector2(1.0f, 1.0f),
                new Vector2(-1.0f, 1.0f),
            });

            ObjectRenderer.RenderObjects(mountainRange, new Vector2[4] {
                new Vector2(-1.0f, 0.775f),
                new Vector2(1.0f, 0.775f),
                new Vector2(1.0f, 1.0f),
                new Vector2(-1.0f, 1f),
            });
            firstPlayer.Render();
            secondPlayer.Render();

            foreach (Bullet bullet in firstPlayerAmmo)
            {
                bullet.Render();
            }
            foreach (Bullet bullet in secondPlayerAmmo)
            {
                bullet.Render();
            }

            foreach (Prize prize in prizeList)
            {
                prize.Render();
            }
        }
    }
}
