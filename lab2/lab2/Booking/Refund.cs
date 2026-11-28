using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{

    /// <summary>
    /// A refund of a previously paid payment.
    /// </summary>
    public class Refund
    {
        /// <summary>
        /// Gets the payment that is refunded.
        /// </summary>
        public Payment Payment { get; }
        /// <summary>
        /// Gets the amount to be refunded, in euros.
        /// </summary>
        public decimal Amount { get; }
        /// <summary>
        /// Gets the reason for the refund.
        /// </summary>
        public string Reason { get; }
        /// <summary>
        /// Gets the date and time at which the refund was issued.
        /// </summary>
        public DateTime Date { get; private set; }

        /// <summary>
        /// Initializes a new refund.
        /// </summary>
        /// <param name="payment">The original payment.</param>
        /// <param name="amount">The amount, in euros.</param>
        /// <param name="reason">The reason given.</param>
        public Refund(Payment payment, decimal amount, string reason)
        {
            Payment = payment; Amount = amount; Reason = reason;
        }

        /// <summary>
        /// Calculates the refundable amount for a ticket: the full price more than 72 hours before departure, 70 % between 24 and 72 hours, and nothing within 24 hours.
        /// </summary>
        /// <param name="t">The ticket to refund.</param>
        /// <returns>The refundable amount in euros.</returns>
        public static decimal Calculate(Ticket t)
        {
            double hours = (t.Flight.DepartureTime - DateTime.Now).TotalHours;
            return hours > 72 ? t.Price : hours > 24 ? Math.Round(t.Price * 0.7m, 2) : 0m;
        }

        /// <summary>
        /// Credits the refund amount to the card and marks the payment as refunded.
        /// </summary>
        /// <param name="card">The card that receives the money.</param>
        /// <exception cref="PaymentFailedException">The original payment was not paid.</exception>
        public void Issue(PaymentCard card)
        {
            if (!Payment.IsPaid()) throw new PaymentFailedException("Платёж не был оплачен.");
            card.Deposit(Amount); Payment.MarkRefunded(); Date = DateTime.Now;
        }
    }
}
