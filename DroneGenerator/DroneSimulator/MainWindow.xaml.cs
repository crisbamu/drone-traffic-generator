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
using System.Windows.Navigation;
using System.Windows.Shapes;
using utm_country;
using utm_routes;
using System.IO;
using utm_operator;
using utm_drone.DataBase;
using utm_drone;
using utm_routes.VLOS_routes;
using utm_routes.EVLOSRoutes;
using utm_routes.BVLOS_routes;
using utm_routes.DeliveryRoutes;
using System.Diagnostics;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;
using utm_analysis;
using System.Threading;
using Microsoft.Win32;
using utm_operation;

namespace DroneSimulator
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // This programme is made by Lluís Xavier Herranz Bareas. 
       
        public MainWindow()
        {
            InitializeComponent();

        }

        // Attributes

        string dataPath = AppDomain.CurrentDomain.BaseDirectory
            + "..\\..\\..\\..\\DroneData";

        List<utm_utils.Point> ConflictPoints = new List<utm_utils.Point>();
        // operators
        int big_operators = 0;
        int medium_operators = 0;
        int small_operators = 0;
        int delivery_flights = 0;

        // country
        CountryItem country_name;
        Country country;
        double mean_time;
        double devStd;
        TimeSpan lower_limit;
        TimeSpan upper_limit; // these timespans define the limits of the working hours
        double busy_areas_radius = 10000; // the busy areas is in meters to make the calculus in SI although the user is asked to inserted in NM because of the aeronautical regulations.

        // drone list
        List<drone> drone_type_list = new List<drone>();

        //drone
        static XMLDroneReader dronereader = new XMLDroneReader();

        //simulation
        private bool simulation_done = false;

        // Route parameters

        VLOSParameters vlos_parameters;
        EVLOSParameters evlos_parameters;
        BVLOSParameters bvlos_parameters;
        DeliveryParameters delivery_parameters;
        int parts = 20;

        //operator
        static OperatorGenerator operator_generator = new OperatorGenerator();
        List<Operator> operator_list;

        // forbidden flight areas zones
        private bool forbidden_flight_areas = true;
        private bool controlled_airspace = true;
        private bool cities_airspace = true;
        private bool airports_airspace = true;


       

        private void Make_simulation_Click(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random();
            OperatorsForm operator_f1 = new OperatorsForm();
            RouteForm routes_f1 = new RouteForm();
            DroneForm drone_f1 = new DroneForm();
            MavlinkFilesForm mavlink_f1 = new MavlinkFilesForm();
            //for getting the country
            CountryInterface country_f1 = new CountryInterface();
            country_f1.ShowDialog();
            country_name = country_f1.GetCountryItem();

            // loading the country
            if (country_name == CountryItem.Spain )
            {
                country = CountryGenerator.LoadCountry(dataPath, "Spain", "spanish_contour_2.0.xml", "spanish forbidden areas 2.0.xml", "spanish airspace 2.0.xml", "spanish airports.xml", "spain cities.xml", "Delivery centers.xml", "delivery forbidden areas 2.0.xml", "spanish working hours.xml");
                country.SetAvailability(forbidden_flight_areas, controlled_airspace, cities_airspace, airports_airspace);
                mean_time = country.GetMeanTime().TotalHours;
                devStd = country.GetVariance();
                lower_limit = country.GetInitialTime();
                upper_limit = country.GetFinalTime();

                // for displaying the country contourn

                List<double> latitudes = country.GetLatitudeContourn(country.GetLandside());
                List<double> longitudes = country.GetLongitudeContourn(country.GetLandside());

                var xlong = longitudes.ToArray().AsXDataSource();
                var ylat = latitudes.ToArray().AsYDataSource();

                
               
            }
            else if (country_name == CountryItem.Martorell) // loading the country
            {
                
                country = CountryGenerator.LoadCountry(dataPath, "Martorell", "contour-v2.xml", "prohibited areas.xml", "airspace-v2.xml", "airports.xml", "cities.xml", "Delivery centers.xml", "delivery forbidden areas.xml", "working hours.xml");
                if (country == null)
                {
                    MessageBox.Show("Check files in");
                    Environment.Exit(0);
                }
                country.SetAvailability(forbidden_flight_areas, controlled_airspace, cities_airspace, airports_airspace);
                mean_time = country.GetMeanTime().TotalHours;
                devStd = country.GetVariance();
                lower_limit = country.GetInitialTime();
                upper_limit = country.GetFinalTime();

                // for displaying the country contourn

                List<double> latitudes = country.GetLatitudeContourn(country.GetLandside());
                List<double> longitudes = country.GetLongitudeContourn(country.GetLandside());

                var xlong = longitudes.ToArray().AsXDataSource();
                var ylat = latitudes.ToArray().AsYDataSource();



            }

            if (country_f1.IsCountryInterfaceFilled())
            {
                // for getting the operators to be simulated
                operator_f1.ShowDialog();
                operator_list = operator_f1.GetOperatorList();
               
            }

            if (operator_f1.IsOperatorsFormFilled())
            {
                // for getting the route parameters
                routes_f1.ShowDialog();
                var routesparameters = routes_f1.GetRouteParameters();
                vlos_parameters = routesparameters.Item1;
                evlos_parameters = routesparameters.Item2;
                bvlos_parameters = routesparameters.Item3;
                delivery_parameters = routesparameters.Item4;
                operator_generator = new OperatorGenerator(country, vlos_parameters, evlos_parameters, bvlos_parameters, delivery_parameters);
            }
            
            //

           

            if (routes_f1.IsRouteFormFilled())
            {
                drone_f1.ShowDialog();
                drone_type_list = drone_f1.GetListOfDrones();
                
                
            }

            if (drone_f1.IsDroneFormFilled())
            {
                


                // make simulation
                //


                MessageBox.Show("The simulation is going to start and it might take several minutes to work. Please, wait until a meessage shows the end of the simulation.");
              
                // loading the route parameters

                operator_generator.LoadOperations(operator_list, drone_type_list, parts, mean_time, devStd, lower_limit, upper_limit, rnd);
                // for creating the output files


                // once the generation is over

                // for displaying data


                TimeSpan initial = lower_limit;
                TimeSpan final = upper_limit;
                List<double> x = new List<double>();
                List<double> y = new List<double>();

                var dist = OperationAnalizer.GetScheduleOperation(operator_list, initial, final);
                x = dist.Item1;
                y = dist.Item2;

                var xdata = x.ToArray().AsXDataSource();
                var ydata = y.ToArray().AsYDataSource();

                linegraph.DataSource = null;

                CompositeDataSource datas = xdata.Join(ydata);
                linegraph.DataSource = datas;

                string fleetofdrones = "fleetofdrones.txt";
                OperatorWriter.WriteOperatorsAndDrones(operator_list, fleetofdrones); // write the operators to a .txt file 
                string timeroute = "timeroute.kml";
                OperatorWriter.WriteKMLOperations(timeroute, operator_list);

                // open the form to download the files
                DownloadForm downloadform = new DownloadForm();
                downloadform.SetOperatorList(operator_list);
                downloadform.ShowDialog();

              
                String date = DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss");
                string record_name = @"Record " + date + ".xml";
                OperatorWriter.WriteXMLOperators(country, operator_list, "Operator_records", record_name);


               


                //MessageBox.Show("The generation has properly ended.");

                simulation_done = true;

            }
            else
            {
                //country_linegraph.DataSource = null;
                simulation_done = false;
            }
            
        }

        private void Close_button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Application.Current.Shutdown();
        }


        private void Operator_button_Click(object sender, RoutedEventArgs e)
        {
            OperatorsForm f1 = new OperatorsForm();
            f1.SetOperators(big_operators, medium_operators, small_operators, delivery_flights);
            f1.ShowOperators();
            f1.ShowDialog();
            var operators = f1.GetOperators();
            big_operators = operators.Item1;
            medium_operators = operators.Item2;
            small_operators = operators.Item3;
            delivery_flights = operators.Item4;
        }

        private void selectparameters_button_Click(object sender, RoutedEventArgs e)
        {
            RouteParameters f1 = new RouteParameters();
            f1.ShowDialog();
        }

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (operator_list != null && simulation_done == true)
            {
                String directory = AppDomain.CurrentDomain.BaseDirectory + "timeroute.kml";
                if (System.IO.File.Exists(directory))
                    Process.Start("\"C:\\Program Files (x86)\\Google\\Google Earth\\client\\googleearth.exe\"", directory);
                else
                {
                    MessageBox.Show("There is no route to show in Google Earth.");
                }
            }
        }



        

        

        private List<Conflict> FindConflictOperation()
        {
            List<Conflict> ConflictPoints = new List<Conflict>();
            if (operator_list != null)
            {
                //MessageBox.Show("The simulator is about to work out the conflicts between operations. This function might take several minutes to execute.");
                ConflictPoints = OperationAnalizer.FindTemporalConflicts(operator_list);
                AnalysisWriter.WriteKMLConflicts(ConflictPoints, "BADformat_conflicts.kml");
                // how to call instead to?? AnalisysWriter.WriteKMLConflicts(ConflictPoints, "conflicts.kml");


                return ConflictPoints;
            }
            else
            {
                MessageBox.Show("There is no simulation done yet.\n Please, simulate a day.");
                return null;
            }
            
        }

        private void Review_simulation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                string binaryPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                string directory = binaryPath + "\\Operator_records";
                OpenFileDialog filedialog = new OpenFileDialog();
                filedialog.InitialDirectory = directory;
                filedialog.ShowDialog();
                string foldername = filedialog.FileName;
                if (!string.IsNullOrEmpty(foldername))
                {
                    //MavlinkFilesForm mavlink_f1 = new MavlinkFilesForm();
                    //mavlink_f1.ShowDialog();
                    //fragment_parts = mavlink_f1.GetFragmentedFiles();
                   // bool mavlinkform_filled = mavlink_f1.IsMavLinkFormFilled();
                    if (!String.IsNullOrEmpty(foldername))
                    {
                        var generation_obtained = OperatorWriter.ReadOperatorsFromXML(foldername);
                        Country new_country = generation_obtained.Item1;
                        if (new_country.GetCountryName() == utm_country.CountryItem.Spain)
                        {
                            country = CountryGenerator.LoadCountry(dataPath, "Spain", "spanish_contour_2.0.xml", "spanish forbidden areas 2.0.xml", "spanish airspace 2.0.xml", "spanish airports.xml", "spain cities.xml", "Delivery centers.xml", "delivery forbidden areas 2.0.xml", "spanish working hours.xml");

                        }

                        operator_list = generation_obtained.Item2;
                        // for displaying data

                        var timeresults = OperatorWriter.FindTimeLimits(operator_list);
                        TimeSpan initial = timeresults.Item1;
                        TimeSpan final = timeresults.Item2;
                        List<double> x = new List<double>();
                        List<double> y = new List<double>();

                        var dist = OperationAnalizer.GetScheduleOperation(operator_list, initial, final);
                        x = dist.Item1;
                        y = dist.Item2;

                        var xdata = x.ToArray().AsXDataSource();
                        var ydata = y.ToArray().AsYDataSource();

                        linegraph.DataSource = null;

                        CompositeDataSource datas = xdata.Join(ydata);

                        linegraph.DataSource = datas;

                        string timeroute = "timeroute.kml";
                        OperatorWriter.WriteKMLOperations(timeroute, operator_list);

                        // open the form to download the files
                        DownloadForm downloadform = new DownloadForm();
                        downloadform.SetOperatorList(operator_list);
                        downloadform.ShowDialog();
                        // releasing the output files
                        //string mavlinkname = "mavlinkfolder";
                        //OperatorWriter.WriteMavLinkOperations(operator_list, mavlinkname);
                        //string operationsname = "operations.txt";
                        //OperatorWriter.WriteOperators(operationsname, mavlinkname, operator_list);
                        //string fragmentedfile = "fragmentedoperations.txt";
                        //OperatorWriter.FragmentFile(operationsname, mavlinkname, fragmentedfile, fragment_parts);


                        

                       // MessageBox.Show("The simulation has been restored.");
                        simulation_done = true;

                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("The review function could not perform its function properly. Please, check the file that it is going to be opened.");
                simulation_done = false;
            }

        }

       

     

        private void XML_button_Click(object sender, RoutedEventArgs e)
        {
            String directory = @"C:\Users\xavi\Desktop\ADDG\ADDG\addg\Nuevo_Intento\DroneSimulator\DroneSimulator\bin\Debug\Operators";
            OpenFileDialog filedialog = new OpenFileDialog();
            filedialog.InitialDirectory = directory;
            filedialog.ShowDialog();
            string foldername = filedialog.FileName;
            if (!String.IsNullOrEmpty(foldername))
            {
                 operator_list = operator_generator.LoadOperators(foldername);
            }
        }

        private void CloseMainWindows(object sender, EventArgs e)
        {
            this.Close();
            //Application.Current.Shutdown();
        }

        private void Simulation_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (operator_list != null && simulation_done==true)
            {
               
                    ResultsForm f1 = new ResultsForm();
                    f1.SetParameters(operator_list, busy_areas_radius, country);
                    f1.ShowResults(operator_list);
                    
                f1.ShowDialog();
                   
            }
            else
            {
                MessageBox.Show("There is no simulation generated yet. Please, make a simulation to show the results.");
            }
        
        }

        private void ClosingMainWindows(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {

        }

        private void busy_areas_radius_Click(object sender, RoutedEventArgs e)
        {
            BusyAreasForm f1 = new BusyAreasForm();
            f1.SetBusyAreasRadius(this.busy_areas_radius);
            f1.ShowBusyAreasRadius();
            f1.ShowDialog();
            this.busy_areas_radius = f1.GetBusyAreasRadius();
        }

        private void Options_generation_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Forbidden_zones_options_Click(object sender, RoutedEventArgs e)
        {
            ZonesOptions f1 = new ZonesOptions();
            f1.SetZonesOptions(forbidden_flight_areas, controlled_airspace, cities_airspace, airports_airspace);
            f1.ShowDialog();
            var zones_results = f1.GetZones();
            forbidden_flight_areas = zones_results.Item1;
            controlled_airspace = zones_results.Item2;
            cities_airspace = zones_results.Item3;
            airports_airspace = zones_results.Item4;
        }
    }
}
 