using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// A single aircraft engine that tracks the operating hours since its last overhaul.
    /// </summary>
    public class Engine
    {
        /// <summary>
        /// Gets the engine serial number.
        /// </summary>
        public string SerialNumber { get; }
        /// <summary>
        /// Gets the operating hours accumulated since the last overhaul.
        /// </summary>
        public double HoursSinceOverhaul { get; private set; }
        /// <summary>
        /// Gets the number of operating hours allowed between overhauls.
        /// </summary>
        public double MaxHours { get; }

        /// <summary>
        /// Initializes a new engine.
        /// </summary>
        /// <param name="serial">The engine serial number.</param>
        /// <param name="maxHours">The number of operating hours allowed between overhauls.</param>
        /// <param name="hours">The hours already accumulated since the last overhaul.</param>
        public Engine(string serial, double maxHours, double hours = 0)
        {
            SerialNumber = serial; MaxHours = maxHours; HoursSinceOverhaul = hours;
        }

        /// <summary>
        /// Adds operating hours. Values that are zero or negative are ignored.
        /// </summary>
        /// <param name="h">The hours to add.</param>
        public void AddHours(double h) { if (h > 0) HoursSinceOverhaul += h; }
        /// <summary>
        /// Determines whether the engine has reached its overhaul limit.
        /// </summary>
        /// <returns><c>true</c> if the accumulated hours are at least <see cref="MaxHours"/>.</returns>
        public bool NeedsOverhaul() => HoursSinceOverhaul >= MaxHours;
        /// <summary>
        /// Performs an overhaul and resets the accumulated hours to zero.
        /// </summary>
        public void Overhaul() => HoursSinceOverhaul = 0;
        /// <summary>
        /// Calculates the hours left until the next overhaul.
        /// </summary>
        /// <returns>The remaining hours, never less than zero.</returns>
        public double RemainingHours() => Math.Max(0, MaxHours - HoursSinceOverhaul);
    }


}
