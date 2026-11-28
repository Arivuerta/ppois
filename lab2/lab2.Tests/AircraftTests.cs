using System;
using System.Linq;
using AirlineSystem;
using NUnit.Framework;

namespace lab2.Tests
{
    [TestFixture]
    public class EngineTests
    {
        [Test]
        public void AddHours_PositiveValue_Accumulates()
        {
            var e = new Engine("E1", 1000);
            e.AddHours(10); e.AddHours(5.5);
            Assert.AreEqual(15.5, e.HoursSinceOverhaul, 1e-9);
        }

        [TestCase(0)]
        [TestCase(-5)]
        public void AddHours_NonPositive_Ignored(double hours)
        {
            var e = new Engine("E1", 1000, 100);
            e.AddHours(hours);
            Assert.AreEqual(100, e.HoursSinceOverhaul, 1e-9);
        }

        [TestCase(999.9, false)]
        [TestCase(1000, true)]
        [TestCase(1500, true)]
        public void NeedsOverhaul_Boundary(double hours, bool expected)
        {
            Assert.AreEqual(expected, new Engine("E1", 1000, hours).NeedsOverhaul());
        }

        [Test]
        public void Overhaul_ResetsHours()
        {
            var e = new Engine("E1", 1000, 900);
            e.Overhaul();
            Assert.AreEqual(0, e.HoursSinceOverhaul, 1e-9);
        }

        [Test]
        public void RemainingHours_NeverNegative()
        {
            Assert.AreEqual(400, new Engine("E1", 1000, 600).RemainingHours(), 1e-9);
            Assert.AreEqual(0, new Engine("E1", 1000, 1200).RemainingHours(), 1e-9);
        }
    }

    [TestFixture]
    public class FuelSystemTests
    {
        [Test]
        public void Constructor_StartsFull()
        {
            var f = new FuelSystem(1000, 3);
            Assert.AreEqual(1000, f.CurrentL, 1e-9);
            Assert.AreEqual(100, f.PercentFull(), 1e-9);
        }

        [Test]
        public void Consume_ReducesFuel_AndNeverGoesBelowZero()
        {
            var f = new FuelSystem(1000, 3);
            f.Consume(100);
            Assert.AreEqual(700, f.CurrentL, 1e-9);
            Assert.AreEqual(70, f.PercentFull(), 1e-9);
            f.Consume(10000);
            Assert.AreEqual(0, f.CurrentL, 1e-9);
        }

        [Test]
        public void Refuel_IsCappedByCapacity()
        {
            var f = new FuelSystem(1000, 3);
            f.Consume(100);
            f.Refuel(5000);
            Assert.AreEqual(1000, f.CurrentL, 1e-9);
        }

        [TestCase(0)]
        [TestCase(-10)]
        public void Refuel_NonPositive_Throws(double liters)
        {
            Assert.Throws<ArgumentException>(() => new FuelSystem(1000, 3).Refuel(liters));
        }

        [TestCase(100, true)]   // нужно 330 л, есть 700
        [TestCase(200, true)]   // нужно 660 л, есть 700
        [TestCase(220, false)]  // нужно 726 л, есть 700
        public void HasEnoughFor_IncludesTenPercentReserve(double km, bool expected)
        {
            var f = new FuelSystem(1000, 3);
            f.Consume(100); // осталось 700
            Assert.AreEqual(expected, f.HasEnoughFor(km));
        }
    }

    [TestFixture]
    public class AircraftTypeTests
    {
        [TestCase(6100, true)]
        [TestCase(6100.1, false)]
        [TestCase(0, true)]
        public void CanFly_RangeBoundary(double km, bool expected)
        {
            Assert.AreEqual(expected, TestData.A320().CanFly(km));
        }

        [TestCase(15600, true)]   // ровно 20% от MTOW
        [TestCase(15600.1, false)]
        public void CanCarry_Is20PercentOfMtow(double kg, bool expected)
        {
            Assert.AreEqual(expected, TestData.A320().CanCarry(kg));
        }

        [Test]
        public void RequiredRunwayM_IsMtowDividedBy100()
        {
            Assert.AreEqual(780, TestData.A320().RequiredRunwayM(), 1e-9);
        }
    }

    [TestFixture]
    public class SeatTests
    {
        [Test]
        public void Label_CombinesRowAndLetter()
        {
            Assert.AreEqual("12C", new Seat(12, 'C', SeatClass.Economy).Label());
        }

        [Test]
        public void Reserve_MarksOccupied_SecondReserveThrows()
        {
            var s = new Seat(1, 'A', SeatClass.First);
            Assert.IsFalse(s.IsOccupied);
            s.Reserve();
            Assert.IsTrue(s.IsOccupied);
            Assert.Throws<SeatUnavailableException>(() => s.Reserve());
        }

        [Test]
        public void Release_MakesSeatFreeAgain()
        {
            var s = new Seat(1, 'A', SeatClass.First);
            s.Reserve();
            s.Release();
            Assert.IsFalse(s.IsOccupied);
            Assert.DoesNotThrow(() => s.Reserve());
        }
    }

    [TestFixture]
    public class SeatMapTests
    {
        private Aircraft _aircraft;

        [SetUp]
        public void SetUp() => _aircraft = TestData.MakeAircraft(); // 180 мест, 6 колонок = 30 рядов

        [Test]
        public void Constructor_CreatesSeatsForWholeCapacity()
        {
            Assert.AreEqual(180, _aircraft.Seats.Seats.Count);
            Assert.AreEqual(180, _aircraft.Seats.CountFree());
        }

        [Test]
        public void Constructor_AssignsClassesByRow()
        {
            var seats = _aircraft.Seats.Seats;
            Assert.AreEqual(12, seats.Count(s => s.Class == SeatClass.First));     // ряды 1-2
            Assert.AreEqual(18, seats.Count(s => s.Class == SeatClass.Business));  // ряды 3-5
            Assert.AreEqual(150, seats.Count(s => s.Class == SeatClass.Economy));  // ряды 6-30
        }

        [Test]
        public void Constructor_PartialLastRow_DoesNotExceedCapacity()
        {
            var type = new AircraftType("T", "M", 1000, 10, 10000); // 10 мест / 6 = 2 ряда, во втором 4 места
            var a = new Aircraft("PH-XXX", type, 2020, 1, 1000);
            Assert.AreEqual(10, a.Seats.Seats.Count);
        }

        [Test]
        public void FindFreeSeat_ReturnsFirstFreeSeatOfClass()
        {
            var seat = _aircraft.Seats.FindFreeSeat(SeatClass.Economy);
            Assert.AreEqual("6A", seat.Label());
            Assert.AreEqual(SeatClass.Economy, seat.Class);
        }

        [Test]
        public void FindFreeSeat_ClassSoldOut_Throws()
        {
            for (int i = 0; i < 12; i++) _aircraft.Seats.FindFreeSeat(SeatClass.First).Reserve();
            Assert.Throws<SeatUnavailableException>(() => _aircraft.Seats.FindFreeSeat(SeatClass.First));
        }

        [Test]
        public void ReserveSeat_IsCaseInsensitive()
        {
            var seat = _aircraft.Seats.ReserveSeat("1a");
            Assert.AreEqual("1A", seat.Label());
            Assert.IsTrue(seat.IsOccupied);
        }

        [Test]
        public void ReserveSeat_UnknownLabel_Throws()
        {
            Assert.Throws<SeatUnavailableException>(() => _aircraft.Seats.ReserveSeat("99Z"));
        }

        [Test]
        public void ReserveSeat_AlreadyTaken_Throws()
        {
            _aircraft.Seats.ReserveSeat("1A");
            Assert.Throws<SeatUnavailableException>(() => _aircraft.Seats.ReserveSeat("1A"));
        }

        [Test]
        public void ReleaseSeat_FreesSeat_AndIgnoresUnknownLabel()
        {
            _aircraft.Seats.ReserveSeat("1A");
            _aircraft.Seats.ReleaseSeat("1a");
            Assert.AreEqual(180, _aircraft.Seats.CountFree());
            Assert.DoesNotThrow(() => _aircraft.Seats.ReleaseSeat("99Z"));
        }

        [Test]
        public void OccupancyPercent_ReflectsReservedSeats()
        {
            Assert.AreEqual(0, _aircraft.Seats.OccupancyPercent(), 1e-9);
            for (int i = 0; i < 18; i++) _aircraft.Seats.FindFreeSeat(SeatClass.Business).Reserve();
            Assert.AreEqual(10, _aircraft.Seats.OccupancyPercent(), 1e-9);
            Assert.AreEqual(162, _aircraft.Seats.CountFree());
        }
    }

    [TestFixture]
    public class AircraftTests
    {
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_EmptyRegistration_Throws(string reg)
        {
            Assert.Throws<InvalidFlightDataException>(() => new Aircraft(reg, TestData.A320(), 2015, 2, 1000));
        }

        [Test]
        public void Constructor_NullRegistration_Throws()
        {
            Assert.Throws<InvalidFlightDataException>(() => new Aircraft(null, TestData.A320(), 2015, 2, 1000));
        }

        [Test]
        public void Constructor_InitialState()
        {
            var a = TestData.MakeAircraft("PH-AAA", null, 2, 24000);
            Assert.AreEqual(AircraftStatus.Active, a.Status);
            Assert.AreEqual(2, a.Engines.Count);
            Assert.AreEqual("PH-AAA-E1", a.Engines[0].SerialNumber);
            Assert.AreEqual("PH-AAA-E2", a.Engines[1].SerialNumber);
            Assert.AreEqual(24000, a.Fuel.CapacityL, 1e-9);
            Assert.AreEqual(180, a.Seats.Seats.Count);
        }

        [Test]
        public void GetAge_IsCurrentYearMinusManufactureYear()
        {
            Assert.AreEqual(DateTime.Today.Year - 2015, TestData.MakeAircraft().GetAge());
        }

        [Test]
        public void IsAirworthy_ActiveWithFreshEngines_True()
        {
            Assert.IsTrue(TestData.MakeAircraft().IsAirworthy());
        }

        [Test]
        public void IsAirworthy_InMaintenance_False()
        {
            var a = TestData.MakeAircraft();
            a.SendToMaintenance();
            Assert.AreEqual(AircraftStatus.Maintenance, a.Status);
            Assert.IsFalse(a.IsAirworthy());
        }

        [Test]
        public void IsAirworthy_EngineNeedsOverhaul_False()
        {
            var a = TestData.MakeAircraft();
            a.RegisterFlightTime(20000); // ресурс двигателя - 20000 ч
            Assert.IsFalse(a.IsAirworthy());
        }

        [Test]
        public void RegisterFlightTime_AddsHoursToEveryEngine()
        {
            var a = TestData.MakeAircraft();
            a.RegisterFlightTime(3.5);
            Assert.IsTrue(a.Engines.All(e => Math.Abs(e.HoursSinceOverhaul - 3.5) < 1e-9));
        }

        [Test]
        public void ReturnToService_OverhaulsWornEngines_AndActivates()
        {
            var a = TestData.MakeAircraft();
            a.RegisterFlightTime(25000);
            a.SendToMaintenance();
            a.ReturnToService();
            Assert.AreEqual(AircraftStatus.Active, a.Status);
            Assert.IsTrue(a.IsAirworthy());
            Assert.IsTrue(a.Engines.All(e => e.HoursSinceOverhaul == 0));
        }

        [Test]
        public void ReturnToService_Retired_Throws()
        {
            var a = TestData.MakeAircraft();
            a.Retire();
            Assert.AreEqual(AircraftStatus.Retired, a.Status);
            Assert.Throws<AircraftNotAvailableException>(() => a.ReturnToService());
        }

        [Test]
        public void CanServe_ShortRouteAndLongRunway_True()
        {
            Assert.IsTrue(TestData.MakeAircraft().CanServe(TestData.MakeRoute(370)));
        }

        [Test]
        public void CanServe_RouteLongerThanRange_False()
        {
            Assert.IsFalse(TestData.MakeAircraft().CanServe(TestData.MakeRoute(7000)));
        }

        [Test]
        public void CanServe_OriginRunwayTooShort_False()
        {
            var shortRunway = new Route(TestData.MakeAirport("AMS", "Netherlands", 500),
                                        TestData.MakeAirport("LHR", "UK"), 370);
            Assert.IsFalse(TestData.MakeAircraft().CanServe(shortRunway));
        }
    }

    [TestFixture]
    public class HangarTests
    {
        [Test]
        public void Park_UntilFull_ThenThrows()
        {
            var h = new Hangar("H1", 1);
            Assert.IsFalse(h.IsFull());
            h.Park(TestData.MakeAircraft("PH-AAA"));
            Assert.IsTrue(h.IsFull());
            Assert.AreEqual(0, h.FreeSpots());
            Assert.Throws<AircraftNotAvailableException>(() => h.Park(TestData.MakeAircraft("PH-BBB")));
        }

        [Test]
        public void Release_RemovesByRegistration()
        {
            var h = new Hangar("H1", 2);
            h.Park(TestData.MakeAircraft("PH-AAA"));
            Assert.IsTrue(h.Release("PH-AAA"));
            Assert.IsFalse(h.Release("PH-AAA"));
            Assert.AreEqual(2, h.FreeSpots());
        }
    }

    [TestFixture]
    public class MaintenanceRecordTests
    {
        [TestCase(50000, false)]
        [TestCase(50000.01, true)]
        [TestCase(100, false)]
        public void IsCostly_Boundary(double cost, bool expected)
        {
            var rec = new MaintenanceRecord(DateTime.Today, "Check", TestData.MakeMechanic(),
                                            TestData.MakeAircraft(), (decimal)cost);
            Assert.AreEqual(expected, rec.IsCostly());
        }

        [Test]
        public void GetSummary_ContainsKeyData()
        {
            var rec = new MaintenanceRecord(DateTime.Today, "Engine check", TestData.MakeMechanic(),
                                            TestData.MakeAircraft("PH-AAA"), 1234m);
            var s = rec.GetSummary();
            StringAssert.Contains("PH-AAA", s);
            StringAssert.Contains("Engine check", s);
            StringAssert.Contains("Rob Hendriks", s);
        }
    }
}
