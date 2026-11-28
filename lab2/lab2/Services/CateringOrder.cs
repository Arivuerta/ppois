
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// An order of in-flight meals for a flight.
    /// </summary>
    public class CateringOrder
    {
        /// <summary>
        /// Gets the flight.
        /// </summary>
        public Flight Flight { get; }
        /// <summary>
        /// Gets the meal types in the order.
        /// </summary>
        public List<Meal> Meals { get; } = new List<Meal>();
        /// <summary>
        /// Gets the number of portions of each meal.
        /// </summary>
        public int Quantity { get; }
        /// <summary>
        /// Gets the date and time of the order.
        /// </summary>
        public DateTime OrderedAt { get; }

        /// <summary>
        /// Initializes a new order.
        /// </summary>
        /// <param name="flight">The flight.</param>
        /// <param name="quantity">The number of meal portions ordered.</param>
        /// <exception cref="InvalidFlightDataException">The quantity is zero or negative.</exception>
        public CateringOrder(Flight flight, int quantity)
        {
            if (quantity <= 0) throw new InvalidFlightDataException("Количество порций должно быть > 0.");
            Flight = flight; Quantity = quantity; OrderedAt = DateTime.Now;
        }

        /// <summary>
        /// Adds a meal to the order.
        /// </summary>
        /// <param name="m">The meal to add.</param>
        public void AddMeal(Meal m) => Meals.Add(m);
        /// <summary>
        /// Calculates the total cost.
        /// </summary>
        /// <returns>The sum of the meal prices multiplied by the quantity, in euros.</returns>
        public decimal TotalCost() => Meals.Sum(m => m.Price) * Quantity;
        /// <summary>
        /// Calculates the average energy value of the meals.
        /// </summary>
        /// <returns>The average in kilocalories, or 0 if there are no meals.</returns>
        public double AverageCalories() => Meals.Count == 0 ? 0 : Meals.Average(m => m.Calories);
        /// <summary>
        /// Determines whether the quantity is enough for every passenger with an active ticket.
        /// </summary>
        /// <returns><c>true</c> if there are enough portions.</returns>
        public bool CoversAllPassengers() => Quantity >= Flight.Tickets.Count(t => t.Status != TicketStatus.Cancelled);
    }
}
