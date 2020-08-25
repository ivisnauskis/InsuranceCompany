using System;
using System.Collections.Generic;

namespace InsuranceProvider
{
    public interface IPolicy
    {
        string NameOfInsuredObject { get; }

        DateTime ValidFrom { get; }

        DateTime ValidTill { get; }

        decimal Premium { get; }

        IList<Risk> InsuredRisks { get; }
    }
}