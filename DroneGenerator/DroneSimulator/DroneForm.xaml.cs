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
using utm_drone;

namespace DroneSimulator
{
    /// <summary>
    /// Interaction logic for DroneForm.xaml
    /// </summary>
    public partial class DroneForm : Window
    {
        public DroneForm()
        {
            InitializeComponent();
        }

        // Attribute

        private List<drone> dronelist;
        private bool drone_filled = false;
        private bool next_button_pressed = false;
        static XMLDroneReader dronereader = new XMLDroneReader();

        public List<drone> GetListOfDrones()
        {
            return this.dronelist;
        }

        public bool IsDroneFormFilled()
        {
            return drone_filled;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(drone_filled==true)
            {
                next_button_pressed = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("There is no file loaded. Please, load a file to continue the simulation");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            drone_filled = false;
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //String directory = @"C:\Users\xavi\Desktop\ADDG\ADDG\addg\Nuevo_Intento\DroneSimulator\DroneSimulator\bin\Debug\Drones";
            string binaryPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            string directory = binaryPath + "\\Drones";
            OpenFileDialog filedialog = new OpenFileDialog();
            filedialog.InitialDirectory = directory;
            filedialog.ShowDialog();
            string foldername = filedialog.FileName;
            if (!String.IsNullOrEmpty(foldername))
            {
                dronelist = dronereader.LoadDroneList(foldername);
                drone_filled = true;
            }
            if (drone_filled == true)
            {
                string[] names = foldername.Split('\\');
                file_name.Content = names[names.Length - 1];
            }
            this.Button_Click(sender, e);
        }

        private void CloseWindow(object sender, EventArgs e)
        {
            if (!next_button_pressed)
            {
                drone_filled = false;
            }
            this.Close();
           
        }
    }
}
