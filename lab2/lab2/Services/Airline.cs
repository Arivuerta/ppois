
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// The airline aggregate. It owns the fleet, the employees, the bookings, the schedule and the hangars and implements the main business operations such as creating flights and booking tickets.
    /// </summary>
    public class Airline
    {
        /// <summary>
        /// Gets the airline name.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets the two-letter airline code that prefixes ticket numbers.
        /// </summary>
        public string Code { get; }
        /// <summary>
        /// Gets the aircraft of the airline.
        /// </summary>
        public List<Aircraft> Fleet { get; } = new List<Aircraft>();
        /// <summary>
        /// Gets the employees of the airline.
        /// </summary>
        public List<Employee> Employees { get; } = new List<Employee>();
        /// <summary>
        /// Gets the known passengers.
        /// </summary>
        public List<Passenger> Passengers { get; } = new List<Passenger>();
        /// <summary>
        /// Gets all bookings.
        /// </summary>
        public List<Booking> Bookings { get; } = new List<Booking>();
        /// <summary>
        /// Gets the flight schedule.
        /// </summary>
        public FlightSchedule Schedule { get; } = new FlightSchedule();
        /// <summary>
        /// Gets the hangars.
        /// </summary>
        public List<Hangar> Hangars { get; } = new List<Hangar>();
        private int _ticketCounter = 1000;

        /// <summary>
        /// Initializes a new airline without fleet and staff.
        /// </summary>
        /// <param name="name">The airline name.</param>
        /// <param name="code">The airline code.</param>
        public Airline(string name, string code) { Name = name; Code = code; }

        /// <summary>
        /// Adds an aircraft to the fleet.
        /// </summary>
        /// <param name="a">The aircraft to add.</param>
        public void AddAircraft(Aircraft a) => Fleet.Add(a);
        /// <summary>
        /// Hires an employee.
        /// </summary>
        /// <param name="e">The employee to hire.</param>
        public void HireEmployee(Employee e) => Employees.Add(e);
        /// <summary>
        /// Dismisses the employee with the given identifier.
        /// </summary>
        /// <param name="id">The employee identifier.</param>
        /// <returns><c>true</c> if an employee was removed.</returns>
        public bool FireEmployee(int id) => Employees.RemoveAll(e => e.Id == id) > 0;
        /// <summary>
        /// Calculates the monthly payroll: every salary plus one twelfth of the yearly bonus.
        /// </summary>
        /// <returns>The payroll total in euros.</returns>
        public decimal PayPayroll() => Employees.Sum(e => e.Salary + e.CalculateBonus() / 12m);

        /// <summary>
        /// Finds the pilots who are qualified for an aircraft type.
        /// </summary>
        /// <param name="type">The aircraft type.</param>
        /// <returns>The list of qualified pilots.</returns>
        public List<Pilot> FindPilots(AircraftType type) =>
            Employees.OfType<Pilot>().Where(p => p.CanFly(type)).ToList();

        /// <summary>
        /// Finds aircraft that are airworthy, able to serve the route and free during the flight.
        /// </summary>
        /// <param name="route">The route.</param>
        /// <param name="start">The planned departure time.</param>
        /// <returns>The list of suitable aircraft.</returns>
        public List<Aircraft> FindAvailableAircraft(Route route, DateTime start) =>
            Fleet.Where(a => a.IsAirworthy() && a.CanServe(route) &&
                             !Schedule.HasConflict(a, start, start + route.EstimateDuration())).ToList();

        /// <summary>
        /// Creates a flight and adds it to the schedule.
        /// </summary>
        /// <param name="number">The flight number.</param>
        /// <param name="route">The route concerned.</param>
        /// <param name="aircraft">The aircraft concerned.</param>
        /// <param name="departure">The scheduled departure date and time.</param>
        /// <returns>The new flight.</returns>
        /// <exception cref="InvalidFlightDataException">The flight data is invalid.</exception>
        /// <exception cref="AircraftNotAvailableException">The aircraft is unsuitable or already busy.</exception>
        public Flight CreateFlight(string number, Route route, Aircraft aircraft, DateTime departure)
        {
            var f = new Flight(number, route, aircraft, departure);
            Schedule.AddFlight(f);
            return f;
        }

        /// <summary>
        /// Finds a booking by code.
        /// </summary>
        /// <param name="code">The booking code.</param>
        /// <returns>The booking.</returns>
        /// <exception cref="BookingNotFoundException">No such booking exists.</exception>
        public Booking FindBooking(string code) =>
            Bookings.FirstOrDefault(b => b.Code == code) ?? throw new BookingNotFoundException($"Бронь {code} не найдена.");

        /// <summary>
        /// Books a ticket: checks availability and visa rules, reserves a seat, prices the ticket and creates a booking.
        /// </summary>
        /// <param name="p">The passenger.</param>
        /// <param name="flight">The flight concerned.</param>
        /// <param name="cls">The requested cabin class.</param>
        /// <param name="seatLabel">The desired seat label, or <c>null</c> to pick any free seat of the requested class.</param>
        /// <returns>The new booking.</returns>
        /// <exception cref="SeatUnavailableException">The flight is full or the seat is not available.</exception>
        /// <exception cref="ExpiredDocumentException">A visa is required but not valid.</exception>
        public Booking BookTicket(Passenger p, Flight flight, SeatClass cls, string seatLabel = null)
        {
            if (flight.IsFull()) throw new SeatUnavailableException("Рейс заполнен.");
            if (flight.Route.IsInternational() &&
                !p.HasValidVisa(flight.Route.Destination.Country, flight.GetArrivalTime()))
                throw new ExpiredDocumentException($"Нет действующей визы в {flight.Route.Destination.Country}.");

            var seat = seatLabel == null ? flight.Aircraft.Seats.FindFreeSeat(cls) : flight.Aircraft.Seats.ReserveSeat(seatLabel);
            if (seatLabel == null) seat.Reserve();

            var price = Fare.ForRoute(flight.Route, seat.Class).Calculate();
            var ticket = new Ticket($"{Code}{++_ticketCounter}", p, flight, seat, price);
            flight.AddTicket(ticket);

            var booking = new Booking($"BK{_ticketCounter}", p);
            booking.AddTicket(ticket);
            Bookings.Add(booking);
            if (!Passengers.Contains(p)) Passengers.Add(p);
            return booking;
        }
    }

}
