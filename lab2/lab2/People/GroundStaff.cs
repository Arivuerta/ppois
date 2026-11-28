
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// A member of the ground staff who works shifts at an airport.
    /// </summary>
    public class GroundStaff : Employee
    {
        /// <summary>
        /// Gets the airport where the employee works.
        /// </summary>
        public Airport BaseAirport { get; private set; }
        /// <summary>
        /// Gets the shift: <c>Day</c> or night.
        /// </summary>
        public string Shift { get; private set; }

        /// <summary>
        /// Initializes a new ground staff member.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="first">The given name.</param>
        /// <param name="last">The family name.</param>
        /// <param name="birth">The date of birth.</param>
        /// <param name="salary">The monthly salary, in euros.</param>
        /// <param name="hireDate">The date on which the employee was hired.</param>
        /// <param name="airport">The airport concerned.</param>
        /// <param name="shift">The shift name: <c>Day</c> or any other value for the night shift.</param>
        public GroundStaff(int id, string first, string last, DateTime birth, decimal salary,
                           DateTime hireDate, Airport airport, string shift)
            : base(id, first, last, birth, "Ground Staff", salary, hireDate)
        {
            BaseAirport = airport; Shift = shift;
        }

        /// <summary>
        /// Moves the employee to another airport.
        /// </summary>
        /// <param name="airport">The airport concerned.</param>
        public void AssignToAirport(Airport airport) => BaseAirport = airport;
        /// <summary>
        /// Changes the shift.
        /// </summary>
        /// <param name="shift">The shift name: <c>Day</c> or any other value for the night shift.</param>
        public void ChangeShift(string shift) => Shift = shift;
        /// <summary>
        /// Determines whether the employee is working at a given hour. The day shift is 08:00-20:00.
        /// </summary>
        /// <param name="hour">The hour of the day, from 0 to 23.</param>
        /// <returns><c>true</c> if the employee is on shift.</returns>
        public bool IsOnShift(int hour)
        {
            if (Shift == "Day")
                return hour >= 8 && hour < 20;
            else
                return hour >= 20 || hour < 8;
        }
    }
}
