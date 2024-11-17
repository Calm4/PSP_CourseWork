namespace GameLibrary
{
    public class NetworkData
    {
        public float PositionX;
        public float PositionY;

        public int Health;
        public int Armor;
        public int Fuel;
        public int Ammo;
        public float Speed;
        public int NumberOfPrizesReceived;

        public bool IsTurningLeft;

        public BulletData BulletData;
    }

    public class BulletData
    {
        public int ShooterID;
        public float PositionX;
        public float PositionY;
        public bool IsLeft;
        public int BulletType;
    }
}
