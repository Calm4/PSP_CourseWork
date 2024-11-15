using System.Drawing;
using OpenTK;
using GameLibrary;

namespace AmmunitionLibrary
{
    /// <summary>
    /// Абстрактный класс пули
    /// </summary>
    public abstract class Bullet
    {
        /// <summary>
        /// Урон который будет у пули
        /// </summary>
        public abstract int Damage { get; set; }
        /// <summary>
        /// Скорость полета пули
        /// </summary>        
        public abstract float Speed { get; set; }
        /// <summary>
        /// уникальное ID текстуры пули
        /// </summary>
        protected int TextureID { get; set; }

        /// <summary>
        /// Позиция расположения пули
        /// </summary>
        public Vector2 PositionCenter;

        /// <summary>
        /// Изменение направления пули через 
        /// </summary>
        public Vector2 Direction;


        /// <summary>
        /// Стрельба в определенном направление
        /// </summary>
        public virtual void Fire()
        {
            PositionCenter += Direction;
        }
        /// <summary>
        /// Получение коллайдера объекта пули 
        /// </summary>
        /// <returns></returns>
        public virtual RectangleF GetCollider()
        {
            Vector2[] colliderPosition = GetPosition();

            float colliderWidth = (colliderPosition[2].X - colliderPosition[3].X) / 2.0f;
            float colliderHeight = (colliderPosition[3].Y - colliderPosition[0].Y) / 2.0f;

            float[] convertedLeftTop = Convert(colliderPosition[3].X, colliderPosition[3].Y);

            RectangleF collider = new RectangleF(convertedLeftTop[0], convertedLeftTop[1], colliderWidth, colliderHeight);

            return collider;
        }
        /// <summary>
        /// Задание размера пули
        /// </summary>
        /// <returns></returns>
        public virtual Vector2[] GetPosition()
        {
            return new Vector2[4]
            {
                PositionCenter + new Vector2(-0.05f, -0.03f),
                PositionCenter + new Vector2(0.05f, -0.03f),
                PositionCenter + new Vector2(0.05f, 0.03f),
                PositionCenter + new Vector2(-0.05f, 0.03f),
            };
        }
        /// <summary>
        /// Метод для преобразования координат в OpenGL
        /// </summary>
        /// <param name="pointX">Точка по оси X</param>
        /// <param name="pointY">Точка по оси Y</param>
        /// <returns></returns>
        protected static float[] Convert(float pointX, float pointY)
        {
            float centralPointX = 0.5f;
            float centralPointY = 0.5f;

            float[] resultPoint = new float[2];

            resultPoint[0] = centralPointX + pointX / 2.0f;
            resultPoint[1] = centralPointY - pointY / 2.0f;

            return resultPoint;
        }
        /// <summary>
        /// Производит рендеринг текстуры
        /// </summary>
        public virtual void Render()
        {
            ObjectRenderer.RenderObjects(TextureID, GetPosition());
        }

       

       

    }
}
