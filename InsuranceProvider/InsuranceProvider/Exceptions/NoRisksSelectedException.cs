using System;

namespace InsuranceProvider.Exceptions
{
    public class NoRisksSelectedException : Exception
    {
        public NoRisksSelectedException(string message) : base(message)
        {
        }
    }
}