using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AirlineSystem
{
    /// <summary>
    /// A route between two different airports.
    /// </summary>
    public class Route
    {
        /// <summary>
        /// Gets the departure airport.
        /// </summary>
        public Airport Origin { get; }
        /// <summary>
        /// Gets the arrival airport.
        /// </summary>
        public Airport Destination { get; }
        /// <summary>
        /// Gets the route length, in kilometres.
        /// </summary>
        public double DistanceKm { get; }

        /// <summary>
        /// Initializes a new route.
        /// </summary>
        /// <param name="origin">The departure airport.</param>
        /// <param name="destination">The arrival airport.</param>
        /// <param name="distanceKm">The route length, in kilometres.</param>
        /// <exception cref="InvalidFlightDataException">Both airports are the same or the distance is not positive.</exception>
        public Route(Airport origin, Airport destination, double distanceKm)
        {
            if (origin.Code == destination.Code)
                throw new InvalidFlightDataException("Пункты отправления и назначения совпадают.");
            if (distanceKm <= 0) throw new InvalidFlightDataException("Расстояние должно быть > 0.");
            Origin = origin; Destination = destination; DistanceKm = distanceKm;
        }

        /// <summary>
        /// Gets the route code.
        /// </summary>
        /// <returns>The origin and destination IATA codes separated by a hyphen, for example <c>AMS-LHR</c>.</returns>
        public string GetCode() => $"{Origin.Code}-{Destination.Code}";
        /// <summary>
        /// Determines whether the route crosses a country border.
        /// </summary>
        /// <returns><c>true</c> if the two airports are in different countries.</returns>
        public bool IsInternational() => Origin.Country != Destination.Country;
        /// <summary>
        /// Estimates the flight duration at a cruise speed of 800 km/h plus 30 minutes for take-off and landing.
        /// </summary>
        /// <returns>The estimated duration.</returns>
        public TimeSpan EstimateDuration() => TimeSpan.FromHours(DistanceKm / 800.0 + 0.5);
    }

}
