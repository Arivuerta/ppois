
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// Base class for all airline employees. Adds position, salary and hire date to <see cref="Person"/> and provides bonus calculation.
    /// </summary>
    public class Employee : Person
    {
        /// <summary>
        /// Gets the job title.
        /// </summary>
        public string Position { get; protected set; }
        /// <summary>
        /// Gets the monthly salary, in euros.
        /// </summary>
        public decimal Salary { get; protected set; }
        /// <summary>
        /// Gets the date on which the employee was hired.
        /// </summary>
        public DateTime HireDate { get; }

        /// <summary>
        /// Initializes a new employee.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="first">The given name.</param>
        /// <param name="last">The family name.</param>
        /// <param name="birth">The date of birth.</param>
        /// <param name="position">The job title.</param>
        /// <param name="salary">The monthly salary, in euros.</param>
        /// <param name="hireDate">The date on which the employee was hired.</param>
        /// <exception cref="InvalidPassengerDataException">The salary is negative.</exception>
        public Employee(int id, string first, string last, DateTime birth,
                        string position, decimal salary, DateTime hireDate)
            : base(id, first, last, birth)
        {
            if (salary < 0) throw new InvalidPassengerDataException("Отрицательная зарплата.");
            Position = position; Salary = salary; HireDate = hireDate;
        }

        /// <summary>
        /// Increases the salary by a percentage.
        /// </summary>
        /// <param name="percent">The raise in percent. Must be greater than zero.</param>
        /// <exception cref="ArgumentException">The percentage is zero or negative.</exception>
        public void RaiseSalary(decimal percent)
        {
            if (percent <= 0) throw new ArgumentException("Процент должен быть > 0.");
            Salary += Salary * percent / 100m;
        }

        /// <summary>
        /// Calculates the length of service.
        /// </summary>
        /// <returns>The number of full years since the hire date.</returns>
        public int GetExperienceYears() => (int)((DateTime.Today - HireDate).Days / 365.25);
        /// <summary>
        /// Calculates the yearly bonus: 2 % of the salary for every year of service. Derived classes may override the rule.
        /// </summary>
        /// <returns>The bonus in euros.</returns>
        public virtual decimal CalculateBonus() => Salary * 0.02m * GetExperienceYears();
    }
}
