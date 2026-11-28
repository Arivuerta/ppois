using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// The schedule of all flights of the airline. It prevents double-booking of aircraft and offers several lookup methods.
    /// </summary>
    public class FlightSchedule
    {
        /// <summary>
        /// Gets all scheduled flights.
        /// </summary>
        public List<Flight> Flights { get; } = new List<Flight>();
        /// <summary>
        /// Gets the date and time of the last change to the schedule.
        /// </summary>
        public DateTime LastUpdated { get; private set; } = DateTime.Now;

        /// <summary>
        /// Determines whether the aircraft is already used by another active flight during the given interval.
        /// </summary>
        /// <param name="aircraft">The aircraft to check.</param>
        /// <param name="start">The start of the time interval.</param>
        /// <param name="end">The end of the time interval.</param>
        /// <returns><c>true</c> if an overlapping flight exists.</returns>
        public bool HasConflict(Aircraft aircraft, DateTime start, DateTime end) =>
            Flights.Any(f => f.Aircraft == aircraft && f.Status != FlightStatus.Cancelled &&
                             f.DepartureTime < end && f.GetArrivalTime() > start);

        /// <summary>
        /// Adds a flight to the schedule.
        /// </summary>
        /// <param name="f">The flight to add.</param>
        /// <exception cref="AircraftNotAvailableException">The aircraft is already used during the flight.</exception>
        public void AddFlight(Flight f)
        {
            if (HasConflict(f.Aircraft, f.DepartureTime, f.GetArrivalTime()))
                throw new AircraftNotAvailableException($"{f.Aircraft.RegistrationNumber} занят в это время.");
            Flights.Add(f); LastUpdated = DateTime.Now;
        }

        /// <summary>
        /// Finds a flight by its number (case-insensitive).
        /// </summary>
        /// <param name="number">The flight number.</param>
        /// <returns>The matching flight.</returns>
        /// <exception cref="FlightNotFoundException">No such flight exists.</exception>
        public Flight FindByNumber(string number) =>
            Flights.FirstOrDefault(f => f.Number.Equals(number, StringComparison.OrdinalIgnoreCase))
            ?? throw new FlightNotFoundException($"Рейс {number} не найден.");

        /// <summary>
        /// Finds all flights between two airports.
        /// </summary>
        /// <param name="from">The IATA code of the departure airport.</param>
        /// <param name="to">The IATA code of the arrival airport.</param>
        /// <returns>The list of matching flights, which may be empty.</returns>
        public List<Flight> FindByRoute(string from, string to) =>
            Flights.Where(f => f.Route.Origin.Code == from.ToUpper() && f.Route.Destination.Code == to.ToUpper()).ToList();

        /// <summary>
        /// Gets the flights that depart on a given day.
        /// </summary>
        /// <param name="day">The calendar day to look up. Only the date part is used.</param>
        /// <returns>The flights ordered by departure time.</returns>
        public List<Flight> GetDeparturesOn(DateTime day) =>
            Flights.Where(f => f.DepartureTime.Date == day.Date).OrderBy(f => f.DepartureTime).ToList();

        /// <summary>
        /// Removes a flight from the schedule.
        /// </summary>
        /// <param name="number">The flight number.</param>
        /// <returns><c>true</c> if a flight was removed.</returns>
        public bool RemoveFlight(string number)
        {
            bool removed = Flights.RemoveAll(f => f.Number == number) > 0;
            if (removed) LastUpdated = DateTime.Now;
            return removed;
        }
    }
}
