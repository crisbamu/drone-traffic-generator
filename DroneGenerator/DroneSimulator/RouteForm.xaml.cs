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
using utm_routes.VLOS_routes;
using utm_routes.BVLOS_routes;
using utm_routes.DeliveryRoutes;
using utm_routes.EVLOSRoutes;
using utm_routes;
using Microsoft.Win32;

namespace DroneSimulator
{
    /// <summary>
    /// Interaction logic for RouteForm.xaml
    /// </summary>
    public partial class RouteForm : Window
    {
        public RouteForm()
        {
            InitializeComponent();
        }

        // Attributes

        private VLOSParameters vlosparameters;
        private EVLOSParameters evlosparameters;
        private BVLOSParameters bvlosparameters;
        private DeliveryParameters deliveryparameters;

        private bool filled = false;
        private bool file_filled = false;
        private bool Parameters_filled = false;
        private bool next_button_pressed = false;

        public Tuple<VLOSParameters, EVLOSParameters, BVLOSParameters, DeliveryParameters> GetRouteParameters()
        {
            return new Tuple<VLOSParameters, EVLOSParameters, BVLOSParameters, DeliveryParameters>(vlosparameters, evlosparameters, bvlosparameters, deliveryparameters);
        }

        public bool IsRouteFormFilled()
        {
            return this.Parameters_filled;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void default_parameters_radioButton_Click(object sender, RoutedEventArgs e)
        {
            if (default_parameters_radioButton.IsChecked == true)
            {
                selectparameters_button.IsEnabled = false;
                Examine_button.IsEnabled = true;
            }
        }

        private void select_parameters_radioButton_Click(object sender, RoutedEventArgs e)
        {
            if (select_parameters_radioButton.IsChecked == true)
            {
                selectparameters_button.IsEnabled = true;
                Examine_button.IsEnabled = false;
            }
        }

        private void selectparameters_button_Click(object sender, RoutedEventArgs e)
        {
            RouteParameters f1 = new RouteParameters();
            f1.ShowDialog();
            vlosparameters = f1.GetVLOSParameters();
            bvlosparameters = f1.GetBVLOSParameters();
            evlosparameters = f1.GetEVLOSParameters();
            deliveryparameters = f1.GetDeliveryParameters();
            filled = true;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            
                if (default_parameters_radioButton.IsChecked == true && file_filled == true)
                {
                    Parameters_filled = true;
                    next_button_pressed = true;
                    this.Close();
                }

                if (select_parameters_radioButton.IsChecked == true && filled == true)
                {
                    next_button_pressed = true;
                    Parameters_filled = true;
                    this.Close();
                }
           
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                //String directory = @"C:\Users\xavi\Desktop\ADDG\ADDG\addg\Nuevo_Intento\DroneSimulator\DroneSimulator\bin\Debug\Route_Parameters";
                string binaryPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                string directory = binaryPath + "\\Route_Parameters";
                OpenFileDialog filedialog = new OpenFileDialog();
                filedialog.InitialDirectory = directory;
                filedialog.ShowDialog();
                string foldername = filedialog.FileName;
                if (!String.IsNullOrEmpty(foldername))
                {
                    var parameters = new RouteGenerator().LoadRouteParameters(foldername);
                    vlosparameters = parameters.Item1;
                    evlosparameters = parameters.Item2;
                    bvlosparameters = parameters.Item3;
                    deliveryparameters = parameters.Item4;
                    if (new RouteGenerator().AreTheRouteParametersWellLoaded(vlosparameters, evlosparameters, bvlosparameters, deliveryparameters))
                    {
                        file_filled = true;
                        
                    }
                    else
                    {
                        vlosparameters = new VLOSParameters();
                        evlosparameters = new EVLOSParameters();
                        bvlosparameters = new BVLOSParameters();
                        deliveryparameters = new DeliveryParameters();
                        file_filled = false;
                        MessageBox.Show("The route parameters inserted are not correct. Please check the file again or select others.");
                    }
                }

                if (file_filled == true)
                {
                    string[] names = foldername.Split('\\');
                    file_name.Content = names[names.Length - 1];
                }

            }
            catch (Exception)
            {
                MessageBox.Show("The file could not be opened because it may contain some mistakes. Please, revise it or load other files.");
            }
            this.Button_Click_2(sender, e);
        }

       

        private void CloseWindow(object sender, EventArgs e)
        {
            if (!next_button_pressed)
            {
                Parameters_filled = false;
            }
            
            this.Close();
        }

        
    }
}
