using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace HalfWerk.BffWebshop.Spec
{
    [Binding]
    public class MagazijnMedewerkerSteps
    {
        public MagazijnMedewerkerSteps()
        {
            SpecFlowTestLock.Lock();
        }

        ~MagazijnMedewerkerSteps()
        {
            SpecFlowTestLock.Unlock();
        }

        [Given(@"ik ben ingelogd als magazijnmedewerker")]
        public void GivenIkBenIngelogdAlsMagazijnmedewerker()
        {
            Assert.IsTrue(true);
        }
        
        [Given(@"ik geef aan een volgende bestelling te willen inpakken")]
        public void GivenIkGeefAanEenVolgendeBestellingTeWillenInpakken()
        {
            Assert.IsTrue(true);
        }
        
        [Given(@"ik heb een in te pakken bestelling geopend")]
        public void GivenIkHebEenInTePakkenBestellingGeopend()
        {
            Assert.IsTrue(true);
        }
        
        [Given(@"ik druk op de knop factuur printen")]
        public void GivenIkDrukOpDeKnopFactuurPrinten()
        {
            Assert.IsTrue(true);
        }
        
        [Given(@"ik druk op de knop adreslabel printen")]
        public void GivenIkDrukOpDeKnopAdreslabelPrinten()
        {
            Assert.IsTrue(true);
        }
        
        [Given(@"ik heb alle artikelen in de bestelling verzameld")]
        public void GivenIkHebAlleArtikelenInDeBestellingVerzameld()
        {
            Assert.IsTrue(true);
        }
        
        [Given(@"ik heb een factuur geprint")]
        public void GivenIkHebEenFactuurGeprint()
        {
            Assert.IsTrue(true);
        }
        
        [Given(@"ik heb een adreslabel geprint")]
        public void GivenIkHebEenAdreslabelGeprint()
        {
            Assert.IsTrue(true);
        }
        
        [When(@"ik de bestelling klaarmeld")]
        public void WhenIkDeBestellingKlaarmeld()
        {
            Assert.IsTrue(true);
        }
        
        [Then(@"opent de eerstvolgende bestelling met de status Goedgekeurd")]
        public void ThenOpentDeEerstvolgendeBestellingMetDeStatusGoedgekeurd()
        {
            Assert.IsTrue(true);
        }
        
        [Then(@"print er een factuur")]
        public void ThenPrintErEenFactuur()
        {
            Assert.IsTrue(true);
        }
        
        [Then(@"print er een adreslabel")]
        public void ThenPrintErEenAdreslabel()
        {
            Assert.IsTrue(true);
        }
        
        [Then(@"wordt de status van de bestelling verzonden")]
        public void ThenWordtDeStatusVanDeBestellingVerzonden()
        {
            Assert.IsTrue(true);
        }
        
        [Then(@"ga ik terug naar het nieuwe bestelling inzien scherm")]
        public void ThenGaIkTerugNaarHetNieuweBestellingInzienScherm()
        {
            Assert.IsTrue(true);
        }
    }
}
