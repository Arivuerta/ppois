using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// A reservation that groups one or more tickets of a passenger. It supports discounts and payment by card.
    /// </summary>
    public class Booking
    {
        /// <summary>
        /// Gets the booking code.
        /// </summary>
        public string Code { get; }
        /// <summary>
        /// Gets the passenger who made the booking.
        /// </summary>
        public Passenger Passenger { get; }
        /// <summary>
        /// Gets the tickets that belong to the booking.
        /// </summary>
        public List<Ticket> Tickets { get; } = new List<Ticket>();
        /// <summary>
        /// Gets the date and time at which the booking was created.
        /// </summary>
        public DateTime CreatedAt { get; }
        /// <summary>
        /// Gets the payment of the booking, or <c>null</c> if it has not been paid yet.
        /// </summary>
        public Payment Payment { get; private set; }
        /// <summary>
        /// Gets the discount that was applied, or <c>null</c>.
        /// </summary>
        public Discount AppliedDiscount { get; private set; }

        /// <summary>
        /// Initializes a new booking without tickets.
        /// </summary>
        /// <param name="code">The booking code.</param>
        /// <param name="passenger">The passenger who makes the booking.</param>
        public Booking(string code, Passenger passenger)
        {
            Code = code; Passenger = passenger; CreatedAt = DateTime.Now;
        }

        /// <summary>
        /// Adds a ticket to the booking.
        /// </summary>
        /// <param name="t">The ticket to add.</param>
        /// <exception cref="PaymentFailedException">The booking has already been paid.</exception>
        public void AddTicket(Ticket t)
        {
            if (Payment != null) throw new PaymentFailedException("Бронь уже оплачена.");
            Tickets.Add(t);
        }

        /// <summary>
        /// Applies a discount to the booking.
        /// </summary>
        /// <param name="d">The discount to apply.</param>
        public void ApplyDiscount(Discount d) => AppliedDiscount = d;

        /// <summary>
        /// Calculates the total price of all non-cancelled tickets, reduced by the applied discount if there is one.
        /// </summary>
        /// <returns>The total price in euros.</returns>
        /// <exception cref="InvalidPromoCodeException">The applied discount is no longer valid.</exception>
        public decimal TotalPrice()
        {
            decimal sum = Tickets.Where(t => t.Status != TicketStatus.Cancelled).Sum(t => t.Price);
            return AppliedDiscount == null ? sum : AppliedDiscount.Apply(sum);
        }

        /// <summary>
        /// Creates a payment for the total price and charges the card.
        /// </summary>
        /// <param name="card">The card to charge.</param>
        /// <exception cref="PaymentFailedException">The card number is invalid or the amount is not positive.</exception>
        /// <exception cref="ExpiredDocumentException">The card has expired.</exception>
        /// <exception cref="InsufficientFundsException">The card balance is too low.</exception>
        public void Pay(PaymentCard card)
        {
            var p = new Payment(TotalPrice(), PaymentMethod.Card);
            Payment = p;
            p.Process(card);
        }

        /// <summary>
        /// Cancels every ticket of the booking and releases the seats.
        /// </summary>
        public void Cancel()
        {
            foreach (var t in Tickets) t.Cancel();
        }
    }
}
