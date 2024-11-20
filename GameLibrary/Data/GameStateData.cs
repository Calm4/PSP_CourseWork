namespace GameLibrary
{
    public class NetworkData
    {
        public float PositionX { get; set; }
        public float PositionY { get; set; }

        public int Health { get; set; }
        public int Armor { get; set; }
        public int Fuel { get; set; }
        public int Ammo { get; set; }
        public float Speed { get; set; }
        public int NumberOfPrizesReceived { get; set; }

        public bool IsTurningLeft { get; set; }

        public BulletData BulletData { get; set; }
    }

    public class BulletData
    {
        public int ShooterID { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public bool IsLeft { get; set; }
        public int BulletType { get; set; }
    }

}
