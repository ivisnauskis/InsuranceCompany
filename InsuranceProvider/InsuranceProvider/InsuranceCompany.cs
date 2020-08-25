using System;
using System.Collections.Generic;
using InsuranceProvider.Exceptions;

namespace InsuranceProvider
{
    public class InsuranceCompany : IInsuranceCompany
    {
        private readonly List<IPolicy> _policies = new List<IPolicy>();

        public InsuranceCompany(string name, IList<Risk> availableRisks)
        {
            Name = name;
            AvailableRisks = availableRisks;
        }

        public InsuranceCompany(string name, IList<Risk> availableRisks, List<IPolicy> policies)
            : this(name, availableRisks)
        {
            _policies = policies;
        }

        public string Name { get; }

        public IList<Risk> AvailableRisks { get; set; }

        public IPolicy SellPolicy(string nameOfInsuredObject, DateTime validFrom, short validMonths,
            IList<Risk> selectedRisks)
        {
            if (IsExistingName(validFrom, validMonths, nameOfInsuredObject))
                throw new NameNotUniqueException($"Insured object '{nameOfInsuredObject}' is not unique.");

            if (validFrom < DateTime.Now)
                throw new TimeNotValidException("Policy starting time cannot be retroactive.");

            var policy = new Policy(nameOfInsuredObject, validFrom, validFrom.AddMonths(validMonths), 1M,
                selectedRisks);
            _policies.Add(policy);
            return policy;
        }

        public void AddRisk(string nameOfInsuredObject, Risk risk, DateTime validFrom)
        {
            throw new NotImplementedException();
        }

        public IPolicy GetPolicy(string nameOfInsuredObject, DateTime effectiveDate)
        {
            throw new NotImplementedException();
        }

        private bool IsExistingName(DateTime validFrom, short validMonths, string nameOfInsuredObject)
        {
            return _policies.Exists(p =>
                p.ValidFrom <= validFrom.AddMonths(validMonths) && validFrom <= p.ValidTill &&
                p.NameOfInsuredObject == nameOfInsuredObject);
        }
    }
}