using AirlineSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2
{
    /// <summary>
    /// The text-based user interface. It shows a menu, reads the user's choice and calls the operations of the application. Domain errors are caught and shown to the user.
    /// </summary>
    public class ConsoleMenu
    {
        private readonly AirlineApp _app;

        /// <summary>
        /// Initializes a new menu.
        /// </summary>
        /// <param name="app">The application state the menu operates on.</param>
        public ConsoleMenu(AirlineApp app) { _app = app; }

        /// <summary>
        /// Runs the main loop until the user chooses to exit.
        /// </summary>
        public void Run()
        {
            while (true)
            {
                PrintMenu();
                string choice = Read("Ваш выбор");
                if (choice == "0") { Console.WriteLine("До свидания!"); return; }
                try
                {
                    Dispatch(choice);
                }
                catch (AirlineException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{ex.GetType().Name}] {ex.Message}");
                    Console.ResetColor();
                }
                catch (FormatException ex)
                {
                    Console.WriteLine($"Ошибка ввода: {ex.Message}");
                }
                catch (ArgumentException ex)   // покрывает ArgumentNull и ArgumentOutOfRange
                {
                    Console.WriteLine($"Ошибка ввода: {ex.Message}");
                }
                Console.WriteLine("\nНажмите Enter..."); Console.ReadLine();
            }
        }

        private void PrintMenu()
        {
            Console.Clear();
            Console.WriteLine($"===== {_app.Airline.Name} =====");
            Console.WriteLine(@" 1. Расписание рейсов              11. Задержка рейса + уведомления
 2. Зарегистрировать пассажира     12. Отмена рейса
 3. Забронировать билет            13. Техобслуживание самолёта
 4. Оплатить бронь                 14. Бортовое питание
 5. Отмена брони + возврат         15. Жалобы пассажиров
 6. Регистрация / посадочный       16. Программа лояльности
 7. Добавить багаж                 17. Страхование
 8. Назначить экипаж и гейт        18. Груз на рейс
 9. Погода и разрешение на вылет   19. Статистика и зарплаты
10. Вылет / прибытие рейса         20. Демонстрация всех исключений
 0. Выход");
        }

        private void Dispatch(string c)
        {
            switch (c)
            {
                case "1": ShowSchedule(); break;
                case "2": AddPassenger(); break;
                case "3": BookTicket(); break;
                case "4": PayBooking(); break;
                case "5": CancelBooking(); break;
                case "6": CheckIn(); break;
                case "7": AddBaggage(); break;
                case "8": AssignCrewAndGate(); break;
                case "9": WeatherAndClearance(); break;
                case "10": DepartOrArrive(); break;
                case "11": DelayFlight(); break;
                case "12": CancelFlight(); break;
                case "13": MaintenanceMenu(); break;
                case "14": Catering(); break;
                case "15": Complaints(); break;
                case "16": LoyaltyMenu(); break;
                case "17": InsuranceMenu(); break;
                case "18": CargoMenu(); break;
                case "19": Statistics(); break;
                case "20": ExceptionDemo(); break;
                default: Console.WriteLine("Неизвестный пункт меню."); break;
            }
        }

        // ---------------- Ввод ----------------
        private static string Read(string prompt) { Console.Write($"{prompt}: "); return Console.ReadLine()?.Trim() ?? ""; }
        private static int ReadInt(string prompt) => int.TryParse(Read(prompt), out var v) ? v : throw new FormatException("Ожидалось целое число.");
        private static double ReadDouble(string prompt) => double.TryParse(Read(prompt), out var v) ? v : throw new FormatException("Ожидалось число.");

        private Flight PickFlight()
        {
            foreach (var f in _app.Airline.Schedule.Flights)
                Console.WriteLine($"  {f.Number}  {f.Route.GetCode()}  {f.DepartureTime:g}  [{f.Status}]");
            return _app.Airline.Schedule.FindByNumber(Read("Номер рейса"));
        }

        private Passenger PickPassenger()
        {
            for (int i = 0; i < _app.Passengers.Count; i++) Console.WriteLine($"  {i + 1}. {_app.Passengers[i].GetFullName()}");
            int n = ReadInt("№ пассажира");
            if (n < 1 || n > _app.Passengers.Count) throw new InvalidPassengerDataException("Нет такого пассажира.");
            return _app.Passengers[n - 1];
        }

        private Booking PickBooking()
        {
            foreach (var b in _app.Airline.Bookings)
                Console.WriteLine($"  {b.Code}: {b.Passenger.GetFullName()}, {b.Tickets.Count} бил., {b.TotalPrice():N2} €, оплата: {b.Payment?.Status.ToString() ?? "нет"}");
            return _app.Airline.FindBooking(Read("Код брони"));
        }

        // ---------------- Действия ----------------
        private void ShowSchedule()
        {
            foreach (var f in _app.Airline.Schedule.Flights)
                Console.WriteLine($"{f.Number} {f.Route.GetCode()} {f.Aircraft.RegistrationNumber}({f.Aircraft.Type.Name}) " +
                    $"{f.DepartureTime:g}→{f.GetArrivalTime():t} [{f.Status}] загрузка {f.GetLoadFactor():F1}%, свободно мест: {f.Aircraft.Seats.CountFree()}");
        }

        private void AddPassenger()
        {
            var p = new Passenger(_app.Passengers.Count + 1, Read("Имя"), Read("Фамилия"),
                DateTime.Parse(Read("Дата рождения (гггг-мм-дд)")), Read("Паспорт"), Read("Гражданство"), Read("E-mail"));
            _app.RegisterPassenger(p);
            if (Read("Добавить визу? (y/n)") == "y")
                p.AddVisa(new Visa(Read("Номер визы"), Read("Страна"), DateTime.Today.AddYears(1)));
            Console.WriteLine($"Пассажир {p.GetFullName()} зарегистрирован, возраст {p.GetAge()}.");
        }

        private void BookTicket()
        {
            var p = PickPassenger(); var f = PickFlight();
            var cls = (SeatClass)Enum.Parse(typeof(SeatClass), Read("Класс (Economy/Business/First)"), true);
            string seat = Read("Место (например 10A, Enter — любое)");
            var b = _app.Airline.BookTicket(p, f, cls, seat == "" ? null : seat);
            var t = b.Tickets[0];
            if (!_app.Manifests.TryGetValue(f.Number, out var man)) _app.Manifests[f.Number] = man = new PassengerManifest(f);
            man.Add(p);
            Console.WriteLine($"В манифесте рейса {f.Number}: {man.GetCount()} пасс. (несовершеннолетних: {man.CountMinors()})");
            Console.WriteLine($"Бронь {b.Code}: билет {t.Number}, место {t.Seat.Label()}, цена {t.Price:N2} €");
        }

        private void PayBooking()
        {
            var b = PickBooking();
            string code = Read("Промокод (Enter — без)");
            if (code != "")
            {
                if (!_app.Promo.TryGetValue(code.ToUpper(), out var d)) throw new InvalidPromoCodeException("Такого промокода нет.");
                b.ApplyDiscount(d);
            }
            b.Pay(_app.DemoCard);
            _app.Cards[b] = _app.DemoCard;
            Console.WriteLine($"Оплачено {b.Payment.Amount:N2} €. Остаток на карте: {_app.DemoCard.Balance:N2} €");
        }

        private void CancelBooking()
        {
            var b = PickBooking();
            decimal refundAmount = b.Tickets.Sum(Refund.Calculate);
            b.Cancel();
            if (b.Payment != null && b.Payment.IsPaid())
            {
                var r = new Refund(b.Payment, refundAmount, "Отмена пассажиром");
                r.Issue(_app.DemoCard);
                Console.WriteLine($"Возврат: {r.Amount:N2} €");
            }
            Console.WriteLine("Бронь отменена, места освобождены.");
        }

        private void CheckIn()
        {
            var b = PickBooking();
            var t = b.Tickets[0];
            if (!_app.Security.ScreenPassenger(t.Passenger)) throw new InvalidPassengerDataException("Не прошёл контроль.");
            if (b.Payment == null || !b.Payment.IsPaid()) throw new PaymentFailedException("Сначала оплатите бронь.");
            var gate = t.Flight.Gate ?? _app.Airports["AMS"].FindFreeGate();
            var pass = _app.Desk.CheckIn(t, gate);
            Console.WriteLine(pass.Describe());
            Console.WriteLine($"Посадка открыта сейчас? {pass.IsBoardingOpen(DateTime.Now)}");
        }

        private void AddBaggage()
        {
            var p = PickPassenger(); var f = PickFlight();
            var bag = new Baggage($"BG{_app.Baggage.Count + 1}", ReadDouble("Вес, кг"), p, f, Read("Хрупкий? (y/n)") == "y");
            if (!_app.Security.ScreenBaggage(bag)) throw new BaggageOverweightException("Багаж не прошёл досмотр.");
            decimal fee = bag.CalculateFee(_app.BaggagePolicy);
            _app.Baggage.Add(bag);
            Console.WriteLine($"Багаж {bag.Tag} принят. Сверх нормы: {bag.IsOverweight(_app.BaggagePolicy)}, доплата {fee:N2} €");
        }

        private void AssignCrewAndGate()
        {
            var f = PickFlight();
            var pilots = _app.Airline.FindPilots(f.Aircraft.Type);
            if (pilots.Count < 2) throw new PilotNotQualifiedException("Нет двух квалифицированных пилотов.");
            var crew = new Crew(pilots.OrderByDescending(p => p.FlightHours).First(), pilots.OrderBy(p => p.FlightHours).First());
            int need = Math.Max(1, f.Aircraft.Type.SeatCapacity / 50);
            foreach (var a in _app.Airline.Employees.OfType<FlightAttendant>().Take(need)) crew.AddAttendant(a);
            f.AssignCrew(crew);
            var gate = _app.Airports[f.Route.Origin.Code].FindFreeGate() ?? throw new AircraftNotAvailableException("Нет свободных гейтов.");
            f.AssignGate(gate);
            Console.WriteLine($"Экипаж: {crew.GetSize()} чел., командир {crew.Captain.GetFullName()}, гейт {gate.Number}");
        }

        private void WeatherAndClearance()
        {
            var f = PickFlight();
            var w = _app.Weather[f.Route.Origin.Code];
            Console.WriteLine(w.Describe());
            if (Read("Изменить ветер? (y/n)") == "y")
            {
                w = new WeatherReport(w.Airport, w.TemperatureC, ReadDouble("Ветер, км/ч"), w.VisibilityKm);
                _app.Weather[w.Airport.Code] = w;
            }
            bool approved = _app.Dispatcher.ApproveFlight(f, w);
            bool cleared = _app.Atc.ClearForTakeoff(f, w, w.Airport.Runways[0]);
            Console.WriteLine($"Диспетчер одобрил: {approved}; УВД разрешило: {cleared}; шторм: {w.IsStorm()}");
        }

        private void DepartOrArrive()
        {
            var f = PickFlight();
            if (Read("1 — вылет, 2 — прибытие") == "2") { f.Arrive(); Console.WriteLine($"Рейс прибыл. Командир налетал {f.Crew.Captain.FlightHours:F1} ч."); return; }
            var w = _app.Weather[f.Route.Origin.Code];
            if (!_app.Atc.ClearForTakeoff(f, w, w.Airport.Runways[0])) throw new AircraftNotAvailableException("УВД не дало разрешение на вылет.");
            f.StartBoarding();
            f.Depart();
            Console.WriteLine($"Рейс {f.Number} вылетел. Топлива осталось {f.Aircraft.Fuel.PercentFull():F0}%.");
        }

        private void DelayFlight()
        {
            var f = PickFlight();
            var d = f.Delay(ReadInt("Минуты"), Read("Причина"));
            _app.Stats.RecordDelay(d);
            Console.WriteLine(d.Describe() + (d.IsSevere() ? " (серьёзная!)" : ""));
            foreach (var t in f.Tickets)
            {
                var n = Notification.ForDelay(t.Passenger, d);
                _app.Notifications.Add(n);
                Console.WriteLine(n.Send());
            }
        }

        private void CancelFlight()
        {
            var f = PickFlight(); f.Cancel();
            foreach (var t in f.Tickets)
                Console.WriteLine(new Notification(t.Passenger, $"Рейс {f.Number} отменён.").Send());
            Console.WriteLine("Рейс отменён, билеты аннулированы.");
        }

        private void MaintenanceMenu()
        {
            foreach (var a in _app.Airline.Fleet) Console.WriteLine($"  {a.RegistrationNumber} {a.Type.Name} [{a.Status}] возраст {a.GetAge()}, к полёту годен: {a.IsAirworthy()}");
            var ac = _app.Airline.Fleet.FirstOrDefault(a => a.RegistrationNumber == Read("Регистрация")) ?? throw new AircraftNotAvailableException("Самолёт не найден.");
            if (Read("1 — осмотр, 2 — вернуть в строй") == "2") { _app.Hangar.Release(ac.RegistrationNumber); ac.ReturnToService(); Console.WriteLine("Самолёт в строю."); return; }
            if (!_app.Mechanic.CanWorkOn(ac)) throw new AircraftNotAvailableException("Квалификации механика недостаточно.");
            var rec = _app.Mechanic.Inspect(ac, Read("Описание работ"), (decimal)ReadDouble("Стоимость"));
            _app.Hangar.Park(ac);
            _app.Maintenance.Add(rec);
            Console.WriteLine(rec.GetSummary() + (rec.IsCostly() ? " [дорогое ТО]" : ""));
        }

        private void Catering()
        {
            var f = PickFlight();
            var order = new CateringOrder(f, ReadInt("Количество порций"));
            order.AddMeal(new Meal("Курица с рисом", MealType.Standard, 520, 6.5m));
            order.AddMeal(new Meal("Овощное рагу", MealType.Vegan, 310, 5m));
            Console.WriteLine($"Стоимость {order.TotalCost():N2} €, ср. калорийность {order.AverageCalories():F0}, хватит всем: {order.CoversAllPassengers()}");
            Console.WriteLine($"Лёгкое блюдо: {order.Meals[1].IsLight()}, спец. диета: {order.Meals[1].IsSpecialDiet()}");
        }

        private void Complaints()
        {
            string a = Read("1 — подать, 2 — обработать, 3 — статистика");
            if (a == "1") _app.Support.Register(new Complaint(PickPassenger(), PickFlight(), Read("Текст жалобы")));
            else if (a == "2")
            {
                for (int i = 0; i < _app.Support.Complaints.Count; i++) Console.WriteLine($"  {i}: {_app.Support.Complaints[i].Text} [{_app.Support.Complaints[i].Status}]");
                int idx = ReadInt("Индекс");
                if (Read("1 — эскалировать, 2 — закрыть") == "1") _app.Support.Complaints[idx].Escalate(); else _app.Support.ResolveComplaint(idx);
            }
            Console.WriteLine($"Открытых жалоб: {_app.Support.CountOpen()}");
        }

        private void LoyaltyMenu()
        {
            var p = PickPassenger(); var acc = _app.Loyalty[p];
            string a = Read("1 — начислить мили за перелёт (км), 2 — потратить мили");
            if (a == "1") acc.AddMiles(ReadDouble("Км")); else acc.SpendMiles(ReadInt("Мили"));
            Console.WriteLine($"{p.GetFullName()}: {acc.Miles} миль, уровень {acc.Tier}");
        }

        private void InsuranceMenu()
        {
            var p = PickPassenger(); var f = PickFlight();
            var ins = new Insurance(p, f, (decimal)ReadDouble("Сумма покрытия"));
            _app.Insurances.Add(ins);
            Console.WriteLine($"Премия {ins.Premium:N2} €, активна: {ins.IsActive(DateTime.Now)}, выплата при ущербе 1000: {ins.Claim(1000):N2} €");
        }

        private void CargoMenu()
        {
            var f = PickFlight();
            var cargo = _app.NewCargo(ReadDouble("Вес груза, кг"), Read("Описание"), Read("Опасный? (y/n)") == "y");
            Console.WriteLine($"Перегруз для типа: {cargo.IsOverweightFor(f.Aircraft.Type)}; тариф {cargo.CalculateFee():N2} €");
            f.LoadCargo(cargo);
            Console.WriteLine($"Груз загружен. Всего на борту: {f.CargoList.Sum(c => c.WeightKg)} кг.");
        }

        private void Statistics()
        {
            var s = _app.Stats;
            Console.WriteLine($"Выручка: {s.TotalRevenue():N2} €\nСредняя загрузка: {s.AverageLoadFactor():F1}%\n" +
                              $"Средняя задержка: {s.AverageDelayMinutes():F0} мин\nОтменено рейсов: {s.CancelledCount()}");
            Console.WriteLine($"Ежемесячный фонд зарплат: {_app.Airline.PayPayroll():N2} €");
            foreach (var p in _app.Airline.Employees.OfType<Pilot>())
                Console.WriteLine($"  {p.GetFullName()} ({p.Rank}), стаж {p.GetExperienceYears()} лет, налёт {p.FlightHours} ч, лицензия {p.License.DaysUntilExpiry()} дн.");
        }

        // ---------------- Демонстрация исключений ----------------
        private void ExceptionDemo()
        {
            var ams = _app.Airports["AMS"]; var jfk = _app.Airports["JFK"];
            var a320 = _app.Airline.Fleet[0]; var b777 = _app.Airline.Fleet[1];
            var cases = new List<(string, Action)>
        {
            ("InvalidPassengerDataException", () => new Passenger(9, "A", "B", new DateTime(2000,1,1), "AB1234567", "NL", "no-at-sign")),
            ("InvalidFlightDataException", () => new Route(ams, ams, 100)),
            ("FlightNotFoundException", () => _app.Airline.Schedule.FindByNumber("XX000")),
            ("BookingNotFoundException", () => _app.Airline.FindBooking("BK-NONE")),
            ("SeatUnavailableException", () => a320.Seats.ReserveSeat("99Z")),
            ("AircraftNotAvailableException", () => new Flight("SD999", new Route(ams, jfk, 20000), a320, DateTime.Now.AddDays(1))),
            ("PilotNotQualifiedException", () => new Crew(_app.Airline.FindPilots(a320.Type)[0], _app.Airline.FindPilots(a320.Type)[1]).ValidateFor(b777.Type)),
            ("InvalidLicenseException", () => new PilotLicense("", DateTime.Today, "A320", "EASA")),
            ("BaggageOverweightException", () => _app.BaggagePolicy.Evaluate(new Baggage("T", 80, _app.Passengers[0], _app.Airline.Schedule.Flights[0]))),
            ("InsufficientFundsException", () => new PaymentCard("4111111111111111", "X", DateTime.Today.AddYears(1), 1m).Charge(100m)),
            ("PaymentFailedException", () => new Payment(0m, PaymentMethod.Card)),
            ("InvalidPromoCodeException", () => _app.Promo["OLD50"].Apply(100m)),
            ("ExpiredDocumentException", () => new Payment(10m, PaymentMethod.Card).Process(new PaymentCard("4111111111111111", "X", DateTime.Today.AddDays(-1), 100m))),
            ("CrewShortageException", () => new Crew(_app.Airline.FindPilots(a320.Type)[0], _app.Airline.FindPilots(a320.Type)[1]).ValidateFor(a320.Type)),
            ("AirportClosedException", () => { var c = new Airport("TST", "Test", "T", "T"); c.CloseAirport(); c.FindFreeGate(); }),
        };
            foreach (var (name, action) in cases)
            {
                try { action(); Console.WriteLine($"{name}: исключение НЕ возникло!"); }
                catch (AirlineException ex) { Console.WriteLine($"✔ {ex.GetType().Name,-32} {ex.Message}"); }
            }
        }
    }

}
