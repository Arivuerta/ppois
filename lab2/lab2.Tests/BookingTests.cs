using System;
using AirlineSystem;
using NUnit.Framework;

namespace lab2.Tests
{
    [TestFixture]
    public class BaggagePolicyTests
    {
        private BaggagePolicy _policy;

        [SetUp]
        public void SetUp() => _policy = new BaggagePolicy(23, 15m, 45);

        private static Baggage Bag(double kg) => new Baggage("TAG1", kg, null, null);

        [TestCase(10, 0)]
        [TestCase(23, 0)]
        [TestCase(30, 105)]   // (30 - 23) * 15
        [TestCase(45, 330)]   // (45 - 23) * 15
        public void Evaluate_CalculatesFee(double kg, int expectedFee)
        {
            Assert.AreEqual((decimal)expectedFee, _policy.Evaluate(Bag(kg)));
        }

        [Test]
        public void Evaluate_OverMaxWeight_Throws()
        {
            Assert.Throws<BaggageOverweightException>(() => _policy.Evaluate(Bag(45.1)));
        }

        [TestCase(45, true)]
        [TestCase(45.1, false)]
        public void IsAllowed_Boundary(double kg, bool expected)
        {
            Assert.AreEqual(expected, _policy.IsAllowed(Bag(kg)));
        }

        [TestCase(23, false)]
        [TestCase(23.1, true)]
        public void Baggage_IsOverweight_Boundary(double kg, bool expected)
        {
            Assert.AreEqual(expected, Bag(kg).IsOverweight(_policy));
        }

        [Test]
        public void Baggage_CalculateFee_DelegatesToPolicy()
        {
            Assert.AreEqual(105m, Bag(30).CalculateFee(_policy));
        }
    }

    [TestFixture]
    public class BoardingPassTests
    {
        private Flight _flight;
        private Ticket _ticket;

        [SetUp]
        public void SetUp()
        {
            _flight = TestData.MakeFlight(DateTime.Now.AddDays(1));
            _ticket = TestData.MakeTicket(_flight, 100m, 12, 'C');
        }

        [Test]
        public void IsBoardingOpen_BetweenBoardingTimeAndDeparture()
        {
            var boarding = _flight.DepartureTime.AddMinutes(-40);
            var pass = new BoardingPass(_ticket, new Gate("G1"), boarding, 1);

            Assert.IsFalse(pass.IsBoardingOpen(boarding.AddMinutes(-1)));
            Assert.IsTrue(pass.IsBoardingOpen(boarding));
            Assert.IsTrue(pass.IsBoardingOpen(_flight.DepartureTime.AddMinutes(-1)));
            Assert.IsFalse(pass.IsBoardingOpen(_flight.DepartureTime));
        }

        [Test]
        public void Getters_ReturnSeatAndPassengerName()
        {
            var pass = new BoardingPass(_ticket, new Gate("G1"), DateTime.Now, 1);
            Assert.AreEqual("12C", pass.GetSeatLabel());
            Assert.AreEqual("Anna Test", pass.GetPassengerName());
        }

        [Test]
        public void Describe_ContainsFlightSeatAndGate()
        {
            var text = new BoardingPass(_ticket, new Gate("G7"), DateTime.Now, 2).Describe();
            StringAssert.Contains("SD101", text);
            StringAssert.Contains("12C", text);
            StringAssert.Contains("G7", text);
        }

        [Test]
        public void Describe_WithoutGate_UsesDash()
        {
            var text = new BoardingPass(_ticket, null, DateTime.Now, 1).Describe();
            StringAssert.Contains("\u2014", text);
        }
    }

    [TestFixture]
    public class DiscountTests
    {
        [Test]
        public void IsValid_IgnoresTimeOfDay_AndChecksExpiry()
        {
            var d = new Discount("X", 10, new DateTime(2030, 1, 1));
            Assert.IsTrue(d.IsValid(new DateTime(2030, 1, 1, 23, 59, 0)));
            Assert.IsFalse(d.IsValid(new DateTime(2030, 1, 2)));
        }

        [TestCase(0, false)]
        [TestCase(-5, false)]
        [TestCase(100, true)]
        [TestCase(101, false)]
        [TestCase(10, true)]
        public void IsValid_PercentRange(int percent, bool expected)
        {
            var d = new Discount("X", percent, DateTime.Today.AddDays(10));
            Assert.AreEqual(expected, d.IsValid(DateTime.Today));
        }

        [Test]
        public void Apply_ReducesPriceByPercent()
        {
            var d = new Discount("X", 10, DateTime.Today.AddDays(10));
            Assert.AreEqual(90.00m, d.Apply(100m));
        }

        [Test]
        public void Apply_RoundsToTwoDecimals()
        {
            var d = new Discount("X", 33, DateTime.Today.AddDays(10));
            Assert.AreEqual(6.70m, d.Apply(10m)); // 10 * 0.67
            Assert.AreEqual(13.40m, d.Apply(20m));
        }

        [Test]
        public void Apply_ExpiredPromo_Throws()
        {
            var d = new Discount("OLD", 50, DateTime.Today.AddDays(-1));
            Assert.Throws<InvalidPromoCodeException>(() => d.Apply(100m));
        }
    }

    [TestFixture]
    public class FareTests
    {
        [Test]
        public void Calculate_AddsTax()
        {
            var fare = new Fare(TestData.MakeRoute(), SeatClass.Economy, 100m);
            Assert.AreEqual(112.00m, fare.Calculate());
        }

        [Test]
        public void Calculate_CustomTaxRate()
        {
            var fare = new Fare(TestData.MakeRoute(), SeatClass.Economy, 200m, 0.5m);
            Assert.AreEqual(300.00m, fare.Calculate());
        }

        [TestCase(SeatClass.Economy, 1.0)]
        [TestCase(SeatClass.Business, 2.5)]
        [TestCase(SeatClass.First, 4.0)]
        public void ClassMultiplier_KnownClasses(SeatClass cls, double expected)
        {
            Assert.AreEqual((decimal)expected, Fare.ClassMultiplier(cls));
        }

        [Test]
        public void ClassMultiplier_UnknownClass_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Fare.ClassMultiplier((SeatClass)99));
        }

        [TestCase(SeatClass.Economy, 90.00)]    // 1000 км * 0.09
        [TestCase(SeatClass.Business, 225.00)]  // * 2.5
        [TestCase(SeatClass.First, 360.00)]     // * 4
        public void ForRoute_BasePriceDependsOnDistanceAndClass(SeatClass cls, double expectedBase)
        {
            var fare = Fare.ForRoute(TestData.MakeRoute(1000), cls);
            Assert.AreEqual((decimal)expectedBase, fare.BasePrice);
            Assert.AreEqual(cls, fare.Class);
        }

        [Test]
        public void ForRoute_FinalPriceIncludesTax()
        {
            var fare = Fare.ForRoute(TestData.MakeRoute(1000), SeatClass.Economy);
            Assert.AreEqual(100.80m, fare.Calculate()); // 90 * 1.12
        }
    }

    [TestFixture]
    public class InsuranceTests
    {
        [Test]
        public void CalculatePremium_DomesticIsOnePercent()
        {
            var f = TestData.MakeFlight(null, null, TestData.MakeDomesticRoute());
            Assert.AreEqual(100.00m, Insurance.CalculatePremium(f, 10000m));
        }

        [Test]
        public void CalculatePremium_InternationalHasSurcharge()
        {
            var f = TestData.MakeFlight();
            Assert.AreEqual(150.00m, Insurance.CalculatePremium(f, 10000m));
        }

        [Test]
        public void Constructor_SetsPremium()
        {
            var ins = new Insurance(TestData.MakePassenger(), TestData.MakeFlight(), 10000m);
            Assert.AreEqual(150.00m, ins.Premium);
        }

        [TestCase(500, 500)]
        [TestCase(10000, 10000)]
        [TestCase(25000, 10000)]
        public void Claim_IsCappedByCoverage(int loss, int expected)
        {
            var ins = new Insurance(TestData.MakePassenger(), TestData.MakeFlight(), 10000m);
            Assert.AreEqual((decimal)expected, ins.Claim(loss));
        }

        [Test]
        public void IsActive_BeforeArrival_True_AfterArrival_False()
        {
            var f = TestData.MakeFlight();
            var ins = new Insurance(TestData.MakePassenger(), f, 1000m);
            Assert.IsTrue(ins.IsActive(DateTime.Now));
            Assert.IsFalse(ins.IsActive(f.GetArrivalTime().AddMinutes(1)));
        }

        [Test]
        public void IsActive_CancelledFlight_False()
        {
            var f = TestData.MakeFlight();
            var ins = new Insurance(TestData.MakePassenger(), f, 1000m);
            f.Cancel();
            Assert.IsFalse(ins.IsActive(DateTime.Now));
        }
    }

    [TestFixture]
    public class LoyaltyAccountTests
    {
        [Test]
        public void NewAccount_IsBasicWithZeroMiles()
        {
            var acc = new LoyaltyAccount(TestData.MakePassenger());
            Assert.AreEqual(0, acc.Miles);
            Assert.AreEqual(LoyaltyTier.Basic, acc.Tier);
        }

        [Test]
        public void AddMiles_Basic_OneMilePerKm()
        {
            var acc = new LoyaltyAccount(TestData.MakePassenger());
            acc.AddMiles(1000);
            Assert.AreEqual(1000, acc.Miles);
        }

        [TestCase(19999, LoyaltyTier.Basic)]
        [TestCase(20000, LoyaltyTier.Silver)]
        [TestCase(50000, LoyaltyTier.Gold)]
        [TestCase(100000, LoyaltyTier.Platinum)]
        public void AddMiles_UpdatesTier(int km, LoyaltyTier expected)
        {
            var acc = new LoyaltyAccount(TestData.MakePassenger());
            acc.AddMiles(km); // на уровне Basic коэффициент 1.0
            Assert.AreEqual(expected, acc.Tier);
        }

        [Test]
        public void AddMiles_HigherTierGetsBonusMultiplier()
        {
            var acc = new LoyaltyAccount(TestData.MakePassenger());
            acc.AddMiles(20000);              // -> Silver
            acc.AddMiles(1000);               // Silver: x1.25
            Assert.AreEqual(21250, acc.Miles);
        }

        [Test]
        public void SpendMiles_ReducesBalance()
        {
            var acc = new LoyaltyAccount(TestData.MakePassenger());
            acc.AddMiles(1000);
            acc.SpendMiles(400);
            Assert.AreEqual(600, acc.Miles);
        }

        [Test]
        public void SpendMiles_MoreThanBalance_Throws_AndKeepsBalance()
        {
            var acc = new LoyaltyAccount(TestData.MakePassenger());
            acc.AddMiles(100);
            Assert.Throws<InsufficientFundsException>(() => acc.SpendMiles(101));
            Assert.AreEqual(100, acc.Miles);
        }
    }

    [TestFixture]
    public class PaymentCardTests
    {
        [TestCase("4111111111111111", true)]
        [TestCase("4111111111111112", false)]  // неверная контрольная цифра
        [TestCase("411111111111111", false)]   // 15 цифр
        [TestCase("41111111111111111", false)] // 17 цифр
        [TestCase("411111111111111a", false)]  // нецифровой символ
        [TestCase("", false)]
        public void ValidateNumber_LuhnAndFormat(string number, bool expected)
        {
            Assert.AreEqual(expected, TestData.MakeCard(100m, number).ValidateNumber());
        }

        [Test]
        public void IsExpired_YesterdayExpired_TodayStillValid()
        {
            Assert.IsTrue(TestData.MakeCard(100m, "4111111111111111", -1).IsExpired());
            Assert.IsFalse(TestData.MakeCard(100m, "4111111111111111", 0).IsExpired());
        }

        [Test]
        public void Charge_ReducesBalance()
        {
            var c = TestData.MakeCard(100m);
            c.Charge(40m);
            Assert.AreEqual(60m, c.Balance);
        }

        [Test]
        public void Charge_ExactBalance_Allowed()
        {
            var c = TestData.MakeCard(100m);
            c.Charge(100m);
            Assert.AreEqual(0m, c.Balance);
        }

        [Test]
        public void Charge_MoreThanBalance_Throws_AndKeepsBalance()
        {
            var c = TestData.MakeCard(100m);
            Assert.Throws<InsufficientFundsException>(() => c.Charge(100.01m));
            Assert.AreEqual(100m, c.Balance);
        }

        [Test]
        public void Deposit_PositiveAdds_NonPositiveIgnored()
        {
            var c = TestData.MakeCard(100m);
            c.Deposit(50m);
            c.Deposit(-10m);
            c.Deposit(0m);
            Assert.AreEqual(150m, c.Balance);
        }
    }

    [TestFixture]
    public class PaymentTests
    {
        [TestCase(0)]
        [TestCase(-1)]
        public void Constructor_NonPositiveAmount_Throws(int amount)
        {
            Assert.Throws<PaymentFailedException>(() => new Payment(amount, PaymentMethod.Card));
        }

        [Test]
        public void NewPayment_IsPending()
        {
            var p = new Payment(10m, PaymentMethod.Card);
            Assert.AreEqual(PaymentStatus.Pending, p.Status);
            Assert.IsFalse(p.IsPaid());
        }

        [Test]
        public void Process_ValidCard_ChargesAndMarksPaid()
        {
            var card = TestData.MakeCard(500m);
            var p = new Payment(120m, PaymentMethod.Card);
            p.Process(card);
            Assert.IsTrue(p.IsPaid());
            Assert.AreEqual(380m, card.Balance);
        }

        [Test]
        public void Process_InvalidNumber_FailsWithoutCharging()
        {
            var card = TestData.MakeCard(500m, "4111111111111112");
            var p = new Payment(120m, PaymentMethod.Card);
            Assert.Throws<PaymentFailedException>(() => p.Process(card));
            Assert.AreEqual(PaymentStatus.Failed, p.Status);
            Assert.AreEqual(500m, card.Balance);
        }

        [Test]
        public void Process_ExpiredCard_FailsWithoutCharging()
        {
            var card = TestData.MakeCard(500m, "4111111111111111", -1);
            var p = new Payment(120m, PaymentMethod.Card);
            Assert.Throws<ExpiredDocumentException>(() => p.Process(card));
            Assert.AreEqual(PaymentStatus.Failed, p.Status);
            Assert.AreEqual(500m, card.Balance);
        }

        [Test]
        public void Process_InsufficientFunds_FailsAndRethrows()
        {
            var card = TestData.MakeCard(50m);
            var p = new Payment(120m, PaymentMethod.Card);
            Assert.Throws<InsufficientFundsException>(() => p.Process(card));
            Assert.AreEqual(PaymentStatus.Failed, p.Status);
            Assert.AreEqual(50m, card.Balance);
        }

        [Test]
        public void MarkRefunded_ChangesStatus()
        {
            var p = new Payment(10m, PaymentMethod.Card);
            p.Process(TestData.MakeCard());
            p.MarkRefunded();
            Assert.AreEqual(PaymentStatus.Refunded, p.Status);
            Assert.IsFalse(p.IsPaid());
        }
    }

    [TestFixture]
    public class RefundTests
    {
        [TestCase(100, 1000, 1000)]  // больше 72 часов - полный возврат
        [TestCase(48, 1000, 700)]    // 24..72 часа - 70%
        [TestCase(10, 1000, 0)]      // менее 24 часов - ничего
        public void Calculate_DependsOnHoursBeforeDeparture(int hoursAhead, int price, int expected)
        {
            var flight = TestData.MakeFlight(DateTime.Now.AddHours(hoursAhead));
            var ticket = TestData.MakeTicket(flight, price);
            Assert.AreEqual((decimal)expected, Refund.Calculate(ticket));
        }

        [Test]
        public void Issue_PaidPayment_DepositsMoneyAndMarksRefunded()
        {
            var card = TestData.MakeCard(500m);
            var payment = new Payment(100m, PaymentMethod.Card);
            payment.Process(card);
            Assert.AreEqual(400m, card.Balance);

            var refund = new Refund(payment, 100m, "Cancelled by passenger");
            refund.Issue(card);

            Assert.AreEqual(500m, card.Balance);
            Assert.AreEqual(PaymentStatus.Refunded, payment.Status);
        }

        [Test]
        public void Issue_UnpaidPayment_Throws()
        {
            var card = TestData.MakeCard(500m);
            var refund = new Refund(new Payment(100m, PaymentMethod.Card), 100m, "x");
            Assert.Throws<PaymentFailedException>(() => refund.Issue(card));
            Assert.AreEqual(500m, card.Balance);
        }
    }

    [TestFixture]
    public class TicketTests
    {
        [Test]
        public void NewTicket_IsBooked()
        {
            Assert.AreEqual(TicketStatus.Booked, TestData.MakeTicket(TestData.MakeFlight()).Status);
        }

        [Test]
        public void CheckIn_ChangesStatus_SecondCheckInThrows()
        {
            var t = TestData.MakeTicket(TestData.MakeFlight());
            t.CheckIn();
            Assert.AreEqual(TicketStatus.CheckedIn, t.Status);
            Assert.Throws<InvalidFlightDataException>(() => t.CheckIn());
        }

        [Test]
        public void CheckIn_CancelledTicket_Throws()
        {
            var t = TestData.MakeTicket(TestData.MakeFlight());
            t.Cancel();
            Assert.Throws<InvalidFlightDataException>(() => t.CheckIn());
        }

        [Test]
        public void Cancel_SetsStatus_AndReleasesSeat()
        {
            var t = TestData.MakeTicket(TestData.MakeFlight());
            t.Seat.Reserve();
            t.Cancel();
            Assert.AreEqual(TicketStatus.Cancelled, t.Status);
            Assert.IsFalse(t.Seat.IsOccupied);
        }

        [Test]
        public void Cancel_Twice_IsIdempotent()
        {
            var t = TestData.MakeTicket(TestData.MakeFlight());
            t.Cancel();
            Assert.DoesNotThrow(() => t.Cancel());
            Assert.AreEqual(TicketStatus.Cancelled, t.Status);
        }

        [Test]
        public void MarkUsed_ChangesStatus()
        {
            var t = TestData.MakeTicket(TestData.MakeFlight());
            t.MarkUsed();
            Assert.AreEqual(TicketStatus.Used, t.Status);
        }

        [Test]
        public void IsRefundable_MoreThan24HoursAndBooked_True()
        {
            Assert.IsTrue(TestData.MakeTicket(TestData.MakeFlight(DateTime.Now.AddHours(48))).IsRefundable());
        }

        [Test]
        public void IsRefundable_LessThan24Hours_False()
        {
            Assert.IsFalse(TestData.MakeTicket(TestData.MakeFlight(DateTime.Now.AddHours(10))).IsRefundable());
        }

        [Test]
        public void IsRefundable_AlreadyCheckedIn_False()
        {
            var t = TestData.MakeTicket(TestData.MakeFlight(DateTime.Now.AddHours(48)));
            t.CheckIn();
            Assert.IsFalse(t.IsRefundable());
        }
    }

    [TestFixture]
    public class BookingTests
    {
        private Flight _flight;
        private Passenger _passenger;

        [SetUp]
        public void SetUp()
        {
            _flight = TestData.MakeFlight();
            _passenger = TestData.MakePassenger();
        }

        private Booking NewBooking(params decimal[] prices)
        {
            var b = new Booking("BK1", _passenger);
            for (int i = 0; i < prices.Length; i++)
                b.AddTicket(TestData.MakeTicket(_flight, prices[i], 1 + i, 'A', _passenger, "T" + i));
            return b;
        }

        [Test]
        public void TotalPrice_SumsTickets()
        {
            Assert.AreEqual(300m, NewBooking(100m, 200m).TotalPrice());
        }

        [Test]
        public void TotalPrice_NoTickets_IsZero()
        {
            Assert.AreEqual(0m, NewBooking().TotalPrice());
        }

        [Test]
        public void TotalPrice_IgnoresCancelledTickets()
        {
            var b = NewBooking(100m, 200m);
            b.Tickets[0].Cancel();
            Assert.AreEqual(200m, b.TotalPrice());
        }

        [Test]
        public void TotalPrice_AppliesDiscount()
        {
            var b = NewBooking(100m, 200m);
            b.ApplyDiscount(new Discount("D10", 10, DateTime.Today.AddDays(5)));
            Assert.AreEqual(270.00m, b.TotalPrice());
        }

        [Test]
        public void TotalPrice_ExpiredDiscount_Throws()
        {
            var b = NewBooking(100m);
            b.ApplyDiscount(new Discount("OLD", 50, DateTime.Today.AddDays(-1)));
            Assert.Throws<InvalidPromoCodeException>(() => b.TotalPrice());
        }

        [Test]
        public void Pay_ValidCard_CreatesPaidPayment()
        {
            var card = TestData.MakeCard(1000m);
            var b = NewBooking(100m, 200m);
            b.Pay(card);
            Assert.IsTrue(b.Payment.IsPaid());
            Assert.AreEqual(300m, b.Payment.Amount);
            Assert.AreEqual(700m, card.Balance);
        }

        [Test]
        public void Pay_InsufficientFunds_PaymentFailed()
        {
            var card = TestData.MakeCard(10m);
            var b = NewBooking(100m);
            Assert.Throws<InsufficientFundsException>(() => b.Pay(card));
            Assert.AreEqual(PaymentStatus.Failed, b.Payment.Status);
        }

        [Test]
        public void Pay_EmptyBooking_Throws()
        {
            Assert.Throws<PaymentFailedException>(() => NewBooking().Pay(TestData.MakeCard()));
        }

        [Test]
        public void AddTicket_AfterPayment_Throws()
        {
            var b = NewBooking(100m);
            b.Pay(TestData.MakeCard());
            Assert.Throws<PaymentFailedException>(() =>
                b.AddTicket(TestData.MakeTicket(_flight, 50m, 5, 'B', _passenger, "T9")));
        }

        [Test]
        public void Cancel_CancelsAllTickets()
        {
            var b = NewBooking(100m, 200m);
            b.Cancel();
            Assert.IsTrue(b.Tickets.TrueForAll(t => t.Status == TicketStatus.Cancelled));
        }
    }
}
