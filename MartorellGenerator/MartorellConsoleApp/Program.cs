using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using utm_analysis;
using utm_country;
using utm_drone;
using utm_drone.DataBase;
using utm_operator;
using utm_routes;
using utm_routes.BVLOS_routes;
using utm_routes.DeliveryRoutes;



namespace MartorellConsoleApp
{
    class Simulation
    {
        Country country;
        // Attributes needed for a simulation 
        List<utm_utils.Point> ConflictPoints = new List<utm_utils.Point>();

        // working hours
        double mean_time;
        double devStd;
        TimeSpan lower_limit;
        TimeSpan upper_limit; // these timespans define the limits of the working hours

        // drone list
        List<drone> drone_type_list = new List<drone>();

        //drone
        static XMLDroneReader dronereader = new XMLDroneReader();

        // Route parameters
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

        public void ScaleFleet(float scale)
        {
            // This function scales the fleet of all the operators in operator_list
            foreach (Operator op in operator_list)
                op.SetNumberOfDrones(Convert.ToInt32(op.GetNumberOfDrones()*scale));
        }

        public void MakeSimulation(float scale)
        {
            Random rnd = new Random();
            string binaryPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            // loading Martorell
            country = CountryGenerator.LoadCountry("Martorell", "contour-v2+alt.xml", "prohibited areas.xml", "airspace-v2.xml", "airports.xml", "cities.xml", "Delivery centers+alt.xml", "delivery forbidden areas.xml", "working hours.xml");
            if (country == null)
            {
                Console.WriteLine("Check files exits for Martorell in Countries");
                Environment.Exit(0);
            }
            country.SetAvailability(forbidden_flight_areas, controlled_airspace, cities_airspace, airports_airspace);
            mean_time = country.GetMeanTime().TotalHours;
            devStd = country.GetVariance();
            lower_limit = country.GetInitialTime();
            upper_limit = country.GetFinalTime();

            // operator_list = operator_f1.GetOperatorList();
            string file_opers = binaryPath + "\\Operators\\Martorell operators delivery.xml";
            if (String.IsNullOrEmpty(file_opers))
            {
                Console.WriteLine("Error in openning file {0}", file_opers);
                System.Environment.Exit(1);
            }
            operator_list = new OperatorGenerator().LoadOperators(file_opers);
            if (scale!=1)
                ScaleFleet(scale);
            
            //var routesparameters = routes_f1.GetRouteParameters();
            string file_ops = binaryPath + "\\Route_Parameters\\Martorell parameters.xml";
            if (String.IsNullOrEmpty(file_ops))
            {
                Console.WriteLine("Error in openning file {0}", file_ops);
                System.Environment.Exit(1);
            }
            var parameters = new RouteGenerator().LoadRouteParameters(file_ops);
            bvlos_parameters = parameters.Item3;
            delivery_parameters = parameters.Item4;
            operator_generator = new OperatorGenerator(country, null, null, bvlos_parameters, delivery_parameters);

            // drone_type_list = drone_f1.GetListOfDrones();
            string file_drones = binaryPath + "\\Drones\\DroneList.xml";
            if (String.IsNullOrEmpty(file_drones))
            {
                Console.WriteLine("Error in openning file {0}", file_drones);
                System.Environment.Exit(1);
            }
            drone_type_list = dronereader.LoadDroneList(file_drones);

            //DO generation!!
            operator_generator.LoadOperations(operator_list, drone_type_list, parts, mean_time, devStd, lower_limit, upper_limit, rnd);        }

        public void SaveResults(String date)
        {
            TimeSpan initial = lower_limit;
            TimeSpan final = upper_limit;
            List<double> x = new List<double>();
            List<double> y = new List<double>();

            var dist = OperationAnalizer.GetScheduleOperation(operator_list, initial, final);
            x = dist.Item1;
            y = dist.Item2;

            string record_name = @"Record " + date + ".xml";
            OperatorWriter.WriteXMLOperators(country, operator_list, "Results", record_name);

            string fleetofdrones = "Results/fleetofdrones.txt";
            OperatorWriter.WriteOperatorsAndDrones(operator_list, fleetofdrones); // write the operators to a .txt file 
            string timeroute = "Results/Routes_" + date+".kml";
            OperatorWriter.WriteKMLOperations(timeroute, operator_list);
            timeroute = "Results/Routes_" + date + ".csv";
            OperatorWriter.WriteOperationsCSVs(timeroute, operator_list);
            Console.WriteLine("Routes have been saved in {0}", timeroute);
        }
        public void SetSimulation(string filename)
        {
            if (string.IsNullOrEmpty(filename) || String.IsNullOrEmpty(filename))
            {
                Console.WriteLine("Error in opening file {0}", filename);
                System.Environment.Exit(1);
            }
 
            var generation_obtained = OperatorWriter.ReadOperatorsFromXML(filename);
            country = generation_obtained.Item1;
            operator_list = generation_obtained.Item2;
            Console.WriteLine("ALERT: Generating conflicts as for today");
        }
        public void AnalyzeConflicts(float scale, String date)
        {
            string conflicts = "Results/Conflicts_" + date;
            OperationAnalizer.SetSafeDistance(scale);
            List<Conflict> conflict_points = OperationAnalizer.FindTemporalConflicts(operator_list);
            OperationAnalizer.WriteConflicts(conflict_points, conflicts + ".csv");
            //AnalysisWriter.WriteKMLConflicts(conflict_points, conflicts + ".kml");
            //AnalysisWriter.WriteJSONConflicts(conflict_points, conflicts);
            Console.WriteLine("{1} conflicts saved in {0}", conflicts, conflict_points.LongCount());
        }
        public void ShowStatistics()
        {
            Console.WriteLine("MAIN STATISTICS");
            // statistics
            Console.WriteLine("The number of operators was {0}", operator_list.Count);

            int num_ops = 0;
            foreach (Operator op in operator_list)
                num_ops += op.GetOperations().Count;
            Console.WriteLine("The number of operations was {0}", num_ops);

            var dest = OperationAnalizer.GetDistanceStatistics(operator_list);

            double meandistance = Math.Round(dest.Item1, 2);
            double bvlosmeandistance = Math.Round(dest.Item4, 2);
            double deliverymeandistance = Math.Round(dest.Item5, 2);


            Console.WriteLine("Mean Distance {0} meters", meandistance);
            //Console.WriteLine("Mean Distance for Security {0} meters", bvlosmeandistance);
            //Console.WriteLine("Mean Distance for Delivery {0} meters", deliverymeandistance);

            var dams = OperationAnalizer.GetTimeStatistics(operator_list);
            double meaningtime = Math.Round(dams.Item1, 2);
            double bvlosmeaningtime = Math.Round(dams.Item4, 2);
            double deliverymeaningtime = Math.Round(dams.Item5, 2);

            Console.WriteLine("Mean Time {0} minutes", meaningtime);
            //Console.WriteLine("Mean Time for Security {0} minutes", bvlosmeaningtime);
            //Console.WriteLine("Mean Time for Delivery {0} minutes", deliverymeaningtime);
        }
    }
       
    class Program
    {
        static string routes = "";
        static float traffic_scale = 1;
        static float distance_scale = 1;

        static void usage()
        {
            Console.WriteLine("usage: MartorellConsoleApp.exe <options>");
            Console.WriteLine("The <options> can be:");
            Console.WriteLine("  -h             Print this usage statement");
            Console.WriteLine("  -t #           Scale traffic by # factor");
            Console.WriteLine("  -d #           Scale separation distance by # factor");
            Console.WriteLine("  -sim <file>    Use the specified Routesfile");

            System.Environment.Exit(0);
        }

        static void parseCmdLineArgs(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-h")
                    usage();
                else if (args[i] == "-t")
                {
                    Console.WriteLine("...setting traffic scale to {0}", args[i + 1]);
                    traffic_scale = float.Parse(args[i + 1]);
                    i++;
                }
                else if (args[i] == "-d")
                {
                    Console.WriteLine("...setting safe distance scale to {0}", args[i + 1]);
                    distance_scale = float.Parse(args[i + 1]);
                    i++;
                }
                else if (args[i] == "-sim")
                    if (i == args.Length - 1)
                        usage();
                    else
                    {
                        Console.WriteLine("Routes file is {0}", args[i + 1]);
                        routes = args[i + 1];
                        i++;
                    }
                else
                {
                    Console.WriteLine("Unrecognised option {0}", args[i]);
                    usage();
                }
            }
        }
         
        static void Main(string[] args)
        {
            String simdate = DateTime.Now.ToString("dd-MM-yyyy_HHmm");

            Console.WriteLine("Martorell Industrial Polygon traffic generator");
            if (args.Length == 0)
                Console.WriteLine("....Using default configuration");
            else
                parseCmdLineArgs(args);

            Simulation run = new Simulation();
            if (routes == "")
            {
                Console.WriteLine("....Starting simulation....");
                run.MakeSimulation(traffic_scale);
                run.ShowStatistics();
                run.SaveResults(simdate);
                Console.WriteLine("DONE");
            }
            else
            {
                run.SetSimulation(routes);
                run.SaveResults(simdate); //TO BE DELETE !!!
            }

            Console.WriteLine("....Starting conflict calculation....");
            run.AnalyzeConflicts(distance_scale, simdate);
            Console.WriteLine("DONE");


            Console.ReadKey();
        }
    }
}
