using System;
using Newtonsoft.Json;

namespace Trading.Foundation.Dtos
{
    public struct Side : IEquatable<Side>
    {
        public bool Equals(Side other)
        {
            return string.Compare(Value, other.Value, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public override bool Equals(object obj)
        {
            return obj is Side other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (Value != null ? Value.GetHashCode() : 0);
        }

        public static Side Buy = new Side("buy");
        public static Side Sell = new Side("sell");

        public string Value { get; set; }

        [JsonIgnore]
        public bool IsBuy => Equals(Buy);

        [JsonIgnore]
        public bool IsSell => Equals(Sell);

        private Side(string value)
        {
            Value = value;
        }

        public static Side Parse(string source)
        {
            var side = new Side(source);

            if (side.Equals(Buy))
            {
                return Buy;
            }

            if (side.Equals(Sell))
            {
                return Sell;
            }

            throw new ArgumentException($"Value is neither buy or sell", nameof(source));
        }

        public static implicit operator string(Side side)
        {
            return side.Value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
