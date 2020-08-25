using System;
using System.Collections.Generic;
using FluentAssertions;
using InsuranceProvider;
using Xunit;

namespace InsuranceProviderTests
{
    public class PolicyTests
    {
        private readonly IPolicy _policy;

        public PolicyTests()
        {
            var from = new DateTime(2025, 1, 1, 12, 0, 0);
            var till = new DateTime(2026, 1, 1, 12, 0, 0);
            var risks = new List<Risk> { new Risk("1", 5M), new Risk("2", 6M) };

            _policy = new Policy("obj", from, till, 10M, risks);
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
            _policy.ValidTill.Should().Be(new DateTime(2026, 1, 1, 12, 0, 0));
        }

        [Fact]
        public void GetPremium()
        {
            _policy.Premium.Should().Be(10M);
        }

        [Fact]
        public void GetRisks()
        {
            _policy.InsuredRisks.Count.Should().Be(2);
        }
    }
}