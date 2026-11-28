using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// An air ticket for one passenger and one seat on one flight.
    /// </summary>
    public class Ticket
    {
        /// <summary>
        /// Gets the ticket number.
        /// </summary>
        public string Number { get; }
        /// <summary>
        /// Gets the passenger.
        /// </summary>
        public Passenger Passenger { get; }
        /// <summary>
        /// Gets the flight.
        /// </summary>
        public Flight Flight { get; }
        /// <summary>
        /// Gets the assigned seat.
        /// </summary>
        public Seat Seat { get; }
        /// <summary>
        /// Gets the price, in euros.
        /// </summary>
        public decimal Price { get; }
        /// <summary>
        /// Gets the current status of the ticket.
        /// </summary>
        public TicketStatus Status { get; private set; } = TicketStatus.Booked;

        /// <summary>
        /// Initializes a new ticket in the booked status.
        /// </summary>
        /// <param name="number">The ticket number.</param>
        /// <param name="p">The passenger.</param>
        /// <param name="f">The flight.</param>
        /// <param name="seat">The assigned seat.</param>
        /// <param name="price">The price, in euros.</param>
        public Ticket(string number, Passenger p, Flight f, Seat seat, decimal price)
        {
            Number = number; Passenger = p; Flight = f; Seat = seat; Price = price;
        }

        /// <summary>
        /// Determines whether the ticket can be refunded.
        /// </summary>
        /// <returns><c>true</c> if the ticket is booked and the flight departs in more than 24 hours.</returns>
        public bool IsRefundable() => Status == TicketStatus.Booked && (Flight.DepartureTime - DateTime.Now).TotalHours > 24;

        /// <summary>
        /// Checks the ticket in.
        /// </summary>
        /// <exception cref="InvalidFlightDataException">The ticket is not in the booked status.</exception>
        public void CheckIn()
        {
            if (Status != TicketStatus.Booked) throw new InvalidFlightDataException($"Билет {Number}: регистрация невозможна ({Status}).");
            Status = TicketStatus.CheckedIn;
        }

        /// <summary>
        /// Marks the ticket as used after the flight has been completed.
        /// </summary>
        public void MarkUsed() => Status = TicketStatus.Used;

        /// <summary>
        /// Cancels the ticket and releases its seat. Cancelling an already cancelled ticket has no effect.
        /// </summary>
        public void Cancel()
        {
            if (Status == TicketStatus.Cancelled) return;
            Status = TicketStatus.Cancelled;
            Seat.Release();
        }
    }
}
