using GameLibrary.Dirigible;
using System;


namespace GameLibrary.DirigibleDecorators
{
    /// <summary>
    /// Декоратор на изменение скорости передвижения
    /// </summary>
    public class SpeedBoostDecorator : DirigibleDecorator
    {
        private float _extraSpeed;
        private const float _maxSpeed = 0.15f;
        /// <summary>
        /// Конструктор класса SpeedBoostDecorator
        /// </summary>
        /// <param name="dirigible">Базовый объект дирижабля</param>
        /// <param name="extraSpeed">Дополнителные пули</param>
        public SpeedBoostDecorator(AbstractDirigible dirigible, float extraSpeed) : base(dirigible)
        {
            _extraSpeed = extraSpeed;
            if (_dirigible.Speed <= _maxSpeed)
            {
                if (_dirigible.Speed <= _maxSpeed - _extraSpeed)
                {
                    _dirigible.Speed += _extraSpeed;
                }
                else
                {
                    _dirigible.Speed = _maxSpeed;
                }
            }
            else
            {
                _dirigible.Speed = _maxSpeed;
            }
        }

        /// <summary>
        /// Скорость передвижения
        /// </summary>
        public override float Speed
        {
            get { return _dirigible.Speed; }
            set { _dirigible.Speed = value; }
        }
    }
}
