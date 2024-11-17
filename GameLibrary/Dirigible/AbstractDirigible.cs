using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GameLibrary.Dirigible
{
    public abstract class AbstractDirigible
    {
        public abstract int DirigibleID { get; set; }
        public Vector2 PositionCenter;
        protected Vector2 gunOffset;
        public Vector2 velocity;
        public Vector2 dirigibleWindEffect;

        public bool IsMove { get; set; }
        public bool IsShoot { get; set; }
        public bool IsWindWork { get; set; }
        
        public abstract int Health { get; set; }
        public abstract int Armor { get; set; }
        public abstract int Fuel { get; set; }
        public abstract int Ammo { get; set; }
        public abstract float Speed { get; set; }
        public abstract int NumberOfPrizesReceived {  get; set; }

        public abstract bool IsTurnedLeft { get; set;}

        private Random random;


        public abstract void GetDamage(int damage);
        public abstract void ChangeDirectionWithWind(Vector2 newWindSpeed);
        public abstract void ChangeWindDirection(bool turnOver);

       /* public abstract void Control(List<Key> keys, int textureIdLeft, int textureIdRight, RectangleF checkPlayArea);*/
        public abstract Vector2 GetGunPosition();
        public abstract void Move(Vector2 movement);
        public abstract void Idle();


        public virtual float[] Convert(float pointX, float pointY)
        {
            float centralPointX = 0.5f;
            float centralPointY = 0.5f;

            float[] resultPoint = new float[2];

            resultPoint[0] = centralPointX + pointX / 2.0f;
            resultPoint[1] = centralPointY - pointY / 2.0f;

            return resultPoint;
        }
      
        public virtual RectangleF GetCollider()
        {
            Vector2[] colliderPosition = GetPosition();

            float colliderWidth = (colliderPosition[2].X - colliderPosition[3].X) / 2.0f;
            float colliderHeight = (colliderPosition[3].Y - colliderPosition[0].Y) / 2.0f;

            float[] convertedLeftTop = Convert(colliderPosition[3].X, colliderPosition[3].Y);

            RectangleF collider = new RectangleF(convertedLeftTop[0], convertedLeftTop[1], colliderWidth - 0.005f, colliderHeight - 0.03f);

            return collider;
        }
        
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
        
        public virtual void Render()
        {
            ObjectRenderer.RenderObjects(DirigibleID, GetPosition());
        }

        public void SetRandom(Random random)
        {
            this.random = random;
        }
    }
}
