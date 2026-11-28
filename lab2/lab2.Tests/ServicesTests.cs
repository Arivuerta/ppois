using System;
using System.Linq;
using AirlineSystem;
using NUnit.Framework;

namespace lab2.Tests
{
    [TestFixture]
    public class AirlineTests
    {
        private Airline _airline;
        private Aircraft _aircraft;
        private Flight _flight;      // AMS -> LHR, международный
        private Passenger _passenger; // нидерландец с визой в UK

        [SetUp]
        public void SetUp()
        {
            _airline = new Airline("Sky Dutch", "SD");
            _aircraft = TestData.MakeAircraft("PH-AAA");
            _airline.AddAircraft(_aircraft);
            _flight = _airline.CreateFlight("SD101", TestData.MakeRoute(370), _aircraft, DateTime.Now.AddDays(5));
            _passenger = TestData.MakePassenger(1, "Netherlands");
            _passenger.AddVisa(new Visa("V-1", "UK", DateTime.Today.AddYears(1)));
        }

        [Test]
        public void CreateFlight_AddsFlightToSchedule()
        {
            Assert.AreEqual(1, _airline.Schedule.Flights.Count);
            Assert.AreSame(_flight, _airline.Schedule.FindByNumber("SD101"));
        }

        [Test]
        public void CreateFlight_SameAircraftSameTime_Throws_AndScheduleUnchanged()
        {
            Assert.Throws<AircraftNotAvailableException>(() =>
                _airline.CreateFlight("SD102", TestData.MakeRoute(370), _aircraft, _flight.DepartureTime));
            Assert.AreEqual(1, _airline.Schedule.Flights.Count);
        }

        [Test]
        public void BookTicket_Economy_CreatesBookingWithTicketAndPrice()
        {
            var booking = _airline.BookTicket(_passenger, _flight, SeatClass.Economy);

            Assert.AreEqual("BK1001", booking.Code);
            Assert.AreEqual(1, booking.Tickets.Count);
            var ticket = booking.Tickets[0];
            Assert.AreEqual("SD1001", ticket.Number);
            Assert.AreEqual("6A", ticket.Seat.Label());
            Assert.IsTrue(ticket.Seat.IsOccupied);
            Assert.AreEqual(37.30m, ticket.Price); // 370 * 0.09 * 1 * 1.12
            Assert.AreSame(booking, _airline.FindBooking("BK1001"));
            Assert.AreEqual(1, _flight.Tickets.Count);
            Assert.Contains(_passenger, _airline.Passengers);
        }

        [Test]
        public void BookTicket_SpecificSeat_UsesSeatClassForPrice()
        {
            var booking = _airline.BookTicket(_passenger, _flight, SeatClass.Business, "3A");
            Assert.AreEqual("3A", booking.Tickets[0].Seat.Label());
            Assert.AreEqual(93.24m, booking.Tickets[0].Price); // 370 * 0.09 * 2.5 * 1.12
        }

        [Test]
        public void BookTicket_TwoBookings_IncrementCounters_AndPassengerListedOnce()
        {
            _airline.BookTicket(_passenger, _flight, SeatClass.Economy);
            var second = _airline.BookTicket(_passenger, _flight, SeatClass.Economy);
            Assert.AreEqual("BK1002", second.Code);
            Assert.AreEqual("SD1002", second.Tickets[0].Number);
            Assert.AreEqual(1, _airline.Passengers.Count);
            Assert.AreEqual(2, _airline.Bookings.Count);
        }

        [Test]
        public void BookTicket_SameSeatTwice_Throws()
        {
            _airline.BookTicket(_passenger, _flight, SeatClass.Economy, "10A");
            Assert.Throws<SeatUnavailableException>(() =>
                _airline.BookTicket(_passenger, _flight, SeatClass.Economy, "10A"));
            Assert.AreEqual(1, _airline.Bookings.Count);
        }

        [Test]
        public void BookTicket_UnknownSeat_Throws()
        {
            Assert.Throws<SeatUnavailableException>(() =>
                _airline.BookTicket(_passenger, _flight, SeatClass.Economy, "99Z"));
        }

        [Test]
        public void BookTicket_InternationalWithoutVisa_Throws_AndNoSeatTaken()
        {
            var noVisa = TestData.MakePassenger(2, "Russia");
            Assert.Throws<ExpiredDocumentException>(() => _airline.BookTicket(noVisa, _flight, SeatClass.Economy));
            Assert.AreEqual(180, _aircraft.Seats.CountFree());
            Assert.AreEqual(0, _airline.Bookings.Count);
        }

        [Test]
        public void BookTicket_DomesticFlight_NoVisaRequired()
        {
            var plane = TestData.MakeAircraft("PH-DOM");
            _airline.AddAircraft(plane);
            var domestic = _airline.CreateFlight("SD300", TestData.MakeDomesticRoute(), plane, DateTime.Now.AddDays(4));
            var noVisa = TestData.MakePassenger(3, "Russia");
            Assert.DoesNotThrow(() => _airline.BookTicket(noVisa, domestic, SeatClass.Economy));
        }

        [Test]
        public void BookTicket_FullFlight_Throws()
        {
            var tiny = TestData.MakeAircraft("PH-TNY", TestData.Tiny(), 1, 5000);
            var f = _airline.CreateFlight("SD400", TestData.MakeDomesticRoute(), tiny, DateTime.Now.AddDays(4));
            f.AddTicket(TestData.MakeTicket(f, 10m, 1, 'A', null, "T1"));
            f.AddTicket(TestData.MakeTicket(f, 10m, 1, 'B', null, "T2"));
            Assert.Throws<SeatUnavailableException>(() =>
                _airline.BookTicket(TestData.MakePassenger(5), f, SeatClass.First));
        }

        [Test]
        public void FindBooking_Unknown_Throws()
        {
            Assert.Throws<BookingNotFoundException>(() => _airline.FindBooking("BK0"));
        }

        [Test]
        public void FireEmployee_RemovesById()
        {
            _airline.HireEmployee(new Employee(7, "A", "B", new DateTime(1990, 1, 1), "Clerk", 1000m, DateTime.Today));
            Assert.IsTrue(_airline.FireEmployee(7));
            Assert.IsFalse(_airline.FireEmployee(7));
            Assert.AreEqual(0, _airline.Employees.Count);
        }

        [Test]
        public void PayPayroll_SumsSalaryAndMonthlyBonus()
        {
            var hired = DateTime.Today.AddYears(-5).AddDays(-30);
            // бонус = 1200 * 0.02 * 5 = 120 в год -> 10 в месяц
            _airline.HireEmployee(new Employee(1, "A", "B", new DateTime(1990, 1, 1), "Clerk", 1200m, hired));
            Assert.AreEqual(1210m, _airline.PayPayroll());
        }

        [Test]
        public void FindPilots_ReturnsOnlyPilotsRatedForType()
        {
            _airline.HireEmployee(TestData.MakePilot(1, "A320"));
            _airline.HireEmployee(TestData.MakePilot(2, "B777"));
            _airline.HireEmployee(new Employee(3, "A", "B", new DateTime(1990, 1, 1), "Clerk", 1000m, DateTime.Today));
            var pilots = _airline.FindPilots(TestData.A320());
            Assert.AreEqual(1, pilots.Count);
            Assert.AreEqual(1, pilots[0].Id);
        }

        [Test]
        public void FindAvailableAircraft_ExcludesBusyAndNotAirworthy()
        {
            var second = TestData.MakeAircraft("PH-BBB");
            var third = TestData.MakeAircraft("PH-CCC");
            _airline.AddAircraft(second); _airline.AddAircraft(third);
            third.SendToMaintenance();

            var result = _airline.FindAvailableAircraft(TestData.MakeRoute(370), _flight.DepartureTime);

            Assert.AreEqual(1, result.Count);
            Assert.AreSame(second, result[0]);
        }
    }

    [TestFixture]
    public class MealAndCateringTests
    {
        [TestCase(399, true)]
        [TestCase(400, false)]
        public void Meal_IsLight_Boundary(int calories, bool expected)
        {
            Assert.AreEqual(expected, new Meal("M", MealType.Standard, calories, 5m).IsLight());
        }

        [Test]
        public void Meal_IsSpecialDiet_AnythingButStandard()
        {
            Assert.IsFalse(new Meal("M", MealType.Standard, 500, 5m).IsSpecialDiet());
            Assert.IsTrue(new Meal("M", MealType.Vegan, 500, 5m).IsSpecialDiet());
        }

        [TestCase(0)]
        [TestCase(-3)]
        public void CateringOrder_NonPositiveQuantity_Throws(int qty)
        {
            Assert.Throws<InvalidFlightDataException>(() => new CateringOrder(TestData.MakeFlight(), qty));
        }

        [Test]
        public void CateringOrder_TotalCost_IsSumOfMealsTimesQuantity()
        {
            var o = new CateringOrder(TestData.MakeFlight(), 10);
            o.AddMeal(new Meal("A", MealType.Standard, 300, 4m));
            o.AddMeal(new Meal("B", MealType.Vegan, 500, 6m));
            Assert.AreEqual(100m, o.TotalCost());
            Assert.AreEqual(400, o.AverageCalories(), 1e-9);
        }

        [Test]
        public void CateringOrder_NoMeals_AverageCaloriesZero()
        {
            Assert.AreEqual(0, new CateringOrder(TestData.MakeFlight(), 1).AverageCalories(), 1e-9);
        }

        [Test]
        public void CoversAllPassengers_ComparesWithActiveTickets()
        {
            var f = TestData.MakeFlight();
            var t1 = TestData.MakeTicket(f, 10m, 1, 'A', null, "T1");
            var t2 = TestData.MakeTicket(f, 10m, 1, 'B', null, "T2");
            f.AddTicket(t1); f.AddTicket(t2);

            Assert.IsFalse(new CateringOrder(f, 1).CoversAllPassengers());
            Assert.IsTrue(new CateringOrder(f, 2).CoversAllPassengers());

            t2.Cancel();
            Assert.IsTrue(new CateringOrder(f, 1).CoversAllPassengers());
        }
    }

    [TestFixture]
    public class ComplaintAndSupportTests
    {
        private Complaint NewComplaint() => new Complaint(TestData.MakePassenger(), TestData.MakeFlight(), "Late");

        [Test]
        public void Complaint_Lifecycle_OpenEscalatedResolved()
        {
            var c = NewComplaint();
            Assert.AreEqual(ComplaintStatus.Open, c.Status);
            Assert.IsTrue(c.IsOpen());
            c.Escalate();
            Assert.AreEqual(ComplaintStatus.Escalated, c.Status);
            Assert.IsTrue(c.IsOpen());
            c.Resolve();
            Assert.AreEqual(ComplaintStatus.Resolved, c.Status);
            Assert.IsFalse(c.IsOpen());
        }

        [Test]
        public void Complaint_EscalateAfterResolve_KeepsResolved()
        {
            var c = NewComplaint();
            c.Resolve();
            c.Escalate();
            Assert.AreEqual(ComplaintStatus.Resolved, c.Status);
        }

        private static CustomerSupport MakeSupport() =>
            new CustomerSupport(new Employee(1, "Mark", "Dijk", new DateTime(1990, 1, 1), "Agent", 3000m, DateTime.Today));

        [Test]
        public void CustomerSupport_CountOpen_AndResolve()
        {
            var s = MakeSupport();
            s.Register(NewComplaint()); s.Register(NewComplaint());
            Assert.AreEqual(2, s.CountOpen());
            s.ResolveComplaint(0);
            Assert.AreEqual(1, s.CountOpen());
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(5)]
        public void CustomerSupport_ResolveComplaint_BadIndex_Throws(int index)
        {
            var s = MakeSupport();
            Assert.Throws<ArgumentOutOfRangeException>(() => s.ResolveComplaint(index));
        }

        [Test]
        public void CustomerSupport_FindByPassenger_FiltersByPassenger()
        {
            var s = MakeSupport();
            var p1 = TestData.MakePassenger(1);
            var p2 = TestData.MakePassenger(2);
            var f = TestData.MakeFlight();
            s.Register(new Complaint(p1, f, "a"));
            s.Register(new Complaint(p2, f, "b"));
            s.Register(new Complaint(p1, f, "c"));
            Assert.AreEqual(2, s.FindByPassenger(p1).Count);
            Assert.AreEqual(1, s.FindByPassenger(p2).Count);
        }
    }

    [TestFixture]
    public class NotificationTests
    {
        [Test]
        public void Send_SetsSentAt_AndFormatsMessage()
        {
            var p = TestData.MakePassenger(1);
            var n = new Notification(p, "Hello");
            Assert.IsFalse(n.IsSent);
            Assert.IsNull(n.SentAt);

            var text = n.Send();

            Assert.IsTrue(n.IsSent);
            StringAssert.Contains(p.Email, text);
            StringAssert.Contains("Hello", text);
        }

        [Test]
        public void ForDelay_UsesDelayDescription()
        {
            var f = TestData.MakeFlight();
            var delay = f.Delay(60, "Fog");
            var n = Notification.ForDelay(TestData.MakePassenger(), delay);
            StringAssert.Contains("SD101", n.Message);
            StringAssert.Contains("Fog", n.Message);
        }
    }
}
