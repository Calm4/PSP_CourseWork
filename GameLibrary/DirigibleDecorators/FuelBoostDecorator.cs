using GameLibrary.Dirigible;
using OpenTK;


namespace GameLibrary.DirigibleDecorators
{
    /// <summary>
    /// Декоратор на изменение запаса топлива
    /// </summary>
    public class FuelBoostDecorator : DirigibleDecorator
    {
        /// <summary>
        /// Дополнительное топливо
        /// </summary>
        int _extraFuel;
        /// <summary>
        /// Максимальный запас топлива
        /// </summary>
        private const int _maxFuel = 3000; // 2000
        /// <summary>
        /// Конструктор класса FuelBoostDecorator
        /// </summary>
        /// <param name="dirigible">Базовый объект дирижабля</param>
        /// <param name="extraFuel">Дополнителные пули</param>
        public FuelBoostDecorator(AbstractDirigible dirigible,int extraFuel) : base(dirigible) 
        {
              _extraFuel = extraFuel;
            if (_dirigible.Fuel <= _maxFuel)
            {
                if (_dirigible.Fuel <= _maxFuel - _extraFuel)
                {
                    _dirigible.Fuel += _extraFuel;
                }
                else
                {
                    _dirigible.Fuel = _maxFuel;
                }
            }
            else
            {
                _dirigible.Fuel = _maxFuel;
            }
            
        }

        /// <summary>
        /// Запас топлива
        /// </summary>
        public override int Fuel
        {
            get { return _dirigible.Fuel; }
            set { _dirigible.Fuel = value; }
        }

        /// <summary>
        /// Передвижение дирижабля
        /// </summary>
        /// <param name="movement">Передвижение</param>
        public override void Move(Vector2 movement)
        {
            if (IsMove || Fuel <= 0)
                return;

            if (_extraFuel > 0)
            {
                _extraFuel--;
            }
            else
            {
                _dirigible.Move(movement);
                
            }
        }




    }
}
