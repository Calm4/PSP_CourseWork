using System;
using PrizesLibrary.Prizes;
using OpenTK;

namespace PrizesLibrary.Factories
{
    /// <summary>
    /// Класс фабрики по генерации призов
    /// </summary>
    public class PrizeFactory
    {
        /// <summary>
        /// Создание нового приза
        /// </summary>
        /// <returns>Случайный подарок</returns>
        public Prize AddNewPrize()
        {

            Random random = new Random();
            float randomPosX, randomPosY;
            Prize prize = null;
            int prizeNumber = random.Next(0, 5);
            randomPosX = (float)(random.NextDouble() * 1.5 - 0.75); // -0.75 до 0.75
            randomPosY = (float)(random.NextDouble() * 1.5 - 0.75); // -0.75 до 0.75


            switch (prizeNumber)
            {
                case 0:
                    prize = new AmmoPrize(new Vector2(randomPosX, randomPosY));
                    break;
                case 1:
                    prize = new ArmorPrize(new Vector2(randomPosX, randomPosY));
                    break;
                case 2:
                    prize = new HealthPrize(new Vector2(randomPosX, randomPosY));
                    break;
                case 3:
                    prize = new SpeedBoostPrize(new Vector2(randomPosX, randomPosY));
                    break;
                case 4:
                    prize = new FuelPrize(new Vector2(randomPosX, randomPosY));
                    break;
                default:
                    break;
            }
            return prize;
        }
    }
}
