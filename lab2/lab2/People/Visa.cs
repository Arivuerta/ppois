using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// A visa that allows a passenger to enter a country.
    /// </summary>
    public class Visa
    {
        /// <summary>
        /// Gets the visa number.
        /// </summary>
        public string Number { get; }
        /// <summary>
        /// Gets the country for which the visa is valid.
        /// </summary>
        public string Country { get; }
        /// <summary>
        /// Gets the expiry date.
        /// </summary>
        public DateTime ExpiryDate { get; }

        /// <summary>
        /// Initializes a new visa.
        /// </summary>
        /// <param name="number">The visa number.</param>
        /// <param name="country">The country name.</param>
        /// <param name="expiry">The expiry date.</param>
        public Visa(string number, string country, DateTime expiry)
        {
            Number = number; Country = country; ExpiryDate = expiry;
        }

        /// <summary>
        /// Determines whether the visa is valid on a date.
        /// </summary>
        /// <param name="date">The date to check.</param>
        /// <returns><c>true</c> if the date is not after the expiry date.</returns>
        public bool IsValidOn(DateTime date) => date.Date <= ExpiryDate.Date;
        /// <summary>
        /// Determines whether the visa is for a country. The comparison is case-insensitive.
        /// </summary>
        /// <param name="country">The country name.</param>
        /// <returns><c>true</c> if the country matches.</returns>
        public bool IsForCountry(string country) =>
            string.Equals(Country, country, StringComparison.OrdinalIgnoreCase);
        /// <summary>
        /// Calculates the remaining days.
        /// </summary>
        /// <returns>The days until expiry, or 0 if the visa has expired.</returns>
        public int DaysLeft() => Math.Max(0, (ExpiryDate.Date - DateTime.Today).Days);
    }

}
