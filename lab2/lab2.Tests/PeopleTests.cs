using System;
using AirlineSystem;
using NUnit.Framework;

namespace lab2.Tests
{
    [TestFixture]
    public class PersonTests
    {
        private static Person Make(DateTime birth) => new Person(1, "Ivan", "Petrov", birth);

        [TestCase("", "Petrov")]
        [TestCase("Ivan", "")]
        [TestCase("  ", "Petrov")]
        public void Constructor_EmptyName_Throws(string first, string last)
        {
            Assert.Throws<InvalidPassengerDataException>(() => new Person(1, first, last, new DateTime(1990, 1, 1)));
        }

        [Test]
        public void Constructor_BirthDateInFuture_Throws()
        {
            Assert.Throws<InvalidPassengerDataException>(() => Make(DateTime.Today.AddDays(1)));
        }

        [Test]
        public void Constructor_BirthToday_IsAllowed()
        {
            Assert.AreEqual(0, Make(DateTime.Today).GetAge());
        }

        [Test]
        public void GetFullName_JoinsFirstAndLast()
        {
            Assert.AreEqual("Ivan Petrov", Make(new DateTime(1990, 1, 1)).GetFullName());
        }

        [Test]
        public void GetAge_BirthdayToday_CountsFullYear()
        {
            var p = Make(DateTime.Today.AddYears(-18));
            Assert.AreEqual(18, p.GetAge());
            Assert.IsTrue(p.IsAdult());
        }

        [Test]
        public void GetAge_BirthdayTomorrow_NotYetFullYear()
        {
            var p = Make(DateTime.Today.AddYears(-18).AddDays(1));
            Assert.AreEqual(17, p.GetAge());
            Assert.IsFalse(p.IsAdult());
        }

        [Test]
        public void Rename_Valid_ChangesName()
        {
            var p = Make(new DateTime(1990, 1, 1));
            p.Rename("Petr", "Ivanov");
            Assert.AreEqual("Petr Ivanov", p.GetFullName());
        }

        [TestCase("", "X")]
        [TestCase("X", " ")]
        public void Rename_Invalid_Throws_AndKeepsName(string first, string last)
        {
            var p = Make(new DateTime(1990, 1, 1));
            Assert.Throws<InvalidPassengerDataException>(() => p.Rename(first, last));
            Assert.AreEqual("Ivan Petrov", p.GetFullName());
        }
    }

    [TestFixture]
    public class EmployeeTests
    {
        // +30 дней, чтобы не зависеть от високосных лет при делении на 365.25
        private static readonly DateTime Hired5Years = DateTime.Today.AddYears(-5).AddDays(-30);

        private static Employee Make(decimal salary = 1000m) =>
            new Employee(1, "A", "B", new DateTime(1990, 1, 1), "Clerk", salary, Hired5Years);

        [Test]
        public void Constructor_NegativeSalary_Throws()
        {
            Assert.Throws<InvalidPassengerDataException>(() => Make(-1m));
        }

        [Test]
        public void RaiseSalary_AddsPercent()
        {
            var e = Make(1000m);
            e.RaiseSalary(10m);
            Assert.AreEqual(1100m, e.Salary);
        }

        [TestCase(0)]
        [TestCase(-5)]
        public void RaiseSalary_NonPositivePercent_Throws(int percent)
        {
            Assert.Throws<ArgumentException>(() => Make().RaiseSalary(percent));
        }

        [Test]
        public void GetExperienceYears_CountsFullYears()
        {
            Assert.AreEqual(5, Make().GetExperienceYears());
        }

        [Test]
        public void CalculateBonus_Is2PercentOfSalaryPerYear()
        {
            Assert.AreEqual(100m, Make(1000m).CalculateBonus()); // 1000 * 0.02 * 5
        }
    }

    [TestFixture]
    public class PilotTests
    {
        [TestCase(2999.9, false)]
        [TestCase(3000, true)]
        public void Constructor_RankDependsOnHours(double hours, bool captain)
        {
            Assert.AreEqual(captain, TestData.MakePilot(1, "A320", 365, hours).IsCaptain());
        }

        [Test]
        public void AddFlightHours_PromotesToCaptainAtThreshold()
        {
            var p = TestData.MakePilot(1, "A320", 365, 2999);
            Assert.IsFalse(p.IsCaptain());
            p.AddFlightHours(1);
            Assert.AreEqual(3000, p.FlightHours, 1e-9);
            Assert.IsTrue(p.IsCaptain());
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void AddFlightHours_NonPositive_Throws(double hours)
        {
            Assert.Throws<ArgumentException>(() => TestData.MakePilot().AddFlightHours(hours));
        }

        [Test]
        public void CanFly_RequiresValidLicenseAndMatchingType()
        {
            Assert.IsTrue(TestData.MakePilot(1, "A320").CanFly(TestData.A320()));
            Assert.IsFalse(TestData.MakePilot(1, "B777").CanFly(TestData.A320()));
            Assert.IsFalse(TestData.MakePilot(1, "A320", -1).CanFly(TestData.A320()));
        }

        [Test]
        public void CalculateBonus_AddsHalfPerFlightHour()
        {
            // +30 дней, чтобы стаж гарантированно округлился до 5 полных лет
            var p = new Pilot(1, "Jan", "Visser", new DateTime(1985, 5, 20), 9000m,
                              DateTime.Today.AddYears(-5).AddDays(-30), TestData.MakeLicense(), 1000);
            // 9000 * 0.02 * 5 лет + 1000 ч * 0.5
            Assert.AreEqual(900m + 500m, p.CalculateBonus());
        }
    }

    [TestFixture]
    public class PilotLicenseTests
    {
        [TestCase("")]
        [TestCase("  ")]
        public void Constructor_EmptyNumber_Throws(string number)
        {
            Assert.Throws<InvalidLicenseException>(() =>
                new PilotLicense(number, DateTime.Today.AddYears(1), "A320", "EASA"));
        }

        [Test]
        public void IsValid_TodayStillValid_YesterdayExpired()
        {
            Assert.IsTrue(new PilotLicense("L", DateTime.Today, "A320", "EASA").IsValid());
            Assert.IsFalse(new PilotLicense("L", DateTime.Today.AddDays(-1), "A320", "EASA").IsValid());
        }

        [Test]
        public void CoversAircraft_IsCaseInsensitive()
        {
            var lic = new PilotLicense("L", DateTime.Today.AddYears(1), "a320", "EASA");
            Assert.IsTrue(lic.CoversAircraft(TestData.A320()));
            Assert.IsFalse(new PilotLicense("L", DateTime.Today.AddYears(1), "B777", "EASA").CoversAircraft(TestData.A320()));
        }

        [Test]
        public void DaysUntilExpiry_CountsFromToday()
        {
            Assert.AreEqual(10, new PilotLicense("L", DateTime.Today.AddDays(10), "A320", "EASA").DaysUntilExpiry());
            Assert.AreEqual(-3, new PilotLicense("L", DateTime.Today.AddDays(-3), "A320", "EASA").DaysUntilExpiry());
        }
    }

    [TestFixture]
    public class PassengerTests
    {
        [TestCase("AB123456", true)]    // 8 символов
        [TestCase("AB1234567", true)]   // 9 символов
        [TestCase("AB12345", false)]    // 7 символов
        [TestCase("AB12345678", false)] // 10 символов
        [TestCase("AB12 456", false)]   // пробел
        [TestCase("AB-23456", false)]   // дефис
        [TestCase("", false)]
        public void IsPassportFormatValid_Cases(string passport, bool expected)
        {
            Assert.AreEqual(expected, Passenger.IsPassportFormatValid(passport));
        }

        [Test]
        public void IsPassportFormatValid_Null_False()
        {
            Assert.IsFalse(Passenger.IsPassportFormatValid(null));
        }

        [Test]
        public void Constructor_InvalidPassport_Throws()
        {
            Assert.Throws<InvalidPassengerDataException>(() =>
                new Passenger(1, "A", "B", new DateTime(1990, 1, 1), "123", "NL", "a@b.c"));
        }

        [Test]
        public void Constructor_EmailWithoutAt_Throws()
        {
            Assert.Throws<InvalidPassengerDataException>(() =>
                new Passenger(1, "A", "B", new DateTime(1990, 1, 1), "AB123456", "NL", "mail.test"));
        }

        [Test]
        public void ChangeEmail_Valid_Updates_Invalid_Throws()
        {
            var p = TestData.MakePassenger();
            p.ChangeEmail("new@mail.test");
            Assert.AreEqual("new@mail.test", p.Email);
            Assert.Throws<InvalidPassengerDataException>(() => p.ChangeEmail("broken"));
            Assert.AreEqual("new@mail.test", p.Email);
        }

        [Test]
        public void HasValidVisa_OwnCountry_NoVisaNeeded()
        {
            Assert.IsTrue(TestData.MakePassenger(1, "Netherlands").HasValidVisa("netherlands", DateTime.Today));
        }

        [Test]
        public void HasValidVisa_ForeignCountryWithoutVisa_False()
        {
            Assert.IsFalse(TestData.MakePassenger(1, "Netherlands").HasValidVisa("UK", DateTime.Today));
        }

        [Test]
        public void HasValidVisa_ValidVisa_True_ExpiredOnDate_False()
        {
            var p = TestData.MakePassenger();
            p.AddVisa(new Visa("V1", "UK", DateTime.Today.AddDays(10)));
            Assert.IsTrue(p.HasValidVisa("uk", DateTime.Today));
            Assert.IsFalse(p.HasValidVisa("UK", DateTime.Today.AddDays(11)));
            Assert.IsFalse(p.HasValidVisa("USA", DateTime.Today));
        }
    }

    [TestFixture]
    public class VisaTests
    {
        [Test]
        public void IsValidOn_IncludesExpiryDate()
        {
            var v = new Visa("V", "UK", new DateTime(2030, 5, 5));
            Assert.IsTrue(v.IsValidOn(new DateTime(2030, 5, 5, 18, 0, 0)));
            Assert.IsFalse(v.IsValidOn(new DateTime(2030, 5, 6)));
        }

        [Test]
        public void IsForCountry_CaseInsensitive()
        {
            Assert.IsTrue(new Visa("V", "UK", DateTime.Today).IsForCountry("uk"));
            Assert.IsFalse(new Visa("V", "UK", DateTime.Today).IsForCountry("USA"));
        }

        [Test]
        public void DaysLeft_NeverNegative()
        {
            Assert.AreEqual(7, new Visa("V", "UK", DateTime.Today.AddDays(7)).DaysLeft());
            Assert.AreEqual(0, new Visa("V", "UK", DateTime.Today.AddDays(-7)).DaysLeft());
        }
    }

    [TestFixture]
    public class CrewTests
    {
        [Test]
        public void AddAttendant_ExpiredMedical_Throws()
        {
            var crew = new Crew(TestData.MakePilot(1), TestData.MakePilot(2));
            Assert.Throws<ExpiredDocumentException>(() => crew.AddAttendant(TestData.MakeAttendant(10, false)));
            Assert.AreEqual(0, crew.Attendants.Count);
        }

        [Test]
        public void GetSize_CountsPilotsAndAttendants()
        {
            Assert.AreEqual(5, TestData.MakeCrew("A320", 3).GetSize());
        }

        [TestCase(3, true)]
        [TestCase(4, false)]
        public void IsComplete_ComparesWithMinimum(int min, bool expected)
        {
            Assert.AreEqual(expected, TestData.MakeCrew("A320", 3).IsComplete(min));
        }
    }

    [TestFixture]
    public class FlightAttendantTests
    {
        [Test]
        public void AddLanguage_IgnoresDuplicates()
        {
            var fa = TestData.MakeAttendant();
            fa.AddLanguage("EN"); fa.AddLanguage("EN"); fa.AddLanguage("NL");
            Assert.AreEqual(2, fa.Languages.Count);
            Assert.IsTrue(fa.SpeaksLanguage("NL"));
            Assert.IsFalse(fa.SpeaksLanguage("DE"));
        }

        [Test]
        public void HasValidMedical_ExpiryToday_StillValid()
        {
            var today = new FlightAttendant(1, "A", "B", new DateTime(1990, 1, 1), 1000m,
                                            DateTime.Today.AddYears(-1), DateTime.Today);
            Assert.IsTrue(today.HasValidMedical());
            Assert.IsFalse(TestData.MakeAttendant(1, false).HasValidMedical());
        }
    }

    [TestFixture]
    public class GroundStaffTests
    {
        private static GroundStaff Make(string shift) =>
            new GroundStaff(1, "Sem", "Kok", new DateTime(1990, 1, 1), 2800m, DateTime.Today.AddYears(-2),
                            TestData.MakeAirport(), shift);

        [TestCase(7, false)]
        [TestCase(8, true)]
        [TestCase(19, true)]
        [TestCase(20, false)]
        public void IsOnShift_DayShift(int hour, bool expected)
        {
            Assert.AreEqual(expected, Make("Day").IsOnShift(hour));
        }

        [TestCase(7, true)]
        [TestCase(8, false)]
        [TestCase(19, false)]
        [TestCase(20, true)]
        [TestCase(0, true)]
        public void IsOnShift_NightShift(int hour, bool expected)
        {
            Assert.AreEqual(expected, Make("Night").IsOnShift(hour));
        }

        [Test]
        public void ChangeShift_And_AssignToAirport_UpdateState()
        {
            var g = Make("Day");
            var other = TestData.MakeAirport("LHR", "UK");
            g.ChangeShift("Night");
            g.AssignToAirport(other);
            Assert.AreEqual("Night", g.Shift);
            Assert.AreSame(other, g.BaseAirport);
        }
    }

    [TestFixture]
    public class MechanicTests
    {
        [Test]
        public void Inspect_SendsAircraftToMaintenance_AndReturnsRecord()
        {
            var m = TestData.MakeMechanic();
            var a = TestData.MakeAircraft();
            var rec = m.Inspect(a, "Check", 500m);
            Assert.AreEqual(AircraftStatus.Maintenance, a.Status);
            Assert.AreSame(a, rec.Aircraft);
            Assert.AreSame(m, rec.Mechanic);
            Assert.AreEqual(500m, rec.Cost);
        }

        [Test]
        public void CanWorkOn_RequiresLevel2_OrOldAircraft()
        {
            var young = TestData.MakeAircraft();                                        // 2015 -> моложе 15 лет
            var old = new Aircraft("PH-OLD", TestData.A320(), DateTime.Today.Year - 16, 1, 1000);
            Assert.IsFalse(TestData.MakeMechanic(1).CanWorkOn(young));
            Assert.IsTrue(TestData.MakeMechanic(2).CanWorkOn(young));
            Assert.IsTrue(TestData.MakeMechanic(1).CanWorkOn(old));
        }

        [Test]
        public void Promote_StopsAtLevel5()
        {
            var m = TestData.MakeMechanic(4);
            m.Promote(); m.Promote(); m.Promote();
            Assert.AreEqual(5, m.CertificationLevel);
        }
    }

    [TestFixture]
    public class DispatcherAndControllerTests
    {
        private static WeatherReport Weather(double wind, double vis) =>
            new WeatherReport(TestData.MakeAirport(), 10, wind, vis);

        private static Dispatcher MakeDispatcher() =>
            new Dispatcher(1, "Kees", "Boer", new DateTime(1985, 1, 1), 5000m, DateTime.Today.AddYears(-3), "Desk-A");

        private static AirTrafficController MakeAtc() =>
            new AirTrafficController(2, "Anke", "Dekker", new DateTime(1985, 1, 1), 6000m,
                                     DateTime.Today.AddYears(-3), "AMS-Tower", "Tower-1");

        [Test]
        public void ApproveFlight_GoodWeatherAndAirworthy_Approved_AndCounted()
        {
            var d = MakeDispatcher();
            Assert.IsTrue(d.ApproveFlight(TestData.MakeFlight(), Weather(10, 10)));
            Assert.AreEqual(1, d.ApprovedCount);
        }

        [Test]
        public void ApproveFlight_BadWeather_Rejected()
        {
            var d = MakeDispatcher();
            Assert.IsFalse(d.ApproveFlight(TestData.MakeFlight(), Weather(80, 10)));
            Assert.AreEqual(0, d.ApprovedCount);
        }

        [Test]
        public void ApproveFlight_AircraftNotAirworthy_Rejected()
        {
            var f = TestData.MakeFlight();
            f.Aircraft.SendToMaintenance();
            Assert.IsFalse(MakeDispatcher().ApproveFlight(f, Weather(10, 10)));
        }

        [Test]
        public void RescheduleFlight_DelegatesToFlight()
        {
            var f = TestData.MakeFlight();
            var t = DateTime.Now.AddDays(9);
            MakeDispatcher().RescheduleFlight(f, t);
            Assert.AreEqual(t, f.DepartureTime);
        }

        [Test]
        public void ClearForTakeoff_NeedsRunwayAndSafeWeather()
        {
            var atc = MakeAtc();
            var f = TestData.MakeFlight();
            Assert.IsTrue(atc.ClearForTakeoff(f, Weather(10, 10), new Runway("09", 3000)));
            Assert.IsFalse(atc.ClearForTakeoff(f, Weather(70, 10), new Runway("09", 3000)));
            Assert.IsFalse(atc.ClearForTakeoff(f, Weather(10, 10), new Runway("09", 500)));
        }

        [Test]
        public void ClearForLanding_NeedsOpenSuitableRunway()
        {
            var atc = MakeAtc();
            var f = TestData.MakeFlight();
            var runway = new Runway("09", 3000);
            Assert.IsTrue(atc.ClearForLanding(f, runway));
            runway.Close();
            Assert.IsFalse(atc.ClearForLanding(f, runway));
        }
    }

    [TestFixture]
    public class SecurityOfficerTests
    {
        private static SecurityOfficer Make() =>
            new SecurityOfficer(1, "Tim", "Peters", new DateTime(1985, 1, 1), 3300m,
                                DateTime.Today.AddYears(-3), "B-77", "Checkpoint-2");

        [TestCase(0, false)]
        [TestCase(-1, false)]
        [TestCase(59.9, true)]
        [TestCase(60, false)]
        public void ScreenBaggage_WeightRange(double kg, bool expected)
        {
            Assert.AreEqual(expected, Make().ScreenBaggage(new Baggage("T", kg, null, null)));
        }

        [Test]
        public void ScreenPassenger_AdultAndMinorBothPass()
        {
            Assert.IsTrue(Make().ScreenPassenger(TestData.MakePassenger()));
            Assert.IsTrue(Make().ScreenPassenger(
                TestData.MakePassenger(2, "Netherlands", DateTime.Today.AddYears(-5))));
        }
    }
}
