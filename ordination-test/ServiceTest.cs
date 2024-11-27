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
    [ExpectedException(typeof(ArgumentException))]
    public void TestAtKodenSmiderEnException()
    {
        // Herunder skal man så kalde noget kode,
        // der smider en exception.

        // Hvis koden _ikke_ smider en exception,
        // så fejler testen.

        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        Dosis dosis = new Dosis(DateTime.Now, 2);
        Dosis[] doser = new[]
        {
        new Dosis(DateTime.Now.AddHours(8), 5.0),
        new Dosis(DateTime.Now.AddHours(16), 5.0)
        };

        //// TC1
        //service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId,
        //    -1, 0, 2, 0, DateTime.Now.AddDays(20), DateTime.Now.AddDays(20));

        ////TC2
        //service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId,
        //    0, 0, 0, 0, DateTime.Now.AddDays(20), DateTime.Now.AddDays(20));

        ////TC3
        //service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId,
        //    2, 0, 2, 0, DateTime.Now.AddDays(20), DateTime.Now.AddDays(20));

        ////TC4
        //service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId,
        //    2, 0, 2, 0, DateTime.Now.AddDays(30), DateTime.Now.AddDays(20));

        ////TC5
        //service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId,
        //               2, 0, 2, 0, DateTime.Now.AddDays(20), DateTime.Now.AddDays(20));

        ////TC6
        //service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId,
        //               2, 0, 2, 0, DateTime.Now.AddDays(20), DateTime.Now.AddDays(40));

        //TC11
        //service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId, doser, DateTime.Now.AddDays(10), DateTime.Now.AddDays(5));

        ////TC12
        //service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId, doser, DateTime.Now.AddDays(5), DateTime.Now.AddDays(5));

        ////TC13
        //service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId, doser, DateTime.Now.AddDays(5), DateTime.Now.AddDays(10));

        ////TC14
        //service.OpretPN(patient.PatientId, lm.LaegemiddelId, -1, DateTime.Now.AddDays(10), DateTime.Now.AddDays(5));

        ////TC15
        //service.OpretPN(patient.PatientId, lm.LaegemiddelId, 0, DateTime.Now.AddDays(10), DateTime.Now.AddDays(5));

        //TC16
        //service.OpretPN(patient.PatientId, lm.LaegemiddelId, 100, DateTime.Now.AddDays(5), DateTime.Now.AddDays(5));

        //TC17
        //service.OpretPN(patient.PatientId, lm.LaegemiddelId, 100, DateTime.Now.AddDays(5), DateTime.Now.AddDays(10));

        //TC18
        //service.AnvendOrdination(1, new Dato { dato = DateTime.Now.AddYears(10) });

        ////TC19
        //PN pn = service.GetPNs().First(); // Get an existing PN ordination
        //DateTime validDate = pn.startDen.AddDays(1); // Date within the validity period

        //// Act
        //string result = service.AnvendOrdination(pn.OrdinationId, new Dato { dato = validDate });

        //// Assert
        //Assert.AreEqual("Dosis givet", result);

        //Console.WriteLine("Her kommer der ikke en exception. Testen fejler.");
    }

    [TestMethod]
    public void AnvendOrdination_ValidDateWithinPeriod_DoseGiven()
    {
        // Arrange
        PN pn = service.GetPNs().First(); // Get an existing PN ordination
        DateTime validDate = pn.startDen.AddDays(1); // Date within the validity period

        // Act
        string result = service.AnvendOrdination(pn.OrdinationId, new Dato { dato = validDate });

        // Assert
        Assert.AreEqual("Dosis givet", result);
    }

    [TestMethod]
    public void AnvendOrdination_ValidDateWithinPeriod_DoseNotGiven()
    {
        // Arrange
        PN pn = service.GetPNs().First(); // Get an existing PN ordination
        DateTime validDate = pn.startDen.AddDays(100); // Date within the validity period

        // Act
        string result = service.AnvendOrdination(pn.OrdinationId, new Dato { dato = validDate });

        // Assert
        Assert.AreEqual("Dosis ikke givet", result);
    }


}