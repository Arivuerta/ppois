
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// A flight dispatcher who approves and reschedules flights.
    /// </summary>
    public class Dispatcher : Employee
    {
        /// <summary>
        /// Gets the desk name.
        /// </summary>
        public string Desk { get; }
        /// <summary>
        /// Gets the number of flights approved so far.
        /// </summary>
        public int ApprovedCount { get; private set; }

        /// <summary>
        /// Initializes a new dispatcher.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="first">The given name.</param>
        /// <param name="last">The family name.</param>
        /// <param name="birth">The date of birth.</param>
        /// <param name="salary">The monthly salary, in euros.</param>
        /// <param name="hireDate">The date on which the employee was hired.</param>
        /// <param name="desk">The name of the dispatcher's desk.</param>
        public Dispatcher(int id, string first, string last, DateTime birth, decimal salary,
                          DateTime hireDate, string desk)
            : base(id, first, last, birth, "Dispatcher", salary, hireDate)
        {
            Desk = desk;
        }

        /// <summary>
        /// Approves a flight if the weather is safe and the aircraft is airworthy.
        /// </summary>
        /// <param name="flight">The flight.</param>
        /// <param name="weather">The current weather report.</param>
        /// <returns><c>true</c> if the flight is approved.</returns>
        public bool ApproveFlight(Flight flight, WeatherReport weather)
        {
            bool ok = weather.IsSafeForTakeoff() && flight.Aircraft.IsAirworthy();
            if (ok) ApprovedCount++;
            return ok;
        }

        /// <summary>
        /// Moves a flight to a new departure time.
        /// </summary>
        /// <param name="flight">The flight.</param>
        /// <param name="newTime">The new departure date and time.</param>
        /// <exception cref="InvalidFlightDataException">The flight has already departed.</exception>
        public void RescheduleFlight(Flight flight, DateTime newTime) => flight.Reschedule(newTime);
    }
}
