using System.Net.Sockets;
using Newtonsoft.Json;
using OpenTK;
using System.Text;
using System;

namespace GameLibrary.Dirigible
{
    public class DirigibleState
    {
        public int PlayerId { get; set; }
        public Vector2 Position { get; set; }
        public int Health { get; set; }
        public int Armor { get; set; }
        public int Ammo { get; set; }
        public float Speed { get; set; }
        public int Fuel { get; set; }
        public int PrizesCollected { get; set; }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static DirigibleState Deserialize(string jsonData)
        {
            return JsonConvert.DeserializeObject<DirigibleState>(jsonData);
        }
    }
}
