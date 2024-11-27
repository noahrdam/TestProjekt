using shared.Model;

namespace ordination_test
{

[TestClass]
public class DagligSkævTest
{
    [TestMethod]
    public void TestDoegnDosis()
    {
        // Arrange
        var startDen = new DateTime(2024, 11, 27);
        var slutDen = new DateTime(2024, 11, 30);
        var laegemiddel = new Laegemiddel { navn = "Paracetamol", enhed = "Ml" };
        var dagligSkæv = new DagligSkæv(startDen, slutDen, laegemiddel);

        dagligSkæv.opretDosis(new DateTime(2024, 11, 27, 8, 0, 0), 4); 
        dagligSkæv.opretDosis(new DateTime(2024, 11, 27, 12, 0, 0), 1); 
        dagligSkæv.opretDosis(new DateTime(2024, 11, 27, 18, 0, 0), 2.5); 
        dagligSkæv.opretDosis(new DateTime(2024, 11, 27, 23, 0, 0), 2); 

        // Act
        double doegnDosis = dagligSkæv.doegnDosis();

        // Assert
        Assert.AreEqual(9.5, doegnDosis); // 4 + 1 + 2.5 + 2 = 9.5
    }

    [TestMethod]
    public void TestSamletDosis()
    {
        // Arrange
        var startDen = new DateTime(2024, 11, 27);
        var slutDen = new DateTime(2024, 11, 30);
        var laegemiddel = new Laegemiddel { navn = "Paracetamol", enhed = "Ml" };
        var dagligSkæv = new DagligSkæv(startDen, slutDen, laegemiddel);

        dagligSkæv.opretDosis(new DateTime(2024, 11, 27, 8, 0, 0), 3); 
        dagligSkæv.opretDosis(new DateTime(2024, 11, 27, 12, 0, 0), 3); 
        dagligSkæv.opretDosis(new DateTime(2024, 11, 27, 18, 0, 0), 1); 
        dagligSkæv.opretDosis(new DateTime(2024, 11, 27, 23, 0, 0), 1.5);
        dagligSkæv.opretDosis(new DateTime(2024, 11, 27, 23, 0, 0), 2);

            // Act
            double samletDosis = dagligSkæv.samletDosis();

        // Assert
        Assert.AreEqual(42, samletDosis); // 4 days * 10.5 (doegnDosis)
    }
}

}
