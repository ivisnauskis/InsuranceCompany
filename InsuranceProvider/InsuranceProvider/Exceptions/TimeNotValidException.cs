using System;

namespace InsuranceProvider.Exceptions
{
    public class TimeNotValidException : Exception
    {
        public TimeNotValidException(string message) : base(message)
        {
        }
    }
}