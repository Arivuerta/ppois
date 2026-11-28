using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// A promotional discount identified by a code, expressed as a percentage and valid until a given date.
    /// </summary>
    public class Discount
    {
        /// <summary>
        /// Gets the promo code.
        /// </summary>
        public string Code { get; }
        /// <summary>
        /// Gets the discount percentage (greater than 0 and at most 100).
        /// </summary>
        public decimal Percent { get; }
        /// <summary>
        /// Gets the last day on which the discount can be used.
        /// </summary>
        public DateTime ValidUntil { get; }

        /// <summary>
        /// Initializes a new discount.
        /// </summary>
        /// <param name="code">The promo code.</param>
        /// <param name="percent">The discount percentage (greater than 0 and at most 100).</param>
        /// <param name="validUntil">The last day on which the code is valid.</param>
        public Discount(string code, decimal percent, DateTime validUntil)
        {
            Code = code; Percent = percent; ValidUntil = validUntil;
        }

        /// <summary>
        /// Determines whether the discount can be used on the given date.
        /// </summary>
        /// <param name="date">The date to check.</param>
        /// <returns><c>true</c> if the date is not after <see cref="ValidUntil"/> and the percentage is in the range (0, 100].</returns>
        public bool IsValid(DateTime date) => date.Date <= ValidUntil.Date && Percent > 0 && Percent <= 100;

        /// <summary>
        /// Applies the discount to a price. The result is rounded to two decimal places.
        /// </summary>
        /// <param name="price">The price, in euros.</param>
        /// <returns>The reduced price.</returns>
        /// <exception cref="InvalidPromoCodeException">The discount is not valid today.</exception>
        public decimal Apply(decimal price)
        {
            if (!IsValid(DateTime.Today)) throw new InvalidPromoCodeException($"Промокод {Code} недействителен.");
            return Math.Round(price * (1 - Percent / 100m), 2);
        }
    }

}
