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
using utm_operation;
using utm_analysis;
using System.Threading;
using System.Diagnostics;
using System.Timers;
using Microsoft.Win32;
using utm_country;
namespace DroneSimulator
{
    /// <summary>
    /// Interaction logic for ResultsForm.xaml
    /// </summary>
    public partial class ResultsForm : Window
    {
        public ResultsForm()
        {
            InitializeComponent();
        }

        //Attributes
        private double busy_areas_radius;
        private List<Operator> operator_list;
        private List<Conflict> conflict_points;
        private List<Tuple<Operation, Operation, utm_utils.Point, double>> busy_areas;
        Country country;

        

     

        public void SetOperatorList( List<Operator> list)
        {
            this.operator_list = list;
        }

        public void SetParameters(List<Operator> list, double radius, Country this_country)
        {
            this.operator_list = list;
            this.busy_areas_radius = radius;
            this.country = this_country;
        }

        public void ShowResults(List<Operator> operator_list)
        {
            // statistics

            var dest = OperationAnalizer.GetDistanceStatistics(operator_list);

            double meandistance = Math.Round(dest.Item1, 0);
            double vlosmeandistance = Math.Round(dest.Item2, 0);
            double evlosmeandistance = Math.Round(dest.Item3, 0);
            double bvlosmeandistance = Math.Round(dest.Item4, 0);
            double deliverymeandistance = Math.Round(dest.Item5, 0);

           
            vlosmeandistance_textblock.Content = vlosmeandistance.ToString();
            evlosmeandistance_textblock.Content = evlosmeandistance.ToString();
            bvlosmeandistance_textblock.Content = bvlosmeandistance.ToString();
            deliverymeandistance_textblock.Content = deliverymeandistance.ToString();

            var dams = OperationAnalizer.GetTimeStatistics(operator_list);
            double meaningtime = Math.Round(dams.Item1 , 0);
            double vlosmeaningtime = Math.Round(dams.Item2 , 0);
            double evlosmeaningtime = Math.Round(dams.Item3 , 0);
            double bvlosmeaningtime = Math.Round(dams.Item4 , 0);
            double deliverymeaningtime = Math.Round(dams.Item5 , 0);


            vlosmeantime_textblock.Content = vlosmeaningtime.ToString();
            evlosmeantime_textblock.Content = evlosmeaningtime.ToString();
            bvlosmeantime_textblock.Content = bvlosmeaningtime.ToString();
            deliverymeantime_textblock.Content = deliverymeaningtime.ToString();

            var dart = OperationAnalizer.GetOperationsStatistics(operator_list);
            vlosoperations_textblock.Content = dart.Item1.ToString();
            evlosoperations_textblock.Content = dart.Item2.ToString();
            bvlosoperations_textblock.Content = dart.Item3.ToString();
            deliveryoperations_textblock.Content = dart.Item4.ToString();
            
        }

        private void find_conflict_buttons_Click(object sender, RoutedEventArgs e)
        {
            
            
            
            Thread new_thread = new Thread(() =>
            {

                conflict_points = OperationAnalizer.FindTemporalConflicts(operator_list);
                AnalysisWriter.WriteKMLConflicts(conflict_points, "conflicts.kml");
                
                
                Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    
                    conflict_label.Content = conflict_points.Count.ToString();
                    loading_conflicts_label.Content = "Loaded";
                    loading_conflicts_label.Foreground = Brushes.Green;
                    QuestionForm question_form = new QuestionForm();
                    question_form.ShowDialog();
                    bool result = question_form.GetResult();
                    if (result == true)
                    {
                        String directory = AppDomain.CurrentDomain.BaseDirectory + "conflicts.kml";
                        //Process.Start("\"C:\\Program Files (x86)\\Google\\Google Earth\\client\\googleearth.exe\"", directory);

                        // This is the instruction to open Google Earth. The location of the file might change with each computer.
                        // Please, consider to redirect and change this instruction to the folder where googleearth.exe file is in your computer. 
                        
                            Process.Start("\"C:\\Program Files (x86)\\Google\\Google Earth Pro\\client\\googleearth.exe\"", directory);
                        
                       
                    }
                    see_conflict_buttons.IsEnabled = true;
                    save_conflict_buttons.IsEnabled = true;
                }));

            });
            new_thread.Start();
            loading_conflicts_label.Content = "Loading...";
            loading_conflicts_label.Foreground = Brushes.Red;
           
        }

        



        private void find_busy_areas_button_Click(object sender, RoutedEventArgs e)
        {
            if (operator_list != null)
            {
                //List<Tuple<Operation, Operation, utm_utils.Point, double>> busyareasunderradius = OperationAnalizer.FindBusyAreasUnderARadius(operator_list, busy_areas_radius);
                //analysis_writer.GenerateKMLBusyAreas(busyareasunderradius, "busyareas.kml");
                Thread new_thread = new Thread(() =>
                {
                    busy_areas = OperationAnalizer.FindBusyAreasUnderARadius(operator_list, busy_areas_radius, country);
                    // List<Tuple<Operation, Operation, utm_utils.Point, double>> busyareasunderradius = OperationAnalizer.FindBusyAreasUnderARadius(operator_list, busy_areas_radius,country);
                    AnalysisWriter.GenerateKMLBusyAreas(busy_areas, "busyareas.kml");
                    Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        busy_label.Content = busy_areas.Count.ToString();
                        loading_areas_label.Content = "Loaded";
                        loading_areas_label.Foreground = Brushes.Green;
                        QuestionForm question_form = new QuestionForm();
                        question_form.ShowDialog();
                        bool result = question_form.GetResult();
                        if (result == true)
                        {
                            String directory = AppDomain.CurrentDomain.BaseDirectory + "busyareas.kml";
                            //Process.Start("\"C:\\Program Files (x86)\\Google\\Google Earth\\client\\googleearth.exe\"", directory);
                            Process.Start("\"C:\\Program Files (x86)\\Google\\Google Earth Pro\\client\\googleearth.exe\"", directory);
                        }
                        see_busy_areas_buttons.IsEnabled = true;
                        save_busy_areas_buttons.IsEnabled = true;
                    }));

                });
                new_thread.Start();
                loading_areas_label.Content = "Loading...";
                loading_areas_label.Foreground = Brushes.Red;
                //MessageBox.Show("The simulator is about to work out the busy areas. This function might take several minutes to execute.");

            }
            else
            {
                MessageBox.Show("There is no simulation loaded. Please, make a simulation to work out the busy areas.");
            }
        }

        private void CloseWindow(object sender, EventArgs e)
        {
            this.Close();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //bool? nullableBool;
            //bool boolean;
            String directory = @"C:\Users\xavi\Desktop";
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.AddExtension = true;
            fileDialog.DefaultExt = "csv";
            fileDialog.InitialDirectory = directory;
            if (fileDialog.ShowDialog() == true && operator_list!=null)
            {

                OperationAnalizer.ExportDataToCSV(operator_list, fileDialog.FileName);
                MessageBox.Show("The csv file has been properly created.");
            }
            else
            {
                MessageBox.Show("The exportation could not be done due to a problem.");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            String directory = @"C:\Users\xavi\Desktop";
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.AddExtension = true;
            fileDialog.DefaultExt = "xml";
            fileDialog.InitialDirectory = directory;
            if (fileDialog.ShowDialog() == true && operator_list != null)
            {

                OperationAnalizer.ExportDataToXML(operator_list, fileDialog.FileName);
                MessageBox.Show("The xml file has been properly created.");
            }
            else
            {
                MessageBox.Show("The exportation could not be done due to a problem.");
            }

        }

        private void Visualization_Click(object sender, RoutedEventArgs e)
        {
            if (operator_list != null)
            {
                String directory = AppDomain.CurrentDomain.BaseDirectory + "timeroute.kml";
                if (System.IO.File.Exists(directory))
                    // IMPORTANT: Choose the correct folder where the googleearth.exe file is stored so that the ADDG program can open it. 
                    //Process.Start("\"C:\\Program Files (x86)\\Google\\Google Earth\\client\\googleearth.exe\"", directory);
                    Process.Start("\"C:\\Program Files (x86)\\Google\\Google Earth Pro\\client\\googleearth.exe\"", directory); 
                else
                {
                    MessageBox.Show("There is no route to show in Google Earth.");
                }
            }
        }

        private void see_busy_areas_buttons_Click(object sender, RoutedEventArgs e)
        {
            if (busy_areas != null)
            {
                String directory = AppDomain.CurrentDomain.BaseDirectory + "busyareas.kml";
                //Process.Start("\"C:\\Program Files (x86)\\Google\\Google Earth\\client\\googleearth.exe\"", directory);
                Process.Start("\"C:\\Program Files (x86)\\Google\\Google Earth Pro\\client\\googleearth.exe\"", directory);
            }
            else
            {
                MessageBox.Show("The visualization cannot be done because there are no busy areas.");
            }
        }

        private void save_conflict_buttons_Click(object sender, RoutedEventArgs e)
        {
            String path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.AddExtension = true;
            fileDialog.DefaultExt = "kml";
            fileDialog.InitialDirectory = path;
            if (fileDialog.ShowDialog() == true && conflict_points != null)
            {
                AnalysisWriter.WriteKMLConflicts(conflict_points, fileDialog.FileName);
            }
            else
            {
                MessageBox.Show("The exportation could not be done due to a problem.");
            }
        }

        private void save_busy_areas_buttons_Click(object sender, RoutedEventArgs e)
        {
            String path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.AddExtension = true;
            fileDialog.DefaultExt = "kml";
            fileDialog.InitialDirectory = path;
            if (fileDialog.ShowDialog() == true && busy_areas != null)
            {
                AnalysisWriter.GenerateKMLBusyAreas(busy_areas, fileDialog.FileName);
            }
            else
            {
                MessageBox.Show("The exportation could not be done due to a problem.");
            }
        }

        private void see_conflict_buttons_Click(object sender, RoutedEventArgs e)
        {
            if (conflict_points != null)
            {
                String directory = AppDomain.CurrentDomain.BaseDirectory + "conflicts.kml";
                //Process.Start("\"C:\\Program Files (x86)\\Google\\Google Earth\\client\\googleearth.exe\"", directory);
                Process.Start("\"C:\\Program Files (x86)\\Google\\Google Earth Pro\\client\\googleearth.exe\"", directory);
            }
            else
            {
                MessageBox.Show("The visualization cannot be done because there are no conflicts.");
            }
        }
    }
}
