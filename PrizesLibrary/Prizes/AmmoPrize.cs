using GameLibrary;
using OpenTK;

namespace PrizesLibrary.Prizes
{
    /// <summary>
    /// Класс приза с пулями
    /// </summary>
    public class AmmoPrize : Prize
    {
        /// <summary>
        /// Конструктор класса AmmoPrize
        /// </summary>
        /// <param name="centerPosition">Расположение центра</param>
        public AmmoPrize(Vector2 centerPosition)
        {
            this.textureID = CreateTexture.LoadTexture("ammoPrize.png");
            this.centerPosition = centerPosition;
        }
    }
}
