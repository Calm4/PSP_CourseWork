using GameLibrary;
using OpenTK;

namespace PrizesLibrary.Prizes
{
    public class SpeedBoostPrize : Prize
    {
        /// <summary>
        /// Конструктор класса SpeedBoostPrize
        /// </summary>
        /// <param name="centerPosition">Расположение центра</param>
        public SpeedBoostPrize(Vector2 centerPosition)
        {
            this.textureID = CreateTexture.LoadTexture("speedPrize.png"); ;
            this.centerPosition = centerPosition;
        }

    }
}
