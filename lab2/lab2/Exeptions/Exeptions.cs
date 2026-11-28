using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    //Custom exception classes (16) 
    /// <summary>
    /// Base class for all domain errors of the airline system. Catch this type to handle any business-rule violation.
    /// </summary>
    public class AirlineException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AirlineException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public AirlineException(string message) : base(message) { }
    }
    /// <summary>
    /// Thrown when a payment card or a loyalty account has too little money or too few miles for an operation.
    /// </summary>
    public class InsufficientFundsException : AirlineException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InsufficientFundsException"/> class.
        /// </summary>
        /// <param name="m">The message that describes the error.</param>
        public InsufficientFundsException(string m) : base(m) { }
    }
    /// <summary>
    /// Thrown when personal data (name, birth date, passport, e-mail or salary) is invalid.
    /// </summary>
    public class InvalidPassengerDataException : AirlineException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPassengerDataException"/> class.
        /// </summary>
        /// <param name="m">The message that describes the error.</param>
        public InvalidPassengerDataException(string m) : base(m) { }
    }
    /// <summary>
    /// Thrown when flight, route, airport, ticket or cargo data is invalid or when an operation is not allowed in the current state of a flight.
    /// </summary>
    public class InvalidFlightDataException : AirlineException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidFlightDataException"/> class.
        /// </summary>
        /// <param name="m">The message that describes the error.</param>
        public InvalidFlightDataException(string m) : base(m) { }
    }
    /// <summary>
    /// Thrown when a flight with the requested number does not exist.
    /// </summary>
    public class FlightNotFoundException : AirlineException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlightNotFoundException"/> class.
        /// </summary>
        /// <param name="m">The message that describes the error.</param>
        public FlightNotFoundException(string m) : base(m) { }
    }
    /// <summary>
    /// Thrown when a booking with the requested code does not exist.
    /// </summary>
    public class BookingNotFoundException : AirlineException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BookingNotFoundException"/> class.
        /// </summary>
        /// <param name="m">The message that describes the error.</param>
        public BookingNotFoundException(string m) : base(m) { }
    }
    /// <summary>
    /// Thrown when a seat does not exist, is already occupied or no free seat of the requested class remains.
    /// </summary>
    public class SeatUnavailableException : AirlineException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SeatUnavailableException"/> class.
        /// </summary>
        /// <param name="m">The message that describes the error.</param>
        public SeatUnavailableException(string m) : base(m) { }
    }
    /// <summary>
    /// Thrown when an aircraft, hangar or gate cannot be used, for example because it is busy, retired, full or unsuitable for the route.
    /// </summary>
    public class AircraftNotAvailableException : AirlineException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AircraftNotAvailableException"/> class.
        /// </summary>
        /// <param name="m">The message that describes the error.</param>
        public AircraftNotAvailableException(string m) : base(m) { }
    }
    /// <summary>
    /// Thrown when a pilot has no type rating for the aircraft.
    /// </summary>
    public class PilotNotQualifiedException : AirlineException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PilotNotQualifiedException"/> class.
        /// </summary>
        /// <param name="m">The message that describes the error.</param>
        public PilotNotQualifiedException(string m) : base(m) { }
    }
    /// <summary>
    /// Thrown when a pilot licence is missing or has expired.
    /// </summary>
    public class InvalidLicenseException : AirlineException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidLicenseException"/> class.
        /// </summary>
        /// <param name="m">The message that describes the error.</param>
        public InvalidLicenseException(string m) : base(m) { }
    }
    /// <summary>
    /// Thrown when baggage or cargo exceeds the permitted weight.
    /// </summary>
    public class BaggageOverweightException : AirlineException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaggageOverweightException"/> class.
        /// </summary>
        /// <param name="m">The message that describes the error.</param>
        public BaggageOverweightException(string m) : base(m) { }
    }
    /// <summary>
    /// Thrown when a payment cannot be completed or a refund is not possible.
    /// </summary>
    public class PaymentFailedException : AirlineException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentFailedException"/> class.
        /// </summary>
        /// <param name="m">The message that describes the error.</param>
        public PaymentFailedException(string m) : base(m) { }
    }
    /// <summary>
    /// Thrown when a promo code is expired or invalid.
    /// </summary>
    public class InvalidPromoCodeException : AirlineException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPromoCodeException"/> class.
        /// </summary>
        /// <param name="m">The message that describes the error.</param>
        public InvalidPromoCodeException(string m) : base(m) { }
    }
    /// <summary>
    /// Thrown when a document such as a card, visa or medical certificate has expired or is missing.
    /// </summary>
    public class ExpiredDocumentException : AirlineException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpiredDocumentException"/> class.
        /// </summary>
        /// <param name="m">The message that describes the error.</param>
        public ExpiredDocumentException(string m) : base(m) { }
    }
    /// <summary>
    /// Thrown when a flight has no crew or not enough crew members.
    /// </summary>
    public class CrewShortageException : AirlineException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CrewShortageException"/> class.
        /// </summary>
        /// <param name="m">The message that describes the error.</param>
        public CrewShortageException(string m) : base(m) { }
    }
    /// <summary>
    /// Thrown when an operation needs an airport or check-in desk that is closed.
    /// </summary>
    public class AirportClosedException : AirlineException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AirportClosedException"/> class.
        /// </summary>
        /// <param name="m">The message that describes the error.</param>
        public AirportClosedException(string m) : base(m) { }
    }

}
