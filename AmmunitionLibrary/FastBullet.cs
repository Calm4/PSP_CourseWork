﻿using OpenTK;

namespace AmmunitionLibrary
{
    /// <summary>
    /// Быстрая пуля 
    /// </summary>
    public class FastBullet : Bullet
    {
        /// <summary>
        /// Урон пули
        /// </summary>
        public override int Damage { get; set; } = 20;
        /// <summary>
        /// Скорость полета пули
        /// </summary>
        public override float Speed { get; set; } = 0.035f;

        /// <summary>
        /// Конструктор пули
        /// </summary>
        /// <param name="startPosition">Начальная позиция пули</param>
        /// <param name="textureID">ID текстуры</param>
        /// <param name="direction">Направление пули</param>
        public FastBullet(Vector2 startPosition, int textureID, bool direction) : base()
        {
            PositionCenter = startPosition;
            TextureID = textureID;
            this.Direction = direction ? new Vector2(Speed, 0f) : new Vector2(-Speed, 0f);
        }
        /// <summary>
        /// Задание размера пули
        /// </summary>
        /// <returns></returns>
        public override Vector2[] GetPosition()
        {
            return new Vector2[4]
            {
                PositionCenter + new Vector2(-0.05f, -0.03f),
                PositionCenter + new Vector2(0.05f, -0.03f),
                PositionCenter + new Vector2(0.05f, 0.03f),
                PositionCenter + new Vector2(-0.05f, 0.03f),
            };
        }
    }
}
