using GameLibrary;
using OpenTK;

namespace PrizesLibrary.Prizes
{
    /// <summary>
    /// Класс приза с бронёй
    /// </summary>
    public class ArmorPrize : Prize
    {
        /// <summary>
        /// Конструктор класса ArmorPrize
        /// </summary>
        /// <param name="centerPosition">Расположение центра</param>
        public ArmorPrize(Vector2 centerPosition)
        {
            this.textureID = CreateTexture.LoadTexture("armorPrize.png");
            this.centerPosition = centerPosition;
        }
    }
}
