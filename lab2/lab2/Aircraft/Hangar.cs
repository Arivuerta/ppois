using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// A hangar that can park a limited number of aircraft.
    /// </summary>
    public class Hangar
    {
        /// <summary>
        /// Gets the hangar name.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets the maximum number of aircraft the hangar can hold.
        /// </summary>
        public int Capacity { get; }
        /// <summary>
        /// Gets the aircraft currently parked in the hangar.
        /// </summary>
        public List<Aircraft> Parked { get; } = new List<Aircraft>();

        /// <summary>
        /// Initializes a new, empty hangar.
        /// </summary>
        /// <param name="name">The hangar name.</param>
        /// <param name="capacity">The number of parking spots.</param>
        public Hangar(string name, int capacity) { Name = name; Capacity = capacity; }

        /// <summary>
        /// Determines whether all spots are taken.
        /// </summary>
        /// <returns><c>true</c> if no free spot remains.</returns>
        public bool IsFull() => Parked.Count >= Capacity;
        /// <summary>
        /// Gets the number of free parking spots.
        /// </summary>
        /// <returns>The capacity minus the number of parked aircraft.</returns>
        public int FreeSpots() => Capacity - Parked.Count;

        /// <summary>
        /// Parks an aircraft in the hangar.
        /// </summary>
        /// <param name="a">The aircraft to park.</param>
        /// <exception cref="AircraftNotAvailableException">The hangar is full.</exception>
        public void Park(Aircraft a)
        {
            if (IsFull()) throw new AircraftNotAvailableException($"Ангар {Name} заполнен.");
            Parked.Add(a);
        }

        /// <summary>
        /// Removes the aircraft with the given registration number from the hangar.
        /// </summary>
        /// <param name="reg">The registration number of the aircraft.</param>
        /// <returns><c>true</c> if an aircraft was removed.</returns>
        public bool Release(string reg) => Parked.RemoveAll(a => a.RegistrationNumber == reg) > 0;
    }
}
