using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// Baggage rules of the airline: the free allowance, the fee per extra kilogram and the absolute weight limit.
    /// </summary>
    public class BaggagePolicy
    {
        /// <summary>
        /// Gets the free baggage allowance, in kilograms.
        /// </summary>
        public double FreeKg { get; }
        /// <summary>
        /// Gets the fee for each kilogram above the free allowance, in euros.
        /// </summary>
        public decimal FeePerKg { get; }
        /// <summary>
        /// Gets the maximum weight of one item that is accepted at all, in kilograms.
        /// </summary>
        public double MaxKg { get; }

        /// <summary>
        /// Initializes a new baggage policy.
        /// </summary>
        /// <param name="freeKg">The free baggage allowance, in kilograms.</param>
        /// <param name="feePerKg">The fee charged for every kilogram above the free allowance, in euros.</param>
        /// <param name="maxKg">The maximum baggage weight that is accepted at all, in kilograms.</param>
        public BaggagePolicy(double freeKg, decimal feePerKg, double maxKg)
        {
            FreeKg = freeKg; FeePerKg = feePerKg; MaxKg = maxKg;
        }

        /// <summary>
        /// Determines whether the baggage may be accepted.
        /// </summary>
        /// <param name="b">The baggage item to check.</param>
        /// <returns><c>true</c> if the weight does not exceed <see cref="MaxKg"/>.</returns>
        public bool IsAllowed(Baggage b) => b.WeightKg <= MaxKg;

        /// <summary>
        /// Calculates the fee for the baggage item.
        /// </summary>
        /// <param name="b">The baggage item to evaluate.</param>
        /// <returns>The fee in euros; zero if the weight is within the free allowance.</returns>
        /// <exception cref="BaggageOverweightException">The weight exceeds <see cref="MaxKg"/>.</exception>
        public decimal Evaluate(Baggage b)
        {
            if (!IsAllowed(b)) throw new BaggageOverweightException($"Багаж {b.Tag} ({b.WeightKg} кг) превышает максимум {MaxKg} кг.");
            return b.WeightKg > FreeKg ? (decimal)(b.WeightKg - FreeKg) * FeePerKg : 0m;
        }
    }
}
