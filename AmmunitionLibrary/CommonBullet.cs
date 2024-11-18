using OpenTK;

namespace AmmunitionLibrary
{
    public class CommonBullet : Bullet
    {
        public override int Damage { get; set; } = 30;
        public override float Speed { get; set; } = 0.025f;

        public CommonBullet(Vector2 startPosition, int textureID, bool direction) : base()
        {
            PositionCenter = startPosition;
            TextureID = textureID;
            this.Direction = direction ? new Vector2(Speed, 0f) : new Vector2(-Speed, 0f);
        }
        public override Vector2[] GetPosition()
        {
            return new Vector2[4]
            {
                PositionCenter + new Vector2(-0.06f, -0.04f),
                PositionCenter + new Vector2(0.06f, -0.04f),
                PositionCenter + new Vector2(0.06f, 0.04f),
                PositionCenter + new Vector2(-0.06f, 0.04f),
            };
        }

    }
}
