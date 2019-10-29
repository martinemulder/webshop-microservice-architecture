using System;

namespace HalfWerk.DsBestelService.Helpers
{
    public static class PrijsCalculationHelper
    {
        public static decimal CalculatePrijsInclBtw(this decimal price, decimal multiplier = 1)
        {
            var result = price * multiplier;
            result *= Constants.BtwMultiplier;
            return result.RoundPrice();
        }

        public static decimal CalculatePrijsExclBtw(this decimal price, decimal multiplier = 1)
        {
            var result = price * multiplier;
            return result.RoundPrice();
        }

        public static decimal RoundPrice(this decimal price)
        {
            return decimal.Round(price, Constants.RoundToDecimals, MidpointRounding.ToEven);
        }
    }
}
