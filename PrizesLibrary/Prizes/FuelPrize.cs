using GameLibrary;
using OpenTK;

namespace PrizesLibrary.Prizes
{
    /// <summary>
    /// Класс приза с топливом
    /// </summary>
    public class FuelPrize : Prize
    {
        /// <summary>
        /// Конструктор класса FuelPrize
        /// </summary>
        /// <param name="centerPosition">Расположение центра</param>
        public FuelPrize(Vector2 centerPosition)
        {
            this.textureID = CreateTexture.LoadTexture("fuelPrize.png");
            this.centerPosition = centerPosition;
        }
    }
}
