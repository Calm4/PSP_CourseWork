using GameLibrary;
using OpenTK;
using System.Drawing;

namespace PrizesLibrary.Prizes
{
    /// <summary>
    /// Абстрактный класс приза
    /// </summary>
    public abstract class Prize
    {
        /// <summary>
        /// Расположение центра
        /// </summary>
        protected Vector2 centerPosition;
        /// <summary>
        /// ID текстуры
        /// </summary>
        protected int textureID;

        /// <summary>
        /// Рендер приза
        /// </summary>
        public void Render()
        {
            ObjectRenderer.RenderObjects(textureID, GetPosition());
        }
        /// <summary>
        /// Получение коллайдера
        /// </summary>
        public RectangleF GetCollider()
        {
            Vector2[] colliderPosition = GetPosition();

            float colliderWidth = (colliderPosition[2].X - colliderPosition[3].X) / 2.0f;
            float colliderHeight = (colliderPosition[3].Y - colliderPosition[0].Y) / 2.0f;

            float[] convertedLeftTop = Convert(colliderPosition[3].X, colliderPosition[3].Y);

            RectangleF collider = new RectangleF(convertedLeftTop[0], convertedLeftTop[1], colliderWidth - 0.005f, colliderHeight - 0.03f);

            return collider;
        }
        /// <summary>
        /// Преобразование координат
        /// </summary>
        /// <param name="pointX">Точка по оси X</param>
        /// <param name="pointY">Точка по оси Y</param>
        /// <returns></returns>
        private static float[] Convert(float pointX, float pointY)
        {
            float centralPointX = 0.5f;
            float centralPointY = 0.5f;

            float[] resultPoint = new float[2];

            resultPoint[0] = centralPointX + pointX / 2.0f;
            resultPoint[1] = centralPointY - pointY / 2.0f;

            return resultPoint;
        }
        /// <summary>
        /// Получение размера объекта
        /// </summary>
        /// <returns></returns>
        protected Vector2[] GetPosition()
        {
            return new Vector2[4]
          {
                centerPosition + new Vector2(-0.05f, -0.05f),
                centerPosition + new Vector2(0.05f, -0.05f),
                centerPosition + new Vector2(0.05f, 0.05f),
                centerPosition + new Vector2(-0.05f, 0.05f),
          };
        }

    }
}
