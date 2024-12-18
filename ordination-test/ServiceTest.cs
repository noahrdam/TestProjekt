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
    public void TestOpretDagligFast_TC1()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        // EQ1: Invalid dosis
        service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId, -1, 3, -2, 0, DateTime.Now, DateTime.Now.AddDays(3));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestOpretDagligFast_TC2()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        // EQ1: Grænse (samlet dosis 0)
        service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId, 0, 0, 0, 0, DateTime.Now, DateTime.Now.AddDays(3));
    }

    [TestMethod]
    public void TestOpretDagligFast_TC3()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        // EQ1: Gyldig dosis
        var ordination = service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId, 2, 0, 2, 0, DateTime.Now, DateTime.Now.AddDays(3));
        Assert.IsNotNull(ordination);
    }


    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestOpretDagligFast_TC4()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        // EQ2: Ugyldig (startdato > slutdato)
        service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId, 2, 2, 1, 0, DateTime.Now.AddDays(5), DateTime.Now);
    }

    [TestMethod]
    public void TestOpretDagligFast_TC5()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        // EQ2: Grænse (startdato = slutdato)
        var ordination = service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId, 2, 2, 1, 0, DateTime.Now, DateTime.Now);
        Assert.IsNotNull(ordination);
    }

    [TestMethod]
    public void TestOpretDagligFast_TC6()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        // EQ2: Gyldig (startdato < slutdato)
        var ordination = service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId, 2, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));
        Assert.IsNotNull(ordination);
    }


    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestOpretDagligSkaev_TC7()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        DateTime startDato = DateTime.Now;
        DateTime slutDato = DateTime.Now.AddDays(5);

        // EQ3: Ugyldig (negativ dosis)
        Dosis[] doser = new[]
        {
            new Dosis(DateTime.Now, -1.0),
        };

        // Act
        var dagligSkaev = service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId, doser, startDato, slutDato);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestOpretDagligSkaev_TC8()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        DateTime startDato = DateTime.Now;
        DateTime slutDato = DateTime.Now.AddDays(5);

        // EQ3: Ugyldig (dosis = 0)
        Dosis[] doser = new[]
        {
            new Dosis(DateTime.Now, 0.0),
        };

        // Act
        var dagligSkaev = service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId, doser, startDato, slutDato);
    }

    [TestMethod]
    public void TestOpretDagligSkaev_TC9()
    {
        // Arrange
        // Seed data: Patient "Jane Jensen" (63.4 kg) and Medication "Acetylsalicylsyre"
        Patient patient = service.GetPatienter().First(p => p.cprnr == "121256-0512");
        Laegemiddel laegemiddel = service.GetLaegemidler().First(lm => lm.navn == "Acetylsalicylsyre");

        DateTime startDato = DateTime.Now;
        DateTime slutDato = DateTime.Now.AddDays(5);

        // Calculate recommended dose for "Jane Jensen" and "Acetylsalicylsyre"
        double anbefaletDosis = service.GetAnbefaletDosisPerDøgn(patient.PatientId, laegemiddel.LaegemiddelId);

        // Create doses that does not exceed the recommended dose (e.g., total dose > 9.51)
        Dosis[] doser = new[]
        {
            new Dosis(DateTime.Now, 3.0),
            new Dosis(DateTime.Now, 3.0) // Total = 6.0 < 9.51
        };

        // Act
        // EQ3: Gyldig (total dosis < anbefalet dosis)
        var dagligSkaev = service.OpretDagligSkaev(patient.PatientId, laegemiddel.LaegemiddelId, doser, startDato, slutDato);
        Assert.IsNotNull(dagligSkaev, "DagligSkæv-ordinationen blev ikke oprettet.");

    }

    [TestMethod]
    public void TestOpretDagligSkaev_TC10()
    {
        // Arrange
        // Seed data: Patient "Jane Jensen" (63.4 kg) and Medication "Acetylsalicylsyre"
        Patient patient = service.GetPatienter().First(p => p.cprnr == "121256-0512");
        Laegemiddel laegemiddel = service.GetLaegemidler().First(lm => lm.navn == "Acetylsalicylsyre");

        DateTime startDato = DateTime.Now;
        DateTime slutDato = DateTime.Now.AddDays(5);

        // Calculate recommended dose for "Jane Jensen" and "Acetylsalicylsyre"
        double anbefaletDosis = service.GetAnbefaletDosisPerDøgn(patient.PatientId, laegemiddel.LaegemiddelId);

        // Create doses is the recommended dose (e.g., total dose > 9.51)
        Dosis[] doser = new[]
        {
            new Dosis(DateTime.Now, 9.0),
            new Dosis(DateTime.Now, 0.51) // Total = 9.51 = 9.51
        };

        // Act
        //EQ3: Gyldig (total dosis = anbefalet dosis)
        var dagligSkaev = service.OpretDagligSkaev(patient.PatientId, laegemiddel.LaegemiddelId, doser, startDato, slutDato);
        Assert.IsNotNull(dagligSkaev, "DagligSkæv-ordinationen blev ikke oprettet.");

    }



    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestOpretDagligSkaev_TC11()
    {
        // Arrange
        // Seed data: Patient "Jane Jensen" (63.4 kg) and Medication "Acetylsalicylsyre"
        Patient patient = service.GetPatienter().First(p => p.cprnr == "121256-0512");
        Laegemiddel laegemiddel = service.GetLaegemidler().First(lm => lm.navn == "Acetylsalicylsyre");

        DateTime startDato = DateTime.Now;
        DateTime slutDato = DateTime.Now.AddDays(5);

        // Calculate recommended dose for "Jane Jensen" and "Acetylsalicylsyre"
        double anbefaletDosis = service.GetAnbefaletDosisPerDøgn(patient.PatientId, laegemiddel.LaegemiddelId);

        // Create doses that exceed the recommended dose (e.g., total dose > 9.51)
        Dosis[] doser = new[]
        {
            new Dosis(DateTime.Now, 5.0),
            new Dosis(DateTime.Now, 5.0) // Total = 6.0 < 9.51
        };

        // Act
        // EQ3: Ugyldig (total dosis > anbefalet dosis)
        service.OpretDagligSkaev(patient.PatientId, laegemiddel.LaegemiddelId, doser, startDato, slutDato);
    }

  

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestOpretDagligSkaev_TC12()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        Dosis[] doser = new[]
        {
            new Dosis(DateTime.Now.AddHours(8), 1)
        };

        // EQ4: Ugyldig (startdato > slutdato)
        service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId, doser, DateTime.Now, DateTime.Now.AddDays(-5));
    }


    [TestMethod]
    public void TestOpretDagligSkaev_TC13()
    {
        // Arrange
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        DateTime startDato = DateTime.Now;
        DateTime slutDato = DateTime.Now;

        Dosis[] doser = new[]
        {
        new Dosis(DateTime.Now.AddHours(8), 1.0),
        new Dosis(DateTime.Now.AddHours(16), 2.0)
    };

        // Act
        var dagligSkaev = service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId, doser, startDato, slutDato);

        // Assert
        Assert.IsNotNull(dagligSkaev, "DagligSkæv-ordinationen blev ikke oprettet.");
        Assert.AreEqual(startDato, dagligSkaev.startDen, "Startdato matcher ikke.");
        Assert.AreEqual(slutDato, dagligSkaev.slutDen, "Slutdato matcher ikke.");
        Assert.AreEqual(2, dagligSkaev.doser.Count, "Antal doser matcher ikke.");
    }


    [TestMethod]
    public void TestOpretDagligSkaev_TC14()
    {
        // Arrange
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        DateTime startDato = DateTime.Now;
        DateTime slutDato = DateTime.Now.AddDays(3);

        Dosis[] doser = new[]
        {
            new Dosis(DateTime.Now.AddHours(8), 1.5),
            new Dosis(DateTime.Now.AddHours(16), 2.0)
        };

        // Act
        var dagligSkaev = service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId, doser, startDato, slutDato);

        // Assert
        Assert.IsNotNull(dagligSkaev, "DagligSkæv-ordinationen blev ikke oprettet.");
        Assert.AreEqual(startDato, dagligSkaev.startDen, "Startdato matcher ikke.");
        Assert.AreEqual(slutDato, dagligSkaev.slutDen, "Slutdato matcher ikke.");
        Assert.AreEqual(2, dagligSkaev.doser.Count, "Antal doser matcher ikke.");
        Assert.AreEqual(1.5, dagligSkaev.doser[0].antal, "Første dosis matcher ikke.");
        Assert.AreEqual(2.0, dagligSkaev.doser[1].antal, "Anden dosis matcher ikke.");
    }


    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestOpretPN_TC15()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        // EQ5: Ugyldig (dosis < 0)
        service.OpretPN(patient.PatientId, lm.LaegemiddelId, -1, DateTime.Now, DateTime.Now.AddDays(3));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestOpretPN_TC16()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        // EQ5: Ugyldig (dosis = 0)
        service.OpretPN(patient.PatientId, lm.LaegemiddelId, 0, DateTime.Now, DateTime.Now.AddDays(3));
    }

    [TestMethod]
    public void TestOpretPN_TC17()
    {
        // Arrange: Opsæt testdata
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        DateTime startDato = DateTime.Now;
        DateTime slutDato = DateTime.Now.AddDays(3);
        double dosis = 100;

        // Act: Udfør handlingen (kald metoden, der testes)
        var pn = service.OpretPN(patient.PatientId, lm.LaegemiddelId, dosis, startDato, slutDato);

        // Assert: Verificér resultatet
        Assert.IsNotNull(pn, "PN-ordinationen blev ikke oprettet.");
        Assert.AreEqual(startDato, pn.startDen, "Startdato matcher ikke.");
        Assert.AreEqual(slutDato, pn.slutDen, "Slutdato matcher ikke.");
        Assert.AreEqual(dosis, pn.antalEnheder, "Dosis matcher ikke.");
        Assert.AreEqual(lm.LaegemiddelId, pn.laegemiddel.LaegemiddelId, "Lægemiddel matcher ikke.");
    }


    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestOpretPN_TC18()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        // EQ6: Ugyldig (startdato > slutdato)
        service.OpretPN(patient.PatientId, lm.LaegemiddelId, 100, DateTime.Now.AddDays(5), DateTime.Now);
    }


    [TestMethod]
    public void TestOpretPN_TC19()
    {
        // Arrange: Opsæt testdata
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        DateTime startDato = DateTime.Now;
        DateTime slutDato = DateTime.Now.AddDays(5);
        double dosis = 100;

        // Act: Kald metoden for at oprette PN
        var pn = service.OpretPN(patient.PatientId, lm.LaegemiddelId, dosis, startDato, slutDato);

        // Assert: Valider resultatet
        Assert.IsNotNull(pn, "PN-ordinationen blev ikke oprettet.");
        Assert.AreEqual(startDato, pn.startDen, "Startdato matcher ikke.");
        Assert.AreEqual(slutDato, pn.slutDen, "Slutdato matcher ikke.");
        Assert.AreEqual(dosis, pn.antalEnheder, "Dosis matcher ikke.");
        Assert.AreEqual(lm.LaegemiddelId, pn.laegemiddel.LaegemiddelId, "Lægemiddel matcher ikke.");

        // Valider, at PN-ordinationen er blevet tilføjet til patientens ordinationer
        var patientOrdination = service.GetPatienter().First(p => p.PatientId == patient.PatientId)
                                        .ordinationer.FirstOrDefault(o => o.OrdinationId == pn.OrdinationId);
        Assert.IsNotNull(patientOrdination, "PN-ordinationen blev ikke korrekt tilføjet til patientens ordinationer.");
    }


    [TestMethod]
    public void TestOpretPN_TC20()
    {
        // Arrange
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        DateTime startDato = DateTime.Now;
        DateTime slutDato = DateTime.Now;

        // Act
        var pn = service.OpretPN(patient.PatientId, lm.LaegemiddelId, 100, startDato, slutDato);

        // Assert
        Assert.IsNotNull(pn, "PN-ordinationen blev ikke oprettet.");
        Assert.AreEqual(startDato, pn.startDen, "Startdato matcher ikke.");
        Assert.AreEqual(slutDato, pn.slutDen, "Slutdato matcher ikke.");
        Assert.AreEqual(100, pn.antalEnheder, "Antal matcher ikke.");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestAnvendOrdinationPN_TC21()
    {
        PN pn = service.GetPNs().First();
        DateTime dato = pn.startDen.AddDays(-1); // Dato før gyldighedsperioden

        // EQ7: Ugyldig (dato før gyldighedsperioden)
        service.AnvendOrdination(pn.OrdinationId, new Dato { dato = dato });
    }

    [TestMethod]
    public void TestAnvendOrdinationPN_TC22()
    {
        // Arrange
        PN pn = service.GetPNs().First();
        DateTime dato = pn.startDen; // Dato præcis på startdatoen

        // Act
        var result = service.AnvendOrdination(pn.OrdinationId, new Dato { dato = dato });

        // Assert
        Assert.AreEqual("Dosis givet", result, "Anvendelse af ordinationen på startdatoen fejlede.");
    }

    [TestMethod]
    public void TestAnvendOrdinationPN_TC23()
    {
        // Arrange
        PN pn = service.GetPNs().First();
        DateTime dato = pn.startDen.AddDays(3); //Dato indenfor gyldighedsperioden

        // Act
        var result = service.AnvendOrdination(pn.OrdinationId, new Dato { dato = dato });

        // Assert
        Assert.AreEqual("Dosis givet", result, "Anvendelse af ordinationen fejlede.");
    }

    [TestMethod]
    public void TestAnvendOrdinationPN_TC24()
    {
        // Arrange
        PN pn = service.GetPNs().First();
        DateTime dato = pn.slutDen; //Dato præcis på slutdatoen

        // Act
        var result = service.AnvendOrdination(pn.OrdinationId, new Dato { dato = dato });

        // Assert
        Assert.AreEqual("Dosis givet", result, "Anvendelse");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestAnvendOrdinationPN_TC25()
    {
        PN pn = service.GetPNs().First();
        DateTime dato = pn.slutDen.AddDays(1); // Dato efter gyldighedsperioden

        // EQ8: Ugyldig (dato efter gyldighedsperioden)
        service.AnvendOrdination(pn.OrdinationId, new Dato { dato = dato });
    }

    

}