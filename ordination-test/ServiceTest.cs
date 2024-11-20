namespace ordination_test;

using Microsoft.EntityFrameworkCore;

using Service;
using Data;
using shared.Model;

[TestClass]
public class ServiceTest
{
    private DataService service;

    [TestInitialize]
    public void SetupBeforeEachTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrdinationContext>();
        optionsBuilder.UseInMemoryDatabase(databaseName: "test-database");
        var context = new OrdinationContext(optionsBuilder.Options);
        service = new DataService(context);
        service.SeedData();
    }

    [TestMethod]
    public void PatientsExist()
    {
        Assert.IsNotNull(service.GetPatienter());
    }

    [TestMethod]
    public void OpretDagligFast()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        Assert.AreEqual(1, service.GetDagligFaste().Count());

        service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId,
            2, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));

        Assert.AreEqual(2, service.GetDagligFaste().Count());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestAtKodenSmiderEnException()
    {
        // Herunder skal man så kalde noget kode,
        // der smider en exception.

        // Hvis koden _ikke_ smider en exception,
        // så fejler testen.

        Console.WriteLine("Her kommer der ikke en exception. Testen fejler.");
    }

    [TestMethod]
    public void TestDoegnDosis_DagligSkaev()
    {
        // Arrange
        var startDen = new DateTime(2023, 1, 1);
        var slutDen = new DateTime(2023, 1, 10);
        var laegemiddel = new Laegemiddel("TestMed", 1.0, 1.0, 1.0, "mg");
        var doser = new Dosis[]
        {
            new Dosis(new DateTime(2023, 1, 1, 8, 0, 0), 1.0),
            new Dosis(new DateTime(2023, 1, 1, 12, 0, 0), 2.0),
            new Dosis(new DateTime(2023, 1, 1, 18, 0, 0), 3.0),
            new Dosis(new DateTime(2023, 1, 1, 22, 0, 0), 4.0)
        };
        var dagligSkaev = new DagligSkæv(startDen, slutDen, laegemiddel, doser);

        // Act
        double result = dagligSkaev.doegnDosis();

        // Assert
        Assert.AreEqual(10.0, result);
    }
    }