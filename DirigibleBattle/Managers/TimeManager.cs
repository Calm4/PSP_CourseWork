using OpenTK;
using PrizesLibrary.Factories;
using PrizesLibrary.Prizes;
using System;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace DirigibleBattle.Managers
{
    public class TimeManager
    {

        DispatcherTimer gameTimer;
        DispatcherTimer prizeTimer;
        DispatcherTimer windTimer;

        private GameManager _gameManager;
        private PrizeManager _prizeManager;
        private WindManager _windManager;

        private Random random;
        private PrizeFactory prizeFactory;

        private int windCounter = 0;
        private float windSpeedPlayer = 0.0f;

        private int windTimerTicks = 50;


        public TimeManager(GameManager gameManager, PrizeManager prizeManager, WindManager windManager)
        {
            random = new Random();
            prizeFactory = new PrizeFactory(random);
            
            _gameManager = gameManager;
            _prizeManager = prizeManager;
            _windManager = windManager;
        }

        public void LaunchTimers(NetworkManager networkManager)
        {
            Console.WriteLine("Initializing timers...");

            gameTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(25.0) };
            gameTimer.Tick += (sender, e) => _gameManager.GameTimer_Tick(networkManager, sender, e);
            Console.WriteLine("Game timer initialized");

            prizeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(25.0) };
            prizeTimer.Tick += _prizeManager.PrizeTimer_Tick;
            Console.WriteLine("Prize timer initialized");

            windTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(25.0) };
            windTimer.Tick += WindTimer_Tick;
            Console.WriteLine("Wind timer initialized");

            gameTimer.Start();
            Console.WriteLine("Game timer started");

            prizeTimer.Start();
            Console.WriteLine("Prize timer started");

            windTimer.Start();
            Console.WriteLine("Wind timer started");
        }

        private void WindTimer_Tick(object sender, EventArgs e)
        {
            if (windCounter <= windTimerTicks)
            {
                windSpeedPlayer = (float)(random.NextDouble() * (0.005f - 0.0001f) + 0.0001f);

                _gameManager.FirstPlayer.ChangeDirectionWithWind(new Vector2(windSpeedPlayer, 0.0f));
                _gameManager.FirstPlayer.ChangeWindDirection(true);
                _gameManager.SecondPlayer.ChangeDirectionWithWind(new Vector2(windSpeedPlayer, 0.0f));
                _gameManager.SecondPlayer.ChangeWindDirection(true);
                windCounter++;
            }
            else if (windCounter >= (windTimerTicks + 1) && windCounter <= windTimerTicks * 2)
            {
                windSpeedPlayer = (float)(random.NextDouble() * (0.005f - 0.0001f) + 0.0001f);

                _gameManager.FirstPlayer.ChangeDirectionWithWind(new Vector2(-windSpeedPlayer, 0.0f));
                _gameManager.FirstPlayer.ChangeWindDirection(true);
                _gameManager.SecondPlayer.ChangeDirectionWithWind(new Vector2(-windSpeedPlayer, 0.0f));
                _gameManager.SecondPlayer.ChangeWindDirection(true);
                windCounter++;
            }
            else
            {
                windCounter = 0;
                windTimerTicks = random.Next(100, 301);
            }
            _windManager.WindDirection();
        }

        
    }
}
