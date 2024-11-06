using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Dirigible
{
    internal class DirigibleAction
    {
        public DirigibleActionType ActionType { get; set; } // "MOVE", "SHOOT", "PICK_PRIZE"
        public Vector2 Direction { get; set; } // Использовать для передвижения
        public int PrizeId { get; set; }
    }
}
