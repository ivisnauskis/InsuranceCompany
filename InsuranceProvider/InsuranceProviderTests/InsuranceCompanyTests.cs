using System;
using System.Collections.Generic;
using FluentAssertions;
using InsuranceProvider;
using InsuranceProvider.Exceptions;
using InsuranceProvider.Interfaces;
using Moq;
using Xunit;

namespace InsuranceProviderTests
{
    public class InsuranceCompanyTests
    {
        private readonly Mock<IPremiumCalculator> _calculator;
        private readonly IInsuranceCompany _company;

        public InsuranceCompanyTests()
        {
            _calculator = new Mock<IPremiumCalculator>();
            _company = new InsuranceCompany("Insure", _calculator.Object, new List<Risk>(), GetPolicies());
        }

        [Fact]
        public void NameTest()
        {
            _company.Name.Should().Be("Insure");
        }

        [Fact]
        public void AvailableRisks_Get()
        {
            _company.AvailableRisks.Should().BeEmpty();
        }

        [Fact]
        public void AvailableRisks_Set()
        {
            _company.AvailableRisks = new List<Risk> {new Risk("risk", 1M)};
            _company.AvailableRisks.Should().NotBeEmpty();
        }

        [Fact]
        public void SellPolicy_shouldReturnPolicy()
        {
            var now = DateTime.Now;
            var date = new DateTime(now.Year + 1, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            var policy = _company.SellPolicy("car", date, 3, GetRisks());

            policy.Should().NotBe(null);
            policy.NameOfInsuredObject.Should().Be("car");
            policy.ValidTill.Should().Be(date.AddMonths(3));
        }

        [Fact]
        public void SellPolicy_SameNameDifferentDate_ShouldReturnPolicy()
        {
            var from1 = new DateTime(2025, 3, 1, 12, 0, 0);
            var from2 = new DateTime(2025, 5, 2, 12, 0, 0);

            var policy2 = _company.SellPolicy("1", from1, 2, GetRisks());
            var policy1 = _company.SellPolicy("1", from2, 2, GetRisks());

            policy1.NameOfInsuredObject.Should().Be(policy2.NameOfInsuredObject);
        }

        [Theory]
        [InlineData("2025-01-01 00:00:00", 1)]
        [InlineData("2025-03-01 00:00:00", 1)]
        [InlineData("2025-05-01 00:00:00", 1)]
        public void SellPolicy_NameNotUnique_ShouldThrowNameNotUniqueException(string dateFrom, short validMonths)
        {
            var from = DateTime.Parse(dateFrom);
            _company.Invoking(ic => ic.SellPolicy("obj1", from,
                    validMonths, GetRisks()))
                .ShouldThrow<NameNotUniqueException>();
        }

        [Fact]
        public void SellPolicy_Retroactive_ShouldThrowTimeNotValidException()
        {
            _company.Invoking(ic => ic.SellPolicy("car", DateTime.Now.Subtract(TimeSpan.FromDays(1)),
                    3, GetRisks()))
                .ShouldThrow<TimeNotValidException>();
        }

        [Fact]
        public void SellPolicy_NoRisksSelected_ShouldThrowNoRisksSelectedException()
        {
            var from = new DateTime(2025, 3, 1, 12, 0, 0);

            _company.Invoking(ic => ic.SellPolicy("obj5", from, 2, new List<Risk>()))
                .ShouldThrow<NoRisksSelectedException>();
        }

        [Fact]
        public void SellPolicy_CalculatePremium()
        {
            var risks = GetRisks();
            _company.SellPolicy("obj", DateTime.MaxValue.AddYears(-5), 1, risks);
            _calculator.Verify(c =>
                c.Calculate(It.IsAny<Risk>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Exactly(risks.Count));
        }

        [Fact]
        public void AddRisk_ShouldAddRiskToPolicy()
        {
            var from = new DateTime(2025, 1, 1);
            var policy = _company.GetPolicy("obj1", from);

            policy.InsuredRisks.Count.Should().Be(0);

            _company.AddRisk("obj1", new Risk("Theft", 10M), from);

            policy.InsuredRisks.Count.Should().Be(1);
        }

        [Fact]
        public void AddRisk_Retroactive_ShouldThrowTimeNotValidException()
        {
            _company.Invoking(c => c.AddRisk("obj2", new Risk("Theft", 10M), DateTime.Now))
                .ShouldThrow<TimeNotValidException>();
        }

        [Fact]
        public void AddRisk_CalculatePremium()
        {
            var from = new DateTime(2025, 1, 1);
            _company.AddRisk("obj1", new Risk("Theft", 10M), from);

            _calculator.Verify(c =>
                c.Calculate(It.IsAny<Risk>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public void GetPolicy_ShouldReturnPolicy()
        {
            var policy = _company.GetPolicy("obj1", new DateTime(2025, 1, 1));

            policy.Should().NotBeNull();
            policy.NameOfInsuredObject.Should().Be("obj1");
        }

        [Fact]
        public void GetPolicy_NotFound_ShouldThrowException()
        {
            _company.Invoking(c => c.GetPolicy("policy", DateTime.Now))
                .ShouldThrow<PolicyNotFoundException>();
        }

        private Dictionary<IPolicy, List<RiskData>> GetPolicies()
        {
            var policy1RiskInfo = new List<RiskData>();
            var policy1 = CreatePolicy("obj1",
                new DateTime(2025, 1, 1),
                new DateTime(2025, 7, 1),
                policy1RiskInfo);

            var policy2RiskInfo = new List<RiskData>();
            var policy2 = CreatePolicy("obj2",
                new DateTime(2026, 1, 1),
                new DateTime(2027, 1, 1),
                policy1RiskInfo);

            return new Dictionary<IPolicy, List<RiskData>>
            {
                {policy1, policy1RiskInfo},
                {policy2, policy2RiskInfo}
            };
        }

        private List<Risk> GetRisks()
        {
            return new List<Risk>
            {
                new Risk("risk1", 15M),
                new Risk("risk2", 10M),
                new Risk("risk3", 5M)
            };
        }

        private IPolicy CreatePolicy(string objectName, DateTime from, DateTime till, List<RiskData> list)
        {
            return new Policy(objectName, from, till, list);
        }
    }
}