using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GameLibrary.Dirigible;
using GameLibrary.DirigibleDecorators;
using OpenTK;

namespace GameTests
{
    /// <summary>
    /// Класс для проверки навешивания декораторов
    /// </summary>
    [TestClass]
    public class СharacteristicTests
    {
        /// <summary>
        /// Проверка пополнения запаса пуль
        /// </summary>
        [TestMethod]
        public void AmmoTestMethod()
        {
           // GameWindow window = new GameWindow(1, 1, GraphicsMode.Default, "", GameWindowFlags.Default, DisplayDevice.Default, 3, 0, GraphicsContextFlags.Default);
           // window.Visible = false;
            Random random = new Random();
            AbstractDirigible dirigible = new BasicDirigible(Vector2.Zero,0);

            
            int expectedAmmo = random.Next(1,5);
            int currentDirigibleAmmo = dirigible.Ammo;
            int actualAmmo;

            dirigible = new AmmoBoostDecorator(dirigible,expectedAmmo);
            actualAmmo = dirigible.Ammo;
            Assert.AreEqual(currentDirigibleAmmo + expectedAmmo, actualAmmo);
        }
        /// <summary>
        /// Проверка пополнения запаса брони
        /// </summary>
        [TestMethod]
        public void ArmorTestMethod()
        {
            Random random = new Random();
            AbstractDirigible dirigible = new BasicDirigible(Vector2.Zero, 0);

            int expectedArmor = random.Next(10, 26);
            int currentDirigibleArmor = dirigible.Armor;
            int actualArmor;

            dirigible = new ArmorBoostDecorator(dirigible, expectedArmor);
            actualArmor = dirigible.Armor;

            Assert.AreEqual(currentDirigibleArmor + expectedArmor, actualArmor);
        }
        /// <summary>
        /// Проверка пополнения запаса топлива
        /// </summary>
        [TestMethod]
        public void FuelTestMethod()
        {
            Random random = new Random();
            AbstractDirigible dirigible = new BasicDirigible(Vector2.Zero, 0);

            int expectedFuel = random.Next(100, 200);
            int currentDirigibleArmor = dirigible.Fuel;
            int actualFuel;

            dirigible = new FuelBoostDecorator(dirigible, expectedFuel);
            actualFuel = dirigible.Fuel;

            Assert.AreEqual(currentDirigibleArmor + expectedFuel, actualFuel);
        }
        /// <summary>
        /// Проверка пополнения запаса жизней
        /// </summary>
        [TestMethod]
        public void HealthTestMethod()
        {
            Random random = new Random();
            AbstractDirigible dirigible = new BasicDirigible(Vector2.Zero, 0);

            int expectedHealth = random.Next(10, 21);
            int currentDirigibleHealth = dirigible.Health;
            int actualHealth;

            dirigible = new HealthBoostDecorator(dirigible, expectedHealth);
            actualHealth = dirigible.Health;

            Assert.AreEqual(currentDirigibleHealth + expectedHealth, actualHealth);
        }
        /// <summary>
        /// Проверка на увеличение скорости передвижения
        /// </summary>
        [TestMethod]
        public void SpeedTestMethod()
        {
            Random random = new Random();
            AbstractDirigible dirigible = new BasicDirigible(Vector2.Zero, 0);

            float expectedSpeed = (float)(random.NextDouble() * 0.002 + 0.0005);
            float currentDirigibleSpeed = dirigible.Speed;
            float actualSpeed;

            dirigible = new SpeedBoostDecorator(dirigible, expectedSpeed);
            actualSpeed = dirigible.Speed;

            Assert.AreEqual(currentDirigibleSpeed + expectedSpeed, actualSpeed);
        }
    }
}
