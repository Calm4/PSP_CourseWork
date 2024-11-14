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

        public BulletData BulletData;
        public bool WasAmmoChanged;
    }

    public class BulletData
    {
        public float PositionX;
        public float PositionY;
        public bool IsLeft;
        public int BulletType;
    }
}
