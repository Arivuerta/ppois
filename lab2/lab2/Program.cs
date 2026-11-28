using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2
{
    /// <summary>
    /// Contains the program entry point.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The entry point. Creates the application, seeds it with sample data and starts the console menu.
        /// </summary>
        public static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            var app = new AirlineApp();
            app.Seed();
            new ConsoleMenu(app).Run();
        }
    }
}
