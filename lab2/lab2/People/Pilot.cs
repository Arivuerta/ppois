
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// A pilot. The rank becomes captain after 3,000 flight hours.
    /// </summary>
    public class Pilot : Employee
    {
        /// <summary>
        /// Gets the pilot's licence.
        /// </summary>
        public PilotLicense License { get; }
        /// <summary>
        /// Gets the total flight hours.
        /// </summary>
        public double FlightHours { get; private set; }
        /// <summary>
        /// Gets the rank: <c>Captain</c> or <c>First Officer</c>.
        /// </summary>
        public string Rank { get; private set; }

        /// <summary>
        /// Initializes a new pilot.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="first">The given name.</param>
        /// <param name="last">The family name.</param>
        /// <param name="birth">The date of birth.</param>
        /// <param name="salary">The monthly salary, in euros.</param>
        /// <param name="hireDate">The date on which the employee was hired.</param>
        /// <param name="license">The pilot's licence.</param>
        /// <param name="hours">The flight hours already logged.</param>
        public Pilot(int id, string first, string last, DateTime birth, decimal salary,
                     DateTime hireDate, PilotLicense license, double hours)
            : base(id, first, last, birth, "Pilot", salary, hireDate)
        {
            License = license; FlightHours = hours;
            Rank = hours >= 3000 ? "Captain" : "First Officer";
        }

        /// <summary>
        /// Determines whether the pilot may fly an aircraft type.
        /// </summary>
        /// <param name="type">The aircraft type.</param>
        /// <returns><c>true</c> if the licence is valid and covers the type.</returns>
        public bool CanFly(AircraftType type) => License.IsValid() && License.CoversAircraft(type);

        /// <summary>
        /// Adds flight hours and promotes the pilot to captain at 3,000 hours.
        /// </summary>
        /// <param name="hours">The hours to add.</param>
        /// <exception cref="ArgumentException">The value is zero or negative.</exception>
        public void AddFlightHours(double hours)
        {
            if (hours <= 0) throw new ArgumentException("Часы должны быть > 0.");
            FlightHours += hours;
            if (FlightHours >= 3000) Rank = "Captain";
        }

        /// <summary>
        /// Determines whether the pilot is a captain.
        /// </summary>
        /// <returns><c>true</c> if the rank is captain.</returns>
        public bool IsCaptain() => Rank == "Captain";
        /// <summary>
        /// Calculates the bonus: the base bonus plus 0.5 euros for every flight hour.
        /// </summary>
        /// <returns>The bonus in euros.</returns>
        public override decimal CalculateBonus() => base.CalculateBonus() + (decimal)FlightHours * 0.5m;
    }
}
