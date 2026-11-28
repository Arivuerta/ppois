
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// A pilot licence with a type rating.
    /// </summary>
    public class PilotLicense
    {
        /// <summary>
        /// Gets the licence number.
        /// </summary>
        public string Number { get; }
        /// <summary>
        /// Gets the expiry date.
        /// </summary>
        public DateTime ExpiryDate { get; }
        /// <summary>
        /// Gets the aircraft type that the licence covers.
        /// </summary>
        public string TypeRating { get; }
        /// <summary>
        /// Gets the issuing authority.
        /// </summary>
        public string IssuedBy { get; }

        /// <summary>
        /// Initializes a new licence.
        /// </summary>
        /// <param name="number">The licence number.</param>
        /// <param name="expiry">The expiry date.</param>
        /// <param name="typeRating">The aircraft type name that the licence covers.</param>
        /// <param name="issuedBy">The authority that issued the licence (for example EASA).</param>
        /// <exception cref="InvalidLicenseException">The number is empty.</exception>
        public PilotLicense(string number, DateTime expiry, string typeRating, string issuedBy)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new InvalidLicenseException("Пустой номер лицензии.");
            Number = number; ExpiryDate = expiry; TypeRating = typeRating; IssuedBy = issuedBy;
        }

        /// <summary>
        /// Determines whether the licence is still valid.
        /// </summary>
        /// <returns><c>true</c> if the expiry date is today or later.</returns>
        public bool IsValid() => ExpiryDate.Date >= DateTime.Today;
        /// <summary>
        /// Determines whether the type rating matches an aircraft type. The comparison is case-insensitive.
        /// </summary>
        /// <param name="type">The aircraft type.</param>
        /// <returns><c>true</c> if the type names are equal.</returns>
        public bool CoversAircraft(AircraftType type) =>
            string.Equals(TypeRating, type.Name, StringComparison.OrdinalIgnoreCase);
        /// <summary>
        /// Calculates the days that remain until expiry.
        /// </summary>
        /// <returns>The number of days; negative if the licence has expired.</returns>
        public int DaysUntilExpiry() => (ExpiryDate.Date - DateTime.Today).Days;
    }
}
