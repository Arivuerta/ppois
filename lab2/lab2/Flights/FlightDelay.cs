using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// A report that describes the delay of a flight.
    /// </summary>
    public class FlightDelay
    {
        /// <summary>
        /// Gets the delayed flight.
        /// </summary>
        public Flight Flight { get; }
        /// <summary>
        /// Gets the delay, in minutes.
        /// </summary>
        public int Minutes { get; }
        /// <summary>
        /// Gets the reason for the delay.
        /// </summary>
        public string Reason { get; }
        /// <summary>
        /// Gets the date and time at which the delay was reported.
        /// </summary>
        public DateTime ReportedAt { get; }

        /// <summary>
        /// Initializes a new delay report.
        /// </summary>
        /// <param name="flight">The flight concerned.</param>
        /// <param name="minutes">The delay duration, in minutes.</param>
        /// <param name="reason">The reason given.</param>
        public FlightDelay(Flight flight, int minutes, string reason)
        {
            Flight = flight; Minutes = minutes; Reason = reason; ReportedAt = DateTime.Now;
        }

        /// <summary>
        /// Determines whether the delay is severe.
        /// </summary>
        /// <returns><c>true</c> if the delay is at least 180 minutes.</returns>
        public bool IsSevere() => Minutes >= 180;
        /// <summary>
        /// Builds a one-line description of the delay.
        /// </summary>
        /// <returns>A string with the flight number, the delay in minutes and the reason.</returns>
        public string Describe() => $"Рейс {Flight.Number} задержан на {Minutes} мин: {Reason}";
    }
}
