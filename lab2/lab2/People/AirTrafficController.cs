
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// An air traffic controller who clears flights for take-off and landing.
    /// </summary>
    public class AirTrafficController : Employee
    {
        /// <summary>
        /// Gets the control tower name.
        /// </summary>
        public string Tower { get; }
        /// <summary>
        /// Gets the radio call sign.
        /// </summary>
        public string CallSign { get; }

        /// <summary>
        /// Initializes a new controller.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="first">The given name.</param>
        /// <param name="last">The family name.</param>
        /// <param name="birth">The date of birth.</param>
        /// <param name="salary">The monthly salary, in euros.</param>
        /// <param name="hireDate">The date on which the employee was hired.</param>
        /// <param name="tower">The name of the control tower.</param>
        /// <param name="callSign">The radio call sign of the controller.</param>
        public AirTrafficController(int id, string first, string last, DateTime birth, decimal salary,
                                    DateTime hireDate, string tower, string callSign)
            : base(id, first, last, birth, "ATC", salary, hireDate)
        {
            Tower = tower; CallSign = callSign;
        }

        /// <summary>
        /// Decides whether a flight may take off.
        /// </summary>
        /// <param name="flight">The flight.</param>
        /// <param name="weather">The current weather report.</param>
        /// <param name="runway">The runway to use.</param>
        /// <returns><c>true</c> if the runway can handle the aircraft and the weather is safe.</returns>
        public bool ClearForTakeoff(Flight flight, WeatherReport weather, Runway runway) =>
            runway.CanLand(flight.Aircraft.Type) && weather.IsSafeForTakeoff();

        /// <summary>
        /// Decides whether a flight may land.
        /// </summary>
        /// <param name="flight">The flight.</param>
        /// <param name="runway">The runway to use.</param>
        /// <returns><c>true</c> if the runway is open and long enough for the aircraft.</returns>
        public bool ClearForLanding(Flight flight, Runway runway) =>
            runway.IsOpen && runway.CanLand(flight.Aircraft.Type);
    }
}
