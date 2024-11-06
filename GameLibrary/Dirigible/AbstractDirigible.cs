using OpenTK;
using OpenTK.Input;
using System.Collections.Generic;
using System.Drawing;

namespace GameLibrary.Dirigible
{
    /// <summary>
    /// Абстрактный класс дирижаблей
    /// </summary>
    public abstract class AbstractDirigible
    {
        /// <summary>
        /// ID текстуры дирижабля
        /// </summary>
        public abstract int DirigibleID { get; set; }
        public Vector2 PositionCenter { get; set; }  
        public int PrizesCollected { get; set; }
        /// <summary>
        /// Смещение пушки 
        /// </summary>
        protected Vector2 gunOffset;
        /// <summary>
        /// Скорость
        /// </summary>
        public Vector2 velocity;
        /// <summary>
        /// Еффект смещения вектора ветра
        /// </summary>
        public Vector2 dirigibleWindEffect;

        /// <summary>
        /// Проверка на движение
        /// </summary>
        public bool IsMove { get; set; }
        /// <summary>
        /// Проверка на стрельбу
        /// </summary>
        public bool IsShoot { get; set; }
        /// <summary>
        /// Проверка на работу ветра
        /// </summary>
        public bool IsWindWork { get; set; }

        /// <summary>
        /// Запас здоровья
        /// </summary>
        public abstract int Health { get; set; }
        /// <summary>
        /// Запас брони
        /// </summary>
        public abstract int Armor { get; set; }
        /// <summary>
        /// Запас топлива
        /// </summary>
        public abstract int Fuel { get; set; }
        /// <summary>
        /// Боезапас
        /// </summary>
        public abstract int Ammo { get; set; }
        /// <summary>
        /// Скорость передвижения
        /// </summary>
        public abstract float Speed { get; set; }

        /// <summary>
        /// Получение урона
        /// </summary>
        /// <param name="damage">Урон</param>
        public abstract void GetDamage(int damage);
        /// <summary>
        /// Изменение направление с ветром ветра
        /// </summary>
        /// <param name="newWindSpeed">Новая скорость ветра</param>
        public abstract void ChangeDirectionWithWind(Vector2 newWindSpeed);
        /// <summary>
        /// Изменить направление ветра
        /// </summary>
        /// <param name="turnOver">Изменить направление</param>
        public abstract void ChangeWindDirection(bool turnOver);

        /// <summary>
        /// Управление дирижаблем
        /// </summary>
        /// <param name="keys">Список кнопок</param>
        /// <param name="textureIdLeft">Текстура дирижабля смотрящая влево</param>
        /// <param name="textureIdRight">Текстура дирижабля смотрящая вправо</param>
        /// <param name="checkPlayArea"></param>
        public abstract void Control(List<Key> keys, int textureIdLeft, int textureIdRight, RectangleF checkPlayArea);
        /// <summary>
        /// Получает позицию пушки
        /// </summary>
        /// <returns></returns>
        public abstract Vector2 GetGunPosition();
        /// <summary>
        /// Передвижение дирижабля
        /// </summary>
        /// <param name="movement">Передвижение</param>
        public abstract void Move(Vector2 movement);
        /// <summary>
        /// Статическое состояние, в котором дирижабль просто падает вниз
        /// </summary>
        public abstract void Idle();


        /// <summary>
        /// Преобразование координат
        /// </summary>
        /// <param name="pointX">Точка по оси X</param>
        /// <param name="pointY">Точка по оси Y</param>
        /// <returns></returns>
        public virtual float[] Convert(float pointX, float pointY)
        {
            float centralPointX = 0.5f;
            float centralPointY = 0.5f;

            float[] resultPoint = new float[2];

            resultPoint[0] = centralPointX + pointX / 2.0f;
            resultPoint[1] = centralPointY - pointY / 2.0f;

            return resultPoint;
        }
        /// <summary>
        /// Получение коллайдера
        /// </summary>
        /// <returns></returns>
        public virtual RectangleF GetCollider()
        {
            Vector2[] colliderPosition = GetPosition();

            float colliderWidth = (colliderPosition[2].X - colliderPosition[3].X) / 2.0f;
            float colliderHeight = (colliderPosition[3].Y - colliderPosition[0].Y) / 2.0f;

            float[] convertedLeftTop = Convert(colliderPosition[3].X, colliderPosition[3].Y);

            RectangleF collider = new RectangleF(convertedLeftTop[0], convertedLeftTop[1], colliderWidth - 0.005f, colliderHeight - 0.03f);

            return collider;
        }
        /// <summary>
        /// Получение размера объекта
        /// </summary>
        protected virtual Vector2[] GetPosition()
        {
            return new Vector2[4]
           {
                PositionCenter + new Vector2(-0.07f, -0.12f),
                PositionCenter + new Vector2(0.07f, -0.12f),
                PositionCenter + new Vector2(0.07f, 0.12f),
                PositionCenter + new Vector2(-0.07f, 0.12f),
           };
        }
        /// <summary>
        /// Рендеринг объекта
        /// </summary>
        public virtual void Render()
        {
            ObjectRenderer.RenderObjects(DirigibleID, GetPosition());
        }
    }
}
