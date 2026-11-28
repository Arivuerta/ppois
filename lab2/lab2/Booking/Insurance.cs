using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// A travel insurance policy for one passenger on one flight.
    /// </summary>
    public class Insurance
    {
        /// <summary>
        /// Gets the insured passenger.
        /// </summary>
        public Passenger Passenger { get; }
        /// <summary>
        /// Gets the insured flight.
        /// </summary>
        public Flight Flight { get; }
        /// <summary>
        /// Gets the insured amount, in euros.
        /// </summary>
        public decimal Coverage { get; }
        /// <summary>
        /// Gets the premium, in euros.
        /// </summary>
        public decimal Premium { get; }

        /// <summary>
        /// Initializes a new insurance policy and calculates its premium.
        /// </summary>
        /// <param name="p">The insured passenger.</param>
        /// <param name="f">The insured flight.</param>
        /// <param name="coverage">The insured amount, in euros.</param>
        public Insurance(Passenger p, Flight f, decimal coverage)
        {
            Passenger = p; Flight = f; Coverage = coverage; Premium = CalculatePremium(f, coverage);
        }

        /// <summary>
        /// Calculates the premium: 1 % of the coverage, multiplied by 1.5 for international routes.
        /// </summary>
        /// <param name="f">The flight.</param>
        /// <param name="coverage">The insured amount, in euros.</param>
        /// <returns>The premium in euros, rounded to two decimal places.</returns>
        public static decimal CalculatePremium(Flight f, decimal coverage) =>
            Math.Round(coverage * 0.01m * (f.Route.IsInternational() ? 1.5m : 1m), 2);

        /// <summary>
        /// Determines whether the policy is in force.
        /// </summary>
        /// <param name="now">The reference date and time.</param>
        /// <returns><c>true</c> if the flight has not yet arrived and has not been cancelled.</returns>
        public bool IsActive(DateTime now) => now < Flight.GetArrivalTime() && Flight.Status != FlightStatus.Cancelled;
        /// <summary>
        /// Calculates the payout for a loss.
        /// </summary>
        /// <param name="loss">The amount of the loss, in euros.</param>
        /// <returns>The loss, limited to the coverage amount.</returns>
        public decimal Claim(decimal loss) => Math.Min(loss, Coverage);
    }
}
