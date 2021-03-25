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
using utm_operator;
using utm_routes;
using Microsoft.Win32;

namespace DroneSimulator
{
    /// <summary>
    /// Lógica de interacción para DownloadForm.xaml
    /// </summary>
    public partial class DownloadForm : Window
    {
        public DownloadForm()
        {
            InitializeComponent();
        }

        // Attributes

        private List<Operator> operator_list;


        // setters and getters

        public void SetOperatorList(List<Operator> list)
        {
            this.operator_list = list;
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //String directory = @"C:\Users\xavi\Desktop";
            String path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.AddExtension = true;
            fileDialog.DefaultExt = "kml";
            fileDialog.InitialDirectory = path;
            if (fileDialog.ShowDialog() == true && operator_list != null)
            {
                OperatorWriter.WriteKMLOperations(fileDialog.FileName, operator_list);
            }
            else
            {
                MessageBox.Show("The exportation could not be done due to a problem.");
            }
           
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            //OpenFileDialog f1 = new OpenFileDialog();
            //f1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //f1.ShowDialog();
            //string path = f1.FileName;
            int fragment_parts;
            int numbers;
            bool result = int.TryParse(fragment_textbox.Text, out numbers);
            if (result && numbers > 0)
            {
                fragment_parts = numbers;
                String path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                SaveFileDialog fileDialog = new SaveFileDialog();
                fileDialog.AddExtension = true;
                //fileDialog.DefaultExt = "kml";
                fileDialog.InitialDirectory = path;
                if (fileDialog.ShowDialog() == true && operator_list != null)
                {
                    //string mavlinkname = "mavlinkfolder";
                    OperatorWriter.WriteMavLinkOperations(operator_list, fileDialog.FileName);
                    string operationsname = "operations.txt";
                    OperatorWriter.WriteOperators(operationsname, fileDialog.FileName, operator_list);
                    string fragmentedfile = "fragmentedoperations.txt";
                    OperatorWriter.FragmentFile(operationsname, fileDialog.FileName, fragmentedfile, fragment_parts);
                    bool deleted = FileReaderWriter.DeleteFile(operationsname, fileDialog.FileName);
                }
                else
                {
                    MessageBox.Show("The exportation could not be done due to a problem.");
                }
            }
            else
            {
                MessageBox.Show("The number is wrongly inserted. Please, check it out again and fill it properly.");
            }

           
            
        }

        private void button_Copy1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button_Copy2_Click(object sender, RoutedEventArgs e)
        {
            String path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.AddExtension = true;
            fileDialog.DefaultExt = "csv";
            fileDialog.InitialDirectory = path;
            if (fileDialog.ShowDialog() == true && operator_list != null)
            {
                OperatorWriter.WriteCSVOperations(fileDialog.FileName, operator_list);
            }
            else
            {
                MessageBox.Show("The exportation could not be done due to a problem.");
            }
        }
    }
}
