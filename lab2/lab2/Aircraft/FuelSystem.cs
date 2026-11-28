using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// The fuel tank of an aircraft, with refuelling and consumption logic.
    /// </summary>
    public class FuelSystem
    {
        /// <summary>
        /// Gets the tank capacity, in litres.
        /// </summary>
        public double CapacityL { get; }
        /// <summary>
        /// Gets the amount of fuel currently in the tank, in litres.
        /// </summary>
        public double CurrentL { get; private set; }
        /// <summary>
        /// Gets the fuel burn rate, in litres per kilometre.
        /// </summary>
        public double BurnRateLPerKm { get; }

        /// <summary>
        /// Initializes a new, full fuel tank.
        /// </summary>
        /// <param name="capacity">The tank capacity, in litres.</param>
        /// <param name="burnRate">The fuel burn rate, in litres per kilometre.</param>
        public FuelSystem(double capacity, double burnRate)
        {
            CapacityL = capacity; BurnRateLPerKm = burnRate; CurrentL = capacity;
        }

        /// <summary>
        /// Adds fuel to the tank. The level never exceeds the capacity.
        /// </summary>
        /// <param name="liters">The amount of fuel, in litres.</param>
        /// <exception cref="ArgumentException">The amount is zero or negative.</exception>
        public void Refuel(double liters)
        {
            if (liters <= 0) throw new ArgumentException("Литры должны быть > 0.");
            CurrentL = Math.Min(CapacityL, CurrentL + liters);
        }

        /// <summary>
        /// Determines whether the tank holds enough fuel to fly the given distance including a 10 % reserve.
        /// </summary>
        /// <param name="km">The distance, in kilometres.</param>
        /// <returns><c>true</c> if the fuel is sufficient.</returns>
        public bool HasEnoughFor(double km) => CurrentL >= km * BurnRateLPerKm * 1.1;
        /// <summary>
        /// Burns the fuel needed for the given distance. The level never drops below zero.
        /// </summary>
        /// <param name="km">The distance flown, in kilometres.</param>
        public void Consume(double km) => CurrentL = Math.Max(0, CurrentL - km * BurnRateLPerKm);
        /// <summary>
        /// Gets the tank level as a percentage.
        /// </summary>
        /// <returns>A value from 0 to 100.</returns>
        public double PercentFull() => CurrentL / CapacityL * 100;
    }
}
