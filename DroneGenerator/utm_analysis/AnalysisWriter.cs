using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using utm_country;
using utm_drone;
using utm_operation;
using utm_operator;
using utm_routes;
using utm_routes.BVLOS_routes;
using utm_routes.DeliveryRoutes;
using utm_routes.VLOS_routes;
using utm_utils;
using System.IO;
using System.Xml;
using Newtonsoft.Json;


namespace utm_analysis
{
    // class used to generate a JSON file of each conflict to be used by Antonia
    // Written by CBM on Sep 2020
    public class Geometry
    {
        public string type;
        public string[,] coordinates;
        public Geometry(string s)
        {
            this.type = s;
        }
    }
    public class Property
    {
        public long[] time;
        public Property ()
        {
            this.time = new long[0];
        }
    }

    public class jconflictroute
    {
        public string type;
        public Geometry geometry=new Geometry("");
        public Property properties =new Property();

        public static long ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return (Convert.ToInt64(diff.TotalSeconds));
        }
        public jconflictroute(Operation op)
        {
            this.type = "Feature";
            this.geometry = new Geometry("MultiPoint");
            this.properties = new Property();

            List<Point> r=new List<Point>();
            if (op.GetRouteType() == RouteType.Delivery) {
                r = op.GetDeliveryRoute().GetDiscretizedRoute();
            } else if (op.GetRouteType() == RouteType.BVLOS) {
                r = op.GetBVLOSRoute().GetDiscretizedRoute();
            }
            this.geometry.coordinates = new string[r.Count,2];
            this.properties.time = new long[r.Count];
            int i = 0;
            foreach (Point p in r)
            {
                this.geometry.coordinates[i, 0] = p.GetLongitude().ToString();
                this.geometry.coordinates[i, 1] = p.GetLatitude().ToString();
                this.properties.time[i] = ConvertToUnixTimestamp(p.GetTime())*1000; // in miliseconds
                i++;
            }
        }

    }
    public static class AnalysisWriter
    {
        // this class aims at writing into different files (KML, CSV...) the results of the analysis of the operations performed.

        //Written by Lluís Xavier Herranz on 05/30/2017

       

        // functions
        // this function generates a circle with an inserted a point and a radius.
        public static List<Point> GenerateCircleAroundAPoint(Point centerpoint, double radius)  
        {
            List<Point> circle = new List<Point>();
            double bearing = 0;
            while(bearing<=2*Math.PI)
            {
                Point punto = LatLongProjection.DestinyPoint(centerpoint, radius, bearing);
                circle.Add(punto);
                bearing = bearing + (2 * Math.PI / 180);
            }
            return circle;
        }

        // this function writes the circle created with the previous function to a KML files.
        public static void WriteCircleToKML(List<List<Point>> circle, string filename)
        {
             XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;

            XmlWriter writer = XmlWriter.Create(filename, settings);
            writer.WriteStartDocument();

            //writer.WriteStartElement("kml");  // here we declare that the file is a kml
            //writer.WriteStartElement("kml", "http://www.opengis.net/kml/2.2");
            writer.WriteStartElement("kml", "http://earth.google.com/kml/2.2");

            writer.WriteStartElement("Document"); // start of the element: Document

            writer.WriteStartElement("Style"); // start of the document: Style
            writer.WriteAttributeString("id", "thickBlackLine");
            writer.WriteStartElement("LineStyle"); // start of the element: LineStyle
            writer.WriteElementString("color", "64000000");
            writer.WriteElementString("width", "8");
            writer.WriteEndElement(); // start of the element: LineStyle
            writer.WriteEndElement(); // start of the element: LStyle

            writer.WriteStartElement("Style"); // start of the document: Style
            writer.WriteAttributeString("id", "thickGreyLine");
            writer.WriteStartElement("LineStyle"); // start of the element: LineStyle
            writer.WriteElementString("color", "506E6E6E");
            writer.WriteElementString("width", "8");
            writer.WriteEndElement(); // start of the element: LineStyle
            writer.WriteEndElement(); // start of the element: LStyle
            

            foreach (List<Point> list in circle)
            {
                writer.WriteStartElement("Placemark"); // start of the element: Placemark
                writer.WriteElementString("name", "circle");
                writer.WriteElementString("styleUrl", "#thickBlackLine"); // element: StyleUrl
                writer.WriteStartElement("LineString"); // start of the element: LineString
                writer.WriteElementString("altitudeMode", "relativeToGround");
                writer.WriteElementString("extrude", "1");
                writer.WriteStartElement("coordinates"); // start of the element: coordinates
                foreach (Point punto in list)
                {
                    string coordinates = punto.GetLongitude() + "," + punto.GetLatitude() + ",100 ";
                    writer.WriteString(coordinates);
                }
                writer.WriteEndElement(); // end of the element: coordinates
                writer.WriteEndElement(); // end of the element: LineString
                writer.WriteEndElement(); // end of the element: Placemark
            }
            
            writer.WriteEndElement(); // end of the element: Document

            writer.Close();
        }

        // this function writes a KML containing all the busy areas inserted by the user.
        public static void GenerateKMLBusyAreas(List<Tuple<Operation, Operation, utm_utils.Point, double>> inf, string filename)
        {
            List<List<Point>> circles = new List<List<Point>>();
            foreach(Tuple<Operation, Operation, utm_utils.Point, double> var in inf)
            {
                Point centerpoint = var.Item3;
                double radius = var.Item4;
                List<Point> circle = GenerateCircleAroundAPoint(centerpoint, radius);
                circles.Add(circle);
            }
            WriteCircleToKML(circles, filename);

        }

        
        // this function writes all the conflicts inserted by the user to a KML file.
        // Whats the differences with the function in OperationAnalizer???
        public static void WriteKMLConflicts(List<Conflict> list, string filename)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;

            XmlWriter writer = XmlWriter.Create(filename, settings);
            writer.WriteStartDocument();

            writer.WriteStartElement("kml", "http://www.opengis.net/kml/2.2");

            writer.WriteStartElement("Document"); // start of the element: Document

            list = list.OrderBy( x => x.GetCpa().GetTime()).ToList();
            foreach (Conflict punto in list)
            {
                writer.WriteStartElement("Placemark"); // start of the element: Placemark
                writer.WriteElementString("name", "alt "+punto.GetAlt() + ": flights" + punto.GetOp1().flightId + "-" + punto.GetOp2().flightId+": CPA Dist "+ punto.GetCpaDist());

                writer.WriteStartElement("TimeStamp"); // start of the element: TimeStamp
                string time = punto.GetCpa().GetTime().Year + "-" + punto.GetCpa().GetTime().Month.ToString("d2") + "-" + punto.GetCpa().GetTime().Day.ToString("d2") + "T" +
                    punto.GetCpa().GetTime().Hour.ToString("d2") + ":" + punto.GetCpa().GetTime().Minute.ToString("d2") + ":" + punto.GetCpa().GetTime().Second.ToString("d2") + "Z";
                writer.WriteElementString("when", time);
                writer.WriteEndElement(); //end of the element: TimeStamp

                writer.WriteStartElement("Point"); // start of the element: Point
                writer.WriteElementString("altitudeMode", "relativeToGround");  // start of the element: altitudeMode
                writer.WriteElementString("extrude", "1"); // start of the element: extrude
                writer.WriteElementString("coordinates", punto.GetCpa().GetLongitude() + "," + punto.GetCpa().GetLatitude() + "," + punto.GetCpa().GetAltitude());
                writer.WriteEndElement(); // end of the element: Point
                writer.WriteEndElement(); // end of the element: Placemark
            }
            writer.WriteEndElement(); // end of the element: Document
            writer.WriteEndDocument(); // end of the document
            writer.Close();
        }

        // this function writes all the conflicts inserted by the user to a ste of SJON files.
        public static void WriteJSONConflicts(List<Conflict> list, string pathString)
        {
            if (list.Count == 0)
                return;
            //String day = list[0].GetCpa().GetTime().ToString("dd-MM-yyyy");
            // string pathString = @"Results" + day;
            //Directory.CreateDirectory(pathString);
            Directory.CreateDirectory(pathString);

            // write each conflict as a JSON file
            int n = 1;
            foreach (Conflict punto in list) if (punto.GetAlt()!=0)
            {
                string conflictname = pathString + "/Conflict" + n.ToString() + ".js";
                StreamWriter f = new StreamWriter(conflictname);

                string stime = punto.GetCpa().GetTime().Hour.ToString("d2") + ":" + punto.GetCpa().GetTime().Minute.ToString("d2") + "Z";
                f.WriteLine("// Conflict ALT is "+ punto.GetAlt().ToString()+" m at time "+ stime);
                f.WriteLine("var tillicum = ");
                jconflictroute op1 = new jconflictroute(punto.GetOp1());
                var stringjson = JsonConvert.SerializeObject(op1, Newtonsoft.Json.Formatting.Indented);
                f.WriteLine(stringjson + ";");

                f.WriteLine("var houseToCoordley = ");
                jconflictroute op2 = new jconflictroute(punto.GetOp2());
                stringjson = JsonConvert.SerializeObject(op2, Newtonsoft.Json.Formatting.Indented);
                f.WriteLine(stringjson + ";");

                f.WriteLine("var demoTracks = [houseToCoordley, tillicum];");
                f.Close();
                n++;
            }
        }
    }
}
