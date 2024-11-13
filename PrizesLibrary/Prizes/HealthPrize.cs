using GameLibrary;
using OpenTK;

namespace PrizesLibrary.Prizes
{
    /// <summary>
    /// Класс приза со здоровьем
    /// </summary>
    public class HealthPrize : Prize
    {
        /// <summary>
        /// Конструктор класса HealthPrize
        /// </summary>
        /// <param name="centerPosition">Расположение центра</param>
        public HealthPrize(Vector2 centerPosition)
        {
            this.textureID = CreateTexture.LoadTexture("healthPrize.png");
            this.centerPosition = centerPosition;
        }
        
       

    }
}
