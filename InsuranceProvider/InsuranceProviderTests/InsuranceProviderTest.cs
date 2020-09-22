using System;
using System.Collections.Generic;
using FluentAssertions;
using InsuranceProvider;
using InsuranceProvider.Interfaces;
using Xunit;

namespace InsuranceProviderTests
{
    public class InsuranceProviderTest
    {
        private readonly IInsuranceCompany _company;

        public InsuranceProviderTest()
        {
            var risks = new List<Risk>
            {
                new Risk("fire", 35),
                new Risk("theft", 15)
            };

            _company = new InsuranceCompany("name", new PremiumCalculator(), risks);
        }

        [Fact]
        public void RunCompany()
        {
            var from = new DateTime(2021, 01, 01, 12, 0, 0);
            var policy1 = _company.SellPolicy("car", from, 12, _company.AvailableRisks);
            var policy2 = _company.SellPolicy("car", from.AddMonths(13), 6, _company.AvailableRisks);

            policy1.Premium.Should().Be(50);
            policy2.InsuredRisks.Count.Should().Be(2);

            _company.GetPolicy("car", from).Premium.Should().Be(50);
            _company.GetPolicy("car", from.AddMonths(13)).Premium.Should().Be(25);

            _company.AddRisk("car", new Risk("flood", 10), from);
            _company.AddRisk("car", new Risk("flood", 10), new DateTime(2021, 7, 1, 12, 0 ,0));

            policy1.InsuredRisks.Count.Should().Be(4);

            policy1.Premium.Should().Be(65);
        }
    }
}