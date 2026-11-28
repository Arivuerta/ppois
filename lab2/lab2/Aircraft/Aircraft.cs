using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// A single aircraft of the airline fleet. It owns its engines, fuel system and seat map and knows whether it is fit to fly a given route.
    /// </summary>
    public class Aircraft
    {
        /// <summary>
        /// Gets the unique registration (tail) number, for example <c>PH-AAA</c>.
        /// </summary>
        public string RegistrationNumber { get; }
        /// <summary>
        /// Gets the aircraft type (model) description.
        /// </summary>
        public AircraftType Type { get; }
        /// <summary>
        /// Gets the year in which the aircraft was manufactured.
        /// </summary>
        public int ManufactureYear { get; }
        /// <summary>
        /// Gets the current operational status of the aircraft.
        /// </summary>
        public AircraftStatus Status { get; private set; }
        /// <summary>
        /// Gets the engines installed on the aircraft.
        /// </summary>
        public List<Engine> Engines { get; } = new List<Engine>();
        /// <summary>
        /// Gets the fuel system of the aircraft.
        /// </summary>
        public FuelSystem Fuel { get; }
        /// <summary>
        /// Gets the seat map of the aircraft.
        /// </summary>
        public SeatMap Seats { get; }

        /// <summary>
        /// Initializes a new, active aircraft with the given number of engines, a full fuel tank and a generated seat map.
        /// </summary>
        /// <param name="reg">The registration number. Must not be empty.</param>
        /// <param name="type">The aircraft type.</param>
        /// <param name="year">The year of manufacture.</param>
        /// <param name="engineCount">The number of engines to install.</param>
        /// <param name="fuelCapacity">The fuel tank capacity, in litres.</param>
        /// <exception cref="InvalidFlightDataException">The registration number is empty or white space.</exception>
        public Aircraft(string reg, AircraftType type, int year, int engineCount, double fuelCapacity)
        {
            if (string.IsNullOrWhiteSpace(reg)) throw new InvalidFlightDataException("Пустая регистрация.");
            RegistrationNumber = reg; Type = type; ManufactureYear = year; Status = AircraftStatus.Active;
            for (int i = 1; i <= engineCount; i++) Engines.Add(new Engine($"{reg}-E{i}", 20000));
            Fuel = new FuelSystem(fuelCapacity, 3.0);
            Seats = new SeatMap(this);
        }

        /// <summary>
        /// Calculates the age of the aircraft in whole calendar years.
        /// </summary>
        /// <returns>The current year minus the manufacture year.</returns>
        public int GetAge() => DateTime.Today.Year - ManufactureYear;
        /// <summary>
        /// Determines whether the aircraft may be used for a flight.
        /// </summary>
        /// <returns><c>true</c> if the status is <see cref="AircraftStatus.Active"/> and no engine needs an overhaul.</returns>
        public bool IsAirworthy() => Status == AircraftStatus.Active && Engines.All(e => !e.NeedsOverhaul());
        /// <summary>
        /// Determines whether the aircraft can operate the given route, based on its range and the length of the longest runway at the origin airport.
        /// </summary>
        /// <param name="route">The route concerned.</param>
        /// <returns><c>true</c> if the route is within range and the origin runway is long enough.</returns>
        public bool CanServe(Route route) => Type.CanFly(route.DistanceKm) && route.Origin.GetLongestRunwayM() >= Type.RequiredRunwayM();
        /// <summary>
        /// Sets the aircraft status to <see cref="AircraftStatus.Maintenance"/>.
        /// </summary>
        public void SendToMaintenance() => Status = AircraftStatus.Maintenance;

        /// <summary>
        /// Overhauls every engine that needs it and sets the status back to <see cref="AircraftStatus.Active"/>.
        /// </summary>
        /// <exception cref="AircraftNotAvailableException">The aircraft has been retired.</exception>
        public void ReturnToService()
        {
            if (Status == AircraftStatus.Retired) throw new AircraftNotAvailableException("Самолёт списан.");
            foreach (var e in Engines.Where(e => e.NeedsOverhaul())) e.Overhaul();
            Status = AircraftStatus.Active;
        }

        /// <summary>
        /// Permanently withdraws the aircraft from service by setting its status to <see cref="AircraftStatus.Retired"/>.
        /// </summary>
        public void Retire() => Status = AircraftStatus.Retired;

        /// <summary>
        /// Adds the given flight time to every engine.
        /// </summary>
        /// <param name="hours">The number of flight hours to add.</param>
        public void RegisterFlightTime(double hours)
        {
            foreach (var e in Engines) e.AddHours(hours);
        }
    }

}
