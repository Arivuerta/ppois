using AirlineSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2
{
    /// <summary>
    /// Application state of the demo program. It keeps the airline, the airports and other data that the console menu works with, and can fill them with sample data.
    /// </summary>
    public class AirlineApp
    {
        /// <summary>
        /// Gets the airline.
        /// </summary>
        public Airline Airline { get; } = new Airline("Sky Dutch Airlines", "SD");
        /// <summary>
        /// Gets the airports by IATA code.
        /// </summary>
        public Dictionary<string, Airport> Airports { get; } = new Dictionary<string, Airport>();
        /// <summary>
        /// Gets the weather reports by airport code.
        /// </summary>
        public Dictionary<string, WeatherReport> Weather { get; } = new Dictionary<string, WeatherReport>();
        /// <summary>
        /// Gets the registered passengers.
        /// </summary>
        public List<Passenger> Passengers { get; } = new List<Passenger>();
        /// <summary>
        /// Gets the loyalty account of every passenger.
        /// </summary>
        public Dictionary<Passenger, LoyaltyAccount> Loyalty { get; } = new Dictionary<Passenger, LoyaltyAccount>();
        /// <summary>
        /// Gets the insurance policies sold.
        /// </summary>
        public List<Insurance> Insurances { get; } = new List<Insurance>();
        /// <summary>
        /// Gets the maintenance records.
        /// </summary>
        public List<MaintenanceRecord> Maintenance { get; } = new List<MaintenanceRecord>();
        /// <summary>
        /// Gets the notifications that have been created.
        /// </summary>
        public List<Notification> Notifications { get; } = new List<Notification>();
        /// <summary>
        /// Gets the registered baggage.
        /// </summary>
        public List<Baggage> Baggage { get; } = new List<Baggage>();
        /// <summary>
        /// Gets the card that paid each booking.
        /// </summary>
        public Dictionary<Booking, PaymentCard> Cards { get; } = new Dictionary<Booking, PaymentCard>();
        /// <summary>
        /// Gets the promo codes by code.
        /// </summary>
        public Dictionary<string, Discount> Promo { get; } = new Dictionary<string, Discount>();
        /// <summary>
        /// Gets the passenger manifests by flight number.
        /// </summary>
        public Dictionary<string, PassengerManifest> Manifests { get; } = new Dictionary<string, PassengerManifest>();
        /// <summary>
        /// Gets the card that is used for demonstration payments.
        /// </summary>
        public PaymentCard DemoCard { get; private set; }
        /// <summary>
        /// Gets the baggage policy of the airline.
        /// </summary>
        public BaggagePolicy BaggagePolicy { get; } = new BaggagePolicy(23, 15m, 45);
        /// <summary>
        /// Gets the dispatcher.
        /// </summary>
        public Dispatcher Dispatcher { get; private set; }
        /// <summary>
        /// Gets the air traffic controller.
        /// </summary>
        public AirTrafficController Atc { get; private set; }
        /// <summary>
        /// Gets the mechanic.
        /// </summary>
        public Mechanic Mechanic { get; private set; }
        /// <summary>
        /// Gets the security officer.
        /// </summary>
        public SecurityOfficer Security { get; private set; }
        /// <summary>
        /// Gets the customer support desk.
        /// </summary>
        public CustomerSupport Support { get; private set; }
        /// <summary>
        /// Gets the check-in desk.
        /// </summary>
        public CheckInDesk Desk { get; private set; }
        /// <summary>
        /// Gets the hangar.
        /// </summary>
        public Hangar Hangar { get; private set; }
        /// <summary>
        /// Gets the flight statistics.
        /// </summary>
        public FlightStatistics Stats { get; private set; }
        private int _cargoId = 1;

        /// <summary>
        /// Fills the application with sample airports, aircraft, employees, flights, passengers and promo codes.
        /// </summary>
        public void Seed()
        {
            // Аэропорты
            var ams = NewAirport("AMS", "Schiphol", "Amsterdam", "Netherlands", 3800);
            var lhr = NewAirport("LHR", "Heathrow", "London", "UK", 3900);
            var jfk = NewAirport("JFK", "John F. Kennedy", "New York", "USA", 4400);
            var terminal = new Terminal("T1", true);
            for (int i = 1; i <= 3; i++) terminal.AddGate(new Gate($"G{i}"));
            ams.AddTerminal(terminal);
            foreach (var a in Airports.Values) Weather[a.Code] = new WeatherReport(a, 12, 20, 10);
            Desk = new CheckInDesk(1, ams); Desk.Open();

            // Самолёты
            var a320 = new AircraftType("A320", "Airbus", 6100, 180, 78000);
            var b777 = new AircraftType("B777", "Boeing", 13000, 300, 347000);
            Airline.AddAircraft(new Aircraft("PH-AAA", a320, 2015, 2, 24000));
            Airline.AddAircraft(new Aircraft("PH-BBB", b777, 2012, 2, 200000));
            Hangar = new Hangar("Hangar-1", 1);
            Airline.Hangars.Add(Hangar);

            // Сотрудники
            int id = 1;
            var born = new DateTime(1985, 5, 20);
            var hired = DateTime.Today.AddYears(-8);
            Airline.HireEmployee(new Pilot(id++, "Jan", "Visser", born, 9000, hired,
                new PilotLicense("L-001", DateTime.Today.AddYears(2), "A320", "EASA"), 5200));
            Airline.HireEmployee(new Pilot(id++, "Piet", "Bakker", born, 7000, hired,
                new PilotLicense("L-002", DateTime.Today.AddYears(1), "A320", "EASA"), 1800));
            Airline.HireEmployee(new Pilot(id++, "Erik", "Smit", born, 9500, hired,
                new PilotLicense("L-003", DateTime.Today.AddYears(2), "B777", "EASA"), 6100));
            Airline.HireEmployee(new Pilot(id++, "Lars", "Mulder", born, 7200, hired,
                new PilotLicense("L-004", DateTime.Today.AddYears(1), "B777", "EASA"), 2500));
            string[] names = { "Eva", "Sanne", "Lotte", "Fleur", "Mila", "Noor" };
            foreach (var n in names)
            {
                var fa = new FlightAttendant(id++, n, "Jansen", new DateTime(1994, 3, 3), 3500, hired.AddYears(4),
                                             DateTime.Today.AddYears(1));
                fa.AddLanguage("EN"); fa.AddLanguage("NL");
                Airline.HireEmployee(fa);
            }
            Dispatcher = new Dispatcher(id++, "Kees", "de Boer", born, 5000, hired, "Desk-A");
            Atc = new AirTrafficController(id++, "Anke", "Dekker", born, 6000, hired, "AMS-Tower", "Tower-1");
            Mechanic = new Mechanic(id++, "Rob", "Hendriks", born, 4500, hired, "Engines", 3);
            Security = new SecurityOfficer(id++, "Tim", "Peters", born, 3300, hired, "B-77", "Checkpoint-2");
            var ground = new GroundStaff(id++, "Sem", "Kok", born, 2800, hired, ams, "Day");
            var agent = new Employee(id++, "Mark", "van Dijk", born, "Support Agent", 3000, hired);
            foreach (Employee e in new Employee[] { Dispatcher, Atc, Mechanic, Security, ground, agent }) Airline.HireEmployee(e);
            Support = new CustomerSupport(agent);

            // Рейсы
            var r1 = new Route(ams, lhr, 370);
            var r2 = new Route(ams, jfk, 5860);
            Airline.CreateFlight("SD101", r1, Airline.Fleet[0], DateTime.Now.AddDays(5));
            Airline.CreateFlight("SD202", r2, Airline.Fleet[1], DateTime.Now.AddDays(6));
            Stats = new FlightStatistics(Airline.Schedule, Airline.Bookings);

            // Пассажиры, карта, промокод
            var anna = new Passenger(1, "Anna", "de Vries", new DateTime(1995, 7, 1), "NL1234567", "Netherlands", "anna@mail.nl");
            anna.AddVisa(new Visa("V-1", "UK", DateTime.Today.AddYears(1)));
            anna.AddVisa(new Visa("V-2", "USA", DateTime.Today.AddYears(1)));
            var ivan = new Passenger(2, "Ivan", "Petrov", new DateTime(1990, 1, 15), "RU9876543", "Russia", "ivan@mail.ru");
            foreach (var p in new[] { anna, ivan }) RegisterPassenger(p);
            DemoCard = new PaymentCard("4111111111111111", "ANNA DE VRIES", DateTime.Today.AddYears(2), 5000m);
            Promo["WELCOME10"] = new Discount("WELCOME10", 10, DateTime.Today.AddMonths(6));
            Promo["OLD50"] = new Discount("OLD50", 50, DateTime.Today.AddDays(-1));
        }

        private Airport NewAirport(string code, string name, string city, string country, double runway)
        {
            var a = new Airport(code, name, city, country);
            a.AddRunway(new Runway("09/27", runway));
            Airports[code] = a;
            return a;
        }

        /// <summary>
        /// Registers a passenger and opens a loyalty account for them.
        /// </summary>
        /// <param name="p">The passenger to register.</param>
        public void RegisterPassenger(Passenger p)
        {
            Passengers.Add(p);
            Loyalty[p] = new LoyaltyAccount(p);
        }

        /// <summary>
        /// Creates a cargo item with the next free identifier.
        /// </summary>
        /// <param name="kg">The weight, in kilograms.</param>
        /// <param name="description">The description text.</param>
        /// <param name="hazardous"><c>true</c> if the cargo is classified as hazardous.</param>
        /// <returns>The new cargo item.</returns>
        public Cargo NewCargo(double kg, string description, bool hazardous) => new Cargo(_cargoId++, kg, description, hazardous);
    }

}
