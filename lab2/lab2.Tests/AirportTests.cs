using System;
using AirlineSystem;
using NUnit.Framework;

namespace lab2.Tests
{
    [TestFixture]
    public class AirportTests
    {
        [TestCase("")]
        [TestCase("AB")]
        [TestCase("ABCD")]
        public void Constructor_InvalidIataCode_Throws(string code)
        {
            Assert.Throws<InvalidFlightDataException>(() => new Airport(code, "N", "C", "X"));
        }

        [Test]
        public void Constructor_NullCode_Throws()
        {
            Assert.Throws<InvalidFlightDataException>(() => new Airport(null, "N", "C", "X"));
        }

        [Test]
        public void Constructor_NormalizesCodeToUpperCase()
        {
            var a = new Airport("ams", "Schiphol", "Amsterdam", "Netherlands");
            Assert.AreEqual("AMS", a.Code);
            Assert.IsTrue(a.IsOpen);
        }

        [Test]
        public void GetLongestRunwayM_NoRunways_ReturnsZero()
        {
            Assert.AreEqual(0, TestData.MakeAirport("AMS", "NL", 0).GetLongestRunwayM(), 1e-9);
        }

        [Test]
        public void GetLongestRunwayM_ReturnsMaximum()
        {
            var a = TestData.MakeAirport("AMS", "NL", 3000);
            a.AddRunway(new Runway("18/36", 3800));
            a.AddRunway(new Runway("06/24", 2000));
            Assert.AreEqual(3800, a.GetLongestRunwayM(), 1e-9);
        }

        [Test]
        public void CloseAndOpenAirport_TogglesFlag()
        {
            var a = TestData.MakeAirport();
            a.CloseAirport();
            Assert.IsFalse(a.IsOpen);
            a.OpenAirport();
            Assert.IsTrue(a.IsOpen);
        }

        [Test]
        public void FindFreeGate_ClosedAirport_Throws()
        {
            var a = TestData.MakeAirport();
            a.CloseAirport();
            Assert.Throws<AirportClosedException>(() => a.FindFreeGate());
        }

        [Test]
        public void FindFreeGate_NoTerminals_ReturnsNull()
        {
            Assert.IsNull(TestData.MakeAirport().FindFreeGate());
        }

        [Test]
        public void FindFreeGate_SkipsOccupiedGates_AndLooksIntoNextTerminal()
        {
            var a = TestData.MakeAirport();
            var t1 = new Terminal("T1", true);
            var g1 = new Gate("G1");
            t1.AddGate(g1);
            var t2 = new Terminal("T2", false);
            var g2 = new Gate("G2");
            t2.AddGate(g2);
            a.AddTerminal(t1); a.AddTerminal(t2);

            g1.Occupy(TestData.MakeFlight());
            Assert.AreSame(g2, a.FindFreeGate());

            g2.Occupy(TestData.MakeFlight(null, null, null, "SD202"));
            Assert.IsNull(a.FindFreeGate());
        }
    }

    [TestFixture]
    public class RunwayTests
    {
        [Test]
        public void CanLand_LongEnoughAndOpen_True()
        {
            Assert.IsTrue(new Runway("09", 3000).CanLand(TestData.A320())); // нужно 780 м
        }

        [Test]
        public void CanLand_TooShort_False()
        {
            Assert.IsFalse(new Runway("09", 700).CanLand(TestData.A320()));
        }

        [Test]
        public void CanLand_ExactlyRequiredLength_True()
        {
            Assert.IsTrue(new Runway("09", 780).CanLand(TestData.A320()));
        }

        [Test]
        public void CanLand_ClosedRunway_False_ThenTrueAfterOpen()
        {
            var r = new Runway("09", 3000);
            r.Close();
            Assert.IsFalse(r.CanLand(TestData.A320()));
            r.Open();
            Assert.IsTrue(r.CanLand(TestData.A320()));
        }

        [Test]
        public void Constructor_DefaultSurfaceIsAsphalt()
        {
            Assert.AreEqual("Asphalt", new Runway("09", 1000).Surface);
        }
    }

    [TestFixture]
    public class TerminalAndGateTests
    {
        [Test]
        public void Terminal_FindFreeGate_ReturnsFirstFree_AndCountsFree()
        {
            var t = new Terminal("T1", true);
            var g1 = new Gate("G1"); var g2 = new Gate("G2");
            t.AddGate(g1); t.AddGate(g2);
            Assert.AreEqual(2, t.CountFreeGates());
            g1.Occupy(TestData.MakeFlight());
            Assert.AreSame(g2, t.FindFreeGate());
            Assert.AreEqual(1, t.CountFreeGates());
        }

        [Test]
        public void Terminal_NoGates_FindFreeGateIsNull()
        {
            Assert.IsNull(new Terminal("T1", false).FindFreeGate());
        }

        [Test]
        public void Gate_Occupy_SetsFlight_AndSecondOccupyThrows()
        {
            var g = new Gate("G1");
            var f = TestData.MakeFlight();
            g.Occupy(f);
            Assert.IsTrue(g.IsOccupied);
            Assert.AreSame(f, g.CurrentFlight);
            Assert.Throws<AircraftNotAvailableException>(() => g.Occupy(f));
        }

        [Test]
        public void Gate_Free_ClearsState()
        {
            var g = new Gate("G1");
            g.Occupy(TestData.MakeFlight());
            g.Free();
            Assert.IsFalse(g.IsOccupied);
            Assert.IsNull(g.CurrentFlight);
        }
    }

    [TestFixture]
    public class CheckInDeskTests
    {
        private Airport _airport;
        private Flight _flight;

        [SetUp]
        public void SetUp()
        {
            _airport = TestData.MakeAirport();
            _flight = TestData.MakeFlight(DateTime.Now.AddDays(2));
        }

        [Test]
        public void CheckIn_ClosedDesk_Throws()
        {
            var desk = new CheckInDesk(1, _airport);
            var ticket = TestData.MakeTicket(_flight);
            Assert.Throws<AirportClosedException>(() => desk.CheckIn(ticket, new Gate("G1")));
            Assert.AreEqual(TicketStatus.Booked, ticket.Status);
        }

        [Test]
        public void CheckIn_OpenDesk_ChecksTicketAndBuildsBoardingPass()
        {
            var desk = new CheckInDesk(1, _airport);
            desk.Open();
            var gate = new Gate("G1");
            var ticket = TestData.MakeTicket(_flight, 100m, 1, 'A');

            var pass = desk.CheckIn(ticket, gate);

            Assert.AreEqual(TicketStatus.CheckedIn, ticket.Status);
            Assert.AreSame(gate, pass.Gate);
            Assert.AreEqual(_flight.DepartureTime.AddMinutes(-40), pass.BoardingTime);
            Assert.AreEqual(1, pass.Group);
        }

        [TestCase(1, 1)]
        [TestCase(9, 1)]
        [TestCase(10, 2)]
        [TestCase(25, 3)]
        public void CheckIn_BoardingGroupDependsOnRow(int row, int expectedGroup)
        {
            var desk = new CheckInDesk(1, _airport);
            desk.Open();
            var pass = desk.CheckIn(TestData.MakeTicket(_flight, 100m, row, 'A'), new Gate("G1"));
            Assert.AreEqual(expectedGroup, pass.Group);
        }

        [Test]
        public void CheckIn_SameTicketTwice_Throws()
        {
            var desk = new CheckInDesk(1, _airport);
            desk.Open();
            var ticket = TestData.MakeTicket(_flight);
            desk.CheckIn(ticket, new Gate("G1"));
            Assert.Throws<InvalidFlightDataException>(() => desk.CheckIn(ticket, new Gate("G1")));
        }

        [Test]
        public void OpenAndClose_ToggleState()
        {
            var desk = new CheckInDesk(1, _airport);
            Assert.IsFalse(desk.IsOpen);
            desk.Open();
            Assert.IsTrue(desk.IsOpen);
            desk.Close();
            Assert.IsFalse(desk.IsOpen);
        }
    }

    [TestFixture]
    public class WeatherReportTests
    {
        private static WeatherReport Make(double wind, double visibility) =>
            new WeatherReport(TestData.MakeAirport(), 10, wind, visibility);

        [TestCase(0, 10, true)]
        [TestCase(59.9, 1.5, true)]
        [TestCase(60, 10, false)]
        [TestCase(10, 1.4, false)]
        public void IsSafeForTakeoff_Boundaries(double wind, double visibility, bool expected)
        {
            Assert.AreEqual(expected, Make(wind, visibility).IsSafeForTakeoff());
        }

        [TestCase(89.9, false)]
        [TestCase(90, true)]
        public void IsStorm_Boundary(double wind, bool expected)
        {
            Assert.AreEqual(expected, Make(wind, 10).IsStorm());
        }

        [Test]
        public void Describe_ContainsAirportCode()
        {
            StringAssert.Contains("AMS", Make(20, 10).Describe());
        }
    }
}
