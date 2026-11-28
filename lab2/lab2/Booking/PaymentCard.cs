using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// A bank card with a balance. The card number is validated with the Luhn algorithm.
    /// </summary>
    public class PaymentCard
    {
        /// <summary>
        /// Gets the 16-digit card number.
        /// </summary>
        public string Number { get; }
        /// <summary>
        /// Gets the cardholder name.
        /// </summary>
        public string Holder { get; }
        /// <summary>
        /// Gets the expiry date.
        /// </summary>
        public DateTime ExpiryDate { get; }
        /// <summary>
        /// Gets the current balance, in euros.
        /// </summary>
        public decimal Balance { get; private set; }

        /// <summary>
        /// Initializes a new payment card.
        /// </summary>
        /// <param name="number">The 16-digit card number.</param>
        /// <param name="holder">The cardholder name as printed on the card.</param>
        /// <param name="expiry">The expiry date.</param>
        /// <param name="balance">The initial balance on the card, in euros.</param>
        public PaymentCard(string number, string holder, DateTime expiry, decimal balance)
        {
            Number = number; Holder = holder; ExpiryDate = expiry; Balance = balance;
        }

        /// <summary>
        /// Determines whether the card has expired.
        /// </summary>
        /// <returns><c>true</c> if the expiry date is before today.</returns>
        public bool IsExpired() => ExpiryDate.Date < DateTime.Today;

        /// <summary>
        /// Validates the card number: it must have 16 digits and satisfy the Luhn checksum.
        /// </summary>
        /// <returns><c>true</c> if the number is valid.</returns>
        public bool ValidateNumber()
        {
            if (Number.Length != 16 || !Number.All(char.IsDigit)) return false;
            int sum = 0; bool alt = false;
            for (int i = Number.Length - 1; i >= 0; i--)
            {
                int d = Number[i] - '0';
                if (alt) { d *= 2; if (d > 9) d -= 9; }
                sum += d; alt = !alt;
            }
            return sum % 10 == 0;
        }

        /// <summary>
        /// Withdraws money from the card.
        /// </summary>
        /// <param name="amount">The amount to withdraw, in euros.</param>
        /// <exception cref="InsufficientFundsException">The amount is greater than the balance.</exception>
        public void Charge(decimal amount)
        {
            if (amount > Balance) throw new InsufficientFundsException($"Недостаточно средств: нужно {amount:N2} €, на карте {Balance:N2} €.");
            Balance -= amount;
        }

        /// <summary>
        /// Adds money to the card. Non-positive amounts are ignored.
        /// </summary>
        /// <param name="amount">The amount to add, in euros.</param>
        public void Deposit(decimal amount) { if (amount > 0) Balance += amount; }
    }
}
