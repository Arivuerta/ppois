using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// An immutable record of a maintenance job performed on an aircraft.
    /// </summary>
    public class MaintenanceRecord
    {
        /// <summary>
        /// Gets the date and time of the work.
        /// </summary>
        public DateTime Date { get; }
        /// <summary>
        /// Gets the description of the work performed.
        /// </summary>
        public string Description { get; }
        /// <summary>
        /// Gets the mechanic who performed the work.
        /// </summary>
        public Mechanic Mechanic { get; }
        /// <summary>
        /// Gets the aircraft that was serviced.
        /// </summary>
        public Aircraft Aircraft { get; }
        /// <summary>
        /// Gets the cost of the job, in euros.
        /// </summary>
        public decimal Cost { get; }

        /// <summary>
        /// Initializes a new maintenance record.
        /// </summary>
        /// <param name="date">The date and time of the work.</param>
        /// <param name="description">The description of the work.</param>
        /// <param name="mechanic">The mechanic who performed the work.</param>
        /// <param name="aircraft">The aircraft concerned.</param>
        /// <param name="cost">The cost of the work, in euros.</param>
        public MaintenanceRecord(DateTime date, string description, Mechanic mechanic, Aircraft aircraft, decimal cost)
        {
            Date = date; Description = description; Mechanic = mechanic; Aircraft = aircraft; Cost = cost;
        }

        /// <summary>
        /// Determines whether the job was expensive (more than 50,000 euros).
        /// </summary>
        /// <returns><c>true</c> if the cost exceeds 50,000.</returns>
        public bool IsCostly() => Cost > 50000m;
        /// <summary>
        /// Builds a one-line, human-readable summary of the record.
        /// </summary>
        /// <returns>A string with the date, registration number, description, mechanic name and cost.</returns>
        public string GetSummary() =>
            $"{Date:d}: {Aircraft.RegistrationNumber} — {Description} ({Mechanic.GetFullName()}), {Cost:N2} €";
    }

}
