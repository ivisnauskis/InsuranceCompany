using System;
using InsuranceProvider.Interfaces;

namespace InsuranceProvider
{
    public class PremiumCalculator : IPremiumCalculator
    {
        public decimal Calculate(Risk risk, DateTime validFrom, DateTime validTo)
        {
            var riskMonthlyPrice = risk.YearlyPrice / 12;
            var monthsValid = (validTo.Year - validFrom.Year) * 12 + validTo.Month - validFrom.Month;
            var riskPriceForPeriod = riskMonthlyPrice * monthsValid;
            return Math.Round(riskPriceForPeriod, 2);
        }
    }
}