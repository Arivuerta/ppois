using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// A cargo item that is carried in the hold of a flight.
    /// </summary>
    public class Cargo
    {
        /// <summary>
        /// Gets the cargo identifier.
        /// </summary>
        public int Id { get; }
        /// <summary>
        /// Gets the weight, in kilograms.
        /// </summary>
        public double WeightKg { get; }
        /// <summary>
        /// Gets the description of the goods.
        /// </summary>
        public string Description { get; }
        /// <summary>
        /// Gets a value that indicates whether the cargo is hazardous.
        /// </summary>
        public bool IsHazardous { get; }

        /// <summary>
        /// Initializes a new cargo item.
        /// </summary>
        /// <param name="id">The cargo identifier.</param>
        /// <param name="weight">The weight, in kilograms.</param>
        /// <param name="description">The description text.</param>
        /// <param name="hazardous"><c>true</c> if the cargo is classified as hazardous.</param>
        /// <exception cref="InvalidFlightDataException">The weight is zero or negative.</exception>
        public Cargo(int id, double weight, string description, bool hazardous)
        {
            if (weight <= 0) throw new InvalidFlightDataException("Вес груза должен быть > 0.");
            Id = id; WeightKg = weight; Description = description; IsHazardous = hazardous;
        }

        /// <summary>
        /// Calculates the shipping fee: 2.5 euros per kilogram, doubled for hazardous goods.
        /// </summary>
        /// <returns>The fee in euros.</returns>
        public decimal CalculateFee() => (decimal)WeightKg * 2.5m * (IsHazardous ? 2 : 1);
        /// <summary>
        /// Determines whether the cargo is too heavy for an aircraft type.
        /// </summary>
        /// <param name="t">The aircraft type.</param>
        /// <returns><c>true</c> if the type cannot carry this weight.</returns>
        public bool IsOverweightFor(AircraftType t) => !t.CanCarry(WeightKg);
    }
}
