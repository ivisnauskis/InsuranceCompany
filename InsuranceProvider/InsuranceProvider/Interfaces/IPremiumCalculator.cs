using System;

namespace InsuranceProvider.Interfaces
{
    public interface IPremiumCalculator
    {
        decimal Calculate(Risk risk, DateTime validFrom, DateTime validTill);
    }
}