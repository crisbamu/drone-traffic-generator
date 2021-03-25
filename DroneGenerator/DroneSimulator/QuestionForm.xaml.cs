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
    /// Interaction logic for QuestionForm.xaml
    /// </summary>
    public partial class QuestionForm : Window
    {
        public QuestionForm()
        {
            InitializeComponent();
        }

        // Attribute

        private bool result = false;

        public bool GetResult()
        {
            return this.result;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.result = true;
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.result = false;
            this.Close();
        }
    }
}
