using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// A scheduled flight. It combines a route, an aircraft, a crew and a gate, sells tickets, carries cargo and moves through the flight life cycle from scheduled to arrived.
    /// </summary>
    public class Flight
    {
        /// <summary>
        /// Gets the flight number, for example <c>SD101</c>.
        /// </summary>
        public string Number { get; }
        /// <summary>
        /// Gets the route flown.
        /// </summary>
        public Route Route { get; }
        /// <summary>
        /// Gets the aircraft assigned to the flight.
        /// </summary>
        public Aircraft Aircraft { get; }
        /// <summary>
        /// Gets the scheduled departure time.
        /// </summary>
        public DateTime DepartureTime { get; private set; }
        /// <summary>
        /// Gets the current status of the flight.
        /// </summary>
        public FlightStatus Status { get; private set; } = FlightStatus.Scheduled;
        /// <summary>
        /// Gets the assigned crew, or <c>null</c> if no crew is assigned.
        /// </summary>
        public Crew Crew { get; private set; }
        /// <summary>
        /// Gets the assigned gate, or <c>null</c>.
        /// </summary>
        public Gate Gate { get; private set; }
        /// <summary>
        /// Gets the tickets sold for the flight.
        /// </summary>
        public List<Ticket> Tickets { get; } = new List<Ticket>();
        /// <summary>
        /// Gets the cargo loaded onto the flight.
        /// </summary>
        public List<Cargo> CargoList { get; } = new List<Cargo>();

        /// <summary>
        /// Initializes a new scheduled flight.
        /// </summary>
        /// <param name="number">The flight number. Must not be empty.</param>
        /// <param name="route">The route to fly.</param>
        /// <param name="aircraft">The aircraft to use.</param>
        /// <param name="departure">The scheduled departure date and time.</param>
        /// <exception cref="InvalidFlightDataException">The number is empty or the departure is more than one day in the past.</exception>
        /// <exception cref="AircraftNotAvailableException">The aircraft cannot serve the route.</exception>
        public Flight(string number, Route route, Aircraft aircraft, DateTime departure)
        {
            if (string.IsNullOrWhiteSpace(number)) throw new InvalidFlightDataException("Пустой номер рейса.");
            if (departure < DateTime.Now.AddDays(-1)) throw new InvalidFlightDataException("Рейс в прошлом.");
            if (!aircraft.CanServe(route))
                throw new AircraftNotAvailableException($"{aircraft.RegistrationNumber} не подходит для {route.GetCode()}.");
            Number = number; Route = route; Aircraft = aircraft; DepartureTime = departure;
        }

        /// <summary>
        /// Calculates the estimated arrival time.
        /// </summary>
        /// <returns>The departure time plus the estimated route duration.</returns>
        public DateTime GetArrivalTime() => DepartureTime + Route.EstimateDuration();
        /// <summary>
        /// Determines whether every seat is sold.
        /// </summary>
        /// <returns><c>true</c> if the number of active tickets reaches the seat capacity.</returns>
        public bool IsFull() => Tickets.Count(t => t.Status != TicketStatus.Cancelled) >= Aircraft.Type.SeatCapacity;
        /// <summary>
        /// Calculates the seat load factor.
        /// </summary>
        /// <returns>The percentage of seats that are sold (cancelled tickets are not counted).</returns>
        public double GetLoadFactor() =>
            Tickets.Count(t => t.Status != TicketStatus.Cancelled) * 100.0 / Aircraft.Type.SeatCapacity;

        /// <summary>
        /// Validates and assigns a crew.
        /// </summary>
        /// <param name="crew">The crew to assign.</param>
        /// <exception cref="InvalidLicenseException">A pilot's licence has expired.</exception>
        /// <exception cref="PilotNotQualifiedException">A pilot is not rated for the aircraft type.</exception>
        /// <exception cref="CrewShortageException">There are too few flight attendants.</exception>
        public void AssignCrew(Crew crew)
        {
            crew.ValidateFor(Aircraft.Type);
            Crew = crew;
        }

        /// <summary>
        /// Assigns a gate and marks it as occupied.
        /// </summary>
        /// <param name="gate">The gate to assign.</param>
        /// <exception cref="AircraftNotAvailableException">The gate is already occupied.</exception>
        public void AssignGate(Gate gate)
        {
            gate.Occupy(this);
            Gate = gate;
        }

        /// <summary>
        /// Adds a ticket to the flight.
        /// </summary>
        /// <param name="t">The ticket to add.</param>
        /// <exception cref="InvalidFlightDataException">The flight has been cancelled or has already departed.</exception>
        public void AddTicket(Ticket t)
        {
            if (Status == FlightStatus.Cancelled || Status == FlightStatus.Departed)
                throw new InvalidFlightDataException("Рейс недоступен для продажи билетов.");
            Tickets.Add(t);
        }

        /// <summary>
        /// Loads a cargo item if the total cargo weight stays within the payload limit of the aircraft.
        /// </summary>
        /// <param name="cargo">The cargo to load.</param>
        /// <exception cref="BaggageOverweightException">The total cargo weight would exceed the payload limit.</exception>
        public void LoadCargo(Cargo cargo)
        {
            double total = CargoList.Sum(c => c.WeightKg) + cargo.WeightKg;
            if (!Aircraft.Type.CanCarry(total))
                throw new BaggageOverweightException("Превышена грузоподъёмность самолёта.");
            CargoList.Add(cargo);
        }

        /// <summary>
        /// Postpones the flight and sets its status to delayed.
        /// </summary>
        /// <param name="minutes">The delay, in minutes. Must be greater than zero.</param>
        /// <param name="reason">The reason given.</param>
        /// <returns>A <see cref="FlightDelay"/> that describes the delay.</returns>
        /// <exception cref="InvalidFlightDataException">The delay is zero or negative.</exception>
        public FlightDelay Delay(int minutes, string reason)
        {
            if (minutes <= 0) throw new InvalidFlightDataException("Задержка должна быть > 0 минут.");
            DepartureTime = DepartureTime.AddMinutes(minutes);
            Status = FlightStatus.Delayed;
            return new FlightDelay(this, minutes, reason);
        }

        /// <summary>
        /// Moves the flight to a new time and sets its status back to scheduled.
        /// </summary>
        /// <param name="newTime">The new departure date and time.</param>
        /// <exception cref="InvalidFlightDataException">The flight has already departed.</exception>
        public void Reschedule(DateTime newTime)
        {
            if (Status == FlightStatus.Departed) throw new InvalidFlightDataException("Рейс уже вылетел.");
            DepartureTime = newTime; Status = FlightStatus.Scheduled;
        }

        /// <summary>
        /// Cancels the flight, cancels all of its tickets and frees the gate.
        /// </summary>
        public void Cancel()
        {
            Status = FlightStatus.Cancelled;
            foreach (var t in Tickets) t.Cancel();
            Gate?.Free();
        }

        /// <summary>
        /// Sets the status to boarding.
        /// </summary>
        /// <exception cref="InvalidFlightDataException">The flight has been cancelled.</exception>
        public void StartBoarding()
        {
            if (Status == FlightStatus.Cancelled) throw new InvalidFlightDataException("Рейс отменён.");
            Status = FlightStatus.Boarding;
        }

        /// <summary>
        /// Departs the flight: checks the crew, airworthiness and fuel, burns the fuel and frees the gate.
        /// </summary>
        /// <exception cref="CrewShortageException">No crew is assigned.</exception>
        /// <exception cref="AircraftNotAvailableException">The aircraft is not airworthy or has too little fuel.</exception>
        public void Depart()
        {
            if (Crew == null) throw new CrewShortageException("Экипаж не назначен.");
            if (!Aircraft.IsAirworthy()) throw new AircraftNotAvailableException("Самолёт не годен к полёту.");
            if (!Aircraft.Fuel.HasEnoughFor(Route.DistanceKm)) throw new AircraftNotAvailableException("Недостаточно топлива.");
            Aircraft.Fuel.Consume(Route.DistanceKm);
            Status = FlightStatus.Departed;
            Gate?.Free();
        }

        /// <summary>
        /// Completes the flight: adds the flight time to the engines and pilots and marks checked-in tickets as used.
        /// </summary>
        /// <exception cref="InvalidFlightDataException">The flight has not departed.</exception>
        public void Arrive()
        {
            if (Status != FlightStatus.Departed) throw new InvalidFlightDataException("Рейс ещё не вылетал.");
            double hours = Route.EstimateDuration().TotalHours;
            Aircraft.RegisterFlightTime(hours);
            Crew.Captain.AddFlightHours(hours);
            Crew.FirstOfficer.AddFlightHours(hours);
            foreach (var t in Tickets.Where(t => t.Status == TicketStatus.CheckedIn)) t.MarkUsed();
            Status = FlightStatus.Arrived;
        }
    }
}
