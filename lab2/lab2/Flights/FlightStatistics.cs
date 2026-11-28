using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// Operational and financial statistics derived from the schedule and the bookings.
    /// </summary>
    public class FlightStatistics
    {
        /// <summary>
        /// Gets the analysed flight schedule.
        /// </summary>
        public FlightSchedule Schedule { get; }
        /// <summary>
        /// Gets the recorded delays.
        /// </summary>
        public List<FlightDelay> Delays { get; } = new List<FlightDelay>();
        /// <summary>
        /// Gets the bookings used for revenue calculation.
        /// </summary>
        public List<Booking> Bookings { get; }

        /// <summary>
        /// Initializes a new statistics object.
        /// </summary>
        /// <param name="schedule">The flight schedule to analyse.</param>
        /// <param name="bookings">The shared list of bookings used to calculate revenue.</param>
        public FlightStatistics(FlightSchedule schedule, List<Booking> bookings)
        {
            Schedule = schedule; Bookings = bookings;
        }

        /// <summary>
        /// Records a flight delay.
        /// </summary>
        /// <param name="d">The delay to record.</param>
        public void RecordDelay(FlightDelay d) => Delays.Add(d);
        /// <summary>
        /// Calculates the average delay.
        /// </summary>
        /// <returns>The average in minutes, or 0 if no delays are recorded.</returns>
        public double AverageDelayMinutes() => Delays.Count == 0 ? 0 : Delays.Average(d => d.Minutes);
        /// <summary>
        /// Counts cancelled flights.
        /// </summary>
        /// <returns>The number of flights with the cancelled status.</returns>
        public int CancelledCount() => Schedule.Flights.Count(f => f.Status == FlightStatus.Cancelled);
        /// <summary>
        /// Calculates the average load factor over all flights.
        /// </summary>
        /// <returns>The average percentage, or 0 if there are no flights.</returns>
        public double AverageLoadFactor() => Schedule.Flights.Count == 0 ? 0 : Schedule.Flights.Average(f => f.GetLoadFactor());
        /// <summary>
        /// Calculates the revenue from paid bookings.
        /// </summary>
        /// <returns>The sum of all paid payments, in euros.</returns>
        public decimal TotalRevenue() => Bookings.Where(b => b.Payment != null && b.Payment.IsPaid()).Sum(b => b.Payment.Amount);
    }

}
