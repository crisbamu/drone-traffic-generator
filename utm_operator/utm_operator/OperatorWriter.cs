using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using utm_operation;
using utm_drone;
using utm_routes;
using utm_routes.VLOS_routes;
using utm_routes.BVLOS_routes;
using utm_routes.DeliveryRoutes;
using utm_routes.EVLOSRoutes;
using utm_utils;
using System.Xml.Serialization;
using utm_country;

namespace utm_operator
{
    public static class OperatorWriter
    {
        // Written by Lluís Xavier Herranz on 04/25/2017

        // This class aims at writing files about Operators

        public static void WriteOperators(string filename, string foldername, List<Operator> operators)
        {
            //This function writes the operators into a file

            //String directory = AppDomain.CurrentDomain.BaseDirectory;
            //System.IO.DirectoryInfo di = new DirectoryInfo(directory + "\\" + foldername + "\\");
            System.IO.DirectoryInfo di = new DirectoryInfo(foldername);
            if (!di.Exists)  // if it doesn't exist, create
                di.Create();

            StreamWriter writer = new StreamWriter(di +"\\"+ filename);
            //writer.WriteLine("opidentifier/routetype/droneidentifier/starttime/endtime/LatHome/LongHome/mavlinkfile");

            foreach (Operator op in operators)
            {
                foreach (Operation operation in op.GetOperations())
                {
                    writer.Write(op.GetIdentifier() + "\t");
                    writer.Write(operation.GetRouteType() + "\t");
                    writer.Write(operation.GetAircraft().GetIdentifier() + "\t");
                    writer.Write((operation.GetStartTime().ToString("HH:mm:ss")) + "\t");
                    writer.Write((operation.GetFinalTime().ToString("HH:mm:ss")) + "\t");
                    if (operation.GetRouteType().Equals("vlos"))
                    {
                        writer.Write(operation.GetVLOSRoute().GetOriginPoint().GetLatitude().ToString() + "\t");
                        writer.Write(operation.GetVLOSRoute().GetOriginPoint().GetLongitude().ToString() + "\t");
                    }

                    if (operation.GetRouteType().Equals("bvlos"))
                    {
                        writer.Write(operation.GetBVLOSRoute().GetOriginPoint().GetLatitude().ToString() + "\t");
                        writer.Write(operation.GetBVLOSRoute().GetOriginPoint().GetLongitude().ToString() + "\t");
                    }

                    if (operation.GetRouteType().Equals("delivery"))
                    {
                        writer.Write(operation.GetDeliveryRoute().GetOriginPoint().GetLatitude().ToString() + "\t");
                        writer.Write(operation.GetDeliveryRoute().GetOriginPoint().GetLongitude().ToString() + "\t");
                    }
                    writer.Write((operation.GetMavLinkIdentification()) + "\r\n");
                }

            }
            writer.Close();
        }


        public static void FragmentFile(string filename, String foldername, string outputfilename, int fragments)
        {
            // This function splits the operations into as many files as requested by the user. 

            // count the lines of the file

            //String directory = AppDomain.CurrentDomain.BaseDirectory;
           // System.IO.DirectoryInfo di = new DirectoryInfo(directory + "\\" + foldername + "\\");
            StreamReader counter = new StreamReader(foldername + "\\"+filename);
            int n = 0;
            while (counter.Peek() >= 0)
            {
                string line = counter.ReadLine();
                n++;
            }
            double parts = n / (fragments);
            // if the parts are smaller than one, it becomes 1.
            if(parts<1)
            {
                parts = 1;
            }
            parts = Math.Round(parts, 0);
            counter.Close();
            StreamReader reader = new StreamReader(foldername + "\\"+filename);
            int i = 0;
            int x = 1;
            while (i < n)
            {
                string output = x + "_" + outputfilename;
                StreamWriter writer = new StreamWriter(foldername + "\\"+ output);
                int z = 0;
                double cloneparts = parts;
                if (cloneparts > (n - i - cloneparts))
                {
                    cloneparts = n - i + cloneparts;
                }
                while (z < cloneparts)
                {
                    string line = reader.ReadLine();
                    writer.WriteLine(line);
                    z++;
                }
                cloneparts = parts;
                i = i + z;
                x++;
                writer.Close();
            }

            reader.Close();
        }

        public static void WriteFragmentedOperations(string filename, string foldername, List<Operator> operators, int fragmentedparts)
        {
            // This function writes the fragmented operations into a file inserted by the user. 

            double precalculus = operators.Count / fragmentedparts;
            double fragments = Math.Round(precalculus, 0);

            foreach (Operator op in operators)
            {
                int i = 0;
                int z = 0;


                while (i < op.GetOperations().Count)
                {
                    int n = 0;
                    Operator newoperator = new Operator();
                    newoperator.SetIdentifier(op.GetIdentifier());
                    List<Operation> newoperations = new List<Operation>();

                    // fill the new operator with the operations from the original operator every fragmented time.
                    while (n < fragments)
                    {
                        newoperations.Add(op.GetOperations()[i]);
                        newoperator.SetOperations(newoperations);
                        n++;
                        i++;
                    }

                    if (op.GetOperations().Count - n < fragments)
                    {
                        fragments = op.GetOperations().Count - n;
                    }

                    List<Operator> preliminarylist = new List<Operator>();
                    preliminarylist.Add(newoperator);

                    string newfilename = "z_" + filename;
                    WriteOperators(newfilename, foldername, preliminarylist);
                    //i=i + n;
                    z++;
                }
                fragments = Math.Round(precalculus, 0);
            }

        }

        public static void WriteKMLOperations(string filename, List<Operator> operators)
        {
            // This function writes the operations in a KML file
            List<Operation> operations = GetListOfOperations(operators);
            new Operation().WriteKMLOperations(filename, operations);
        }

        public static void WriteOperationsCSVs(string filename, List<Operator> operators)
        {
            // This function writes the operations in a KML file
            List<Operation> operations = GetListOfOperations(operators);
            new Operation().WriteCSVOperations(filename, operations);
        }
        public static void WriteMavLinkOperations(List<Operator> operators,  String foldername)
        {
            // This function writes the operations in Mavlink files. 
            MavLinkWriter writer = new MavLinkWriter();
            writer.DeleteElementsinFile(foldername);
            int n = 0;
            foreach (Operator op in operators)
            {
                List<Operation> operations = op.GetOperations();
                foreach (Operation operation in operations)
                {
                    if (operation.GetRouteType() == RouteType.VLOS)
                    {
                        string fileid = "flight" + n + "_vlos.waypoints";
                        operation.SetMavLinkIdentification(fileid);
                        writer.WriteMavLinkFile(fileid, foldername, operation);
                        n++;
                    }

                    if (operation.GetRouteType() == RouteType.EVLOS)
                    {
                        string fileid = "flight" + n + "_evlos.waypoints";
                        operation.SetMavLinkIdentification(fileid);
                        writer.WriteMavLinkFile (fileid,foldername, operation);
                        n++;
                    }

                    if (operation.GetRouteType() == RouteType.BVLOS)
                    {
                        string fileid = "flight" + n + "_bvlos.waypoints";
                        operation.SetMavLinkIdentification(fileid);
                        writer.WriteMavLinkFile(fileid,foldername, operation);
                        n++;
                    }

                    if (operation.GetRouteType() == RouteType.Delivery)
                    {
                        string fileid = "flight" + n + "_delivery.waypoints";
                        operation.SetMavLinkIdentification(fileid);
                        writer.WriteMavLinkFile(fileid,foldername, operation);
                        n++;
                    }


                }
            }
        }

        public static void WriteCSVOperations(string filename, List<Operator> operators)
        {
            // This function writes the operations into a csv file
            List<Operation> operations = GetListOfOperations(operators);
            CSVWriter writer = new CSVWriter();
            writer.WriteOperatiosInCSV(filename, operations);
        }

        public static List<Operation> GetListOfOperations(List<Operator> operators)
        {
            // This function obtains the whole amount of operations from a list of operations
            List<Operation> operations = new List<Operation>();

            foreach (Operator op in operators)
            {
                operations.AddRange(op.GetOperations());  // obtain the whole list of operations
            }

            return operations;

        }

        public static void WriteOperatorsAndDrones(List<Operator> operators, string filename)
        {
            // This function writes the operators and their drones in a file inserted by the user.

            StreamWriter writer = new StreamWriter(filename);
            foreach (Operator op in operators)
            {
                writer.WriteLine("operator: " + op.GetIdentifier());
                writer.WriteLine("fleetofdrones:");
                foreach (drone aircraft in op.GetFleetOfDrones())
                {
                    writer.WriteLine(aircraft.GetIdentifier());
                }
                writer.WriteLine(Environment.NewLine);
            }
            writer.Close();
        }

        public static void WriteXMLOperators(Country country, List<Operator> operator_list,string foldername ,string filename)
        {
            // This function wirtes the operators and their operations in a xml file. 
            String directory = AppDomain.CurrentDomain.BaseDirectory;
            System.IO.DirectoryInfo di = new DirectoryInfo(directory + "\\" + foldername + "\\");

            if (!di.Exists)  // if it doesn't exist, create
                di.Create();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;

            XmlWriter writer = XmlWriter.Create(di+filename, settings);
            writer.WriteStartDocument();

            //text

            writer.WriteStartElement("generation"); // start fo the element: generation


            writer.WriteStartElement("country"); // start of the element: country
            writer.WriteElementString("name", country.GetCountryName().ToString());
                writer.WriteEndElement(); // end of the element: country
            
            writer.WriteStartElement("operators"); // start of the element: operators

            foreach(Operator op in operator_list)
            {
                writer.WriteStartElement("operator"); // start of the element: operator
                writer.WriteAttributeString("ID", op.GetIdentifier());
                writer.WriteStartElement("fleet-of-drones"); // start of the element: fleet-of-drone
                foreach(drone aircraft in op.GetFleetOfDrones())
                {
                    writer.WriteStartElement("drone"); // start of the element: drone
                    writer.WriteAttributeString("ICAO-code", aircraft.GetIdentifier());
                    writer.WriteElementString("type", aircraft.GetModel());
                    writer.WriteElementString("cruising-speed", aircraft.GetCruisingSpeed().ToString());
                    writer.WriteElementString("scan-speed", aircraft.GetScanSpeed().ToString());
                    writer.WriteElementString("climbing-speed", aircraft.GetMaxClimbingSpeed().ToString());
                    writer.WriteElementString("descend-speed", aircraft.GetMaxDescendingSpeed().ToString());
                    writer.WriteEndElement(); // end of the element: drone
                    
                }
                writer.WriteEndElement(); // end of the element: fleet-of-drone
                writer.WriteStartElement("operations"); // start of the element: operations
                foreach (Operation operation in op.GetOperations())
                {
                    writer.WriteStartElement("operation"); // start of the element: operation
                    writer.WriteElementString("drone", operation.GetAircraft().GetIdentifier());
                    writer.WriteElementString("departure-time", operation.GetStartTime().ToString());
                    writer.WriteElementString("arrival-time", operation.GetFinalTime().ToString());
                    
                    if(operation.GetRouteType()==RouteType.VLOS)
                    {
                        VLOSRoute route = operation.GetVLOSRoute();
                        writer.WriteElementString("type-of-operation", "VLOS");
                        writer.WriteElementString("flight-altitude", (Math.Round(route.GetAltitude(),5)).ToString());
                        writer.WriteElementString("origin-point", route.GetOriginPoint().GetLatitude() + " " + route.GetOriginPoint().GetLongitude());
                        writer.WriteElementString("first-point", route.GetFirstPoint().GetLatitude() + " " + route.GetFirstPoint().GetLongitude());
                        writer.WriteElementString("second-point", route.GetSecondPoint().GetLatitude() + " " + route.GetSecondPoint().GetLongitude());
                        writer.WriteStartElement("rectangle"); // start of the element: rectangle
                        writer.WriteElementString("first-rectangle-point", route.GetRectangle().GetFirst().GetLatitude() + " " + route.GetRectangle().GetFirst().GetLongitude());
                        writer.WriteElementString("second-rectangle-point", route.GetRectangle().GetSecond().GetLatitude() + " " + route.GetRectangle().GetSecond().GetLongitude());
                        writer.WriteElementString("third-rectangle-point", route.GetRectangle().GetThird().GetLatitude() + " " + route.GetRectangle().GetThird().GetLongitude());
                        writer.WriteElementString("fourth-rectangle-point", route.GetRectangle().GetFourth().GetLatitude() + " " + route.GetRectangle().GetFourth().GetLongitude());
                        writer.WriteEndElement(); // end of the element: rectangle
                        writer.WriteStartElement("scan"); // start of the element: scan
                        foreach(Point point in route.GetRectangle().GetScan())
                        {
                            writer.WriteElementString("scan-point", point.GetLatitude() + " " + point.GetLongitude());
                        }
                        writer.WriteEndElement(); // end of the element: scan
                    }
                    if (operation.GetRouteType() == RouteType.EVLOS)
                    {
                        EVLOSRoute route = operation.GetEVLOSRoute();
                        writer.WriteElementString("type-of-operation", "EVLOS");
                        writer.WriteElementString("flight-altitude", route.GetAltitude().ToString());
                        writer.WriteElementString("origin-point", route.GetOriginPoint().GetLatitude() + " " + route.GetOriginPoint().GetLongitude());
                        writer.WriteElementString("first-point", route.GetFirstPoint().GetLatitude() + " " + route.GetFirstPoint().GetLongitude());
                        writer.WriteElementString("second-point", route.GetSecondPoint().GetLatitude() + " " + route.GetSecondPoint().GetLongitude());
                        writer.WriteStartElement("rectangle"); // start of the element: rectangle
                        writer.WriteElementString("first-rectangle-point", route.GetRectangle().GetFirst().GetLatitude() + " " + route.GetRectangle().GetFirst().GetLongitude());
                        writer.WriteElementString("second-rectangle-point", route.GetRectangle().GetSecond().GetLatitude() + " " + route.GetRectangle().GetSecond().GetLongitude());
                        writer.WriteElementString("third-rectangle-point", route.GetRectangle().GetThird().GetLatitude() + " " + route.GetRectangle().GetThird().GetLongitude());
                        writer.WriteElementString("fourth-rectangle-point", route.GetRectangle().GetFourth().GetLatitude() + " " + route.GetRectangle().GetFourth().GetLongitude());
                        writer.WriteEndElement(); // end of the element: rectangle
                        writer.WriteStartElement("scan"); // start of the element: scan
                        foreach (Point point in route.GetRectangle().GetScan())
                        {
                            writer.WriteElementString("scan-point", point.GetLatitude() + " " + point.GetLongitude());
                        }
                        writer.WriteEndElement(); // end of the element: scan
                    }
                    if(operation.GetRouteType()==RouteType.BVLOS)
                    {
                        BVLOSRoute route = operation.GetBVLOSRoute();
                        writer.WriteElementString("type-of-operation", "BVLOS");
                        writer.WriteElementString("flight-altitude", route.GetAltitude().ToString());
                        writer.WriteElementString("origin-point", route.GetOriginPoint().GetLatitude() + " " + route.GetOriginPoint().GetLongitude());
                        writer.WriteElementString("first-intermediate-point", route.GetFirstIntermediate().GetLatitude() + " " + route.GetFirstIntermediate().GetLongitude());
                        writer.WriteStartElement("BVLOS-points"); // start of the element: BVLOS-points
                        foreach(BVLOSPoint bvlospoint in route.GetBVlosList())
                        {
                            writer.WriteStartElement("BVLOS-point"); // start of the element: bvlos-point
                            writer.WriteElementString("center-point", bvlospoint.GetPoint().GetLatitude() + " " + bvlospoint.GetPoint().GetLongitude());
                            writer.WriteStartElement("rectangle"); // start of the element: rectangle
                            writer.WriteElementString("first-rectangle-point", bvlospoint.GetRectangle().GetFirst().GetLatitude() + " " + bvlospoint.GetRectangle().GetFirst().GetLongitude());
                            writer.WriteElementString("second-rectangle-point", bvlospoint.GetRectangle().GetSecond().GetLatitude() + " " + bvlospoint.GetRectangle().GetSecond().GetLongitude());
                            writer.WriteElementString("third-rectangle-point", bvlospoint.GetRectangle().GetThird().GetLatitude() + " " + bvlospoint.GetRectangle().GetThird().GetLongitude());
                            writer.WriteElementString("fourth-rectangle-point", bvlospoint.GetRectangle().GetFourth().GetLatitude() + " " + bvlospoint.GetRectangle().GetFourth().GetLongitude());
                            writer.WriteEndElement(); // end of the element: rectangle
                            writer.WriteStartElement("scan"); // start of the element: scan
                            foreach(Point point in bvlospoint.GetRectangle().GetScan())
                            {
                                writer.WriteElementString("scan-point", point.GetLatitude() + " " + point.GetLongitude());
                            }
                            writer.WriteEndElement(); // end of the element: scan
                            writer.WriteEndElement(); // end of the element: bvlos-point
                        }
                        writer.WriteEndElement(); // end of the element: BVLOS-points
                        writer.WriteElementString("second-intemediate-point", route.GetSecondIntermediate().GetLatitude() + " " + route.GetSecondIntermediate().GetLongitude());
                    }
                    if(operation.GetRouteType()==RouteType.Delivery)
                    {
                        DeliveryRoute route = operation.GetDeliveryRoute();
                        writer.WriteElementString("type-of-operation", "Delivery");
                        writer.WriteElementString("flight-altitude", route.GetAltitude().ToString());
                        writer.WriteElementString("origin-point", route.GetOriginPoint().GetLatitude() + " " + route.GetOriginPoint().GetLongitude());
                        writer.WriteStartElement("route-points"); // start of the element: route-points
                        foreach (Point point in route.GetRoute())
                        {
                            writer.WriteElementString("route-point", point.GetLatitude() + " " + point.GetLongitude());
                        }
                        writer.WriteEndElement(); // end of the element: route-points
                    }
                    writer.WriteStartElement("discretized-route"); // start of the element: discretized-route
                    foreach(Point punto in operation.GetDiscretizedRoute())
                    {
                        writer.WriteStartElement("point");// start of the element: point
                        writer.WriteElementString("coordinates", punto.GetLatitude() + " " + punto.GetLongitude());
                        writer.WriteElementString("altitude", (Math.Round(punto.GetAltitude(),5)).ToString());
                        writer.WriteElementString("time", punto.GetTime().ToString());
                            writer.WriteEndElement(); // end of the element: point
                    }
                    writer.WriteEndElement(); // end of the element: discretized-route
                    writer.WriteEndElement(); // end of the element: operation
                }
                writer.WriteEndElement(); // end of the element: operations
                writer.WriteEndElement(); // end of the element: operator
            }
            writer.WriteEndElement(); // end of the element: operators

            writer.WriteEndElement(); // end of the element: generation
           
            writer.WriteEndDocument();
            //
            writer.Close();
        }

      


        public static drone FindDroneInList(List<drone> drone_list, string ICAO_code)
        {
            // This function finds the drone in a list of drone by comparing its ICAO code.
            drone new_drone = new drone();
            foreach(drone aircraft in drone_list)
            {
                if(aircraft.GetIdentifier().Equals(ICAO_code))
                {
                    new_drone = aircraft.GetCopy();
                    break;
                }

            }
            return new_drone;
        }

        public static Point ConvertStringToPoint(string information)
        {
            //This function converts the string containing the latitude and longitude of a point and transforms it in a point. 
            string[] words = information.Split(' ');
            double latitude =Convert.ToDouble(words[0]);
            double longitude =Convert.ToDouble(words[1]);
            Point new_point = new Point(latitude, longitude);
            return new_point;
        }

        public static Tuple<Country, List<Operator>> ReadOperatorsFromXML(String filename)
        {
            // This function reads the operators from a XML file and generate a list of operators and their country associated. 
            XmlSerializer serializer = new XmlSerializer(typeof(generation));

            // Declare an object variable of the type to be deserialized.
            generation i;

            using (Stream reader = new FileStream(filename, FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                i = (generation)serializer.Deserialize(reader);
            }

            XMLOperatorStructure structurer = new XMLOperatorStructure();
            var results = structurer.GetGeneration(i);
            
            return results;
        }

        public static Tuple<TimeSpan,TimeSpan> FindTimeLimits(List<Operator> operator_list)
        {
            // This function finds the time limits of working hours from a list of operator. 
            List<Operation> operation_list = GetListOfOperations(operator_list);
            TimeSpan lower_limit = operation_list[0].GetStartTime().TimeOfDay;
            TimeSpan upper_limit = operation_list[0].GetFinalTime().TimeOfDay;

            for (int i = 1; i < operation_list.Count; i++)
            {
                if(operation_list[i].GetStartTime().TimeOfDay<lower_limit)
                {
                    lower_limit = operation_list[i].GetStartTime().TimeOfDay;
                }

                if(operation_list[i].GetFinalTime().TimeOfDay>upper_limit)
                {
                    upper_limit = operation_list[i].GetFinalTime().TimeOfDay;
                }
            }

            return new Tuple<TimeSpan,TimeSpan>(lower_limit,upper_limit);
        }

       


    }
}
