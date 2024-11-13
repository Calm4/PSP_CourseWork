using GameLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirigibleBattle.Managers
{
    public static class TextureManager
    {
        public static int commonBulletTexture { get; private set; }
        public static int fastBulletTexture { get; private set; }
        public static int heavyBulletTexture { get; private set; }
        public static int firstDirigibleTextureRight { get; private set; }
        public static int firstDirigibleTextureLeft { get; private set; }
        public static int secondDirigibleTextureRight { get; private set; }
        public static int secondDirigibleTextureLeft { get; private set; }
        public  static int backGroundTexture { get; private set; }
        public static int mountainRange { get; private set; }

        public static void SetupTexture()
        {
            firstDirigibleTextureRight = CreateTexture.LoadTexture("dirigible_red_right_side.png");
            firstDirigibleTextureLeft = CreateTexture.LoadTexture("dirigible_red_left_side.png");
            secondDirigibleTextureRight = CreateTexture.LoadTexture("dirigible_blue_right_side.png");
            secondDirigibleTextureLeft = CreateTexture.LoadTexture("dirigible_blue_left_side.png");
            commonBulletTexture = CreateTexture.LoadTexture("CommonBullet.png");
            fastBulletTexture = CreateTexture.LoadTexture("FastBullet.png");
            heavyBulletTexture = CreateTexture.LoadTexture("HeavyBullet.png");
            backGroundTexture = CreateTexture.LoadTexture("clouds2.png");
            mountainRange = CreateTexture.LoadTexture("mountine2.png");
        }


    }
}
