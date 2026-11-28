using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// A checked baggage item that belongs to a passenger and is carried on a flight.
    /// </summary>
    public class Baggage
    {
        /// <summary>
        /// Gets the baggage tag identifier.
        /// </summary>
        public string Tag { get; }
        /// <summary>
        /// Gets the weight, in kilograms.
        /// </summary>
        public double WeightKg { get; }
        /// <summary>
        /// Gets the passenger who owns the baggage.
        /// </summary>
        public Passenger Owner { get; }
        /// <summary>
        /// Gets the flight on which the baggage travels.
        /// </summary>
        public Flight Flight { get; }
        /// <summary>
        /// Gets a value that indicates whether the baggage is fragile.
        /// </summary>
        public bool IsFragile { get; }

        /// <summary>
        /// Initializes a new baggage item.
        /// </summary>
        /// <param name="tag">The baggage tag identifier.</param>
        /// <param name="weight">The weight, in kilograms.</param>
        /// <param name="owner">The owner of the item.</param>
        /// <param name="flight">The flight on which the baggage travels.</param>
        /// <param name="fragile"><c>true</c> if the item is fragile.</param>
        public Baggage(string tag, double weight, Passenger owner, Flight flight, bool fragile = false)
        {
            Tag = tag; WeightKg = weight; Owner = owner; Flight = flight; IsFragile = fragile;
        }

        /// <summary>
        /// Determines whether the baggage exceeds the free allowance of the policy.
        /// </summary>
        /// <param name="policy">The baggage policy to apply.</param>
        /// <returns><c>true</c> if the weight is above <see cref="BaggagePolicy.FreeKg"/>.</returns>
        public bool IsOverweight(BaggagePolicy policy) => WeightKg > policy.FreeKg;
        /// <summary>
        /// Calculates the excess baggage fee according to the policy.
        /// </summary>
        /// <param name="policy">The baggage policy to apply.</param>
        /// <returns>The fee in euros, or 0 if the baggage is within the free allowance.</returns>
        /// <exception cref="BaggageOverweightException">The baggage exceeds the maximum permitted weight.</exception>
        public decimal CalculateFee(BaggagePolicy policy) => policy.Evaluate(this);
    }
}
