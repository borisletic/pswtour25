using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Domain.ValueObjects;

namespace TourApp.Tests.Entities
{
    public class MoneyTests
    {
        [Fact]
        public void Constructor_WithNegativeAmount_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Money(-10, "EUR"));
        }

        [Fact]
        public void Constructor_WithValidAmount_CreatesMoney()
        {
            // Arrange & Act
            var money = new Money(100.50m, "EUR");

            // Assert
            money.Amount.Should().Be(100.50m);
            money.Currency.Should().Be("EUR");
        }

        [Fact]
        public void Add_SameCurrency_ReturnsSum()
        {
            // Arrange
            var money1 = new Money(100, "EUR");
            var money2 = new Money(50, "EUR");

            // Act
            var result = money1.Add(money2);

            // Assert
            result.Amount.Should().Be(150);
            result.Currency.Should().Be("EUR");
        }

        [Fact]
        public void Add_DifferentCurrency_ThrowsException()
        {
            // Arrange
            var money1 = new Money(100, "EUR");
            var money2 = new Money(50, "USD");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => money1.Add(money2));
        }

        [Fact]
        public void Subtract_WithSufficientAmount_ReturnsRemainder()
        {
            // Arrange
            var money1 = new Money(100, "EUR");
            var money2 = new Money(30, "EUR");

            // Act
            var result = money1.Subtract(money2);

            // Assert
            result.Amount.Should().Be(70);
        }

        [Fact]
        public void Subtract_WithInsufficientAmount_ThrowsException()
        {
            // Arrange
            var money1 = new Money(50, "EUR");
            var money2 = new Money(100, "EUR");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => money1.Subtract(money2));
        }

        [Fact]
        public void Equals_WithSameAmountAndCurrency_ReturnsTrue()
        {
            // Arrange
            var money1 = new Money(100, "EUR");
            var money2 = new Money(100, "EUR");

            // Act & Assert
            money1.Equals(money2).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithDifferentAmount_ReturnsFalse()
        {
            // Arrange
            var money1 = new Money(100, "EUR");
            var money2 = new Money(200, "EUR");

            // Act & Assert
            money1.Equals(money2).Should().BeFalse();
        }
    }
}
