namespace ordination_test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using shared.Model;
    using System;

    [TestClass]
    public class PNTEST
    {
        // TC14: EQ5 - Tester at dosis ikke kan være -1
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void OpretPN_NegativDosis_KasterArgumentException()
        {
            var startDato = DateTime.Now;
            var slutDato = startDato.AddDays(1);
            var laegemiddel = new Laegemiddel();  

            PN pn = new PN(startDato, slutDato, -1, laegemiddel); // Fejl da dosis ikke kan være negativ
        }

        // TC15: EQ5 - Tester at dosis ikke kan være 0
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void OpretPN_ZeroDosis_KasterArgumentException()
        {
            var startDato = DateTime.Now;
            var slutDato = startDato.AddDays(1);
            var laegemiddel = new Laegemiddel();  

            PN pn = new PN(startDato, slutDato, 0, laegemiddel); // Fejl da dosis ikke kan være 0
        }

        // TC16: EQ5 - Tester at dosis kan være 100 
        [TestMethod]
        public void OpretPN_ValidDosis_ReturnererKorrektObjekt()
        {
            var startDato = DateTime.Now;
            var slutDato = startDato.AddDays(1);
            var laegemiddel = new Laegemiddel();  

            PN pn = new PN(startDato, slutDato, 100, laegemiddel); // Success da dosis er positiv

            Assert.IsNotNull(pn); 
            Assert.AreEqual(100, pn.antalEnheder); // Dosis skal være 100
        }

        // TC17: EQ6 - Tester at startdatosenere end slutdato
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void OpretPN_StartdatoErSenereEndSlutdato_KasterArgumentException()
        {
            var startDato = new DateTime(2024, 11, 24);
            var slutDato = new DateTime(2024, 11, 20); 
            var laegemiddel = new Laegemiddel();  

            PN pn = new PN(startDato, slutDato, 1, laegemiddel); // fejl, startdato er senere end slutdato
        }

        // TC18: EQ6 - Teste(startdato = slutdato)
        [TestMethod]
        public void OpretPN_StartdatoLigeSlutdato_Success()
        {
            var startDato = new DateTime(2024, 11, 24);
            var slutDato = new DateTime(2024, 11, 24);  
            var laegemiddel = new Laegemiddel();  

            PN pn = new PN(startDato, slutDato, 1, laegemiddel); // Success da startdato = slutdato

            Assert.IsNotNull(pn); 
            Assert.AreEqual(startDato, pn.startDen); // Startdato og slutdato skal være ens
            Assert.AreEqual(slutDato, pn.slutDen); 
        }

        // TC19: EQ7 - Anvend Ordination den 22/11/2024 (Fejl)
        [TestMethod]
        public void GivDosis_22Nov2024_Fejl()
        {
            var startDato = new DateTime(2024, 11, 23);
            var slutDato = new DateTime(2024, 11, 26);
            var laegemiddel = new Laegemiddel();
            PN pn = new PN(startDato, slutDato, 1, laegemiddel);

            var anvendelse = new Dato { dato = new DateTime(2024, 11, 22) };

            var resultat = pn.givDosis(anvendelse); // Fejl da datoen er før gyldighedsperioden

            Assert.IsFalse(resultat); 
        }

        // TC20: EQ7 - Anvend Ordination den 23/11/2024 (Success)
        [TestMethod]
        public void GivDosis_23Nov2024_Success()
        {
            var startDato = new DateTime(2024, 11, 23);
            var slutDato = new DateTime(2024, 11, 26);
            var laegemiddel = new Laegemiddel();
            PN pn = new PN(startDato, slutDato, 1, laegemiddel);

            var anvendelse = new Dato { dato = new DateTime(2024, 11, 23) };

            var resultat = pn.givDosis(anvendelse); // Success da datoen er indenfor gyldighedsperioden

            Assert.IsTrue(resultat); 
        }

        // TC21: EQ7 - Anvend Ordination den 25/11/2024 (Success)
        [TestMethod]
        public void GivDosis_25Nov2024_Success()
        {
            var startDato = new DateTime(2024, 11, 23);
            var slutDato = new DateTime(2024, 11, 26);
            var laegemiddel = new Laegemiddel();
            PN pn = new PN(startDato, slutDato, 1, laegemiddel);

            var anvendelse = new Dato { dato = new DateTime(2024, 11, 25) };

            var resultat = pn.givDosis(anvendelse); // Success da datoen er indenfor gyldighedsperioden

            Assert.IsTrue(resultat); 
        }

        // TC22: EQ7 - Anvend Ordination den 26/11/2024 (Success)
        [TestMethod]
        public void GivDosis_26Nov2024_Success()
        {
            var startDato = new DateTime(2024, 11, 23);
            var slutDato = new DateTime(2024, 11, 26);
            var laegemiddel = new Laegemiddel();
            PN pn = new PN(startDato, slutDato, 1, laegemiddel);

            var anvendelse = new Dato { dato = new DateTime(2024, 11, 26) };

            var resultat = pn.givDosis(anvendelse); 

            Assert.IsTrue(resultat); 
        }

       // EQ7 - Anvend Ordination den 27/11/2024 (Fejl)
        [TestMethod]
        public void GivDosis_27Nov2024_Fejl()
        {
            var startDato = new DateTime(2024, 11, 23);
            var slutDato = new DateTime(2024, 11, 26);
            var laegemiddel = new Laegemiddel();
            PN pn = new PN(startDato, slutDato, 1, laegemiddel);

            var anvendelse = new Dato { dato = new DateTime(2024, 11, 27) };

            var resultat = pn.givDosis(anvendelse);

            Assert.IsFalse(resultat); 
        }
    }
}
