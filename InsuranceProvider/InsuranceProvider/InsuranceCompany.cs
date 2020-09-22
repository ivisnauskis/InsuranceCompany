using System;
using System.Collections.Generic;
using System.Linq;
using InsuranceProvider.Exceptions;
using InsuranceProvider.Interfaces;

namespace InsuranceProvider
{
    public class InsuranceCompany : IInsuranceCompany
    {
        private readonly Dictionary<IPolicy, List<RiskData>> _policies = new Dictionary<IPolicy, List<RiskData>>();
        private readonly IPremiumCalculator _premiumCalculator;

        public InsuranceCompany(string name, IPremiumCalculator premiumCalculator, IList<Risk> availableRisks)
        {
            Name = name;
            _premiumCalculator = premiumCalculator;
            AvailableRisks = availableRisks;
        }

        public InsuranceCompany(string name, IPremiumCalculator premiumCalculator, IList<Risk> availableRisks,
            Dictionary<IPolicy, List<RiskData>> policies)
            : this(name, premiumCalculator, availableRisks)
        {
            _policies = policies;
        }

        public string Name { get; }

        public IList<Risk> AvailableRisks { get; set; }

        public IPolicy SellPolicy(string nameOfInsuredObject, DateTime validFrom, short validMonths,
            IList<Risk> selectedRisks)
        {
            var validTill = validFrom.AddMonths(validMonths);
            ValidatePolicy(nameOfInsuredObject, validFrom, validTill, selectedRisks);
            var riskDataList = selectedRisks.Select(risk => CreateRiskData(risk, validFrom, validTill)).ToList();
            var policy = new Policy(nameOfInsuredObject, validFrom, validTill, riskDataList);
            _policies.Add(policy, riskDataList);
            return policy;
        }

        public void AddRisk(string nameOfInsuredObject, Risk risk, DateTime validFrom)
        {
            if (validFrom < DateTime.Now)
                throw new TimeNotValidException("Risk starting time cannot be retroactive.");

            var policy = GetPolicy(nameOfInsuredObject, validFrom);
            _policies[policy].Add(CreateRiskData(risk, validFrom, policy.ValidTill));
        }

        public IPolicy GetPolicy(string nameOfInsuredObject, DateTime effectiveDate)
        {
            var policy = _policies.Keys.FirstOrDefault(p =>
                p.ValidFrom <= effectiveDate && effectiveDate < p.ValidTill &&
                p.NameOfInsuredObject == nameOfInsuredObject);

            if (policy == null) throw new PolicyNotFoundException("Policy not found.");

            return policy;
        }

        private void ValidatePolicy(string nameOfInsuredObject, DateTime validFrom, DateTime validTill,
            IList<Risk> selectedRisks)
        {
            if (IsExistingName(validFrom, validTill, nameOfInsuredObject))
                throw new NameNotUniqueException(
                    $"Insured object '{nameOfInsuredObject}' is not unique in this period.");

            if (selectedRisks == null || selectedRisks.Count == 0)
                throw new NoRisksSelectedException("Selected risks cannot be null or empty");

            if (validFrom < DateTime.Now)
                throw new TimeNotValidException("Policy starting date/time cannot be retroactive.");
        }

        private bool IsExistingName(DateTime validFrom, DateTime validTill, string nameOfInsuredObject)
        {
            return _policies.Any(kvp =>
                kvp.Key.ValidFrom <= validTill && validFrom <= kvp.Key.ValidTill &&
                kvp.Key.NameOfInsuredObject == nameOfInsuredObject);
        }

        private RiskData CreateRiskData(Risk risk, DateTime from, DateTime till)
        {
            return new RiskData(risk, from, till, _premiumCalculator.Calculate(risk, from, till));
        }
    }
}