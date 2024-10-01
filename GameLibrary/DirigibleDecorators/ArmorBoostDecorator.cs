using GameLibrary.Dirigible;
using System;

namespace GameLibrary.DirigibleDecorators
{
    /// <summary>
    /// Декоратор на изменение запаса брони
    /// </summary>
    public class ArmorBoostDecorator : DirigibleDecorator
    {
        /// <summary>
        /// Дополнительная броня
        /// </summary>
        private int _extraArmor;
        /// <summary>
        /// Максимальное количество брони
        /// </summary>
        private const int _maxArmor = 50;
        /// <summary>
        /// Конструктор класса ArmorBoostDecorator
        /// </summary>
        /// <param name="dirigible">Базовый объект дирижабля</param>
        /// <param name="extraArmor">Дополнителные пули</param>
        public ArmorBoostDecorator(AbstractDirigible dirigible, int extraArmor) : base(dirigible)
        {
            _extraArmor = extraArmor;
        }

        /// <summary>
        /// Запас брони
        /// </summary>
        public override int Armor
        {
            get { return Math.Min(_dirigible.Armor + _extraArmor, _maxArmor); }
            set { _dirigible.Armor = value; }
        }

        /// <summary>
        /// Получение урона
        /// </summary>
        /// <param name="damage">урон</param>
        public override void GetDamage(int damage)
        {
            if (_extraArmor > 0)
            {
                int tempDamage = damage - _extraArmor;
                if (tempDamage > 0)
                {
                    _dirigible.GetDamage(tempDamage);
                }
                _extraArmor -= damage;
                if (_extraArmor < 0)
                {
                    _extraArmor = 0;
                }
            }
            else
            {
                _dirigible.GetDamage(damage);
            }
        }

    }
}
