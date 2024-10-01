using GameLibrary.Dirigible;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenTK;

namespace GameTests
{
    /// <summary>
    /// Класс на проверку конвертации координат
    /// </summary>
    [TestClass]
    public class ConverterTest
    {
        /// <summary>
        /// Проверка конвертации координат
        /// </summary>
        /// <param name="concretePointX">Точка по X</param>
        /// <param name="concretePointY">Точка по Y</param>
        /// <param name="concreteExpectedPoints">Ожидаемые точки по X и Y</param>
        [DataTestMethod]
      
        [DataRow(0f, 0f, new float[] { 0.5f, 0.5f })]
        [DataRow(1f, 1f, new float[] { 1f, 0f })]
        [DataRow(0.25f, -0.25f, new float[] { 0.625f, 0.625f })]
        [DataRow(-0.2f, -0.3f, new float[] { 0.4f, 0.65f })]
        [DataRow(1f, 0.5f, new float[] { 1.0f, 0.25f })]

        public void ConverterTestMethod(float concretePointX,float concretePointY, float[] concreteExpectedPoints)
        {
            float[] actualPoints;
            AbstractDirigible dirigible = new BasicDirigible(Vector2.Zero,0);

            actualPoints = dirigible.Convert(concretePointX,concretePointY);

            Assert.AreEqual(concreteExpectedPoints[0], actualPoints[0]);
            Assert.AreEqual(concreteExpectedPoints[1], actualPoints[1]);

        }
    }
}
