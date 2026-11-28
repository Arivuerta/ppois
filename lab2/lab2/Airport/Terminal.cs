using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// An airport terminal that groups gates.
    /// </summary>
    public class Terminal
    {
        /// <summary>
        /// Gets the terminal name.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets the gates of the terminal.
        /// </summary>
        public List<Gate> Gates { get; } = new List<Gate>();
        /// <summary>
        /// Gets a value that indicates whether the terminal handles international flights.
        /// </summary>
        public bool IsInternational { get; }

        /// <summary>
        /// Initializes a new terminal without gates.
        /// </summary>
        /// <param name="name">The terminal name.</param>
        /// <param name="international"><c>true</c> if the terminal handles international flights.</param>
        public Terminal(string name, bool international) { Name = name; IsInternational = international; }

        /// <summary>
        /// Adds a gate to the terminal.
        /// </summary>
        /// <param name="g">The gate to add.</param>
        public void AddGate(Gate g) => Gates.Add(g);
        /// <summary>
        /// Finds the first free gate.
        /// </summary>
        /// <returns>The first free gate, or <c>null</c> if all gates are occupied.</returns>
        public Gate FindFreeGate() => Gates.FirstOrDefault(g => !g.IsOccupied);
        /// <summary>
        /// Counts the free gates.
        /// </summary>
        /// <returns>The number of free gates.</returns>
        public int CountFreeGates() => Gates.Count(g => !g.IsOccupied);
    }


}
