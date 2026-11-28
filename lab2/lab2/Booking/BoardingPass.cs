
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// A boarding pass issued at check-in.
    /// </summary>
    public class BoardingPass
    {
        /// <summary>
        /// Gets the ticket for which the pass was issued.
        /// </summary>
        public Ticket Ticket { get; }
        /// <summary>
        /// Gets the boarding gate.
        /// </summary>
        public Gate Gate { get; }
        /// <summary>
        /// Gets the time at which boarding opens.
        /// </summary>
        public DateTime BoardingTime { get; }
        /// <summary>
        /// Gets the boarding group number.
        /// </summary>
        public int Group { get; }

        /// <summary>
        /// Initializes a new boarding pass.
        /// </summary>
        /// <param name="ticket">The ticket concerned.</param>
        /// <param name="gate">The boarding gate.</param>
        /// <param name="boardingTime">The time at which boarding opens.</param>
        /// <param name="group">The boarding group number.</param>
        public BoardingPass(Ticket ticket, Gate gate, DateTime boardingTime, int group)
        {
            Ticket = ticket; Gate = gate; BoardingTime = boardingTime; Group = group;
        }

        /// <summary>
        /// Determines whether boarding is currently open for this pass.
        /// </summary>
        /// <param name="now">The reference date and time.</param>
        /// <returns><c>true</c> if the reference time is between the boarding time and the departure time.</returns>
        public bool IsBoardingOpen(DateTime now) => now >= BoardingTime && now < Ticket.Flight.DepartureTime;
        /// <summary>
        /// Gets the seat label printed on the pass.
        /// </summary>
        /// <returns>The label of the ticket's seat.</returns>
        public string GetSeatLabel() => Ticket.Seat.Label();
        /// <summary>
        /// Gets the passenger name printed on the pass.
        /// </summary>
        /// <returns>The full name of the passenger.</returns>
        public string GetPassengerName() => Ticket.Passenger.GetFullName();
        /// <summary>
        /// Builds a one-line, human-readable description of the pass.
        /// </summary>
        /// <returns>A string with the flight number, passenger, seat, gate, boarding time and group.</returns>
        public string Describe() =>
            $"[{Ticket.Flight.Number}] {GetPassengerName()}, место {GetSeatLabel()}, гейт {Gate?.Number ?? "—"}, посадка {BoardingTime:t}, группа {Group}";
    }
}
