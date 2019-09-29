using Newtonsoft.Json;
using System;

namespace Trading.Foundation.Dtos
{
    public class CurrencyPair : IEquatable<CurrencyPair>
    {
        private const string InvalidCurrencyPairFormatMessage = "Currency pair is not correctly formed: {0}. E.g. BTC-USD, ETH-GBP etc";

        public bool Equals(CurrencyPair other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Compare(Value, other.Value, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CurrencyPair) obj);
        }

        public override int GetHashCode()
        {
            return (Value != null ? Value.GetHashCode() : 0);
        }

        private readonly Lazy<string> lazyBase;
        private readonly Lazy<string> lazyQuote;

        public string Value { get; set; }

        [JsonIgnore]
        public string Base => lazyBase.Value;

        [JsonIgnore]
        public string Quote => lazyQuote.Value;

        private CurrencyPair(string pair) 
            : this()
        {
            Value = pair;
        }

        public CurrencyPair()
        {
            lazyBase = new Lazy<string>(CreateBase);
            lazyQuote = new Lazy<string>(CreateQuote);
        }

        private string CreateBase()
        {
            var components = Value.Split('-');
            return components[0];
        }

        private string CreateQuote()
        {
            var components = Value.Split('-');
            return components[1];
        }

        private CurrencyPair(string baseOfPair, string quoteOfPair)
            : this()
        {
            Value = $"{baseOfPair}-{quoteOfPair}";
        }

        public override string ToString()
        {
            return Value;
        }

        [JsonIgnore]
        public bool IsValid => Base.Length > 0 && Quote.Length > 0;

        public static CurrencyPair Parse(string source)
        {
            CurrencyPair pair;

            try
            {
                var replaced = source.Replace("/", "-");
                pair = new CurrencyPair(replaced);

                if (!pair.IsValid)
                {
                    throw new InvalidCurrencyPairException(string.Format(InvalidCurrencyPairFormatMessage, source));
                }
            }
            catch (Exception)
            {
                throw new InvalidCurrencyPairException(string.Format(InvalidCurrencyPairFormatMessage, source));
            }

            return pair;
        }

        public static implicit operator string(CurrencyPair pair)
        {
            return pair.ToString();
        }
    }
}
