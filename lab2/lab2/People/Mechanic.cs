
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// An aircraft mechanic with a certification level.
    /// </summary>
    public class Mechanic : Employee
    {
        /// <summary>
        /// Gets the specialization.
        /// </summary>
        public string Specialization { get; }
        /// <summary>
        /// Gets the certification level (1 to 5).
        /// </summary>
        public int CertificationLevel { get; private set; }

        /// <summary>
        /// Initializes a new mechanic.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="first">The given name.</param>
        /// <param name="last">The family name.</param>
        /// <param name="birth">The date of birth.</param>
        /// <param name="salary">The monthly salary, in euros.</param>
        /// <param name="hireDate">The date on which the employee was hired.</param>
        /// <param name="specialization">The mechanic's field of specialization.</param>
        /// <param name="level">The initial certification level.</param>
        public Mechanic(int id, string first, string last, DateTime birth, decimal salary,
                        DateTime hireDate, string specialization, int level)
            : base(id, first, last, birth, "Mechanic", salary, hireDate)
        {
            Specialization = specialization; CertificationLevel = level;
        }

        /// <summary>
        /// Inspects an aircraft, puts it into maintenance and records the job.
        /// </summary>
        /// <param name="aircraft">The aircraft to inspect.</param>
        /// <param name="notes">The inspection notes.</param>
        /// <param name="cost">The cost of the work, in euros.</param>
        /// <returns>The maintenance record.</returns>
        public MaintenanceRecord Inspect(Aircraft aircraft, string notes, decimal cost)
        {
            aircraft.SendToMaintenance();
            return new MaintenanceRecord(DateTime.Now, notes, this, aircraft, cost);
        }

        /// <summary>
        /// Determines whether the mechanic may work on an aircraft.
        /// </summary>
        /// <param name="aircraft">The aircraft.</param>
        /// <returns><c>true</c> if the level is at least 2 or the aircraft is older than 15 years.</returns>
        public bool CanWorkOn(Aircraft aircraft) => CertificationLevel >= 2 || aircraft.GetAge() > 15;
        /// <summary>
        /// Raises the certification level by one, up to a maximum of 5.
        /// </summary>
        public void Promote() { if (CertificationLevel < 5) CertificationLevel++; }
    }

}
