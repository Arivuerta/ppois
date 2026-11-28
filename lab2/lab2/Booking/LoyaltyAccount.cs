using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// A frequent-flyer account that collects miles and determines the loyalty tier.
    /// </summary>
    public class LoyaltyAccount
    {
        /// <summary>
        /// Gets the passenger who owns the account.
        /// </summary>
        public Passenger Owner { get; }
        /// <summary>
        /// Gets the current mile balance.
        /// </summary>
        public int Miles { get; private set; }
        /// <summary>
        /// Gets the current loyalty tier.
        /// </summary>
        public LoyaltyTier Tier { get; private set; } = LoyaltyTier.Basic;

        /// <summary>
        /// Initializes a new account with a zero balance in the basic tier.
        /// </summary>
        /// <param name="owner">The account owner.</param>
        public LoyaltyAccount(Passenger owner) { Owner = owner; }

        /// <summary>
        /// Credits miles for a flown distance. The tier bonus adds 25 % per tier level, and the tier is recalculated afterwards.
        /// </summary>
        /// <param name="km">The distance flown, in kilometres.</param>
        public void AddMiles(double km)
        {
            Miles += (int)(km * (1 + (int)Tier * 0.25));
            UpdateTier();
        }

        /// <summary>
        /// Deducts miles from the balance.
        /// </summary>
        /// <param name="miles">The number of miles to spend.</param>
        /// <exception cref="InsufficientFundsException">The balance is lower than the requested amount.</exception>
        public void SpendMiles(int miles)
        {
            if (miles > Miles) throw new InsufficientFundsException($"Недостаточно миль: {Miles}.");
            Miles -= miles;
        }

        /// <summary>
        /// Recalculates the tier from the balance: 20,000 miles for silver, 50,000 for gold and 100,000 for platinum.
        /// </summary>
        public void UpdateTier() =>
            Tier = Miles >= 100000 ? LoyaltyTier.Platinum : Miles >= 50000 ? LoyaltyTier.Gold
                 : Miles >= 20000 ? LoyaltyTier.Silver : LoyaltyTier.Basic;
    }

}
