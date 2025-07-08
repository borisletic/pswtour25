using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourApp.Domain.ValueObjects
{
    public class Money : IEquatable<Money>
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency = "EUR")
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative");

            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency cannot be empty");

            Amount = amount;
            Currency = currency.ToUpper();
        }

        public Money Add(Money other)
        {
            if (other.Currency != Currency)
                throw new InvalidOperationException("Cannot add money with different currencies");

            return new Money(Amount + other.Amount, Currency);
        }

        public Money Subtract(Money other)
        {
            if (other.Currency != Currency)
                throw new InvalidOperationException("Cannot subtract money with different currencies");

            if (Amount < other.Amount)
                throw new InvalidOperationException("Cannot subtract more than available amount");

            return new Money(Amount - other.Amount, Currency);
        }

        public bool Equals(Money other)
        {
            if (other is null) return false;
            return Amount == other.Amount && Currency == other.Currency;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Money);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Amount, Currency);
        }

        public override string ToString()
        {
            return $"{Amount:F2} {Currency}";
        }
    }
}
