using AmmunitionLibrary;
using GameLibrary.Dirigible;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirigibleBattle.Managers
{
    public static class ColliderManager
    {
        public static RectangleF screenBorderCollider { get; private set; }
        public static RectangleF mountineCollider { get; private set; }

        public static void SetupColliders()
        {
            screenBorderCollider = new RectangleF(0f, 0.125f, 1.025f, 0.875f);
            mountineCollider = new RectangleF(0.0f, -0.1f, 1.0f, 0.185f);
        }
    }
}
