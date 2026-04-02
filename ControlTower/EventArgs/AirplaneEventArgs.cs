using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlTower.EventArgs
{
    /// <summary>
    /// Event arguments passed when an airplane takes off or lands.
    /// Carries flight details so subscribers don't need a direct reference to the Airplane.
    /// </summary>
    public class AirplaneEventArgs : System.EventArgs
    {
        private string flightNumber;
        private string destination;
        private string message;

      
        // Gets the flight number of the airplane that raised the event.
        public string FlightNumber
        {
            get { return flightNumber; }
        }

       
        // Gets the destination of the flight.
        public string Destination
        {
            get { return destination; }
        }

      
        // Gets the status message describing the event.
        public string Message
        {
            get { return message; }
        }

     
        // Initializes a new instance of AirplaneEventArgs.
        public AirplaneEventArgs(string flightNumber, string destination, string message)
        {
            this.flightNumber = flightNumber;
            this.destination = destination;
            this.message = message;
        }


    }
}

