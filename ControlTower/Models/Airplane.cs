using ControlTower.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ControlTower.Models
{
    public class Airplane
    {
     
        private string id;
        private string name;
        private string destination;
        private double flightTime;
        private int flightHeight;
        private bool inFlight;
        private TimeOnly departureTime;
        private DispatcherTimer dispatchTimer;

      
        // Regular delegate  not an event and returns the new altitude.
        // ControlTower invokes this method directly to command an altitude change
        public Func<int, int> AltitudeChangeDelegate { get; set; }

       
        // Raised when the airplane has completed take-off
        public event EventHandler<AirplaneEventArgs> TakeOff;

    
        // Raised when the airplane has landed.
        public event EventHandler<AirplaneEventArgs> Landed;

      
        //Gets or sets the unique flight ID.
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        //Gets or sets the airplane name.
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        //Gets or sets the flight destination.
        public string Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        //Gets or sets the flight duration in hours.
        public double FlightTime
        {
            get { return flightTime; }
            set { flightTime = value; }
        }

        //Gets or sets the current altitude in feet.
        public int FlightHeight
        {
            get { return flightHeight; }
            set { flightHeight = value; }
        }

        //Gets or sets whether the airplane is currently airborne.
        public bool InFlight
        {
            get { return inFlight; }
            set { inFlight = value; }
        }

      
        // Initializes a new Airplane with the given flight details.
        public Airplane(string id, string name, string destination, double flightTime)
        {
            this.id = id;
            this.name = name;
            this.destination = destination;
            this.flightTime = flightTime;
            this.flightHeight = 0;
            this.inFlight = false;

            // Wire up the altitude change delegate to the local handler
            AltitudeChangeDelegate = ChangeAltitude;

            InitialiseTimer();
        }

     
        // Initialises the DispatcherTimer. Called once in the constructor.
        //The timer is started by StartFlight() and stopped when landing occurs.
        private void InitialiseTimer()
        {
            dispatchTimer = new DispatcherTimer();
            dispatchTimer.Interval = TimeSpan.FromSeconds(1);
            dispatchTimer.Tick += DispatcherTimer_Tick;
        }

        /// <summary>
        /// Starts the flight  and sets departure time, marks as airborne,
        /// sets initial altitude, starts the timer, and raises the TakeOff event.
        /// Called by ControlTower when take off is authorised.
        /// </summary>
        public void StartFlight()
        {
            departureTime = TimeOnly.FromDateTime(DateTime.Now);
            inFlight = true;
            flightHeight = 35000; // used Standard cruising altitude in feet
            dispatchTimer.Start();
            OnTakeOff(new AirplaneEventArgs(id, destination,
                $"{name}, {id}, heading for {destination} is taking off, " +
                $"{DateTime.Now:HH:mm:ss}"));
        }

        /// <summary>
        /// Fires once per second and Checks if enough time has elapsed to trigger landing.
        /// One second represents one flight hour.
        /// </summary>
        private void DispatcherTimer_Tick(object sender, System.EventArgs e)
        {
            TimeOnly currentTime = TimeOnly.FromDateTime(DateTime.Now);
            double secondsElapsed = (currentTime - departureTime).TotalSeconds;

            if (secondsElapsed >= flightTime)
                OnLanding();
        }

  
        // Stops the timer, resets flight state, and raises the Landed event.
        private void OnLanding()
        {
            dispatchTimer.Stop();
            inFlight = false;
            flightHeight = 0;
            string landedDestination = destination;
            destination = "Home";

            OnLanded(new AirplaneEventArgs(id, landedDestination,
                $"{name} has landed in {landedDestination}, " +
                $"{DateTime.Now:HH:mm:ss}"));
        }

      
        // Raises the TakeOff event safely. Checks for null subscribers.
        protected void OnTakeOff(AirplaneEventArgs e)
        {
            TakeOff?.Invoke(this, e);
        }

        // Raises the Landed event safely. Checks for null subscribers.
        protected void OnLanded(AirplaneEventArgs e)
        {
            Landed?.Invoke(this, e);
        }

       

        //Changes the airplane's altitude by the specified amount.
        // Only responds if the airplane is currently airborne.
        private int ChangeAltitude(int change)
        {
            flightHeight += change;
            return flightHeight;
        }

   
        // Returns a formatted string summarising the airplane's current state.
        // Used to populate the airplane list in the UI.
        public override string ToString()
        {
            string status = inFlight ? $"In flight — {flightHeight} ft" : "Grounded";
            return $"{name}  |  {id}  |  {destination}  |  {status}";
        }
    }
}
