using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// A meal that is served on board.
    /// </summary>
    public class Meal
    {
        /// <summary>
        /// Gets the meal name.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets the diet type.
        /// </summary>
        public MealType Type { get; }
        /// <summary>
        /// Gets the energy value, in kilocalories.
        /// </summary>
        public int Calories { get; }
        /// <summary>
        /// Gets the price, in euros.
        /// </summary>
        public decimal Price { get; }

        /// <summary>
        /// Initializes a new meal.
        /// </summary>
        /// <param name="name">The meal name.</param>
        /// <param name="type">The diet type.</param>
        /// <param name="calories">The energy value of the meal, in kilocalories.</param>
        /// <param name="price">The price, in euros.</param>
        public Meal(string name, MealType type, int calories, decimal price)
        {
            Name = name; Type = type; Calories = calories; Price = price;
        }

        /// <summary>
        /// Determines whether the meal is light.
        /// </summary>
        /// <returns><c>true</c> if it has fewer than 400 kcal.</returns>
        public bool IsLight() => Calories < 400;
        /// <summary>
        /// Determines whether the meal is for a special diet.
        /// </summary>
        /// <returns><c>true</c> if the type is not standard.</returns>
        public bool IsSpecialDiet() => Type != MealType.Standard;
    
    }

}
