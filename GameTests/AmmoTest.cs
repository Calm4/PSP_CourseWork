using AmmunitionLibrary;
using GameLibrary.Dirigible;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenTK;
using System.Collections.Generic;

namespace GameTests
{
    /// <summary>
    /// Класс для тестирования запаса пуль
    /// </summary>
    [TestClass]
    public class AmmoTest
    {
        /// <summary>
        /// Проверка запаса пуль
        /// </summary>
        [TestMethod]
        public void AmmoTestMethod()
        {
            AbstractDirigible dirigible = new BasicDirigible(Vector2.Zero, 0);
            List<Bullet> bulletsList = new List<Bullet>();
            dirigible.Ammo = 10;
            while (bulletsList.Count < dirigible.Ammo)
            {
                bulletsList.Add(new CommonBullet(Vector2.Zero, 0, true));
            }
            int expectedBulletsCount = 10;

            int actualBulletsCount = bulletsList.Count;

            Assert.AreEqual(expectedBulletsCount, actualBulletsCount);

        }
        /// <summary>
        /// Проверка запаса пуль после выстрела
        /// </summary>
        [TestMethod]
        public void AmmoWithShootTestMethod()
        {
            AbstractDirigible dirigible = new BasicDirigible(Vector2.Zero, 0);
            List<Bullet> bulletsList = new List<Bullet>();
            dirigible.Ammo = 10;
            while (bulletsList.Count < dirigible.Ammo)
            {
                bulletsList.Add(new CommonBullet(Vector2.Zero, 0, true));
            }
            int expectedBulletsCount = 3;
            int actualBulletsCount;

            for (int i = bulletsList.Count - 1; i >= 3; i--)
            {
                bulletsList[i].Fire();
                bulletsList.RemoveAt(i); // удаляем в любом случае, попал в игрока или просто вылетел за область экрана
            }
            actualBulletsCount = bulletsList.Count;


            Assert.AreEqual(expectedBulletsCount, actualBulletsCount);
        }
    }
}
