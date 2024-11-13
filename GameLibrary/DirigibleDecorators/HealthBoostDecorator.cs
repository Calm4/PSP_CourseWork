using GameLibrary.Dirigible;
using System;



namespace GameLibrary.DirigibleDecorators
{
    /// <summary>
    /// Декоратор на изменение запаса здоровья
    /// </summary>
    public class HealthBoostDecorator : DirigibleDecorator
    {
        /// <summary>
        /// Дополнительное здоровье
        /// </summary>
        private int _extraHealth;
        /// <summary>
        /// Максимальный запас здоровья
        /// </summary>
        private const int _maxHealth = 200;
        /// <summary>
        /// Конструктор класса HealthBoostDecorator
        /// </summary>
        /// <param name="dirigible">Базовый объект дирижабля</param>
        /// <param name="extraHealth">Дополнителные пули</param>
        public HealthBoostDecorator(AbstractDirigible dirigible, int extraHealth) : base(dirigible)
        {
            _extraHealth = extraHealth;
        }

        /// <summary>
        /// Запас здоровья
        /// </summary>
        public override int Health
        {
            get { return Math.Min(_dirigible.Health + _extraHealth, _maxHealth); }
            set { _dirigible.Health = value; }
        }



    }
}
