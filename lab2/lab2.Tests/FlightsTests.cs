using System;
using System.Linq;
using AirlineSystem;
using NUnit.Framework;

namespace lab2.Tests
{
    [TestFixture]
    public class RouteTests
    {
        [Test]
        public void Constructor_SameAirportCode_Throws()
        {
            Assert.Throws<InvalidFlightDataException>(() =>
                new Route(TestData.MakeAirport("AMS"), TestData.MakeAirport("AMS"), 100));
        }

        [TestCase(0)]
        [TestCase(-10)]
        public void Constructor_NonPositiveDistance_Throws(double km)
        {
            Assert.Throws<InvalidFlightDataException>(() => TestData.MakeRoute(km));
        }

        [Test]
        public void GetCode_IsOriginDashDestination()
        {
            Assert.AreEqual("AMS-LHR", TestData.MakeRoute().GetCode());
        }

        [Test]
        public void IsInternational_DependsOnCountries()
        {
            Assert.IsTrue(TestData.MakeRoute().IsInternational());
            Assert.IsFalse(TestData.MakeDomesticRoute().IsInternational());
        }

        [TestCase(800, 1.5)]
        [TestCase(0.0001, 0.5)]
        [TestCase(1600, 2.5)]
        public void EstimateDuration_Is800KmhPlusHalfHour(double km, double hours)
        {
            Assert.AreEqual(hours, TestData.MakeRoute(km).EstimateDuration().TotalHours, 1e-3);
        }
    }

    [TestFixture]
    public class CargoTests
    {
        [TestCase(0)]
        [TestCase(-1)]
        public void Constructor_NonPositiveWeight_Throws(double kg)
        {
            Assert.Throws<InvalidFlightDataException>(() => new Cargo(1, kg, "x", false));
        }

        [Test]
        public void CalculateFee_Regular()
        {
            Assert.AreEqual(250m, new Cargo(1, 100, "Boxes", false).CalculateFee());
        }

        [Test]
        public void CalculateFee_HazardousCostsDouble()
        {
            Assert.AreEqual(500m, new Cargo(1, 100, "Chemicals", true).CalculateFee());
        }

        [TestCase(10000, false)]
        [TestCase(15600, false)]
        [TestCase(20000, true)]
        public void IsOverweightFor_UsesTypeLimit(double kg, bool expected)
        {
            Assert.AreEqual(expected, new Cargo(1, kg, "x", false).IsOverweightFor(TestData.A320()));
        }
    }

    [TestFixture]
    public class FlightDelayTests
    {
        [TestCase(179, false)]
        [TestCase(180, true)]
        public void IsSevere_Boundary(int minutes, bool expected)
        {
            Assert.AreEqual(expected, new FlightDelay(TestData.MakeFlight(), minutes, "x").IsSevere());
        }

        [Test]
        public void Describe_ContainsFlightNumberMinutesAndReason()
        {
            var text = new FlightDelay(TestData.MakeFlight(), 45, "Fog").Describe();
            StringAssert.Contains("SD101", text);
            StringAssert.Contains("45", text);
            StringAssert.Contains("Fog", text);
        }
    }

    [TestFixture]
    public class FlightTests
    {
        [TestCase("")]
        [TestCase("  ")]
        public void Constructor_EmptyNumber_Throws(string number)
        {
            Assert.Throws<InvalidFlightDataException>(() =>
                new Flight(number, TestData.MakeRoute(), TestData.MakeAircraft(), DateTime.Now.AddDays(1)));
        }

        [Test]
        public void Constructor_DepartureInThePast_Throws()
        {
            Assert.Throws<InvalidFlightDataException>(() => TestData.MakeFlight(DateTime.Now.AddDays(-2)));
        }

        [Test]
        public void Constructor_AircraftCannotServeRoute_Throws()
        {
            Assert.Throws<AircraftNotAvailableException>(() =>
                TestData.MakeFlight(null, null, TestData.MakeRoute(7000)));
        }

        [Test]
        public void Constructor_ValidData_StartsScheduled()
        {
            var f = TestData.MakeFlight();
            Assert.AreEqual(FlightStatus.Scheduled, f.Status);
            Assert.AreEqual("SD101", f.Number);
            Assert.IsNull(f.Crew);
            Assert.IsNull(f.Gate);
            Assert.AreEqual(0, f.Tickets.Count);
        }

        [Test]
        public void GetArrivalTime_IsDeparturePlusEstimatedDuration()
        {
            var f = TestData.MakeFlight();
            Assert.AreEqual(f.DepartureTime + f.Route.EstimateDuration(), f.GetArrivalTime());
        }

        [Test]
        public void IsFull_AndLoadFactor_IgnoreCancelledTickets()
        {
            var f = TestData.MakeFlight(null, TestData.MakeAircraft("PH-TNY", TestData.Tiny(), 1, 5000)); // 2 места
            var t1 = TestData.MakeTicket(f, 100m, 1, 'A', null, "T1");
            var t2 = TestData.MakeTicket(f, 100m, 1, 'B', null, "T2");

            f.AddTicket(t1);
            Assert.IsFalse(f.IsFull());
            Assert.AreEqual(50, f.GetLoadFactor(), 1e-9);

            f.AddTicket(t2);
            Assert.IsTrue(f.IsFull());
            Assert.AreEqual(100, f.GetLoadFactor(), 1e-9);

            t1.Cancel();
            Assert.IsFalse(f.IsFull());
            Assert.AreEqual(50, f.GetLoadFactor(), 1e-9);
        }

        [Test]
        public void AssignCrew_ValidCrew_IsStored()
        {
            var f = TestData.MakeFlight();
            var crew = TestData.MakeCrew();
            f.AssignCrew(crew);
            Assert.AreSame(crew, f.Crew);
        }

        [Test]
        public void AssignCrew_ExpiredLicense_Throws_AndCrewNotAssigned()
        {
            var f = TestData.MakeFlight();
            var crew = new Crew(TestData.MakePilot(1, "A320", -1), TestData.MakePilot(2));
            for (int i = 0; i < 3; i++) crew.AddAttendant(TestData.MakeAttendant(10 + i));
            Assert.Throws<InvalidLicenseException>(() => f.AssignCrew(crew));
            Assert.IsNull(f.Crew);
        }

        [Test]
        public void AssignCrew_PilotsNotRatedForType_Throws()
        {
            var f = TestData.MakeFlight();
            Assert.Throws<PilotNotQualifiedException>(() => f.AssignCrew(TestData.MakeCrew("B777")));
        }

        [Test]
        public void AssignCrew_NotEnoughAttendants_Throws()
        {
            var f = TestData.MakeFlight();
            Assert.Throws<CrewShortageException>(() => f.AssignCrew(TestData.MakeCrew("A320", 2)));
        }

        [Test]
        public void AssignGate_OccupiesGate_AndSecondFlightCannotTakeIt()
        {
            var gate = new Gate("G1");
            var f1 = TestData.MakeFlight();
            var f2 = TestData.MakeFlight(null, TestData.MakeAircraft("PH-BBB"), null, "SD202");
            f1.AssignGate(gate);
            Assert.AreSame(gate, f1.Gate);
            Assert.IsTrue(gate.IsOccupied);
            Assert.Throws<AircraftNotAvailableException>(() => f2.AssignGate(gate));
        }

        [TestCase(FlightStatus.Cancelled)]
        [TestCase(FlightStatus.Departed)]
        public void AddTicket_UnavailableFlight_Throws(FlightStatus status)
        {
            var f = status == FlightStatus.Cancelled ? CancelledFlight() : DepartedFlight();
            Assert.Throws<InvalidFlightDataException>(() => f.AddTicket(TestData.MakeTicket(f)));
        }

        private static Flight CancelledFlight() { var f = TestData.MakeFlight(); f.Cancel(); return f; }
        private static Flight DepartedFlight() { var f = TestData.MakeReadyFlight(); f.Depart(); return f; }

        [Test]
        public void LoadCargo_WithinLimit_Added()
        {
            var f = TestData.MakeFlight(); // лимит 15600 кг
            f.LoadCargo(new Cargo(1, 10000, "A", false));
            Assert.AreEqual(1, f.CargoList.Count);
        }

        [Test]
        public void LoadCargo_TotalExceedsLimit_Throws_AndNotAdded()
        {
            var f = TestData.MakeFlight();
            f.LoadCargo(new Cargo(1, 10000, "A", false));
            Assert.Throws<BaggageOverweightException>(() => f.LoadCargo(new Cargo(2, 6000, "B", false)));
            Assert.AreEqual(1, f.CargoList.Count);
        }

        [TestCase(0)]
        [TestCase(-15)]
        public void Delay_NonPositiveMinutes_Throws(int minutes)
        {
            Assert.Throws<InvalidFlightDataException>(() => TestData.MakeFlight().Delay(minutes, "x"));
        }

        [Test]
        public void Delay_ShiftsDeparture_SetsStatus_ReturnsDelay()
        {
            var f = TestData.MakeFlight();
            var original = f.DepartureTime;
            var delay = f.Delay(90, "Fog");
            Assert.AreEqual(original.AddMinutes(90), f.DepartureTime);
            Assert.AreEqual(FlightStatus.Delayed, f.Status);
            Assert.AreEqual(90, delay.Minutes);
            Assert.AreEqual("Fog", delay.Reason);
            Assert.AreSame(f, delay.Flight);
        }

        [Test]
        public void Reschedule_ChangesTime_AndResetsStatus()
        {
            var f = TestData.MakeFlight();
            f.Delay(30, "x");
            var newTime = DateTime.Now.AddDays(10);
            f.Reschedule(newTime);
            Assert.AreEqual(newTime, f.DepartureTime);
            Assert.AreEqual(FlightStatus.Scheduled, f.Status);
        }

        [Test]
        public void Reschedule_AlreadyDeparted_Throws()
        {
            Assert.Throws<InvalidFlightDataException>(() => DepartedFlight().Reschedule(DateTime.Now.AddDays(9)));
        }

        [Test]
        public void Cancel_CancelsTickets_AndFreesGate()
        {
            var f = TestData.MakeFlight();
            var gate = new Gate("G1");
            f.AssignGate(gate);
            var t = TestData.MakeTicket(f);
            f.AddTicket(t);

            f.Cancel();

            Assert.AreEqual(FlightStatus.Cancelled, f.Status);
            Assert.AreEqual(TicketStatus.Cancelled, t.Status);
            Assert.IsFalse(gate.IsOccupied);
        }

        [Test]
        public void StartBoarding_SetsStatus()
        {
            var f = TestData.MakeFlight();
            f.StartBoarding();
            Assert.AreEqual(FlightStatus.Boarding, f.Status);
        }

        [Test]
        public void StartBoarding_CancelledFlight_Throws()
        {
            Assert.Throws<InvalidFlightDataException>(() => CancelledFlight().StartBoarding());
        }

        [Test]
        public void Depart_WithoutCrew_Throws()
        {
            Assert.Throws<CrewShortageException>(() => TestData.MakeFlight().Depart());
        }

        [Test]
        public void Depart_AircraftInMaintenance_Throws()
        {
            var f = TestData.MakeReadyFlight();
            f.Aircraft.SendToMaintenance();
            Assert.Throws<AircraftNotAvailableException>(() => f.Depart());
            Assert.AreNotEqual(FlightStatus.Departed, f.Status);
        }

        [Test]
        public void Depart_NotEnoughFuel_Throws()
        {
            var f = TestData.MakeReadyFlight();
            f.Aircraft.Fuel.Consume(7900); // сожгли 23700 из 24000 л
            Assert.Throws<AircraftNotAvailableException>(() => f.Depart());
        }

        [Test]
        public void Depart_Success_ConsumesFuel_SetsStatus_FreesGate()
        {
            var f = TestData.MakeReadyFlight();
            var gate = new Gate("G1");
            f.AssignGate(gate);
            double before = f.Aircraft.Fuel.CurrentL;

            f.Depart();

            Assert.AreEqual(FlightStatus.Departed, f.Status);
            Assert.AreEqual(before - 370 * 3.0, f.Aircraft.Fuel.CurrentL, 1e-6);
            Assert.IsFalse(gate.IsOccupied);
        }

        [Test]
        public void Arrive_BeforeDeparture_Throws()
        {
            Assert.Throws<InvalidFlightDataException>(() => TestData.MakeReadyFlight().Arrive());
        }

        [Test]
        public void Arrive_UpdatesEnginesPilotsAndTickets()
        {
            var f = TestData.MakeReadyFlight();
            var checkedIn = TestData.MakeTicket(f, 100m, 1, 'A', null, "T1");
            var booked = TestData.MakeTicket(f, 100m, 1, 'B', null, "T2");
            f.AddTicket(checkedIn); f.AddTicket(booked);
            checkedIn.CheckIn();
            double captainBefore = f.Crew.Captain.FlightHours;
            double hours = f.Route.EstimateDuration().TotalHours;

            f.Depart();
            f.Arrive();

            Assert.AreEqual(FlightStatus.Arrived, f.Status);
            Assert.AreEqual(captainBefore + hours, f.Crew.Captain.FlightHours, 1e-6);
            Assert.IsTrue(f.Aircraft.Engines.All(e => Math.Abs(e.HoursSinceOverhaul - hours) < 1e-6));
            Assert.AreEqual(TicketStatus.Used, checkedIn.Status);
            Assert.AreEqual(TicketStatus.Booked, booked.Status);
        }
    }

    [TestFixture]
    public class FlightScheduleTests
    {
        private FlightSchedule _schedule;
        private Aircraft _a1, _a2;

        [SetUp]
        public void SetUp()
        {
            _schedule = new FlightSchedule();
            _a1 = TestData.MakeAircraft("PH-AAA");
            _a2 = TestData.MakeAircraft("PH-BBB");
        }

        private Flight MakeAt(string number, Aircraft a, DateTime dep) =>
            new Flight(number, TestData.MakeRoute(), a, dep);

        [Test]
        public void AddFlight_AddsAndFindsByNumber_CaseInsensitive()
        {
            var f = MakeAt("SD101", _a1, DateTime.Now.AddDays(2));
            _schedule.AddFlight(f);
            Assert.AreSame(f, _schedule.FindByNumber("sd101"));
        }

        [Test]
        public void FindByNumber_Unknown_Throws()
        {
            Assert.Throws<FlightNotFoundException>(() => _schedule.FindByNumber("ZZ999"));
        }

        [Test]
        public void AddFlight_SameAircraftOverlapping_Throws()
        {
            var t = DateTime.Now.AddDays(2);
            _schedule.AddFlight(MakeAt("SD101", _a1, t));
            Assert.Throws<AircraftNotAvailableException>(() =>
                _schedule.AddFlight(MakeAt("SD102", _a1, t.AddMinutes(30))));
            Assert.AreEqual(1, _schedule.Flights.Count);
        }

        [Test]
        public void AddFlight_DifferentAircraftSameTime_Allowed()
        {
            var t = DateTime.Now.AddDays(2);
            _schedule.AddFlight(MakeAt("SD101", _a1, t));
            Assert.DoesNotThrow(() => _schedule.AddFlight(MakeAt("SD102", _a2, t)));
        }

        [Test]
        public void AddFlight_SameAircraftAfterPreviousArrival_Allowed()
        {
            var first = MakeAt("SD101", _a1, DateTime.Now.AddDays(2));
            _schedule.AddFlight(first);
            Assert.DoesNotThrow(() =>
                _schedule.AddFlight(MakeAt("SD102", _a1, first.GetArrivalTime().AddMinutes(1))));
        }

        [Test]
        public void HasConflict_IgnoresCancelledFlights()
        {
            var t = DateTime.Now.AddDays(2);
            var f = MakeAt("SD101", _a1, t);
            _schedule.AddFlight(f);
            f.Cancel();
            Assert.IsFalse(_schedule.HasConflict(_a1, t, t.AddHours(2)));
        }

        [Test]
        public void FindByRoute_IsCaseInsensitiveForCodes()
        {
            _schedule.AddFlight(MakeAt("SD101", _a1, DateTime.Now.AddDays(2)));
            Assert.AreEqual(1, _schedule.FindByRoute("ams", "lhr").Count);
            Assert.AreEqual(0, _schedule.FindByRoute("LHR", "AMS").Count);
        }

        [Test]
        public void GetDeparturesOn_ReturnsOnlyThatDay_SortedByTime()
        {
            var day = DateTime.Today.AddDays(3);
            _schedule.AddFlight(MakeAt("SD2", _a2, day.AddHours(15)));
            _schedule.AddFlight(MakeAt("SD1", _a1, day.AddHours(9)));
            _schedule.AddFlight(MakeAt("SD3", TestData.MakeAircraft("PH-CCC"), day.AddDays(1).AddHours(9)));

            var list = _schedule.GetDeparturesOn(day);

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("SD1", list[0].Number);
            Assert.AreEqual("SD2", list[1].Number);
        }

        [Test]
        public void RemoveFlight_ReturnsWhetherSomethingWasRemoved()
        {
            _schedule.AddFlight(MakeAt("SD101", _a1, DateTime.Now.AddDays(2)));
            Assert.IsTrue(_schedule.RemoveFlight("SD101"));
            Assert.IsFalse(_schedule.RemoveFlight("SD101"));
            Assert.AreEqual(0, _schedule.Flights.Count);
        }
    }

    [TestFixture]
    public class FlightStatisticsTests
    {
        private FlightSchedule _schedule;
        private System.Collections.Generic.List<Booking> _bookings;
        private FlightStatistics _stats;

        [SetUp]
        public void SetUp()
        {
            _schedule = new FlightSchedule();
            _bookings = new System.Collections.Generic.List<Booking>();
            _stats = new FlightStatistics(_schedule, _bookings);
        }

        [Test]
        public void EmptyStatistics_AreZero()
        {
            Assert.AreEqual(0, _stats.AverageDelayMinutes(), 1e-9);
            Assert.AreEqual(0, _stats.AverageLoadFactor(), 1e-9);
            Assert.AreEqual(0, _stats.CancelledCount());
            Assert.AreEqual(0m, _stats.TotalRevenue());
        }

        [Test]
        public void AverageDelayMinutes_IsMeanOfRecordedDelays()
        {
            var f = TestData.MakeFlight();
            _stats.RecordDelay(new FlightDelay(f, 30, "a"));
            _stats.RecordDelay(new FlightDelay(f, 90, "b"));
            Assert.AreEqual(60, _stats.AverageDelayMinutes(), 1e-9);
        }

        [Test]
        public void CancelledCount_CountsOnlyCancelledFlights()
        {
            var f1 = TestData.MakeFlight(null, TestData.MakeAircraft("PH-AAA"), null, "SD1");
            var f2 = TestData.MakeFlight(null, TestData.MakeAircraft("PH-BBB"), null, "SD2");
            _schedule.AddFlight(f1); _schedule.AddFlight(f2);
            f2.Cancel();
            Assert.AreEqual(1, _stats.CancelledCount());
        }

        [Test]
        public void AverageLoadFactor_IsMeanOverFlights()
        {
            var tiny = TestData.Tiny();
            var f1 = TestData.MakeFlight(null, TestData.MakeAircraft("PH-AAA", tiny, 1, 5000), null, "SD1");
            var f2 = TestData.MakeFlight(null, TestData.MakeAircraft("PH-BBB", tiny, 1, 5000), null, "SD2");
            f1.AddTicket(TestData.MakeTicket(f1, 100m, 1, 'A', null, "T1")); // 50%
            _schedule.AddFlight(f1); _schedule.AddFlight(f2);                // f2 - 0%
            Assert.AreEqual(25, _stats.AverageLoadFactor(), 1e-9);
        }

        [Test]
        public void TotalRevenue_CountsOnlyPaidBookings()
        {
            var flight = TestData.MakeFlight();
            var paid = new Booking("B1", TestData.MakePassenger());
            paid.AddTicket(TestData.MakeTicket(flight, 150m));
            paid.Pay(TestData.MakeCard(1000m));

            var unpaid = new Booking("B2", TestData.MakePassenger(2));
            unpaid.AddTicket(TestData.MakeTicket(flight, 999m));

            _bookings.Add(paid); _bookings.Add(unpaid);
            Assert.AreEqual(150m, _stats.TotalRevenue());
        }
    }

    [TestFixture]
    public class PassengerManifestTests
    {
        private PassengerManifest _manifest; // самолёт на 2 места

        [SetUp]
        public void SetUp()
        {
            var flight = TestData.MakeFlight(null, TestData.MakeAircraft("PH-TNY", TestData.Tiny(), 1, 5000));
            _manifest = new PassengerManifest(flight);
        }

        [Test]
        public void Add_SamePassengerTwice_CountedOnce()
        {
            var p = TestData.MakePassenger(1);
            _manifest.Add(p); _manifest.Add(p);
            Assert.AreEqual(1, _manifest.GetCount());
            Assert.IsTrue(_manifest.Contains(p));
        }

        [Test]
        public void Add_BeyondCapacity_Throws()
        {
            _manifest.Add(TestData.MakePassenger(1));
            _manifest.Add(TestData.MakePassenger(2));
            Assert.Throws<SeatUnavailableException>(() => _manifest.Add(TestData.MakePassenger(3)));
        }

        [Test]
        public void Remove_ReturnsWhetherPassengerWasPresent()
        {
            var p = TestData.MakePassenger(1);
            _manifest.Add(p);
            Assert.IsTrue(_manifest.Remove(p));
            Assert.IsFalse(_manifest.Remove(p));
            Assert.IsFalse(_manifest.Contains(p));
        }

        [Test]
        public void CountMinors_CountsPassengersUnder18()
        {
            _manifest.Add(TestData.MakePassenger(1));
            _manifest.Add(TestData.MakePassenger(2, "Netherlands", DateTime.Today.AddYears(-10)));
            Assert.AreEqual(1, _manifest.CountMinors());
        }
    }
}
