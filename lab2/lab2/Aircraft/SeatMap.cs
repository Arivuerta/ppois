using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// The seating layout of an aircraft. Seats are generated from the seat capacity of the aircraft type; rows 1-2 are first class, rows 3-5 business class and the remaining rows economy class.
    /// </summary>
    public class SeatMap
    {
        /// <summary>
        /// Gets the aircraft that this map belongs to.
        /// </summary>
        public Aircraft Aircraft { get; }
        /// <summary>
        /// Gets all seats of the aircraft.
        /// </summary>
        public List<Seat> Seats { get; } = new List<Seat>();
        /// <summary>
        /// Gets the number of seat columns per row.
        /// </summary>
        public int Columns { get; }

        /// <summary>
        /// Initializes the seat map and generates the seats.
        /// </summary>
        /// <param name="aircraft">The aircraft that owns the map.</param>
        /// <param name="columns">The number of seat columns per row.</param>
        public SeatMap(Aircraft aircraft, int columns = 6)
        {
            Aircraft = aircraft; Columns = columns;
            int rows = (int)Math.Ceiling(aircraft.Type.SeatCapacity / (double)columns);
            for (int r = 1; r <= rows; r++)
                for (int c = 0; c < columns && Seats.Count < aircraft.Type.SeatCapacity; c++)
                {
                    var cls = r <= 2 ? SeatClass.First : r <= 5 ? SeatClass.Business : SeatClass.Economy;
                    Seats.Add(new Seat(r, (char)('A' + c), cls));
                }
        }

        /// <summary>
        /// Finds the first unoccupied seat of the given class without reserving it.
        /// </summary>
        /// <param name="cls">The cabin class to search in.</param>
        /// <returns>The first free seat of the requested class.</returns>
        /// <exception cref="SeatUnavailableException">No free seat of that class exists.</exception>
        public Seat FindFreeSeat(SeatClass cls)
        {
            var s = Seats.FirstOrDefault(x => x.Class == cls && !x.IsOccupied);
            if (s == null) throw new SeatUnavailableException($"Нет свободных мест класса {cls}.");
            return s;
        }

        /// <summary>
        /// Finds a seat by its label and reserves it.
        /// </summary>
        /// <param name="label">The seat label, for example <c>12A</c>. The comparison is case-insensitive.</param>
        /// <returns>The reserved seat.</returns>
        /// <exception cref="SeatUnavailableException">The seat does not exist or is already occupied.</exception>
        public Seat ReserveSeat(string label)
        {
            var s = Seats.FirstOrDefault(x => x.Label().Equals(label, StringComparison.OrdinalIgnoreCase))
                    ?? throw new SeatUnavailableException($"Места {label} не существует.");
            s.Reserve();
            return s;
        }

        /// <summary>
        /// Frees the seat with the given label. Unknown labels are ignored.
        /// </summary>
        /// <param name="label">The seat label, for example <c>12A</c>. The comparison is case-insensitive.</param>
        public void ReleaseSeat(string label) =>
            Seats.FirstOrDefault(x => x.Label().Equals(label, StringComparison.OrdinalIgnoreCase))?.Release();

        /// <summary>
        /// Counts the unoccupied seats.
        /// </summary>
        /// <returns>The number of free seats.</returns>
        public int CountFree() => Seats.Count(s => !s.IsOccupied);
        /// <summary>
        /// Calculates the share of occupied seats.
        /// </summary>
        /// <returns>A value from 0 to 100, or 0 if there are no seats.</returns>
        public double OccupancyPercent() => Seats.Count == 0 ? 0 : (Seats.Count - CountFree()) * 100.0 / Seats.Count;
    }
}
