using System;

namespace Trading.Foundation.Dtos
{
    public class InvalidCurrencyPairException : Exception
    {
        public InvalidCurrencyPairException(string message) : base(message)
        {
        }
    }
}
