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
    /// Lógica de interacción para ZonesOptions.xaml
    /// </summary>
    public partial class ZonesOptions : Window
    {
        public ZonesOptions()
        {
            InitializeComponent();
        }
        // Created by Lluís Xavier Herranz Bareas on 08/26/2017

        // This class aims at choosing the forbidden flight zones which are taking into account for the generations.

        // Attributes

        bool forbidden_areas = false;
        bool controlled_air=false;
        bool cities_zones=false;
        bool airports_zone=false;

        // builders

        public void  SetZonesOptions(bool forbidden_insert, bool controlled_insert, bool city_insert, bool airports_insert)
        {
            forbidden_areas = forbidden_insert;
            if(forbidden_areas==true)
            {
                Forbidden_flight_areas.IsChecked = true;
            }
            controlled_air = controlled_insert;
            if(controlled_air==true)
            {
                controlled_airspace.IsChecked = true;
            }
            cities_zones = city_insert;
            if(cities_zones==true)
            {
                Cities.IsChecked = true;
            }
            airports_zone = airports_insert;
            if(airports_zone==true)
            {
                Airports.IsChecked = true;
            }

            
        }

        // setters and getter

       
        public Tuple<bool, bool, bool, bool> GetZones()
        {
            return new Tuple<bool, bool, bool, bool>(forbidden_areas, controlled_air, cities_zones, airports_zone);
        }

        private void Apply_button_Click(object sender, RoutedEventArgs e)
        {
            if(Forbidden_flight_areas.IsChecked==true)
            {
                forbidden_areas = true;
            }
            else
            {
                forbidden_areas = false;
            }
            if(controlled_airspace.IsChecked==true)
            {
                controlled_air = true;
            }
            else
            {
                controlled_air = false;
            }
            if(Cities.IsChecked==true)
            {
                cities_zones = true;
            }
            else
            {
                cities_zones = false;
            }
            if(Airports.IsChecked==true)
            {
                airports_zone = true;
            }
            else
            {
                airports_zone = false;
            }
            this.Close();
        }

        private void Cancel_button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
