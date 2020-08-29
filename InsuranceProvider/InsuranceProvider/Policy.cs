using System;
using System.Collections.Generic;
using System.Linq;
using InsuranceProvider.Interfaces;

namespace InsuranceProvider
{
    public class Policy : IPolicy
    {
        private readonly List<RiskData> _riskDatList;

        public Policy(string nameOfInsuredObject, DateTime validFrom, DateTime validTill, List<RiskData> insuredRisks)
        {
            NameOfInsuredObject = nameOfInsuredObject;
            ValidFrom = validFrom;
            ValidTill = validTill;
            _riskDatList = insuredRisks;
        }

        public string NameOfInsuredObject { get; }
        public DateTime ValidFrom { get; }
        public DateTime ValidTill { get; }
        public decimal Premium => _riskDatList.Sum(rd => rd.Price);
        public IList<Risk> InsuredRisks => _riskDatList.Select(rd => rd.Risk).ToList();
    }
}