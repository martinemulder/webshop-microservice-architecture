using HalfWerk.BffWebshop.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DsBestelServiceBestelStatus = HalfWerk.CommonModels.DsBestelService.Models.BestelStatus;
using BffWebshopBestelStatus = HalfWerk.CommonModels.BffWebshop.BestellingService.BestelStatus;

namespace HalfWerk.BffWebshop.Test.Helpers
{
    [TestClass]
    public class BestelStatusEnumTest
    {
        [TestMethod]
        public void BestelStatusEnumShouldMatch()
        {
            Assert.IsTrue((int) BffWebshopBestelStatus.Geplaatst == (int) DsBestelServiceBestelStatus.Geplaatst, "Status: Geplaatst should match");
            Assert.IsTrue((int) BffWebshopBestelStatus.Goedgekeurd == (int) DsBestelServiceBestelStatus.Goedgekeurd, "Status: Goedgekeurd should match");
            Assert.IsTrue((int) BffWebshopBestelStatus.WordtIngepakt == (int) DsBestelServiceBestelStatus.WordtIngepakt, "Status: WordtIngepakt should match");
            Assert.IsTrue((int) BffWebshopBestelStatus.Verzonden == (int) DsBestelServiceBestelStatus.Verzonden, "Status: Verzonden should match");
            Assert.IsTrue((int) BffWebshopBestelStatus.Betaald == (int) DsBestelServiceBestelStatus.Betaald, "Status: Betaald should match");
            Assert.IsTrue((int) BffWebshopBestelStatus.Afgekeurd == (int) DsBestelServiceBestelStatus.Afgekeurd, "Status: Afgekeurd should match");
            Assert.IsTrue((int) BffWebshopBestelStatus.WachtenOpAanbetaling == (int) DsBestelServiceBestelStatus.WachtenOpAanbetaling, "Status: WachtenOpAanbetaling should match");
            Assert.IsTrue((int) BffWebshopBestelStatus.Afgerond == (int) DsBestelServiceBestelStatus.Afgerond, "Status: Afgerond should match");
        }

        [TestMethod]
        public void CastTo_ShouldCastStatusToProvidedEnumStatus()
        {
            var status1 = BffWebshopBestelStatus.Geplaatst;
            var status2 = BffWebshopBestelStatus.Verzonden;
            var status3 = BffWebshopBestelStatus.Afgerond;

            Assert.AreEqual(DsBestelServiceBestelStatus.Geplaatst, status1.CastTo<DsBestelServiceBestelStatus>());
            Assert.AreEqual(DsBestelServiceBestelStatus.Verzonden, status2.CastTo<DsBestelServiceBestelStatus>());
            Assert.AreEqual(DsBestelServiceBestelStatus.Afgerond, status3.CastTo<DsBestelServiceBestelStatus>());
        }
    }
}