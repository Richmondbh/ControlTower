using ControlTower.EventArgs;
using ControlTower.Models;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;


namespace ControlTower
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AirportControlTower tower;
        public MainWindow()
        {
            InitializeComponent();
            tower = new AirportControlTower();
            SubscribeToTowerEvents();
            InitialiseGUI();
        }

        // Subscribe to ControlTower events so UI reacts to flight changes
        private void SubscribeToTowerEvents()
        {
            tower.TakeOffNotification += UpdateAirplaneStatus;
            tower.LandedNotification += UpdateAirplaneStatus;
            tower.FlightHeightNotification += UpdateAirplaneFlightHeight;
        }

        // Sets initial UI state
        private void InitialiseGUI()
        {
            UpdateListViewAirplanes();
            UpdateTextBoxes();
        }

        //Button handlers 

        private void BtnAddPlane_Click(object sender, RoutedEventArgs e)
        {
            string name = txtName.Text.Trim();
            string id = txtFlightId.Text.Trim();
            string destination = txtDestination.Text.Trim();

            if (!double.TryParse(txtFlightTime.Text.Trim(), out double flightTime) || flightTime <= 0)
            {
                MessageBox.Show("Please enter a valid flight time.", "Invalid Input");
                return;
            }

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(id) || string.IsNullOrEmpty(destination))
            {
                MessageBox.Show("Please fill in all fields.", "Invalid Input");
                return;
            }

            Airplane airplane = new Airplane(id, name, destination, flightTime);
            tower.AddAirplane(airplane);

            UpdateListViewAirplanes();
            ClearInputFields();
            LogMessage($"{name}, {id}, heading for {destination} added to the system.");
        }

        private void BtnTakeOff_Click(object sender, RoutedEventArgs e)
        {
            int index = lstAirplanes.SelectedIndex;

            if (index < 0)
            {
                MessageBox.Show("Please select an airplane first.", "No Selection");
                return;
            }

            tower.OrderTakeoff(index);
            UpdateListViewAirplanes();
        }

        private void BtnChangeAltitude_Click(object sender, RoutedEventArgs e)
        {
            int index = lstAirplanes.SelectedIndex;

            if (index < 0)
            {
                MessageBox.Show("Please select an airplane first.", "No Selection");
                return;
            }

            if (!int.TryParse(txtAltitudeChange.Text.Trim(), out int altitudeChange))
            {
                MessageBox.Show("Please enter a valid altitude change value.", "Invalid Input");
                return;
            }

            tower.FlightHeight(index, altitudeChange);
            UpdateListViewAirplanes();
        }

        private void BtnRemovePlane_Click(object sender, RoutedEventArgs e)
        {
            int index = lstAirplanes.SelectedIndex;

            if (index < 0)
            {
                MessageBox.Show("Please select an airplane first.", "No Selection");
                return;
            }

            bool removed = tower.RemoveAirplane(index);

            if (!removed)
            {
                MessageBox.Show("Cannot remove an airborne airplane.", "Remove Failed");
                return;
            }

            UpdateListViewAirplanes();
            LogMessage("Flight removed from the system.");
        }

        private void LstAirplanes_SelectionChanged(object sender,SelectionChangedEventArgs e)
        {
            UpdateTextBoxes();
        }

        // Event handlers from ControlTower 

        // Handles both TakeOff and Landed notifications
        private void UpdateAirplaneStatus(object sender, AirplaneEventArgs e)
        {
            UpdateListViewAirplanes();
            LogMessage(e.Message);
        }

        // Handles altitude change notifications
        private void UpdateAirplaneFlightHeight(object sender, FlightHeightEventArgs e)
        {
            UpdateListViewAirplanes();

            // -1 means the plane was not airborne when the command was issued.
            // I did not want to create a property like:  public bool IsAirborne 
            if (e.NewAltitude == -1)
                LogMessage($"{e.FlightNumber} is not airborne so altitude command was ignored.");
            else
                LogMessage($"{e.FlightNumber} changed altitude to {e.NewAltitude} ft.");
        }

        // UI helpers 

        // Refreshes the ListView with current airplane states
        private void UpdateListViewAirplanes()
        {
            lstAirplanes.Items.Clear();

            foreach (Airplane airplane in tower.GetAirplanes())
                lstAirplanes.Items.Add(airplane.ToString());
        }

        // Populates input fields when a plane is selected in the list
        private void UpdateTextBoxes()
        {
            int index = lstAirplanes.SelectedIndex;

            if (index < 0)
            {
                ClearInputFields();
                return;
            }

            Airplane selected = tower.GetAirplanes()[index];
            txtName.Text = selected.Name;
            txtFlightId.Text = selected.Id;
            txtDestination.Text = selected.Destination;
            txtFlightTime.Text = selected.FlightTime.ToString();
        }

        // Clears all input fields after adding a plane
        private void ClearInputFields()
        {
            txtName.Text = string.Empty;
            txtFlightId.Text = string.Empty;
            txtDestination.Text = string.Empty;
            txtFlightTime.Text = string.Empty;
            txtAltitudeChange.Text = string.Empty;
        }

        // Appends a timestamped message to the status log
        private void LogMessage(string message)
        {
            txtLog.AppendText($"{message}{Environment.NewLine}");
            txtLog.ScrollToEnd();
        }
    }
}