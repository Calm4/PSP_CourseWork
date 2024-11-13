using GameLibrary.Dirigible;

namespace GameLibrary.DirigibleDecorators
{
    /// <summary>
    /// Декоратор на изменение запаса амуниции
    /// </summary>
    public class AmmoBoostDecorator : DirigibleDecorator
    {
        /// <summary>
        /// Дополнительные пули
        /// </summary>
        private int _extraAmmo;
        /// <summary>
        /// Максимальное количество пуль
        /// </summary>
        private const int _maxAmmo = 30;
        /// <summary>
        /// Конструктор класса AmmoBoostDecorator
        /// </summary>
        /// <param name="dirigible">Базовый объект дирижабля</param>
        /// <param name="extraAmmo">Дополнителные пули</param>
        public AmmoBoostDecorator(AbstractDirigible dirigible, int extraAmmo) : base(dirigible)
        {
            _extraAmmo = extraAmmo;
            if (_dirigible.Ammo <= _maxAmmo)
            {
                if (_dirigible.Ammo <= _maxAmmo - _extraAmmo)
                {
                    _dirigible.Ammo += _extraAmmo;
                }
                else
                {
                    _dirigible.Ammo = _maxAmmo;
                }
            }
            else
            {
                _dirigible.Ammo = _maxAmmo;
            }

        }
        /// <summary>
        /// Запас пуль
        /// </summary>
        public override int Ammo
        {
            get { return _dirigible.Ammo; }
            set { _dirigible.Ammo = value; }
        }
    }
}
