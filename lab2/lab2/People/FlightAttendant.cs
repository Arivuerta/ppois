
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// A flight attendant. Knows languages and holds a medical certificate.
    /// </summary>
    public class FlightAttendant : Employee
    {
        /// <summary>
        /// Gets the languages that the attendant speaks.
        /// </summary>
        public List<string> Languages { get; } = new List<string>();
        /// <summary>
        /// Gets the expiry date of the medical certificate.
        /// </summary>
        public DateTime MedicalCertExpiry { get; }

        /// <summary>
        /// Initializes a new flight attendant.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="first">The given name.</param>
        /// <param name="last">The family name.</param>
        /// <param name="birth">The date of birth.</param>
        /// <param name="salary">The monthly salary, in euros.</param>
        /// <param name="hireDate">The date on which the employee was hired.</param>
        /// <param name="medicalExpiry">The expiry date of the medical certificate.</param>
        public FlightAttendant(int id, string first, string last, DateTime birth, decimal salary,
                               DateTime hireDate, DateTime medicalExpiry)
            : base(id, first, last, birth, "Flight Attendant", salary, hireDate)
        {
            MedicalCertExpiry = medicalExpiry;
        }

        /// <summary>
        /// Adds a spoken language. Duplicates are ignored.
        /// </summary>
        /// <param name="lang">The language code, for example <c>EN</c>.</param>
        public void AddLanguage(string lang) { if (!Languages.Contains(lang)) Languages.Add(lang); }
        /// <summary>
        /// Determines whether the attendant speaks a language.
        /// </summary>
        /// <param name="lang">The language code, for example <c>EN</c>.</param>
        /// <returns><c>true</c> if the language is listed.</returns>
        public bool SpeaksLanguage(string lang) => Languages.Contains(lang);
        /// <summary>
        /// Determines whether the medical certificate is valid.
        /// </summary>
        /// <returns><c>true</c> if it expires today or later.</returns>
        public bool HasValidMedical() => MedicalCertExpiry.Date >= DateTime.Today;
    }

}
