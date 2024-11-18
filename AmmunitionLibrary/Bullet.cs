using System.Drawing;
using OpenTK;
using GameLibrary;

namespace AmmunitionLibrary
{
    public abstract class Bullet
    {
        public abstract int Damage { get; set; }
        public abstract float Speed { get; set; }
        protected int TextureID { get; set; }

        public Vector2 PositionCenter;

        public Vector2 Direction;


        public virtual void Fire()
        {
            PositionCenter += Direction;
        }
        public virtual RectangleF GetCollider()
        {
            Vector2[] colliderPosition = GetPosition();

            float colliderWidth = (colliderPosition[2].X - colliderPosition[3].X) / 2.0f;
            float colliderHeight = (colliderPosition[3].Y - colliderPosition[0].Y) / 2.0f;

            float[] convertedLeftTop = Convert(colliderPosition[3].X, colliderPosition[3].Y);

            RectangleF collider = new RectangleF(convertedLeftTop[0], convertedLeftTop[1], colliderWidth, colliderHeight);

            return collider;
        }
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
        protected static float[] Convert(float pointX, float pointY)
        {
            float centralPointX = 0.5f;
            float centralPointY = 0.5f;

            float[] resultPoint = new float[2];

            resultPoint[0] = centralPointX + pointX / 2.0f;
            resultPoint[1] = centralPointY - pointY / 2.0f;

            return resultPoint;
        }
        public virtual void Render()
        {
            ObjectRenderer.RenderObjects(TextureID, GetPosition());
        }
    }
}
