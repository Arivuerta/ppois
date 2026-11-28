
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineSystem
{
    /// <summary>
    /// A complaint that a passenger has filed about a flight.
    /// </summary>
    public class Complaint
    {
        /// <summary>
        /// Gets the passenger who filed the complaint.
        /// </summary>
        public Passenger Passenger { get; }
        /// <summary>
        /// Gets the flight that the complaint is about.
        /// </summary>
        public Flight Flight { get; }
        /// <summary>
        /// Gets the complaint text.
        /// </summary>
        public string Text { get; }
        /// <summary>
        /// Gets the processing status.
        /// </summary>
        public ComplaintStatus Status { get; private set; } = ComplaintStatus.Open;

        /// <summary>
        /// Initializes a new, open complaint.
        /// </summary>
        /// <param name="p">The passenger.</param>
        /// <param name="f">The flight.</param>
        /// <param name="text">The text of the complaint.</param>
        public Complaint(Passenger p, Flight f, string text)
        {
            Passenger = p; Flight = f; Text = text;
        }

        /// <summary>
        /// Determines whether the complaint is still unresolved.
        /// </summary>
        /// <returns><c>true</c> if the status is not resolved.</returns>
        public bool IsOpen() => Status != ComplaintStatus.Resolved;
        /// <summary>
        /// Escalates an open complaint. Complaints in any other status are unchanged.
        /// </summary>
        public void Escalate() { if (Status == ComplaintStatus.Open) Status = ComplaintStatus.Escalated; }
        /// <summary>
        /// Marks the complaint as resolved.
        /// </summary>
        public void Resolve() => Status = ComplaintStatus.Resolved;
    }
}
