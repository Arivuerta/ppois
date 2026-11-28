using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// The price of a seat class on a route, including tax.
    /// </summary>
    public class Fare
    {
        /// <summary>
        /// Gets the route that the fare applies to.
        /// </summary>
        public Route Route { get; }
        /// <summary>
        /// Gets the cabin class that the fare applies to.
        /// </summary>
        public SeatClass Class { get; }
        /// <summary>
        /// Gets the price before tax, in euros.
        /// </summary>
        public decimal BasePrice { get; }
        /// <summary>
        /// Gets the tax rate as a fraction (0.12 means 12 %).
        /// </summary>
        public decimal TaxRate { get; }

        /// <summary>
        /// Initializes a new fare.
        /// </summary>
        /// <param name="route">The route concerned.</param>
        /// <param name="cls">The cabin class.</param>
        /// <param name="basePrice">The base price before tax, in euros.</param>
        /// <param name="taxRate">The tax rate as a fraction (0.12 means 12 %).</param>
        public Fare(Route route, SeatClass cls, decimal basePrice, decimal taxRate = 0.12m)
        {
            Route = route; Class = cls; BasePrice = basePrice; TaxRate = taxRate;
        }

        /// <summary>
        /// Calculates the final price including tax.
        /// </summary>
        /// <returns>The base price multiplied by one plus the tax rate, rounded to two decimal places.</returns>
        public decimal Calculate() => Math.Round(BasePrice * (1 + TaxRate), 2);
        /// <summary>
        /// Gets the price multiplier of a cabin class (economy 1, business 2.5, first 4).
        /// </summary>
        /// <param name="c">The cabin class.</param>
        /// <returns>The price multiplier.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The class is not a known value.</exception>
        public static decimal ClassMultiplier(SeatClass c)
        {
            switch (c)
            {
                case SeatClass.First:
                    return 4m;
                case SeatClass.Business:
                    return 2.5m;
                case SeatClass.Economy:
                    return 1m;
                default:
                    throw new ArgumentOutOfRangeException(nameof(c), c, "Неизвестный класс места");
            }
        }
        /// <summary>
        /// Creates a fare for a route and cabin class at 0.09 euros per kilometre multiplied by the class multiplier.
        /// </summary>
        /// <param name="r">The route.</param>
        /// <param name="c">The cabin class.</param>
        /// <returns>The new fare.</returns>
        public static Fare ForRoute(Route r, SeatClass c)
        {
            decimal pricePerKm = 0.09m;
            decimal distance = (decimal)r.DistanceKm;
            decimal multiplier = ClassMultiplier(c);
            decimal rawPrice = distance * pricePerKm * multiplier;
            decimal finalPrice = Math.Round(rawPrice, 2);

            Fare fare = new Fare(r, c, finalPrice);
            return fare;
        }
    }

}
