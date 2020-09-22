using System;
using FluentAssertions;
using InsuranceProvider;
using InsuranceProvider.Interfaces;
using Xunit;

namespace InsuranceProviderTests
{
    public class PremiumCalculatorTests
    {
        private readonly IPremiumCalculator _calculator;

        public PremiumCalculatorTests()
        {
            _calculator = new PremiumCalculator();
        }

        [Fact]
        public void Calculate_RiskValid6Months()
        {
            var from = new DateTime(2020, 1, 1, 12, 0, 0);
            var to = from.AddMonths(6);
            _calculator.Calculate(new Risk("risk", 365), from, to).Should().Be(182.5M);
        }

        [Fact]
        public void Calculate_RiskValid1Month()
        {
            var from = new DateTime(2020, 1, 1, 12, 0, 0);
            var to = from.AddMonths(1);
            _calculator.Calculate(new Risk("risk", 40), from, to).Should().Be(3.33M);
        }
    }
}