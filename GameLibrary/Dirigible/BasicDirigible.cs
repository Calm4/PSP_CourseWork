using OpenTK;
using OpenTK.Input;
using System.Collections.Generic;
using System.Drawing;


namespace GameLibrary.Dirigible
{
    /// <summary>
    /// Базовый класс дирижаблей
    /// </summary>
    public class BasicDirigible : AbstractDirigible
    {
        /// <summary>
        /// Конструктор класса BasicDirigible
        /// </summary>
        /// <param name="startPosition">Стартовая позиция</param>
        /// <param name="textrureID">ID текстуры</param>
        public BasicDirigible(Vector2 startPosition, int textrureID)
        {
            PositionCenter = startPosition;
            DirigibleID = textrureID;
            this.PassiveSpeed = new Vector2(0, 0.001f);

            Health = 100;
            Armor = 20;
            Ammo = 25;
            Speed = 0.1f; // ????
            Fuel = 2000; //2000
            IsShoot = false;
            gunOffset = new Vector2(0, 0f);
            dirigibleWindEffect = new Vector2(0.0f, 0.0f);

        }
        /// <summary>
        /// Изменение направления с ветром
        /// </summary>
        /// <param name="newWindSpeed"></param>
        public override void ChangeDirectionWithWind(Vector2 newWindSpeed)
        {
            dirigibleWindEffect = newWindSpeed;
        }
        /// <summary>
        /// Изменение направления ветра
        /// </summary>
        /// <param name="turnOver">Изменение направлен</param>
        public override void ChangeWindDirection(bool turnOver)
        {
            IsWindWork = turnOver;
        }
        /// <summary>
        /// Пассивная скорость
        /// </summary>
        public Vector2 PassiveSpeed { get; set; }
        /// <summary>
        /// Запас здоровья
        /// </summary>
        public override int Health { get; set; }
        /// <summary>
        /// Запас брони
        /// </summary>
        public override int Armor { get; set; }
        /// <summary>
        /// Запас топлива
        /// </summary>
        public override int Fuel { get; set; }
        /// <summary>
        /// Боезапас дирижабля
        /// </summary>
        public override int Ammo { get; set; }
        /// <summary>
        /// Скорость передвижения
        /// </summary>
        public override float Speed { get; set; }
        /// <summary>
        /// ID текстуры дирижабля
        /// </summary>
        public override int DirigibleID { get; set; }
        public override int NumberOfPrizesReceived { get; set; }
        public override bool IsTurnedLeft { get; set; }

        /// <summary>
        /// Получить позицию пушки
        /// </summary>
        /// <returns></returns>
        public override Vector2 GetGunPosition()
        {
            // Позиция пушки относительно координат дирижабля
            Vector2 gunPosition = PositionCenter + gunOffset;

            // Если дирижабль смотрит влево, инвертируем координату X позиции пушки
            if (!IsShoot)
                gunPosition.X = PositionCenter.X - gunOffset.X;

            return gunPosition;
        }
        /// <summary>
        /// Получение урона
        /// </summary>
        /// <param name="damage">Урон</param>
        public override void GetDamage(int damage)
        {
            int tempHealth = damage - Armor;
            if (Armor > 0)
            {
                if (Armor > damage)
                {
                    Armor -= damage;
                }
                else
                {
                    Armor = 0;
                    Health -= tempHealth;
                }
            }
            else
            {
                Health -= damage;
            }

        }
        /// <summary>
        /// Статическое состояние, в котором дирижабль просто падает вниз
        /// </summary>
        public override void Idle()
        {
            IsMove = true;
            PositionCenter += PassiveSpeed;
            if (Fuel <= 0)
            {
                if (IsWindWork)
                {
                    PositionCenter += dirigibleWindEffect;
                }
            }
            IsMove = false;
        }
        /// <summary>
        /// Передвижение дирижабля
        /// </summary>
        /// <param name="movement">Передвижение</param>
        public override void Move(Vector2 movement)
        {
            if (IsMove || Fuel <= 0)
                return;
            PositionCenter += movement * Speed;
            Fuel--;
            if (IsWindWork)
                PositionCenter += dirigibleWindEffect;
        }
    }
}
