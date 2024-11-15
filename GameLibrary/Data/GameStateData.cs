using System.Collections.Generic;

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

        public List<BulletData> BulletsData = new List<BulletData>();
    }

    public class BulletData
    {
        public int ID;
        public float PositionX;
        public float PositionY;
        public bool IsLeft;
        public int BulletType;
    }
}
