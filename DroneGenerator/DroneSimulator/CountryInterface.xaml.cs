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
    /// Interaction logic for CountryInterface.xaml
    /// </summary>
    public partial class CountryInterface : Window
    {
        public CountryInterface()
        {
            InitializeComponent();
        }
        // Attributes

       private CountryItem country = CountryItem.None;
       private bool filled = false;

        public void SetCountryItem(CountryItem a)
        {
            this.country = a;
        }

        public CountryItem GetCountryItem()
        {
            return this.country;
        }

        public bool IsCountryInterfaceFilled()
        {
            return filled;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
           if(country_comboBox.Text.Equals("Spain"))
           {
               country = CountryItem.Spain;
               filled = true;
               this.Close();
           }
            else if (country_comboBox.Text.Equals("Martorell"))
            {
                country = CountryItem.Martorell;
                filled = true;
                this.Close();
            }

            if (string.IsNullOrEmpty(country_comboBox.Text))
            {
                MessageBox.Show("Please, insert a country.");
            }

        }

        private void country_comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
