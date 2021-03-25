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
using Microsoft.Win32;
using utm_operator;
using utm_drone;

namespace DroneSimulator
{
    /// <summary>
    /// Lógica de interacción para OperatorsForm.xaml
    /// </summary>
    public partial class OperatorsForm : Window
    {
        public OperatorsForm()
        {
            InitializeComponent();
        }

        // Attributes: the quantity of each operator

        List<Operator> operator_list;
        int big_operators;
        int medium_operators;
        int small_operators;
        int delivery_flights;

        bool filled = false;
        bool parameters_filled = false;
        
        static OperatorGenerator operator_generator = new OperatorGenerator();
        static  Random rnd = new Random();

        // setters and getters

        public void SetOperators(int big, int medium, int small, int delivery)
        {
            this.big_operators = big;
            this.medium_operators = medium;
            this.small_operators = small;
            this.delivery_flights = delivery;
        }

        public List<Operator> GetOperatorList()
        {
            return this.operator_list;
        }

        public Tuple<int, int, int, int> GetOperators()
        {
            return new Tuple<int, int, int, int>(this.big_operators, this.medium_operators, this.small_operators, this.delivery_flights);
        }

        public bool IsOperatorsFormFilled()
        {
            return this.parameters_filled;
        }

        public void ShowOperators()
        {
            big_operator_textbox.Text = big_operators.ToString();
            medium_operator_textbox.Text = medium_operators.ToString();
            small_operator_textbox.Text = small_operators.ToString();
            delivery_flights_textbox.Text = delivery_flights.ToString();
        }

        private void big_changed(object sender, TextChangedEventArgs e)
        {
            int numbers;
            bool result = int.TryParse(big_operator_textbox.Text, out numbers);
            if (result)
            {
                big_label.Content = "\u2714";
                big_label.Foreground = Brushes.Green;
            }
            else
            {
                big_label.Content = "\u2718";
                big_label.Foreground = Brushes.Red;
            }
        }

        private void medium_changed(object sender, TextChangedEventArgs e)
        {
            int numbers;
            bool result = int.TryParse(medium_operator_textbox.Text, out numbers);
            if (result)
            {
                medium_label.Content = "\u2714";
                medium_label.Foreground = Brushes.Green;
            }
            else
            {
                medium_label.Content = "\u2718";
                medium_label.Foreground = Brushes.Red;
            }
        }

        private void small_changed(object sender, TextChangedEventArgs e)
        {
            int numbers;
            bool result = int.TryParse(small_operator_textbox.Text, out numbers);
            if (result)
            {
                small_label.Content = "\u2714";
                small_label.Foreground = Brushes.Green;
            }
            else
            {
                small_label.Content = "\u2718";
                small_label.Foreground = Brushes.Red;
            }
        }

        private void Cancel_button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Apply_button_Click(object sender, RoutedEventArgs e)
        {
            if (Operators_radioButton.IsChecked == true)
            {
                if (big_label.Content.Equals("\u2714") && medium_label.Content.Equals("\u2714") && small_label.Content.Equals("\u2714") && delivery_flight_label.Content.Equals("\u2714") && Operators_radioButton.IsChecked == true)
                {
                    this.big_operators = Convert.ToInt32(big_operator_textbox.Text);
                    this.medium_operators = Convert.ToInt32(medium_operator_textbox.Text);
                    this.small_operators = Convert.ToInt32(small_operator_textbox.Text);
                    this.delivery_flights = Convert.ToInt32(delivery_flights_textbox.Text);
                    operator_list = operator_generator.AssignFleetOfDrones(big_operators, medium_operators, small_operators, delivery_flights, 1000, rnd);
                    this.parameters_filled = true;
                    this.Close();
                }
                else
                    if (!(big_label.Content.Equals("\u2714") && medium_label.Content.Equals("\u2714") && small_label.Content.Equals("\u2714") && delivery_flight_label.Content.Equals("\u2714")) && Operators_radioButton.IsChecked == true)
                    {
                        MessageBox.Show("Some operators or flight are not properly inserted. Please, check them out again.");
                    }
            }

            if(XML_radioButton.IsChecked==true)
            {
                if(filled==true)
                {
                    this.parameters_filled = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Please, choose the file where the operators are stored.");
                }
            }
            
        }

        private void delivery_changed(object sender, TextChangedEventArgs e)
        {
            int numbers;
            bool result = int.TryParse(delivery_flights_textbox.Text, out numbers);
            if (result)
            {
                delivery_flight_label.Content = "\u2714";
                delivery_flight_label.Foreground = Brushes.Green;
            }
            else
            {
                delivery_flight_label.Content = "\u2718";
                delivery_flight_label.Foreground = Brushes.Red;
            }

        }

        private void Operators_radioButton_Click(object sender, RoutedEventArgs e)
        {

            {
                if (Operators_radioButton.IsChecked == true)
                {
                    operator_panel.Visibility = Visibility.Visible;
                    XML_button.IsEnabled = false;
                }
                else
                {
                    operator_panel.Visibility = Visibility.Hidden;
                    XML_button.IsEnabled = true;
                }

            }
        }

        private void XML_radioButton_Click(object sender, RoutedEventArgs e)
        {
            if (XML_radioButton.IsChecked == true)
            {
                XML_button.IsEnabled = true;
                operator_panel.Visibility = Visibility.Hidden;
            }
            else
            {
                XML_button.IsEnabled = false;
                operator_panel.Visibility = Visibility.Visible;
            }
        }

        private void XML_button_Click(object sender, RoutedEventArgs e)
        {
            //String directory = @"C:\Users\xavi\Desktop\ADDG\ADDG\addg\Nuevo_Intento\DroneSimulator\DroneSimulator\bin\Debug\Operators";
            string binaryPath =System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            string directory = binaryPath + "\\Operators";
            OpenFileDialog filedialog = new OpenFileDialog();
            filedialog.InitialDirectory = directory;
            filedialog.ShowDialog();
            string foldername = filedialog.FileName;
            if (!String.IsNullOrEmpty(foldername))
            {
                bool mistake = false;
                try
                {
                    operator_list = new OperatorGenerator().LoadOperators(foldername);
                }
                catch (Exception)
                {
                    MessageBox.Show("The file is not valid. Please, choose another file.");
                    filled = false;
                    mistake = true;
                }

                if (mistake == false)
                {
                    if (operator_list.Count!=0)
                    {
                        filled = true;
                        string[] names = foldername.Split('\\');
                        name_file.Content = names[names.Length - 1];
                    }
                    else
                    {
                        MessageBox.Show("The file is not valid. Please, choose another file.");
                    }
                }

            }
            this.Apply_button_Click(sender, e);


        }

        private void XML_radioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

       
    }
}
