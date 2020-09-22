using System;
using System.Collections.Generic;
using FluentAssertions;
using InsuranceProvider;
using InsuranceProvider.Interfaces;
using Xunit;

namespace InsuranceProviderTests
{
    public class PolicyTests
    {
        private readonly IPolicy _policy;

        public PolicyTests()
        {
            var from = new DateTime(2025, 1, 1, 12, 0, 0);
            var till = from.AddMonths(6);
            var risks = new List<RiskData>
            {
                new RiskData(new Risk("1", 5M), from, till, 15M),
                new RiskData(new Risk("2", 6M), from, till, 25M)
            };

            _policy = new Policy("obj", from, till, risks);
        }

        [Fact]
        public void NameOfInsuredObject()
        {
            _policy.NameOfInsuredObject.Should().Be("obj");
        }

        [Fact]
        public void ValidFrom()
        {
            _policy.ValidFrom.Should().Be(new DateTime(2025, 1, 1, 12, 0, 0));
        }

        [Fact]
        public void ValidTill()
        {
            _policy.ValidTill.Should().Be(new DateTime(2025, 7, 1, 12, 0, 0));
        }

        [Fact]
        public void GetPremium()
        {
            _policy.Premium.Should().Be(40);
        }

        [Fact]
        public void GetRisks()
        {
            _policy.InsuredRisks.Count.Should().Be(2);
        }
    }
}