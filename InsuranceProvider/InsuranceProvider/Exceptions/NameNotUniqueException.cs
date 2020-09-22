using System;

namespace InsuranceProvider.Exceptions
{
    public class NameNotUniqueException : Exception
    {
        public NameNotUniqueException(string message) : base(message)
        {
        }
    }
}