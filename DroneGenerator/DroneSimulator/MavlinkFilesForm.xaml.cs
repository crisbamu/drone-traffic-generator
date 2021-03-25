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
    /// Interaction logic for MavlinkFilesForm.xaml
    /// </summary>
    public partial class MavlinkFilesForm : Window
    {
        public MavlinkFilesForm()
        {
            InitializeComponent();
        }

        // Attributes

        private int fragmented_files;
        private bool next_button_pressed = true;
        private bool MavLinkFormfilled = false;

        public int GetFragmentedFiles()
        {
            return this.fragmented_files;
        }

        public bool IsMavLinkFormFilled()
        {
            return this.MavLinkFormfilled;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int numbers;
            bool result = int.TryParse(fragment_textbox.Text, out numbers);
            if(result && numbers>0)
            {
                fragmented_files = numbers;
                MavLinkFormfilled = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("The number is wrongly inserted. Please, check it out again and fill it properly.");
            }
           
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CloseWindow(object sender, EventArgs e)
        {
            if (!next_button_pressed)
            {
                MavLinkFormfilled = false;
            }
            this.Close();
        }

       
    }
}
