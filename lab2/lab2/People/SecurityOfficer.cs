
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// A security officer who screens passengers and baggage.
    /// </summary>
    public class SecurityOfficer : Employee
    {
        /// <summary>
        /// Gets the badge number.
        /// </summary>
        public string BadgeNumber { get; }
        /// <summary>
        /// Gets the checkpoint where the officer works.
        /// </summary>
        public string Checkpoint { get; }

        /// <summary>
        /// Initializes a new security officer.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="first">The given name.</param>
        /// <param name="last">The family name.</param>
        /// <param name="birth">The date of birth.</param>
        /// <param name="salary">The monthly salary, in euros.</param>
        /// <param name="hireDate">The date on which the employee was hired.</param>
        /// <param name="badge">The security badge number.</param>
        /// <param name="checkpoint">The security checkpoint where the officer works.</param>
        public SecurityOfficer(int id, string first, string last, DateTime birth, decimal salary,
                               DateTime hireDate, string badge, string checkpoint)
            : base(id, first, last, birth, "Security Officer", salary, hireDate)
        {
            BadgeNumber = badge; Checkpoint = checkpoint;
        }

        /// <summary>
        /// Screens a baggage item.
        /// </summary>
        /// <param name="bag">The baggage item to screen.</param>
        /// <returns><c>true</c> if the weight is greater than zero and less than 60 kg.</returns>
        public bool ScreenBaggage(Baggage bag) => bag.WeightKg > 0 && bag.WeightKg < 60;
        /// <summary>
        /// Screens a passenger.
        /// </summary>
        /// <param name="p">The passenger.</param>
        /// <returns><c>true</c> if the passenger passes the check.</returns>
        public bool ScreenPassenger(Passenger p) => p.IsAdult() || p.GetAge() >= 0;
    }
}
