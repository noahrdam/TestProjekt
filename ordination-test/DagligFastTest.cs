using Microsoft.VisualStudio.TestTools.UnitTesting;
using shared.Model;
using System;

namespace ordination_test
{
    [TestClass]
    public class DagligFastTest
    {
        [TestMethod]
        public void TestSamletDosis()
        {
            // Arrange
            var startDen = new DateTime(2024, 11, 27);
            var slutDen = startDen.AddDays(3);
            var laegemiddel = new Laegemiddel { navn = "Fucidin", enhed = "Styk" };
            var dagligFast = new DagligFast(startDen, slutDen, laegemiddel, 2, 3, 1.5, 0);

            // Act
            double samletDosis = dagligFast.samletDosis();

            // Assert
            Assert.AreEqual(4 * 6.5, samletDosis); // 4 dage + 6,5 døgndosis
        }

        [TestMethod]
        public void TestDoegnDosis()
        {
            // Arrange
            var startDen = DateTime.Now;
            var slutDen = startDen.AddDays(3);
            var laegemiddel = new Laegemiddel { navn = "Paracetamol", enhed = "Ml" };
            var dagligFast = new DagligFast(startDen, slutDen, laegemiddel, 1, 1.5, 2, 0.5);

            // Act
            double doegnDosis = dagligFast.doegnDosis();

            // Assert
            Assert.AreEqual(5, doegnDosis); // 1 + 1.5 + 2 + 0.5
        }
    }
}
