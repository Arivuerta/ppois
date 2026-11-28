using System;
using AirlineSystem;

namespace lab2.Tests
{
    /// <summary>Фабрики тестовых объектов, чтобы тесты не дублировали подготовку данных.</summary>
    internal static class TestData
    {
        public static AircraftType A320() => new AircraftType("A320", "Airbus", 6100, 180, 78000);

        /// <summary>Маленький самолёт на 2 места - удобно проверять переполнение.</summary>
        public static AircraftType Tiny() => new AircraftType("TINY", "Test", 1000, 2, 10000);

        public static Airport MakeAirport(string code = "AMS", string country = "Netherlands", double runwayM = 3800)
        {
            var a = new Airport(code, code + " Airport", code + " City", country);
            if (runwayM > 0) a.AddRunway(new Runway("09/27", runwayM));
            return a;
        }

        /// <summary>Международный маршрут AMS -> LHR (Netherlands -> UK).</summary>
        public static Route MakeRoute(double km = 370) =>
            new Route(MakeAirport("AMS", "Netherlands"), MakeAirport("LHR", "UK", 3900), km);

        /// <summary>Внутренний маршрут AMS -> RTM (обе в Netherlands).</summary>
        public static Route MakeDomesticRoute(double km = 60) =>
            new Route(MakeAirport("AMS", "Netherlands"), MakeAirport("RTM", "Netherlands", 2000), km);

        public static Aircraft MakeAircraft(string reg = "PH-AAA", AircraftType type = null,
                                            int engines = 2, double fuel = 24000) =>
            new Aircraft(reg, type ?? A320(), 2015, engines, fuel);

        public static Passenger MakePassenger(int id = 1, string nationality = "Netherlands", DateTime? birth = null) =>
            new Passenger(id, "Anna", "Test", birth ?? new DateTime(1995, 7, 1),
                          "AB" + id.ToString("D6"), nationality, "p" + id + "@mail.test");

        public static PilotLicense MakeLicense(string rating = "A320", int daysValid = 365) =>
            new PilotLicense("L-" + rating, DateTime.Today.AddDays(daysValid), rating, "EASA");

        public static Pilot MakePilot(int id = 1, string rating = "A320", int daysValid = 365, double hours = 5000) =>
            new Pilot(id, "Jan", "Visser", new DateTime(1985, 5, 20), 9000m,
                      DateTime.Today.AddYears(-5), MakeLicense(rating, daysValid), hours);

        public static FlightAttendant MakeAttendant(int id = 10, bool validMedical = true) =>
            new FlightAttendant(id, "Eva", "Jansen", new DateTime(1994, 3, 3), 3500m,
                                DateTime.Today.AddYears(-3),
                                validMedical ? DateTime.Today.AddYears(1) : DateTime.Today.AddDays(-1));

        public static Mechanic MakeMechanic(int level = 3) =>
            new Mechanic(50, "Rob", "Hendriks", new DateTime(1985, 5, 20), 4500m,
                         DateTime.Today.AddYears(-8), "Engines", level);

        /// <summary>Экипаж, подходящий для A320 (нужно 3 бортпроводника).</summary>
        public static Crew MakeCrew(string rating = "A320", int attendants = 3)
        {
            var crew = new Crew(MakePilot(1, rating), MakePilot(2, rating, 365, 1800));
            for (int i = 0; i < attendants; i++) crew.AddAttendant(MakeAttendant(10 + i));
            return crew;
        }

        public static Flight MakeFlight(DateTime? departure = null, Aircraft aircraft = null,
                                        Route route = null, string number = "SD101") =>
            new Flight(number, route ?? MakeRoute(), aircraft ?? MakeAircraft(),
                       departure ?? DateTime.Now.AddDays(5));

        /// <summary>Рейс с назначенным экипажем - готов к Depart().</summary>
        public static Flight MakeReadyFlight()
        {
            var f = MakeFlight();
            f.AssignCrew(MakeCrew());
            return f;
        }

        public static Ticket MakeTicket(Flight flight, decimal price = 100m, int row = 1, char letter = 'A',
                                        Passenger passenger = null, string number = "T1") =>
            new Ticket(number, passenger ?? MakePassenger(), flight, new Seat(row, letter, SeatClass.Economy), price);

        /// <summary>Номер 4111111111111111 проходит проверку Луна.</summary>
        public static PaymentCard MakeCard(decimal balance = 1000m, string number = "4111111111111111", int daysValid = 365) =>
            new PaymentCard(number, "TEST HOLDER", DateTime.Today.AddDays(daysValid), balance);
    }
}
