using HalfWerk.CommonModels.DsBestelService.Models;
using HalfWerk.DsBestelService.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HalfWerk.DsBestelService.Test.Helpers
{
    [TestClass]
    public class BestelStatusHelperTest
    {
        [TestMethod]
        public void UpdateIsAllowed_ShouldReturnTrueWhenUpdateIsAllowed()
        {
            Assert.IsTrue(BestelStatus.Geplaatst.UpdateIsAllowed(BestelStatus.Goedgekeurd), "from geplaatst to goedgekeurd");
            Assert.IsTrue(BestelStatus.Geplaatst.UpdateIsAllowed(BestelStatus.Betaald), "from geplaatst to betaald");
            Assert.IsTrue(BestelStatus.Geplaatst.UpdateIsAllowed(BestelStatus.Afgekeurd), "from geplaatst to afgekeurd");
            Assert.IsTrue(BestelStatus.Geplaatst.UpdateIsAllowed(BestelStatus.WachtenOpAanbetaling), "from geplaatst to wachtenOpAanbetaling");

            Assert.IsTrue(BestelStatus.Goedgekeurd.UpdateIsAllowed(BestelStatus.WordtIngepakt), "from goedgekeurd to wordtIngepakt");
            Assert.IsTrue(BestelStatus.Goedgekeurd.UpdateIsAllowed(BestelStatus.Betaald), "from goedgekeurd to betaald");
            Assert.IsTrue(BestelStatus.Goedgekeurd.UpdateIsAllowed(BestelStatus.Afgekeurd), "from goedgekeurd to afgekeurd");

            Assert.IsTrue(BestelStatus.Betaald.UpdateIsAllowed(BestelStatus.Verzonden), "from betaald to verzonden");
            Assert.IsTrue(BestelStatus.Betaald.UpdateIsAllowed(BestelStatus.Afgerond), "from betaald to afgerond");

            Assert.IsTrue(BestelStatus.WachtenOpAanbetaling.UpdateIsAllowed(BestelStatus.Betaald), "from wachtenOpAanbetaling to betaald");
            Assert.IsTrue(BestelStatus.WachtenOpAanbetaling.UpdateIsAllowed(BestelStatus.Goedgekeurd), "from wachtenOpAanbetaling to goedgekeurd");
            Assert.IsTrue(BestelStatus.WachtenOpAanbetaling.UpdateIsAllowed(BestelStatus.Afgekeurd), "from wachtenOpAanbetaling to afgekeurd");

            Assert.IsTrue(BestelStatus.Verzonden.UpdateIsAllowed(BestelStatus.Afgerond), "from verzonden to afgerond");
            Assert.IsTrue(BestelStatus.Afgekeurd.UpdateIsAllowed(BestelStatus.Goedgekeurd), "from afgekeurd to goedgekeurd");
            Assert.IsTrue(BestelStatus.WordtIngepakt.UpdateIsAllowed(BestelStatus.Verzonden), "from wordtIngepakt to verzonden");
        }

        [TestMethod]
        public void UpdateIsAllowed_ShouldReturnFalseWhenUpdateIsNotAllowed()
        {
            Assert.IsFalse(BestelStatus.Geplaatst.UpdateIsAllowed(BestelStatus.Afgerond), "from geplaatst to afgerond");
            Assert.IsFalse(BestelStatus.Geplaatst.UpdateIsAllowed(BestelStatus.Verzonden), "from geplaatst to verzonden");
            Assert.IsFalse(BestelStatus.Geplaatst.UpdateIsAllowed(BestelStatus.WordtIngepakt), "from geplaatst to wordtIngepakt");

            Assert.IsFalse(BestelStatus.Goedgekeurd.UpdateIsAllowed(BestelStatus.Geplaatst), "from goedgekeurd to geplaatst");
            Assert.IsFalse(BestelStatus.Goedgekeurd.UpdateIsAllowed(BestelStatus.Verzonden), "from goedgekeurd to verzonden");
            Assert.IsFalse(BestelStatus.Goedgekeurd.UpdateIsAllowed(BestelStatus.Afgerond), "from goedgekeurd to afgerond");

            Assert.IsFalse(BestelStatus.Betaald.UpdateIsAllowed(BestelStatus.Geplaatst), "from betaald to geplaatst");
            Assert.IsFalse(BestelStatus.Betaald.UpdateIsAllowed(BestelStatus.Goedgekeurd), "from betaald to goedgekeurd");
            Assert.IsFalse(BestelStatus.Betaald.UpdateIsAllowed(BestelStatus.Afgekeurd), "from betaald to afgekeurd");

            Assert.IsFalse(BestelStatus.Afgekeurd.UpdateIsAllowed(BestelStatus.Geplaatst), "from afgekeurd to geplaatst");
            Assert.IsFalse(BestelStatus.Afgekeurd.UpdateIsAllowed(BestelStatus.WordtIngepakt), "from afgekeurd to wordtIngepakt");
            Assert.IsFalse(BestelStatus.Afgekeurd.UpdateIsAllowed(BestelStatus.Verzonden), "from afgekeurd to verzonden");
            Assert.IsFalse(BestelStatus.Afgekeurd.UpdateIsAllowed(BestelStatus.Betaald), "from afgekeurd to betaald");
            Assert.IsFalse(BestelStatus.Afgekeurd.UpdateIsAllowed(BestelStatus.WachtenOpAanbetaling), "from afgekeurd to wachtenOpAanbetaling");
            Assert.IsFalse(BestelStatus.Afgekeurd.UpdateIsAllowed(BestelStatus.Afgerond), "from afgekeurd to afgerond");

            Assert.IsFalse(BestelStatus.WachtenOpAanbetaling.UpdateIsAllowed(BestelStatus.Geplaatst), "from wachtenOpAanbetaling to geplaatst");
            Assert.IsFalse(BestelStatus.WachtenOpAanbetaling.UpdateIsAllowed(BestelStatus.WordtIngepakt), "from wachtenOpAanbetaling to wordtIngepakt");
            Assert.IsFalse(BestelStatus.WachtenOpAanbetaling.UpdateIsAllowed(BestelStatus.Verzonden), "from wachtenOpAanbetaling to verzonden");
            Assert.IsFalse(BestelStatus.WachtenOpAanbetaling.UpdateIsAllowed(BestelStatus.Afgerond), "from wachtenOpAanbetaling to afgerond");

            Assert.IsFalse(BestelStatus.Afgerond.UpdateIsAllowed(BestelStatus.Geplaatst), "from afgerond to geplaatst");
            Assert.IsFalse(BestelStatus.Afgerond.UpdateIsAllowed(BestelStatus.Goedgekeurd), "from afgerond to goedgekeurd");
            Assert.IsFalse(BestelStatus.Afgerond.UpdateIsAllowed(BestelStatus.WordtIngepakt), "from afgerond to wordtIngepakt");
            Assert.IsFalse(BestelStatus.Afgerond.UpdateIsAllowed(BestelStatus.Verzonden), "from afgerond to verzonden");
            Assert.IsFalse(BestelStatus.Afgerond.UpdateIsAllowed(BestelStatus.Betaald), "from afgerond to betaald");
            Assert.IsFalse(BestelStatus.Afgerond.UpdateIsAllowed(BestelStatus.Afgekeurd), "from afgerond to afgekeurd");
            Assert.IsFalse(BestelStatus.Afgerond.UpdateIsAllowed(BestelStatus.WachtenOpAanbetaling), "from afgerond to wachtenOpAanbetaling");
            Assert.IsFalse(BestelStatus.Afgerond.UpdateIsAllowed(BestelStatus.Afgerond), "from afgerond to afgerond");

            Assert.IsFalse(BestelStatus.WordtIngepakt.UpdateIsAllowed(BestelStatus.Afgerond), "from wordtIngepakt to afgerond");
        }
    }
}