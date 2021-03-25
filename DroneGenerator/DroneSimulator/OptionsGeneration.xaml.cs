using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DroneSimulator
{
    /// <summary>
    /// Interaction logic for OptionsGeneration.xaml
    /// </summary>
    public partial class OptionsGeneration : Window
    {
        public OptionsGeneration()
        {
            InitializeComponent();
        }

        //Attributes
        bool forbidden_areas;
        bool controlled_airspace;
        bool airports_space;
        bool cities_space;

        public void SetValues(bool forbidden, bool control, bool airport, bool cit)
        {
            this.forbidden_areas = forbidden;
            if (this.forbidden_areas == true)
            {
                forbidden_flight.IsChecked = true;
            }
            else
            {
                forbidden_flight.IsChecked = false;
            }
            this.controlled_airspace = control;
            if (this.controlled_airspace == true)
            {
                controlled.IsChecked = true;
            }
            else
            {
                controlled.IsChecked = false;
            }
            this.airports_space = airport;
            if (airports_space == true)
            {
                airports.IsChecked = true;
            }
            else
            {
                airports.IsChecked = false;
            }
            this.cities_space = cit;
            if (cities_space == true)
            {
                cities.IsChecked = true;
            }
            else
            {
                cities.IsChecked = false;
            }
        }

        public Tuple<bool, bool, bool, bool> GetValues()
        {
            return new Tuple<bool, bool, bool, bool>(this.forbidden_areas, this.controlled_airspace, this.airports_space, this.cities_space);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(forbidden_flight.IsChecked == true)
            {
                this.forbidden_areas = true;
            }
            else
            {
                this.forbidden_areas =false;
            }

            if(controlled.IsChecked==true)
            {
                this.controlled_airspace = true;
            }
            else
            {
                this.controlled_airspace = false;
            }

            if(airports.IsChecked==true)
            {
                this.airports_space = true;
            }
            else
            {
                this.airports_space = false;
            }

            if(cities.IsChecked==true)
            {
                this.cities_space = true;
            }
            else
            {
                this.cities_space = false;
            }
        }
    }
}
