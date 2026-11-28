using System;
using AirlineSystem;
using NUnit.Framework;

namespace lab2.Tests
{
    /// <summary>Интеграционные тесты: демо-данные приложения + сквозной сценарий бронирования.</summary>
    [TestFixture]
    public class AirlineAppTests
    {
        private AirlineApp _app;

        [SetUp]
        public void SetUp()
        {
            _app = new AirlineApp();
            _app.Seed();
        }

        [Test]
        public void Seed_CreatesAirportsFleetAndFlights()
        {
            Assert.AreEqual(3, _app.Airports.Count);
            Assert.IsTrue(_app.Airports.ContainsKey("AMS"));
            Assert.AreEqual(2, _app.Airline.Fleet.Count);
            Assert.AreEqual(2, _app.Airline.Schedule.Flights.Count);
            Assert.DoesNotThrow(() => _app.Airline.Schedule.FindByNumber("SD101"));
            Assert.DoesNotThrow(() => _app.Airline.Schedule.FindByNumber("SD202"));
        }

        [Test]
        public void Seed_CreatesStaffAndServices()
        {
            Assert.IsNotNull(_app.Dispatcher);
            Assert.IsNotNull(_app.Atc);
            Assert.IsNotNull(_app.Mechanic);
            Assert.IsNotNull(_app.Security);
            Assert.IsNotNull(_app.Support);
            Assert.IsNotNull(_app.Stats);
            Assert.IsTrue(_app.Desk.IsOpen);
            Assert.AreEqual(1, _app.Hangar.Capacity);
            Assert.AreEqual(4, _app.Airline.Employees.FindAll(e => e is Pilot).Count);
        }

        [Test]
        public void Seed_RegistersPassengersWithLoyaltyAccounts()
        {
            Assert.AreEqual(2, _app.Passengers.Count);
            Assert.AreEqual(2, _app.Loyalty.Count);
            foreach (var p in _app.Passengers) Assert.IsTrue(_app.Loyalty.ContainsKey(p));
        }

        [Test]
        public void Seed_DemoCardIsValidAndFunded()
        {
            Assert.IsTrue(_app.DemoCard.ValidateNumber());
            Assert.IsFalse(_app.DemoCard.IsExpired());
            Assert.AreEqual(5000m, _app.DemoCard.Balance);
        }

        [Test]
        public void Seed_PromoCodes_OneActiveOneExpired()
        {
            Assert.IsTrue(_app.Promo["WELCOME10"].IsValid(DateTime.Today));
            Assert.IsFalse(_app.Promo["OLD50"].IsValid(DateTime.Today));
        }

        [Test]
        public void Seed_AllDemoFlightsHaveAvailableCrew()
        {
            // A320 (рейс SD101): два пилота с допуском и шесть бортпроводников
            var a320 = _app.Airline.Fleet[0].Type;
            Assert.AreEqual(2, _app.Airline.FindPilots(a320).Count);
        }

        [Test]
        public void RegisterPassenger_AddsPassengerAndLoyaltyAccount()
        {
            var p = TestData.MakePassenger(77);
            _app.RegisterPassenger(p);
            Assert.Contains(p, _app.Passengers);
            Assert.AreSame(p, _app.Loyalty[p].Owner);
        }

        [Test]
        public void NewCargo_AssignsIncreasingIds()
        {
            var c1 = _app.NewCargo(10, "Boxes", false);
            var c2 = _app.NewCargo(20, "Chem", true);
            Assert.AreEqual(c1.Id + 1, c2.Id);
            Assert.IsTrue(c2.IsHazardous);
        }

        [Test]
        public void Scenario_BookAndPay_WithDemoData()
        {
            var anna = _app.Passengers[0];
            var flight = _app.Airline.Schedule.FindByNumber("SD101");

            var booking = _app.Airline.BookTicket(anna, flight, SeatClass.Economy);
            booking.Pay(_app.DemoCard);

            Assert.IsTrue(booking.Payment.IsPaid());
            Assert.AreEqual(5000m - 37.30m, _app.DemoCard.Balance);
            Assert.AreEqual(37.30m, _app.Stats.TotalRevenue());
        }

        [Test]
        public void Scenario_PassengerWithoutVisa_CannotBookInternationalFlight()
        {
            var ivan = _app.Passengers[1]; // россиянин без виз
            var flight = _app.Airline.Schedule.FindByNumber("SD101");
            Assert.Throws<ExpiredDocumentException>(() =>
                _app.Airline.BookTicket(ivan, flight, SeatClass.Economy));
        }

        [Test]
        public void Scenario_FullFlightLifecycle()
        {
            var flight = _app.Airline.Schedule.FindByNumber("SD101");
            var anna = _app.Passengers[0];
            var booking = _app.Airline.BookTicket(anna, flight, SeatClass.Economy);

            // экипаж: два пилота A320 + три бортпроводника
            var pilots = _app.Airline.FindPilots(flight.Aircraft.Type);
            var crew = new Crew(pilots[0], pilots[1]);
            foreach (var fa in _app.Airline.Employees.FindAll(e => e is FlightAttendant).GetRange(0, 3))
                crew.AddAttendant((FlightAttendant)fa);
            flight.AssignCrew(crew);

            var gate = _app.Airports["AMS"].FindFreeGate();
            flight.AssignGate(gate);
            var pass = _app.Desk.CheckIn(booking.Tickets[0], gate);
            Assert.AreEqual(TicketStatus.CheckedIn, booking.Tickets[0].Status);
            Assert.AreSame(gate, pass.Gate);

            flight.StartBoarding();
            flight.Depart();
            Assert.IsFalse(gate.IsOccupied);
            flight.Arrive();

            Assert.AreEqual(FlightStatus.Arrived, flight.Status);
            Assert.AreEqual(TicketStatus.Used, booking.Tickets[0].Status);
        }
    }
}
