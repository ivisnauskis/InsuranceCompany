using System;

namespace InsuranceProvider
{
    public class RiskData
    {
        public RiskData(Risk risk, DateTime validFrom, DateTime validTill, decimal price)
        {
            Risk = risk;
            ValidFrom = validFrom;
            ValidTill = validTill;
            Price = price;
        }

        public Risk Risk { get; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTill { get; set; }
        public decimal Price { get; set; }
    }
}