using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// A weather observation for an airport, used to decide whether a flight may take off.
    /// </summary>
    public class WeatherReport
    {
        /// <summary>
        /// Gets the airport to which the report applies.
        /// </summary>
        public Airport Airport { get; }
        /// <summary>
        /// Gets the air temperature, in degrees Celsius.
        /// </summary>
        public double TemperatureC { get; }
        /// <summary>
        /// Gets the wind speed, in kilometres per hour.
        /// </summary>
        public double WindSpeedKmh { get; }
        /// <summary>
        /// Gets the visibility, in kilometres.
        /// </summary>
        public double VisibilityKm { get; }

        /// <summary>
        /// Initializes a new weather report.
        /// </summary>
        /// <param name="airport">The airport concerned.</param>
        /// <param name="temp">The air temperature, in degrees Celsius.</param>
        /// <param name="wind">The wind speed, in kilometres per hour.</param>
        /// <param name="visibility">The visibility, in kilometres.</param>
        public WeatherReport(Airport airport, double temp, double wind, double visibility)
        {
            Airport = airport; TemperatureC = temp; WindSpeedKmh = wind; VisibilityKm = visibility;
        }

        /// <summary>
        /// Determines whether the weather permits take-off.
        /// </summary>
        /// <returns><c>true</c> if the wind is below 60 km/h and the visibility is at least 1.5 km.</returns>
        public bool IsSafeForTakeoff() => WindSpeedKmh < 60 && VisibilityKm >= 1.5;
        /// <summary>
        /// Determines whether a storm is in progress.
        /// </summary>
        /// <returns><c>true</c> if the wind speed is at least 90 km/h.</returns>
        public bool IsStorm() => WindSpeedKmh >= 90;
        /// <summary>
        /// Builds a one-line description of the report.
        /// </summary>
        /// <returns>A string with the airport code, temperature, wind speed and visibility.</returns>
        public string Describe() =>
            $"{Airport.Code}: {TemperatureC}°C, ветер {WindSpeedKmh} км/ч, видимость {VisibilityKm} км";
    }
}
