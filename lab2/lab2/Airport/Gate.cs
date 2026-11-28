using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// A boarding gate that can hold one flight at a time.
    /// </summary>
    public class Gate
    {
        /// <summary>
        /// Gets the gate identifier, for example <c>G1</c>.
        /// </summary>
        public string Number { get; }
        /// <summary>
        /// Gets a value that indicates whether a flight is assigned to the gate.
        /// </summary>
        public bool IsOccupied { get; private set; }
        /// <summary>
        /// Gets the flight that currently occupies the gate, or <c>null</c>.
        /// </summary>
        public Flight CurrentFlight { get; private set; }

        /// <summary>
        /// Initializes a new, free gate.
        /// </summary>
        /// <param name="number">The gate identifier.</param>
        public Gate(string number) { Number = number; }

        /// <summary>
        /// Assigns a flight to the gate.
        /// </summary>
        /// <param name="flight">The flight to assign.</param>
        /// <exception cref="AircraftNotAvailableException">The gate is already occupied.</exception>
        public void Occupy(Flight flight)
        {
            if (IsOccupied) throw new AircraftNotAvailableException($"Гейт {Number} занят.");
            IsOccupied = true; CurrentFlight = flight;
        }

        /// <summary>
        /// Releases the gate.
        /// </summary>
        public void Free() { IsOccupied = false; CurrentFlight = null; }

    }
}
