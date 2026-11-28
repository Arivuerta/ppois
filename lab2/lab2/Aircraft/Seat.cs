using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// A single passenger seat of an aircraft.
    /// </summary>
    public class Seat
    {
        /// <summary>
        /// Gets the row number.
        /// </summary>
        public int Row { get; }
        /// <summary>
        /// Gets the seat letter within the row.
        /// </summary>
        public char Letter { get; }
        /// <summary>
        /// Gets the cabin class of the seat.
        /// </summary>
        public SeatClass Class { get; }
        /// <summary>
        /// Gets a value that indicates whether the seat is reserved.
        /// </summary>
        public bool IsOccupied { get; private set; }

        /// <summary>
        /// Initializes a new, free seat.
        /// </summary>
        /// <param name="row">The row number.</param>
        /// <param name="letter">The seat letter within the row.</param>
        /// <param name="cls">The cabin class.</param>
        public Seat(int row, char letter, SeatClass cls)
        {
            Row = row; Letter = letter; Class = cls;
        }

        /// <summary>
        /// Builds the seat label.
        /// </summary>
        /// <returns>The row number followed by the letter, for example <c>12A</c>.</returns>
        public string Label() => $"{Row}{Letter}";

        /// <summary>
        /// Reserves the seat.
        /// </summary>
        /// <exception cref="SeatUnavailableException">The seat is already occupied.</exception>
        public void Reserve()
        {
            if (IsOccupied) throw new SeatUnavailableException($"Место {Label()} уже занято.");
            IsOccupied = true;
        }

        /// <summary>
        /// Frees the seat.
        /// </summary>
        public void Release() => IsOccupied = false;
    }
}
