using System;
using System.Collections.Generic;
using System.Drawing;
using GameLibrary.Dirigible;
using OpenTK;
using OpenTK.Input;

namespace GameLibrary.DirigibleDecorators
{
    /// <summary>
    /// Главный класс декоратор для остальных декораторов
    /// </summary>
    public abstract class DirigibleDecorator : AbstractDirigible
    {
        /// <summary>
        /// Базовый объект дирижабля
        /// </summary>
        protected AbstractDirigible _dirigible;

        /// <summary>
        /// Конструктор класса DirigibleDecorator
        /// </summary>
        /// <param name="dirigible">базовый дирижабль</param>
        public DirigibleDecorator(AbstractDirigible dirigible)
        {
            _dirigible = dirigible;
        }

        /// <summary>
        /// Запас жизней
        /// </summary>
        public override int Health
        {
            get { return _dirigible.Health; }
            set { _dirigible.Health = value; }
        }
        /// <summary>
        /// Запас брони
        /// </summary>
        public override int Armor
        {
            get { return _dirigible.Armor; }
            set { _dirigible.Armor = value; }
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
        /// Запас пуль
        /// </summary>
        public override int Ammo
        {
            get { return _dirigible.Ammo; }
            set { _dirigible.Ammo = value; }
        }
        /// <summary>
        /// Скорость передвижения
        /// </summary>
        public override float Speed
        {
            get { return _dirigible.Speed; }
            set { _dirigible.Speed = value; }
        }
        /// <summary>
        /// ID текстуры дирижабля
        /// </summary>
        public override int DirigibleID
        {
            get { return _dirigible.DirigibleID; }
            set { _dirigible.DirigibleID = value; }
        }

        public override int NumberOfPrizesReceived
        {
            get { return _dirigible.NumberOfPrizesReceived; }
            set { _dirigible.NumberOfPrizesReceived = value; }
        }

        /// <summary>
        /// Получение урона
        /// </summary>
        /// <param name="damage">Урон</param>
        public override void GetDamage(int damage)
        {
            _dirigible.GetDamage(damage);

        }
        /// <summary>
        /// Изменение направление с ветром ветра
        /// </summary>
        /// <param name="newWindSpeed">Новая скорость ветра</param>
        public override void ChangeDirectionWithWind(Vector2 newWindSpeed)
        {
            _dirigible.ChangeDirectionWithWind(newWindSpeed);
        }
        /// <summary>
        /// Изменить направление ветра
        /// </summary>
        /// <param name="turnOver">Изменить направление</param>
        public override void ChangeWindDirection(bool turnOver)
        {
            _dirigible.ChangeWindDirection(turnOver);
        }

        /// <summary>
        /// Управление дирижаблем
        /// </summary>
        /// <param name="keys">Список кнопок</param>
        /// <param name="textureIdLeft">Текстура дирижабля смотрящая влево</param>
        /// <param name="textureIdRight">Текстура дирижабля смотрящая вправо</param>
        /// <param name="checkPlayArea"></param>
        public override void Control(List<Key> keys, int textureIdLeft, int textureIdRight, RectangleF checkPlayArea)
        {
            _dirigible.Control(keys, textureIdLeft, textureIdRight, checkPlayArea);
        }
        /// <summary>
        /// Получает позицию пушки
        /// </summary>
        /// <returns></returns>
        public override Vector2 GetGunPosition()
        {
            // Позиция пушки относительно координат дирижабля
            Vector2 gunPosition = _dirigible.GetGunPosition() + gunOffset;

            // Если дирижабль смотрит влево, инвертируем координату X позиции пушки
            if (!IsShoot)
            {
                gunPosition.X = _dirigible.GetGunPosition().X - gunOffset.X;
            }

            return gunPosition;
        }
        /// <summary>
        /// Статическое состояние, в котором дирижабль просто падает вниз
        /// </summary>
        public override void Idle()
        {
            _dirigible.Idle();
        }
        /// <summary>
        /// Передвижение дирижабля
        /// </summary>
        /// <param name="movement">Передвижение</param>
        public override void Move(Vector2 movement)
        {
            _dirigible.Move(movement);
        }
        /// <summary>
        /// Рендеринг объекта
        /// </summary>
        public override void Render()
        {
            _dirigible.Render();
        }
        /// <summary>
        /// Получение коллайдера
        /// </summary>
        /// <returns></returns>
        public override RectangleF GetCollider()
        {
            return _dirigible.GetCollider();
        }


    }
}
