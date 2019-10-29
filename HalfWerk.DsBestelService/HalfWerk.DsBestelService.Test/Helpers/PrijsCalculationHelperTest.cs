using HalfWerk.DsBestelService.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HalfWerk.DsBestelService.Test.Helpers
{
    [TestClass]
    public class PrijsCalculationHelperTest
    {
        [TestMethod]
        public void CalculatePrijsInclBtw_ShouldCalculatePrijsIncludingBtw()
        {
            var input = 10m;
            var expected = decimal.Round(10 * Constants.BtwMultiplier, Constants.RoundToDecimals, MidpointRounding.ToEven);
            Assert.AreEqual(expected, input.CalculatePrijsInclBtw());
        }

        [TestMethod]
        public void CalculatePrijsInclBtw_ShouldCalculatePrijsIncludingBtwWithMultiplier()
        {
            var input = 10m;

            var multiplier = 5;
            var expected = decimal.Round((10 * multiplier) * Constants.BtwMultiplier, Constants.RoundToDecimals, MidpointRounding.ToEven);

            Assert.AreEqual(expected, input.CalculatePrijsInclBtw(multiplier));
        }

        [TestMethod]
        public void CalculatePrijsExclBtw_ShouldCalculatePrijsExcludingBtw()
        {
            var input = 10m;
            var expected = decimal.Round(10, Constants.RoundToDecimals, MidpointRounding.ToEven);
            Assert.AreEqual(expected, input.CalculatePrijsExclBtw());
        }

        [TestMethod]
        public void CalculatePrijsExclBtw_ShouldCalculatePrijsExcludingBtwWithMultiplier()
        {
            var input = 10m;

            var multiplier = 5;
            var expected = decimal.Round(10 * multiplier, Constants.RoundToDecimals, MidpointRounding.ToEven);

            Assert.AreEqual(expected, input.CalculatePrijsExclBtw(multiplier));
        }

        [TestMethod]
        public void RoundPrice_ShouldRoundPriceOnTwoDecimals()
        {
            var input = 10.9984m;
            var expected = decimal.Round(input, Constants.RoundToDecimals, MidpointRounding.ToEven);
            Assert.AreEqual(expected, input.RoundPrice());
        }
    }
}