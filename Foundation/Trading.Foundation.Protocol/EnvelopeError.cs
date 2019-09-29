using System;

namespace Trading.Foundation.Protocol
{
    public class EnvelopeError : IEquatable<EnvelopeError>
    {
        public bool Equals(EnvelopeError other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Message == other.Message && ErrorType == other.ErrorType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EnvelopeError) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Message != null ? Message.GetHashCode() : 0) * 397) ^ (int) ErrorType;
            }
        }

        public string Message { get; set; }
        public ErrorType ErrorType { get; set; }

        public EnvelopeError(string message, ErrorType errorType = ErrorType.Payload)
        {
            Message = message;
            ErrorType = errorType;
        }

        public EnvelopeError()
        {
        }

        public override string ToString()
        {
            return $"{nameof(ErrorType)}:{ErrorType}, {nameof(Message)}:{Message}";
        }
    }
}
