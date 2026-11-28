using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// A payment for a booking. It moves through the statuses pending, paid, failed and refunded.
    /// </summary>
    public class Payment
    {
        /// <summary>
        /// Gets the unique payment identifier.
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid();
        /// <summary>
        /// Gets the amount, in euros.
        /// </summary>
        public decimal Amount { get; }
        /// <summary>
        /// Gets the payment method.
        /// </summary>
        public PaymentMethod Method { get; }
        /// <summary>
        /// Gets the current status of the payment.
        /// </summary>
        public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
        /// <summary>
        /// Gets the date and time at which the payment was completed.
        /// </summary>
        public DateTime Date { get; private set; }

        /// <summary>
        /// Initializes a new pending payment.
        /// </summary>
        /// <param name="amount">The amount, in euros.</param>
        /// <param name="method">The payment method.</param>
        /// <exception cref="PaymentFailedException">The amount is zero or negative.</exception>
        public Payment(decimal amount, PaymentMethod method)
        {
            if (amount <= 0) throw new PaymentFailedException("Сумма платежа должна быть > 0.");
            Amount = amount; Method = method;
        }

        /// <summary>
        /// Validates the card and charges it. On success the status becomes paid; on any failure it becomes failed.
        /// </summary>
        /// <param name="card">The card to charge.</param>
        /// <exception cref="PaymentFailedException">The card number is invalid.</exception>
        /// <exception cref="ExpiredDocumentException">The card has expired.</exception>
        /// <exception cref="InsufficientFundsException">The card balance is too low.</exception>
        public void Process(PaymentCard card)
        {
            if (!card.ValidateNumber()) { Status = PaymentStatus.Failed; throw new PaymentFailedException("Неверный номер карты."); }
            if (card.IsExpired()) { Status = PaymentStatus.Failed; throw new ExpiredDocumentException("Срок действия карты истёк."); }
            try { card.Charge(Amount); }
            catch (InsufficientFundsException) { Status = PaymentStatus.Failed; throw; }
            Status = PaymentStatus.Paid; Date = DateTime.Now;
        }

        /// <summary>
        /// Determines whether the payment has been completed.
        /// </summary>
        /// <returns><c>true</c> if the status is <see cref="PaymentStatus.Paid"/>.</returns>
        public bool IsPaid() => Status == PaymentStatus.Paid;
        /// <summary>
        /// Sets the status to <see cref="PaymentStatus.Refunded"/>.
        /// </summary>
        public void MarkRefunded() => Status = PaymentStatus.Refunded;
    }
}
