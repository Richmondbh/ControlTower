using ControlTower.EventArgs;
using ControlTower.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlTower
{
    public class ControlTower
    {
        // List managing all registered airplanes
        private List<Airplane> airplanes;

        // Events published to MainWindow
        public event EventHandler<AirplaneEventArgs> TakeOffNotification;
        public event EventHandler<AirplaneEventArgs> LandedNotification;
        public event EventHandler<FlightHeightEventArgs> FlightHeightNotification;

        public ControlTower()
        {
            airplanes = new List<Airplane>();
        }

        // Returns the airplane list so MainWindow can refresh the UI
        public List<Airplane> GetAirplanes()
        {
            return airplanes;
        }

        // Adds a new airplane and immediately subscribes to its events
        public void AddAirplane(Airplane airplane)
        {
            airplanes.Add(airplane);
            SubscribeToAirplaneEvents(airplane);
        }

        // Removes a grounded airplane from the list (Grade A)
        public bool RemoveAirplane(int index)
        {
            if (!ControlIndex(index))
                return false;

            // Only allows removal if the plane is not airborne
            if (airplanes[index].InFlight)
                return false;

            UnsubscribeFromAirplaneEvents(airplanes[index]);
            airplanes.RemoveAt(index);
            return true;
        }

        // Authorises takeoff for the selected airplane
        public void OrderTakeoff(int index)
        {
            if (!ControlIndex(index))
                return;

            Airplane airplane = airplanes[index];

            // Prevent take-off if already airborne
            if (airplane.InFlight)
                return;

            // Resubscribe before each departure. This handles repeated flights
            SubscribeToAirplaneEvents(airplane);

            airplane.StartFlight();
        }

        // Commands an airborne airplane to change altitude via the regular delegate.
        public void FlightHeight(int index, int altitudeChange)
        {
            if (!ControlIndex(index))
                return;

            Airplane airplane = airplanes[index];

            // Delegate only invoked while airborne
            if (!airplane.InFlight)
            {
                OnFlightHeightNotification(new FlightHeightEventArgs(
                    airplane.Id, -1)); // -1 signals "not airborne" to MainWindow
                return;
            }

            // Invoke the regular delegate and  returns the new altitude
            int newAltitude = airplane.AltitudeChangeDelegate.Invoke(altitudeChange);

            OnFlightHeightNotification(new FlightHeightEventArgs(
                airplane.Id, newAltitude));
        }

        // Validates that the index is within the list bounds
        private bool ControlIndex(int index)
        {
            return index >= 0 && index < airplanes.Count;
        }

        // Subscribes to all events on a given airplane using +=
        private void SubscribeToAirplaneEvents(Airplane airplane)
        {
            // Unsubscribe first to avoid duplicate handlers on repeated departures
            UnsubscribeFromAirplaneEvents(airplane);
            airplane.TakeOff += AirplaneStatusUpdate;
            airplane.Landed += AirplaneStatusUpdate;
        }

        // Unsubscribes from all events on a given airplane using -=
        private void UnsubscribeFromAirplaneEvents(Airplane airplane)
        {
            airplane.TakeOff -= AirplaneStatusUpdate;
            airplane.Landed -= AirplaneStatusUpdate;
        }

        // Handles both TakeOff and Landed events from any airplane
        private void AirplaneStatusUpdate(object sender, AirplaneEventArgs e)
        {
            Airplane airplane = sender as Airplane;

            if (airplane != null && !airplane.InFlight)
            {
                // when Plane has landed unsubscribtion applies here
                UnsubscribeFromAirplaneEvents(airplane);
                OnLandedNotification(e);
            }
            else
            {
                OnTakeOffNotification(e);
            }
        }

        // Raises TakeOffNotification to MainWindow
        private void OnTakeOffNotification(AirplaneEventArgs e)
        {
            TakeOffNotification?.Invoke(this, e);
        }

        // Raises LandedNotification to MainWindow
        private void OnLandedNotification(AirplaneEventArgs e)
        {
            LandedNotification?.Invoke(this, e);
        }

        // Raises FlightHeightNotification to MainWindow
        private void OnFlightHeightNotification(FlightHeightEventArgs e)
        {
            FlightHeightNotification?.Invoke(this, e);
        }
    }
}
