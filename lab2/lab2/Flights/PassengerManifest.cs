using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// The list of passengers who travel on a flight.
    /// </summary>
    public class PassengerManifest
    {
        /// <summary>
        /// Gets the flight that the manifest belongs to.
        /// </summary>
        public Flight Flight { get; }
        /// <summary>
        /// Gets the passengers on the manifest.
        /// </summary>
        public List<Passenger> Passengers { get; } = new List<Passenger>();

        /// <summary>
        /// Initializes a new, empty manifest.
        /// </summary>
        /// <param name="flight">The flight.</param>
        public PassengerManifest(Flight flight) { Flight = flight; }

        /// <summary>
        /// Adds a passenger. A passenger who is already listed is not added twice.
        /// </summary>
        /// <param name="p">The passenger to add.</param>
        /// <exception cref="SeatUnavailableException">The manifest already holds as many passengers as the aircraft has seats.</exception>
        public void Add(Passenger p)
        {
            if (Passengers.Count >= Flight.Aircraft.Type.SeatCapacity)
                throw new SeatUnavailableException("Манифест переполнен.");
            if (!Passengers.Contains(p)) Passengers.Add(p);
        }

        /// <summary>
        /// Removes a passenger.
        /// </summary>
        /// <param name="p">The passenger to remove.</param>
        /// <returns><c>true</c> if the passenger was on the manifest.</returns>
        public bool Remove(Passenger p) => Passengers.Remove(p);
        /// <summary>
        /// Determines whether a passenger is on the manifest.
        /// </summary>
        /// <param name="p">The passenger to look for.</param>
        /// <returns><c>true</c> if the passenger is listed.</returns>
        public bool Contains(Passenger p) => Passengers.Contains(p);
        /// <summary>
        /// Gets the number of passengers.
        /// </summary>
        /// <returns>The passenger count.</returns>
        public int GetCount() => Passengers.Count;
        /// <summary>
        /// Counts the passengers who are younger than 18.
        /// </summary>
        /// <returns>The number of minors.</returns>
        public int CountMinors() => Passengers.Count(p => !p.IsAdult());
    }

}
