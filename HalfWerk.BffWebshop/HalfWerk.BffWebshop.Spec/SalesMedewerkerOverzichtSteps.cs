using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace HalfWerk.BffWebshop.Spec
{
    [Binding]
    public class SalesMedewerkerOverzichtSteps
    {
        public SalesMedewerkerOverzichtSteps()
        {
            SpecFlowTestLock.Lock();
        }

        ~SalesMedewerkerOverzichtSteps()
        {
            SpecFlowTestLock.Unlock();
        }

        [Given(@"de volgende bestellingen staan in de database")]
        public void GivenDeVolgendeBestellingenStaanInDeDatabase(Table table)
        {
            Assert.IsTrue(true);
        }
        
        [Given(@"de volgende bestelling staat in het sales medewerker overzicht")]
        public void GivenDeVolgendeBestellingStaatInHetSalesMedewerkerOverzicht(Table table)
        {
            Assert.IsTrue(true);
        }
        
        [When(@"het sales medewerker overzicht is opgevraagd")]
        public void WhenHetSalesMedewerkerOverzichtIsOpgevraagd()
        {
            Assert.IsTrue(true);
        }
        
        [When(@"de sales medewerker vraagt om een aanbetaling")]
        public void WhenDeSalesMedewerkerVraagtOmEenAanbetaling()
        {
            Assert.IsTrue(true);
        }
        
        [Then(@"zie ik alleen bestellingen boven de (.*) euro")]
        public void ThenZieIkAlleenBestellingenBovenDeEuro(int p0)
        {
            Assert.IsTrue(true);
        }
        
        [Then(@"alleen bestellingen met de status Geplaatst")]
        public void ThenAlleenBestellingenMetDeStatusGeplaatst()
        {
            Assert.IsTrue(true);
        }
        
        [Then(@"zie ik de bestelling met id (.*) in het overzicht")]
        public void ThenZieIkDeBestellingMetIdInHetOverzicht(int p0)
        {
            Assert.IsTrue(true);
        }
        
        [Then(@"verandert de status van bestelling met id (.*) in BestalStatus\.AfwachtenOpAanbetaling")]
        public void ThenVerandertDeStatusVanBestellingMetIdInBestalStatus_AfwachtenOpAanbetaling(int p0)
        {
            Assert.IsTrue(true);
        }
    }
}
