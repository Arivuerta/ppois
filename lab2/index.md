# Airline System — лабораторная работа №2

Консольное приложение на C#, моделирующее работу **авиакомпании**: самолёты и аэропорты, рейсы и расписание, пассажиры и экипажи, билеты, оплата и возвраты, багаж и груз, программа лояльности, страхование, бортовое питание и работа службы поддержки.

## Содержание

1. [Предметная область](#предметная-область)
2. [Структура проекта](#структура-проекта)
3. [Классы](#классы)
4. [Перечисления](#перечисления-9)
5. [Исключения](#исключения-16)
6. [Итоговая статистика](#итоговая-статистика)


## Предметная область

Авиакомпания «Sky Dutch Airlines» (код `SD`) выполняет рейсы между аэропортами AMS, LHR и JFK. Система поддерживает следующие сценарии:

* **Парк и аэропорты** — самолёты со своими двигателями, топливной системой и схемой салона; аэропорты со взлётно-посадочными полосами, терминалами и гейтами; ангары и техобслуживание.
* **Рейсы** — создание рейса без конфликтов в расписании, назначение экипажа и гейта, посадка, вылет, прибытие, задержка, перенос и отмена.
* **Продажа билетов** — бронирование с проверкой виз, выбор места и класса, расчёт тарифа, промокоды, оплата банковской картой (проверка номера по алгоритму Луна), регистрация и посадочный талон, возврат средств.
* **Сервис** — багаж с тарифами за перевес, грузовые перевозки, страхование, бортовое питание, жалобы пассажиров, уведомления об изменении рейса, бонусные мили.
* **Персонал** — пилоты, бортпроводники, диспетчеры, авиадиспетчеры, механики, служба безопасности и наземный персонал; расчёт зарплаты и премий.

Все нарушения бизнес-правил сообщаются исключениями, унаследованными от `AirlineException`; консольное меню перехватывает их и продолжает работу.

## Структура проекта

```text
lab2.slnx                    решение (проекты lab2 и lab2.Tests)
├── lab2/                    приложение
│   ├── Aircraft/            самолёты, двигатели, топливо, салон, ангар, обслуживание   (8 файлов)
│   ├── Airport/             аэропорт, полосы, терминалы, гейты, стойка, погода          (6 файлов)
│   ├── Booking/             бронь, билет, оплата, возврат, багаж, скидки, страховка     (12 файлов)
│   ├── Flights/             рейс, расписание, задержки, статистика, груз, манифест      (7 файлов)
│   ├── People/              люди и сотрудники, лицензии, визы, экипаж                   (13 файлов)
│   ├── Services/            авиакомпания, питание, жалобы, поддержка, уведомления       (6 файлов)
│   ├── Enums/               перечисления                                                (1 файл)
│   ├── Exeptions/           классы исключений                                           (1 файл)
│   ├── AirlineAppcs.cs      состояние приложения и демонстрационные данные
│   ├── ConsoleMenu.cs       текстовый интерфейс (20 пунктов меню)
│   ├── Program.cs           точка входа
│   ├── docfx.json           конфигурация DocFX
│   ├── index.md, toc.yml    главная страница и навигация документации
│   └── articles/            статьи документации (на английском языке)
└── lab2.Tests/              модульные тесты NUnit
```


### Наследование

```text
Person
├── Passenger
└── Employee
    ├── Pilot                 (переопределяет CalculateBonus)
    ├── FlightAttendant
    ├── GroundStaff
    ├── Mechanic
    ├── Dispatcher
    ├── AirTrafficController
    └── SecurityOfficer
```

## Классы

Формат строки: `Класс | число полей | число методов | связанные классы`.

| Класс | Поля | Методы | Ассоциации (связанные классы) |
|---|---|---|---|
| AirTrafficController | 2 | 3 | Employee, Flight, Runway, WeatherReport |
| Aircraft | 7 | 8 | AircraftType, Engine, FuelSystem, Route, SeatMap |
| AircraftType | 5 | 4 | — |
| Airline | 9 | 10 | Aircraft, AircraftType, Booking, Employee, Flight, FlightSchedule, Hangar, Passenger, Pilot, Route, Ticket |
| AirlineApp | 23 | 4 | AirTrafficController, Aircraft, AircraftType, Airline, Airport, Baggage, BaggagePolicy, Booking, Cargo, CheckInDesk, CustomerSupport, Discount, Dispatcher, Employee, FlightAttendant, FlightStatistics, Gate, GroundStaff, Hangar, Insurance, LoyaltyAccount, MaintenanceRecord, Mechanic, Notification, Passenger, PassengerManifest, PaymentCard, Pilot, PilotLicense, Route, Runway, SecurityOfficer, Terminal, Visa, WeatherReport |
| Airport | 7 | 7 | Gate, Runway, Terminal |
| Baggage | 5 | 3 | BaggagePolicy, Flight, Passenger |
| BaggagePolicy | 3 | 3 | Baggage |
| BoardingPass | 4 | 5 | Gate, Ticket |
| Booking | 6 | 6 | Discount, Passenger, Payment, PaymentCard, Ticket |
| Cargo | 4 | 3 | AircraftType |
| CateringOrder | 4 | 5 | Flight, Meal |
| CheckInDesk | 3 | 4 | Airport, BoardingPass, Gate, Ticket |
| Complaint | 4 | 4 | Flight, Passenger |
| ConsoleMenu | 1 | 30 | AirlineApp, Airport, Baggage, Booking, CateringOrder, Complaint, Crew, Flight, Insurance, Meal, Notification, Passenger, PassengerManifest, Payment, PaymentCard, PilotLicense, Refund, Route, Visa, WeatherReport |
| Crew | 3 | 5 | AircraftType, FlightAttendant, Pilot |
| CustomerSupport | 2 | 5 | Complaint, Employee, Passenger |
| Discount | 3 | 3 | — |
| Dispatcher | 2 | 3 | Employee, Flight, WeatherReport |
| Employee | 3 | 4 | Person |
| Engine | 3 | 5 | — |
| Fare | 4 | 4 | Route |
| Flight | 9 | 14 | Aircraft, Cargo, Crew, FlightDelay, Gate, Route, Ticket |
| FlightAttendant | 2 | 4 | Employee |
| FlightDelay | 4 | 3 | Flight |
| FlightSchedule | 2 | 6 | Aircraft, Flight |
| FlightStatistics | 3 | 6 | Booking, FlightDelay, FlightSchedule |
| FuelSystem | 3 | 5 | — |
| Gate | 3 | 3 | Flight |
| GroundStaff | 2 | 4 | Airport, Employee |
| Hangar | 3 | 5 | Aircraft |
| Insurance | 4 | 4 | Flight, Passenger |
| LoyaltyAccount | 3 | 4 | Passenger |
| MaintenanceRecord | 5 | 3 | Aircraft, Mechanic |
| Meal | 4 | 3 | — |
| Mechanic | 2 | 4 | Aircraft, Employee, MaintenanceRecord |
| Notification | 4 | 3 | FlightDelay, Passenger |
| Passenger | 4 | 5 | Person, Visa |
| PassengerManifest | 2 | 6 | Flight, Passenger |
| Payment | 5 | 4 | PaymentCard |
| PaymentCard | 4 | 5 | — |
| Person | 4 | 5 | — |
| Pilot | 3 | 5 | AircraftType, Employee, PilotLicense |
| PilotLicense | 4 | 4 | AircraftType |
| Program | 0 | 1 | AirlineApp, ConsoleMenu |
| Refund | 4 | 3 | Payment, PaymentCard, Ticket |
| Route | 3 | 4 | Airport |
| Runway | 4 | 4 | AircraftType |
| Seat | 4 | 4 | — |
| SeatMap | 3 | 6 | Aircraft, Seat |
| SecurityOfficer | 2 | 3 | Baggage, Employee, Passenger |
| Terminal | 3 | 4 | Gate |
| Ticket | 6 | 5 | Flight, Passenger, Seat |
| Visa | 3 | 4 | — |
| WeatherReport | 4 | 4 | Airport |

**Как считались показатели** (все значения получены разбором исходного кода):

* **Поля** — данные-члены класса: автосвойства и вычисляемые свойства (например, `Notification.IsSent`) плюс закрытые поля (`_app`, `_cargoId`, `_ticketCounter`). Унаследованные члены в классе-наследнике не учитываются.
* **Методы** — все методы класса (открытые и закрытые) **вместе с конструкторами**; аксессоры свойств не считаются. Явно не объявленный конструктор по умолчанию (`FlightSchedule`) не учитывается. Число конструкторов указано отдельно в итоговой статистике.
* **Ассоциации** — другие классы проекта (кроме перечислений и исключений), упоминаемые в классе: в типах полей и свойств (включая аргументы обобщённых типов `List<T>`, `Dictionary<K,V>`), в типах параметров и возвращаемых значений методов и конструкторов, в базовом классе, а также создаваемые через `new` внутри методов.

## Перечисления (9)

| Перечисление | Назначение | Значения |
|---|---|---|
| SeatClass | класс обслуживания места | Economy, Business, First |
| FlightStatus | статус рейса | Scheduled, Boarding, Departed, Arrived, Delayed, Cancelled |
| TicketStatus | статус билета | Booked, CheckedIn, Used, Cancelled |
| PaymentStatus | статус платежа | Pending, Paid, Failed, Refunded |
| PaymentMethod | способ оплаты | Card, Cash, Miles |
| AircraftStatus | статус самолёта | Active, Maintenance, Retired |
| LoyaltyTier | уровень программы лояльности | Basic, Silver, Gold, Platinum |
| ComplaintStatus | статус жалобы | Open, Escalated, Resolved |
| MealType | тип питания | Standard, Vegetarian, Vegan, Child |

## Исключения (16)

Все исключения находятся в файле `Exeptions/Exeptions.cs`, наследуются от `AirlineException` (а тот — от `System.Exception`) и имеют один конструктор с сообщением об ошибке.

| Исключение | Базовый класс | Когда выбрасывается |
|---|---|---|
| AirlineException | Exception | Базовый класс всех доменных ошибок системы |
| AircraftNotAvailableException | AirlineException | Самолёт, ангар или гейт недоступен: занят, заполнен, списан или не подходит для маршрута |
| AirportClosedException | AirlineException | Аэропорт или стойка регистрации закрыты |
| BaggageOverweightException | AirlineException | Превышен допустимый вес багажа или грузоподъёмность самолёта |
| BookingNotFoundException | AirlineException | Бронь с указанным кодом не найдена |
| CrewShortageException | AirlineException | Экипаж не назначен или не хватает бортпроводников |
| ExpiredDocumentException | AirlineException | Просрочен документ (карта, виза, медсправка) или отсутствует виза |
| FlightNotFoundException | AirlineException | Рейс с указанным номером не найден |
| InsufficientFundsException | AirlineException | Недостаточно денег на карте или миль на бонусном счёте |
| InvalidFlightDataException | AirlineException | Некорректные данные рейса, маршрута, аэропорта, билета или груза; операция недопустима в текущем статусе рейса |
| InvalidLicenseException | AirlineException | Номер лицензии пуст или лицензия пилота просрочена |
| InvalidPassengerDataException | AirlineException | Некорректные личные данные: имя, дата рождения, паспорт, e-mail, отрицательная зарплата |
| InvalidPromoCodeException | AirlineException | Промокод просрочен или недействителен |
| PaymentFailedException | AirlineException | Платёж или возврат невозможен (неверная карта, сумма ≤ 0, бронь уже оплачена) |
| PilotNotQualifiedException | AirlineException | Пилот не имеет допуска к данному типу самолёта |
| SeatUnavailableException | AirlineException | Место занято или не существует, нет свободных мест класса, манифест переполнен |

## Итоговая статистика

| Показатель | Значение |
|---|---|
| Классы (без исключений) | 55 |
| Перечисления | 9 (34 значения) |
| Исключения | 16 |
| Поля | 222 |
| Поведения (методы) | 275 |
| — из них конструкторы | 52 |
| Ассоциации | 163 |