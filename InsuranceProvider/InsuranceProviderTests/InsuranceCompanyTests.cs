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

        private List<IPolicy> GetPolicies()
        {
            var date = new DateTime(2020, 5, 1, 12, 0, 0);

            return new List<IPolicy>()
            {
                new Policy("obj1", date.AddMonths(-3),
                    date, 1M, new List<Risk>())
            };
        }
    }
}