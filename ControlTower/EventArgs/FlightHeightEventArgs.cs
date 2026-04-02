namespace ControlTower.EventArgs
{
    public class FlightHeightEventArgs : System.EventArgs
    {
        /// <summary>
        /// Event arguments passed when an airplane changes altitude.
        /// Carries the flight number and the new altitude value.
        /// </summary>
        private string flightNumber;
        private int newAltitude;


        // Gets the flight number of the airplane that changed altitude.
        public string FlightNumber
        {
            get { return flightNumber; }
        }

        // Gets the new altitude of the airplane after the change command.
        public int NewAltitude
        {
            get { return newAltitude; }
        }

        // Initializes a new instance of FlightHeightEventArgs.
        public FlightHeightEventArgs(string flightNumber, int newAltitude)
        {
            this.flightNumber = flightNumber;
            this.newAltitude = newAltitude;
        }
    }
}
