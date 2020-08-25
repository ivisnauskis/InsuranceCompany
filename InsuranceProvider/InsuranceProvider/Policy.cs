using System;
using System.Collections.Generic;

namespace InsuranceProvider
{
    public class Policy : IPolicy
    {
        public Policy(string nameOfInsuredObject, DateTime validFrom, DateTime validTill, decimal premium, IList<Risk> insuredRisks)
        {
            NameOfInsuredObject = nameOfInsuredObject;
            ValidFrom = validFrom;
            ValidTill = validTill;
            Premium = premium;
            InsuredRisks = insuredRisks;
        }

        public string NameOfInsuredObject { get; }
        public DateTime ValidFrom { get; }
        public DateTime ValidTill { get; }
        public decimal Premium { get; }
        public IList<Risk> InsuredRisks { get; }
    }
}