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


        //TODO Throw Exception if selectedRisks is empty
        public IPolicy SellPolicy(string nameOfInsuredObject, DateTime validFrom, short validMonths,
            IList<Risk> selectedRisks)
        {
            if (IsExistingName(validFrom, validMonths, nameOfInsuredObject))
                throw new NameNotUniqueException($"Insured object '{nameOfInsuredObject}' is not unique.");

            if (selectedRisks == null || selectedRisks.Count == 0)
                throw new NoRisksSelectedException("Selected risks cannot be null or empty");

            if (validFrom < DateTime.Now)
                throw new TimeNotValidException("Policy starting time cannot be retroactive.");

            
            var policy = new Policy(nameOfInsuredObject, validFrom, validFrom.AddMonths(validMonths), 1M, selectedRisks);
            _policies.Add(policy);
            return policy;
        }

        public void AddRisk(string nameOfInsuredObject, Risk risk, DateTime validFrom)
        {
            if (validFrom < DateTime.Now)
                throw new TimeNotValidException("Risk starting time cannot be retroactive.");

            var policy = _policies.Find(p =>
                p.ValidFrom <= validFrom && validFrom >= p.ValidTill && p.NameOfInsuredObject == nameOfInsuredObject);

            policy.InsuredRisks.Add(risk);
        }

        public IPolicy GetPolicy(string nameOfInsuredObject, DateTime effectiveDate)
        {
            IPolicy policy = _policies.Find(p => p.NameOfInsuredObject == nameOfInsuredObject && p.ValidFrom == effectiveDate);

            if (policy == null) throw new PolicyNotFoundException("Policy not found.");

            return policy;
        }

        private bool IsExistingName(DateTime validFrom, short validMonths, string nameOfInsuredObject)
        {
            return _policies.Exists(p =>
                p.ValidFrom <= validFrom.AddMonths(validMonths) && validFrom <= p.ValidTill &&
                p.NameOfInsuredObject == nameOfInsuredObject);
        }
    }
}