using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// A check-in desk that issues boarding passes.
    /// </summary>
    public class CheckInDesk
    {
        /// <summary>
        /// Gets the desk number.
        /// </summary>
        public int Number { get; }
        /// <summary>
        /// Gets a value that indicates whether the desk is open.
        /// </summary>
        public bool IsOpen { get; private set; }
        /// <summary>
        /// Gets the airport where the desk is located.
        /// </summary>
        public Airport Airport { get; }

        /// <summary>
        /// Initializes a new, closed desk.
        /// </summary>
        /// <param name="number">The desk number.</param>
        /// <param name="airport">The airport concerned.</param>
        public CheckInDesk(int number, Airport airport) { Number = number; Airport = airport; }

        /// <summary>
        /// Opens the desk.
        /// </summary>
        public void Open() => IsOpen = true;
        /// <summary>
        /// Closes the desk.
        /// </summary>
        public void Close() => IsOpen = false;

        /// <summary>
        /// Checks the ticket in and issues a boarding pass. Boarding opens 40 minutes before departure.
        /// </summary>
        /// <param name="ticket">The ticket concerned.</param>
        /// <param name="gate">The gate that is printed on the boarding pass.</param>
        /// <returns>The issued boarding pass.</returns>
        /// <exception cref="AirportClosedException">The desk is closed.</exception>
        /// <exception cref="InvalidFlightDataException">The ticket cannot be checked in in its current status.</exception>
        public BoardingPass CheckIn(Ticket ticket, Gate gate)
        {
            if (!IsOpen) throw new AirportClosedException("Стойка регистрации закрыта.");
            ticket.CheckIn();
            return new BoardingPass(ticket, gate, ticket.Flight.DepartureTime.AddMinutes(-40), 1 + ticket.Seat.Row / 10);
        }
    }

}
