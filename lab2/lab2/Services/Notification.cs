
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// A message that is sent to a passenger by e-mail.
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// Gets the recipient.
        /// </summary>
        public Passenger Recipient { get; }
        /// <summary>
        /// Gets the message text.
        /// </summary>
        public string Message { get; }
        /// <summary>
        /// Gets the time of sending, or <c>null</c> if it has not been sent.
        /// </summary>
        public DateTime? SentAt { get; private set; }
        /// <summary>
        /// Gets a value that indicates whether the notification was sent.
        /// </summary>
        public bool IsSent => SentAt.HasValue;

        /// <summary>
        /// Initializes a new, unsent notification.
        /// </summary>
        /// <param name="recipient">The passenger who receives the message.</param>
        /// <param name="message">The message text.</param>
        public Notification(Passenger recipient, string message) { Recipient = recipient; Message = message; }

        /// <summary>
        /// Marks the notification as sent.
        /// </summary>
        /// <returns>A string that shows the e-mail address and the message.</returns>
        public string Send()
        {
            SentAt = DateTime.Now;
            return $"→ {Recipient.Email}: {Message}";
        }

        /// <summary>
        /// Creates a notification that informs a passenger about a delay.
        /// </summary>
        /// <param name="p">The recipient.</param>
        /// <param name="d">The delay.</param>
        /// <returns>The new notification.</returns>
        public static Notification ForDelay(Passenger p, FlightDelay d) => new Notification(p, d.Describe());
    }
}
