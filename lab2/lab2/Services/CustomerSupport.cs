
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// The customer support desk that registers and resolves complaints.
    /// </summary>
    public class CustomerSupport
    {
        /// <summary>
        /// Gets the support agent.
        /// </summary>
        public Employee Agent { get; }
        /// <summary>
        /// Gets the registered complaints.
        /// </summary>
        public List<Complaint> Complaints { get; } = new List<Complaint>();

        /// <summary>
        /// Initializes a new support desk.
        /// </summary>
        /// <param name="agent">The employee who handles passenger complaints.</param>
        public CustomerSupport(Employee agent) { Agent = agent; }

        /// <summary>
        /// Registers a complaint.
        /// </summary>
        /// <param name="c">The complaint to register.</param>
        public void Register(Complaint c) => Complaints.Add(c);
        /// <summary>
        /// Counts unresolved complaints.
        /// </summary>
        /// <returns>The number of open complaints.</returns>
        public int CountOpen() => Complaints.Count(c => c.IsOpen());
        /// <summary>
        /// Finds the complaints of a passenger.
        /// </summary>
        /// <param name="p">The passenger.</param>
        /// <returns>The list of matching complaints.</returns>
        public List<Complaint> FindByPassenger(Passenger p) => Complaints.Where(c => c.Passenger == p).ToList();

        /// <summary>
        /// Resolves a complaint by its position.
        /// </summary>
        /// <param name="index">The zero-based index in the list.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is outside the list.</exception>
        public void ResolveComplaint(int index)
        {
            if (index < 0 || index >= Complaints.Count) throw new ArgumentOutOfRangeException(nameof(index));
            Complaints[index].Resolve();
        }
    }
}
