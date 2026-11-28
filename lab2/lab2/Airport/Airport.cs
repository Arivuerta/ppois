using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{

    /// <summary>
    /// An airport identified by its three-letter IATA code. It owns runways and terminals and can be opened or closed.
    /// </summary>
    public class Airport
    {
        /// <summary>
        /// Gets the three-letter IATA code in upper case.
        /// </summary>
        public string Code { get; }
        /// <summary>
        /// Gets the airport name.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets the city that the airport serves.
        /// </summary>
        public string City { get; }
        /// <summary>
        /// Gets the country in which the airport is located.
        /// </summary>
        public string Country { get; }
        /// <summary>
        /// Gets a value that indicates whether the airport is open for operations.
        /// </summary>
        public bool IsOpen { get; private set; } = true;
        /// <summary>
        /// Gets the runways of the airport.
        /// </summary>
        public List<Runway> Runways { get; } = new List<Runway>();
        /// <summary>
        /// Gets the terminals of the airport.
        /// </summary>
        public List<Terminal> Terminals { get; } = new List<Terminal>();

        /// <summary>
        /// Initializes a new, open airport.
        /// </summary>
        /// <param name="code">The IATA code. Must consist of exactly three characters.</param>
        /// <param name="name">The airport name.</param>
        /// <param name="city">The city served by the airport.</param>
        /// <param name="country">The country name.</param>
        /// <exception cref="InvalidFlightDataException">The code is empty or is not three characters long.</exception>
        public Airport(string code, string name, string city, string country)
        {
            if (string.IsNullOrWhiteSpace(code) || code.Length != 3)
                throw new InvalidFlightDataException("Код аэропорта — 3 буквы IATA.");
            Code = code.ToUpper(); Name = name; City = city; Country = country;
        }

        /// <summary>
        /// Adds a runway to the airport.
        /// </summary>
        /// <param name="r">The runway to add.</param>
        public void AddRunway(Runway r) => Runways.Add(r);
        /// <summary>
        /// Adds a terminal to the airport.
        /// </summary>
        /// <param name="t">The terminal to add.</param>
        public void AddTerminal(Terminal t) => Terminals.Add(t);
        /// <summary>
        /// Closes the airport.
        /// </summary>
        public void CloseAirport() => IsOpen = false;
        /// <summary>
        /// Re-opens the airport.
        /// </summary>
        public void OpenAirport() => IsOpen = true;
        /// <summary>
        /// Gets the length of the longest runway.
        /// </summary>
        /// <returns>The length in metres, or 0 if the airport has no runways.</returns>
        public double GetLongestRunwayM() => Runways.Count == 0 ? 0 : Runways.Max(r => r.LengthM);

        /// <summary>
        /// Finds a free gate in any terminal.
        /// </summary>
        /// <returns>The first free gate, or <c>null</c> if all gates are occupied.</returns>
        /// <exception cref="AirportClosedException">The airport is closed.</exception>
        public Gate FindFreeGate()
        {
            if (!IsOpen) throw new AirportClosedException($"Аэропорт {Code} закрыт.");
            return Terminals.Select(t => t.FindFreeGate()).FirstOrDefault(g => g != null);
        }
    }
}
