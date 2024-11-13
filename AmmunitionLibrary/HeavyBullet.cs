using OpenTK;

namespace AmmunitionLibrary
{
    /// <summary>
    /// Тяжелая пуля 
    /// </summary>
    public class HeavyBullet : Bullet
    {
        /// <summary>
        /// Урон пули
        /// </summary>
        public override int Damage { get; set; } = 40;
        /// <summary>
        /// Скорость пули
        /// </summary>
        public override float Speed { get; set; } = 0.015f;

        /// <summary>
        /// Конструктор пули
        /// </summary>
        /// <param name="startPosition">Начальная позиция пули</param>
        /// <param name="textureID">ID текстуры</param>
        /// <param name="direction">Направление пули</param>
        public HeavyBullet(Vector2 startPosition, int textureID, bool direction) : base()
        {
            PositionCenter = startPosition;
            TextureID = textureID;
            this.direction = direction ? new Vector2(Speed, 0f) : new Vector2(-Speed, 0f);
        }
        /// <summary>
        /// Задание размера пули
        /// </summary>
        /// <returns></returns>
        public override Vector2[] GetPosition()
        {
            return new Vector2[4]
            {
                PositionCenter + new Vector2(-0.06f, -0.045f),
                PositionCenter + new Vector2(0.06f, -0.045f),
                PositionCenter + new Vector2(0.06f, 0.045f),
                PositionCenter + new Vector2(-0.06f, 0.045f),
            };
        }
    }
}
