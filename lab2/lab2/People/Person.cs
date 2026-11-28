
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// Base class for every human in the system. It stores the name and date of birth and validates them.
    /// </summary>
    public class Person
    {
        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        public int Id { get; }
        /// <summary>
        /// Gets the given name.
        /// </summary>
        public string FirstName { get; private set; }
        /// <summary>
        /// Gets the family name.
        /// </summary>
        public string LastName { get; private set; }
        /// <summary>
        /// Gets the date of birth.
        /// </summary>
        public DateTime BirthDate { get; }

        /// <summary>
        /// Initializes a new person.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="firstName">The given name. Must not be empty.</param>
        /// <param name="lastName">The family name. Must not be empty.</param>
        /// <param name="birthDate">The date of birth. Must not be in the future.</param>
        /// <exception cref="InvalidPassengerDataException">A name is empty or the birth date is in the future.</exception>
        public Person(int id, string firstName, string lastName, DateTime birthDate)
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                throw new InvalidPassengerDataException("Имя и фамилия не могут быть пустыми.");
            if (birthDate > DateTime.Today)
                throw new InvalidPassengerDataException("Дата рождения в будущем.");
            Id = id; FirstName = firstName; LastName = lastName; BirthDate = birthDate;
        }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        /// <returns>The given name followed by the family name.</returns>
        public string GetFullName() => $"{FirstName} {LastName}";

        /// <summary>
        /// Calculates the age in whole years as of today.
        /// </summary>
        /// <returns>The age in years.</returns>
        public int GetAge()
        {
            var today = DateTime.Today;
            int age = today.Year - BirthDate.Year;
            if (BirthDate.Date > today.AddYears(-age)) age--;
            return age;
        }

        /// <summary>
        /// Determines whether the person is at least 18 years old.
        /// </summary>
        /// <returns><c>true</c> if the age is 18 or more.</returns>
        public bool IsAdult() => GetAge() >= 18;

        /// <summary>
        /// Changes the name of the person.
        /// </summary>
        /// <param name="first">The new given name.</param>
        /// <param name="last">The new family name.</param>
        /// <exception cref="InvalidPassengerDataException">A name is empty.</exception>
        public void Rename(string first, string last)
        {
            if (string.IsNullOrWhiteSpace(first) || string.IsNullOrWhiteSpace(last))
                throw new InvalidPassengerDataException("Новое имя некорректно.");
            FirstName = first; LastName = last;
        }
    }
}
