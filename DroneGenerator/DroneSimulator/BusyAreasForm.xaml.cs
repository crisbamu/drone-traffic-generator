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
    /// Lógica de interacción para BusyAreasForm.xaml
    /// </summary>
    public partial class BusyAreasForm : Window
    {
        public BusyAreasForm()
        {
            InitializeComponent();
        }

        // this class aims at inserting the radius of the Busy Areas.

        //Attributes

        private double busy_areas_radius; // this radius is in meters.

        // setters and getters

        public void SetBusyAreasRadius(double radius)
        {
            this.busy_areas_radius = radius;
        }

        public double GetBusyAreasRadius()
        {
            return this.busy_areas_radius;
        }

        public void ShowBusyAreasRadius()
        {
            radiusBox.Text = Convert.ToString(Math.Round(this.busy_areas_radius / 1852,2)); // show the busy areas radius in screen in NM.
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (check_label.Content.Equals("\u2714"))
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("The inserted radius is not correct. Please, check it out again.");
            }
        }

        private void radius_change(object sender, TextChangedEventArgs e)
        {
            double numbers;
            bool result = double.TryParse(radiusBox.Text, out numbers);
            if(result==true && numbers>0)
            {
                check_label.Content = "\u2714";
                this.busy_areas_radius = numbers * 1852; // to pass the radius from NM to meters since the generator works with SI
            }
            else
            {
                check_label.Content = "\u2718";
            }
        }
    }
}
