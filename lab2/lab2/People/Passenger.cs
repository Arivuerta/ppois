
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// A passenger of the airline. Holds the passport data, contact e-mail and the visas that are needed for international travel.
    /// </summary>
    public class Passenger : Person
    {
        /// <summary>
        /// Gets the passport number.
        /// </summary>
        public string PassportNumber { get; }
        /// <summary>
        /// Gets the nationality (country name).
        /// </summary>
        public string Nationality { get; }
        /// <summary>
        /// Gets the contact e-mail address.
        /// </summary>
        public string Email { get; private set; }
        /// <summary>
        /// Gets the visas that the passenger holds.
        /// </summary>
        public List<Visa> Visas { get; } = new List<Visa>();

        /// <summary>
        /// Initializes a new passenger.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="first">The given name.</param>
        /// <param name="last">The family name.</param>
        /// <param name="birth">The date of birth.</param>
        /// <param name="passport">The passport number (8 to 9 letters or digits).</param>
        /// <param name="nationality">The passenger's nationality (country name).</param>
        /// <param name="email">The e-mail address. Must contain the <c>@</c> character.</param>
        /// <exception cref="InvalidPassengerDataException">The passport format or the e-mail address is invalid.</exception>
        public Passenger(int id, string first, string last, DateTime birth,
                         string passport, string nationality, string email)
            : base(id, first, last, birth)
        {
            if (!IsPassportFormatValid(passport))
                throw new InvalidPassengerDataException("Паспорт: 8-9 букв/цифр.");
            if (!email.Contains('@'))
                throw new InvalidPassengerDataException("Некорректный e-mail.");
            PassportNumber = passport; Nationality = nationality; Email = email;
        }

        /// <summary>
        /// Checks the passport number format: 8 or 9 letters or digits.
        /// </summary>
        /// <param name="p">The passport number to check.</param>
        /// <returns><c>true</c> if the format is valid.</returns>
        public static bool IsPassportFormatValid(string p)
        {
            if (string.IsNullOrEmpty(p))
                return false;

            if (p.Length < 8 || p.Length > 9)
                return false;

            foreach (char c in p)
            {
                if (!char.IsLetterOrDigit(c))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Changes the contact e-mail address.
        /// </summary>
        /// <param name="email">The e-mail address. Must contain the <c>@</c> character.</param>
        /// <exception cref="InvalidPassengerDataException">The e-mail address is invalid.</exception>
        public void ChangeEmail(string email)
        {
            if (!email.Contains('@')) throw new InvalidPassengerDataException("Некорректный e-mail.");
            Email = email;
        }

        /// <summary>
        /// Adds a visa to the passenger.
        /// </summary>
        /// <param name="visa">The visa to add.</param>
        public void AddVisa(Visa visa) => Visas.Add(visa);

        /// <summary>
        /// Determines whether the passenger may enter a country on a given date. Citizens of the country do not need a visa.
        /// </summary>
        /// <param name="country">The destination country.</param>
        /// <param name="date">The date of entry.</param>
        /// <returns><c>true</c> if the passenger is a citizen or holds a visa that is valid on that date.</returns>
        public bool HasValidVisa(string country, DateTime date) =>
            Nationality.Equals(country, StringComparison.OrdinalIgnoreCase) ||
            Visas.Any(v => v.IsForCountry(country) && v.IsValidOn(date));
    }
}
