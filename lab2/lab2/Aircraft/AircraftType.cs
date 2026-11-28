using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// Describes an aircraft model (for example A320 or B777) and its performance limits.
    /// </summary>
    public class AircraftType
    {
        /// <summary>
        /// Gets the model name, for example <c>A320</c>.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets the manufacturer name.
        /// </summary>
        public string Manufacturer { get; }
        /// <summary>
        /// Gets the maximum range, in kilometres.
        /// </summary>
        public double MaxRangeKm { get; }
        /// <summary>
        /// Gets the total number of passenger seats.
        /// </summary>
        public int SeatCapacity { get; }
        /// <summary>
        /// Gets the maximum take-off weight (MTOW), in kilograms.
        /// </summary>
        public double MaxTakeoffWeightKg { get; }

        /// <summary>
        /// Initializes a new aircraft type.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="manufacturer">The manufacturer name.</param>
        /// <param name="range">The maximum range, in kilometres.</param>
        /// <param name="seats">The seat capacity.</param>
        /// <param name="mtow">The maximum take-off weight, in kilograms.</param>
        public AircraftType(string name, string manufacturer, double range, int seats, double mtow)
        {
            Name = name; Manufacturer = manufacturer; MaxRangeKm = range;
            SeatCapacity = seats; MaxTakeoffWeightKg = mtow;
        }

        /// <summary>
        /// Determines whether the given distance is within the maximum range.
        /// </summary>
        /// <param name="km">The distance, in kilometres.</param>
        /// <returns><c>true</c> if the distance does not exceed <see cref="MaxRangeKm"/>.</returns>
        public bool CanFly(double km) => km <= MaxRangeKm;
        /// <summary>
        /// Determines whether the given payload can be carried. The payload limit is modelled as 20 % of the maximum take-off weight.
        /// </summary>
        /// <param name="weightKg">The total payload weight, in kilograms.</param>
        /// <returns><c>true</c> if the weight is within the payload limit.</returns>
        public bool CanCarry(double weightKg) => weightKg <= MaxTakeoffWeightKg * 0.2;
        /// <summary>
        /// Estimates the minimum runway length needed by this type (MTOW divided by 100).
        /// </summary>
        /// <returns>The required runway length, in metres.</returns>
        public double RequiredRunwayM() => MaxTakeoffWeightKg / 100.0;
    }
}
