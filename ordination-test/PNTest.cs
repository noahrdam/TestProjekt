using shared.Model;

namespace ordination_test
{
    [TestClass]
    public class PNTest
    {
        [TestMethod]
        public void TestGivDosis()
        {
            // Arrange
            var startDen = new DateTime(2024, 11, 27);
            var slutDen = new DateTime(2024, 12, 3);
            var laegemiddel = new Laegemiddel { navn = "Paracetamol", enhed = "Ml" };
            var pn = new PN(startDen, slutDen, 2.0, laegemiddel);

            // Act & Assert
            bool result1 = pn.givDosis(new Dato { dato = new DateTime(2024, 11, 28) });
            Assert.IsTrue(result1);
            Assert.AreEqual(1, pn.getAntalGangeGivet());

            bool result2 = pn.givDosis(new Dato { dato = new DateTime(2024, 12, 5) });
            Assert.IsFalse(result2);
            Assert.AreEqual(1, pn.getAntalGangeGivet()); 
        }

        [TestMethod]
        public void TestGetAntalGangeGivet()
        {
            // Arrange
            var startDen = new DateTime(2024, 11, 27);
            var slutDen = new DateTime(2024, 12, 3);
            var laegemiddel = new Laegemiddel { navn = "Paracetamol", enhed = "Ml" };
            var pn = new PN(startDen, slutDen, 1.0, laegemiddel);

            // Act
            pn.givDosis(new Dato { dato = new DateTime(2024, 11, 28) });
            pn.givDosis(new Dato { dato = new DateTime(2024, 11, 29) });
            pn.givDosis(new Dato { dato = new DateTime(2024, 11, 30) });

            // Assert
            Assert.AreEqual(3, pn.getAntalGangeGivet()); 
        }

        [TestMethod]
        public void TestSamletDosis()
        {
            // Arrange
            var startDen = new DateTime(2024, 11, 27);
            var slutDen = new DateTime(2024, 12, 3);
            var laegemiddel = new Laegemiddel { navn = "Paracetamol", enhed = "Ml" };
            var pn = new PN(startDen, slutDen, 1.5, laegemiddel);

            // Act
            pn.givDosis(new Dato { dato = new DateTime(2024, 11, 28) });
            pn.givDosis(new Dato { dato = new DateTime(2024, 11, 29) });
            pn.givDosis(new Dato { dato = new DateTime(2024, 12, 1) });

            double samletDosis = pn.samletDosis();

            // Assert
            Assert.AreEqual(4.5, samletDosis); 
        }

        [TestMethod]
        public void TestDoegnDosis()
        {
            // Arrange
            var startDen = new DateTime(2024, 11, 27);
            var slutDen = new DateTime(2024, 12, 3);
            var laegemiddel = new Laegemiddel {navn = "Paracetamol", enhed = "Ml" };
            var pn = new PN(startDen, slutDen, 2, laegemiddel);

            // Act
            pn.givDosis(new Dato { dato = new DateTime(2024, 11, 27) });
            pn.givDosis(new Dato { dato = new DateTime(2024, 11, 29) });
            pn.givDosis(new Dato { dato = new DateTime(2024, 11, 30) });

            double doegnDosis = pn.doegnDosis();

            // Assert
            Assert.AreEqual(1.5, doegnDosis); 
        }
    }
}

