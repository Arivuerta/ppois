using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// Cabin class of a seat or a fare.
    /// </summary>
    public enum SeatClass
    {
        /// <summary>Standard economy class.</summary>
        Economy,
        /// <summary>Business class.</summary>
        Business,
        /// <summary>First class.</summary>
        First
    }

    /// <summary>
    /// Life-cycle status of a flight.
    /// </summary>
    public enum FlightStatus
    {
        /// <summary>The flight is planned and has not started yet.</summary>
        Scheduled,
        /// <summary>Passengers are boarding.</summary>
        Boarding,
        /// <summary>The aircraft has left the gate and is in the air.</summary>
        Departed,
        /// <summary>The aircraft has reached its destination.</summary>
        Arrived,
        /// <summary>The departure was postponed.</summary>
        Delayed,
        /// <summary>The flight will not operate.</summary>
        Cancelled
    }

    /// <summary>
    /// Status of a ticket.
    /// </summary>
    public enum TicketStatus
    {
        /// <summary>The ticket is booked but the passenger has not checked in.</summary>
        Booked,
        /// <summary>The passenger has checked in.</summary>
        CheckedIn,
        /// <summary>The flight has been completed.</summary>
        Used,
        /// <summary>The ticket was cancelled and the seat released.</summary>
        Cancelled
    }

    /// <summary>
    /// Status of a payment.
    /// </summary>
    public enum PaymentStatus
    {
        /// <summary>The payment has been created but not processed yet.</summary>
        Pending,
        /// <summary>The payment was completed successfully.</summary>
        Paid,
        /// <summary>The payment was rejected.</summary>
        Failed,
        /// <summary>The money was returned to the payer.</summary>
        Refunded
    }

    /// <summary>
    /// Method used to pay for a booking.
    /// </summary>
    public enum PaymentMethod
    {
        /// <summary>Bank card.</summary>
        Card,
        /// <summary>Cash.</summary>
        Cash,
        /// <summary>Loyalty miles.</summary>
        Miles
    }

    /// <summary>
    /// Operational status of an aircraft.
    /// </summary>
    public enum AircraftStatus
    {
        /// <summary>The aircraft is in service.</summary>
        Active,
        /// <summary>The aircraft is being serviced.</summary>
        Maintenance,
        /// <summary>The aircraft was permanently withdrawn from service.</summary>
        Retired
    }

    /// <summary>
    /// Tier of the frequent-flyer programme.
    /// </summary>
    public enum LoyaltyTier
    {
        /// <summary>Entry tier (fewer than 20,000 miles).</summary>
        Basic,
        /// <summary>From 20,000 miles.</summary>
        Silver,
        /// <summary>From 50,000 miles.</summary>
        Gold,
        /// <summary>From 100,000 miles.</summary>
        Platinum
    }

    /// <summary>
    /// Processing status of a passenger complaint.
    /// </summary>
    public enum ComplaintStatus
    {
        /// <summary>The complaint was received and is waiting for processing.</summary>
        Open,
        /// <summary>The complaint was passed to a higher level.</summary>
        Escalated,
        /// <summary>The complaint is closed.</summary>
        Resolved
    }

    /// <summary>
    /// Diet type of an in-flight meal.
    /// </summary>
    public enum MealType
    {
        /// <summary>Regular meal.</summary>
        Standard,
        /// <summary>Vegetarian meal.</summary>
        Vegetarian,
        /// <summary>Vegan meal.</summary>
        Vegan,
        /// <summary>Children's meal.</summary>
        Child
    }
}
