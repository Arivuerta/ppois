
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// The flight crew: two pilots and a number of flight attendants.
    /// </summary>
    public class Crew
    {
        /// <summary>
        /// Gets the captain.
        /// </summary>
        public Pilot Captain { get; }
        /// <summary>
        /// Gets the first officer.
        /// </summary>
        public Pilot FirstOfficer { get; }
        /// <summary>
        /// Gets the flight attendants.
        /// </summary>
        public List<FlightAttendant> Attendants { get; } = new List<FlightAttendant>();

        /// <summary>
        /// Initializes a new crew without attendants.
        /// </summary>
        /// <param name="captain">The pilot in command.</param>
        /// <param name="firstOfficer">The second pilot (co-pilot).</param>
        public Crew(Pilot captain, Pilot firstOfficer)
        {
            Captain = captain; FirstOfficer = firstOfficer;
        }

        /// <summary>
        /// Adds a flight attendant.
        /// </summary>
        /// <param name="a">The attendant to add.</param>
        /// <exception cref="ExpiredDocumentException">The medical certificate has expired.</exception>
        public void AddAttendant(FlightAttendant a)
        {
            if (!a.HasValidMedical())
                throw new ExpiredDocumentException($"Медсправка {a.GetFullName()} просрочена.");
            Attendants.Add(a);
        }

        /// <summary>
        /// Gets the crew size.
        /// </summary>
        /// <returns>Two pilots plus the number of attendants.</returns>
        public int GetSize() => 2 + Attendants.Count;
        /// <summary>
        /// Determines whether there are enough attendants.
        /// </summary>
        /// <param name="minAttendants">The minimum number of flight attendants required.</param>
        /// <returns><c>true</c> if the attendant count reaches the minimum.</returns>
        public bool IsComplete(int minAttendants) => Attendants.Count >= minAttendants;

        /// <summary>
        /// Checks that the crew may operate an aircraft type: valid licences, type ratings and at least one attendant per 50 seats.
        /// </summary>
        /// <param name="type">The aircraft type.</param>
        /// <exception cref="InvalidLicenseException">A licence has expired.</exception>
        /// <exception cref="PilotNotQualifiedException">A pilot is not rated for the type.</exception>
        /// <exception cref="CrewShortageException">There are too few attendants.</exception>
        public void ValidateFor(AircraftType type)
        {
            foreach (var p in new[] { Captain, FirstOfficer })
            {
                if (!p.License.IsValid())
                    throw new InvalidLicenseException($"Лицензия {p.GetFullName()} просрочена.");
                if (!p.CanFly(type))
                    throw new PilotNotQualifiedException($"{p.GetFullName()} не допущен к {type.Name}.");
            }
            if (!IsComplete(Math.Max(1, type.SeatCapacity / 50)))
                throw new CrewShortageException("Недостаточно бортпроводников.");
        }
    }

}
