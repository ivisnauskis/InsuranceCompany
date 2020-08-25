namespace InsuranceProvider
{
    public struct Risk
    {
        public Risk(string name, decimal yearlyPrice)
        {
            Name = name;
            YearlyPrice = yearlyPrice;
        }

        public string Name { get; set; }

        public decimal YearlyPrice { get; set; }
    }
}