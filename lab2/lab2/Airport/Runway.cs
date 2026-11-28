
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// An airport runway.
    /// </summary>
    public class Runway
    {
        /// <summary>
        /// Gets the runway designator, for example <c>09/27</c>.
        /// </summary>
        public string Number { get; }
        /// <summary>
        /// Gets the runway length, in metres.
        /// </summary>
        public double LengthM { get; }
        /// <summary>
        /// Gets a value that indicates whether the runway is open.
        /// </summary>
        public bool IsOpen { get; private set; } = true;
        /// <summary>
        /// Gets the surface type.
        /// </summary>
        public string Surface { get; }

        /// <summary>
        /// Initializes a new, open runway.
        /// </summary>
        /// <param name="number">The runway designator.</param>
        /// <param name="length">The length, in metres.</param>
        /// <param name="surface">The surface type of the runway.</param>
        public Runway(string number, double length, string surface = "Asphalt")
        {
            Number = number; LengthM = length; Surface = surface;
        }

        /// <summary>
        /// Determines whether an aircraft of the given type can use the runway.
        /// </summary>
        /// <param name="type">The aircraft type.</param>
        /// <returns><c>true</c> if the runway is open and long enough.</returns>
        public bool CanLand(AircraftType type) => IsOpen && LengthM >= type.RequiredRunwayM();
        /// <summary>
        /// Closes the runway.
        /// </summary>
        public void Close() => IsOpen = false;
        /// <summary>
        /// Opens the runway.
        /// </summary>
        public void Open() => IsOpen = true;
    }
}
