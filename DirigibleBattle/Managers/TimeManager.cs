using OpenTK;
using PrizesLibrary.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DirigibleBattle.Managers
{
    public class TimeManager
    {

        DispatcherTimer gameTimer;
        DispatcherTimer prizeTimer;
        DispatcherTimer windTimer;

        private GameManager _gameManager;

        private Random random;
        private PrizeFactory prizeFactory;

        private int windCounter = 0;
        private float windSpeedPlayer = 0.0f;

        private int windTimerTicks = 50;


        public TimeManager(GameManager gameManager)
        {
            random = new Random();
            prizeFactory = new PrizeFactory();
            
            _gameManager = gameManager;
        }

        public void LaunchTimers(NetworkManager networkManager)
        {
            Console.WriteLine("Initializing timers...");

            gameTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(40.0) };
            gameTimer.Tick += (sender, e) => _gameManager.GameTimer_Tick(networkManager, sender, e);
            Console.WriteLine("Game timer initialized");

            prizeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(40.0) };
            prizeTimer.Tick += PrizeTimer_Tick;
            Console.WriteLine("Prize timer initialized");

            windTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(40.0) };
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
            _gameManager.WindDirection();
        }

        private void PrizeTimer_Tick(object sender, EventArgs e)
        {
            if (_gameManager.PrizeList.Count < 3 && (_gameManager.NumberOfFirstPlayerPrizes < 15 || _gameManager.NumberOfSecondPlayerPrizes < 15))
            {
                _gameManager.PrizeList.Add(prizeFactory.AddNewPrize());
            }
            for (int i = 0; i < _gameManager.PrizeList.Count; i++)
            {
                if (_gameManager.NumberOfFirstPlayerPrizes >= 15 && _gameManager.NumberOfSecondPlayerPrizes >= 15)
                {
                    _gameManager.PrizeList.RemoveAt(_gameManager.PrizeList.Count - 1);
                }
            }
        }
    }
}
