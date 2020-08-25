using System;
using System.Collections.Generic;
using FluentAssertions;
using InsuranceProvider;
using InsuranceProvider.Exceptions;
using Xunit;

namespace InsuranceProviderTests
{
    public class InsuranceCompanyTests
    {
        private readonly IInsuranceCompany _company;

        public InsuranceCompanyTests()
        {
            _company = new InsuranceCompany("Insure", new List<Risk>(), GetPolicies());
        }

        [Fact]
        public void NameTest()
        {
            _company.Name.Should().Be("Insure");
        }

        [Fact]
        public void AvailableRisks()
        {
            _company.AvailableRisks.Should().BeEmpty();
        }

        [Fact]
        public void SellPolicy_shouldReturnPolicy()
        {
            var now = DateTime.Now;
            var date = new DateTime(now.Year + 1, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            var policy = _company.SellPolicy("car", date, 3, new List<Risk>());

            policy.Should().NotBe(null);
            policy.NameOfInsuredObject.Should().Be("car");

            policy.ValidTill.Should().Be(date.AddMonths(3));
        }

        [Fact]
        public void SellPolicy_SameNameDifferentDate_ShouldReturnPolicy()
        {
            var from1 = new DateTime(2025, 3, 1, 12, 0, 0);
            var from2 = new DateTime(2025, 5, 2, 12, 0, 0);

            _company.SellPolicy("1", from1, 2, new List<Risk>());
            _company.SellPolicy("1", from2, 2, new List<Risk>());

            IPolicy policy2 = _company.GetPolicy("1", from1);
            IPolicy policy1 = _company.GetPolicy("1", from2);

            policy1.NameOfInsuredObject.Should().Be(policy2.NameOfInsuredObject);
        }

        [Theory]
        [InlineData("2020-01-01 12:00:00", 1)]
        [InlineData("2020-03-01 12:00:00", 1)]
        [InlineData("2020-05-01 12:00:00", 1)]
        public void SellPolicy_NameNotUnique_ShouldThrowNameNotUniqueException1337(string dateFrom, short validMonths)
        {
            var from = DateTime.Parse(dateFrom);
            _company.Invoking(ic => ic.SellPolicy("obj1", from,
                    validMonths, new List<Risk>()))
                .ShouldThrow<NameNotUniqueException>();
        }

        [Fact]
        public void SellPolicy_Retroactive_ShouldThrowTimeNotValidException()
        {
            _company.Invoking(ic => ic.SellPolicy("car", DateTime.Now.Subtract(TimeSpan.FromDays(1)),
                    3, new List<Risk>()))
                .ShouldThrow<TimeNotValidException>();
        }

        [Fact]
        public void AddRisk_ShouldAddRiskToPolicy()
        {
            var date = new DateTime(2025, 2, 1, 12, 0, 0);
            var policy = _company.GetPolicy("obj2", date);

            policy.InsuredRisks.Count.Should().Be(0);

            _company.AddRisk("obj2", new Risk("Theft", 10M), date.AddMonths(5));

            policy.InsuredRisks.Count.Should().Be(1);
        }

        [Fact]
        public void AddRisk_Retroactive_ShouldThrowTimeNotValidException()
        {
            _company.Invoking(c => c.AddRisk("obj2", new Risk("Theft", 10M), DateTime.Now))
                .ShouldThrow<TimeNotValidException>();
        }

        [Fact]
        public void GetPolicy_ShouldReturnPolicy()
        {
            IPolicy policy = _company.GetPolicy("obj1", new DateTime(2020, 2, 1, 12, 0, 0));

            policy.Should().NotBeNull();
            policy.NameOfInsuredObject.Should().Be("obj1");
        }

        [Fact]
        public void GetPolicy_NotFound_ShouldThrowException()
        {
            _company.Invoking(c => c.GetPolicy("policy", DateTime.Now))
                .ShouldThrow<PolicyNotFoundException>();
        }



        private List<IPolicy> GetPolicies()
        {
            var from = new DateTime(2020, 2, 1, 12, 0, 0);
            var till = new DateTime(2020, 5, 1, 12, 0, 0);

            return new List<IPolicy>()
            {
                new Policy("obj1", from,
                    till, 1M, new List<Risk>()),
                new Policy("obj2", from.AddYears(5),
                    till, 1M, new List<Risk>())
            };
        }
    }
}