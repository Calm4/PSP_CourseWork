using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Data
{
    public class PlayerState
    {
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public int Health { get; set; }
        public int Armor { get; set; }
        public int Ammo { get; set; }
        public float Speed { get; set; }
        public int Fuel { get; set; }
    }
}
