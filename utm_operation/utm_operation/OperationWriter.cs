using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using utm_utils;
using System.Xml;

namespace utm_operation
{
    public class OperationWriter
    {
        // Written by Lluís Xavier Herranz on 03/16/2017

        // This class is aimed to write operations performance in csv files.

        // functions


        public static void WriteKMLOperations(string filename, List<Operation> operations)
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
            writer.WriteAttributeString("id", "redLineGreenPoly");
            writer.WriteElementString("name", "BVLOS");
            writer.WriteStartElement("LineStyle"); // start of the element: LineStyle
            writer.WriteElementString("color", "ff0000ff");
            writer.WriteElementString("width", "4");
            writer.WriteEndElement(); // start of the element: LineStyle

            writer.WriteStartElement("PolyStyle"); // start of the element: PolyStyle
            writer.WriteElementString("color", "ff0000ff");
            writer.WriteEndElement(); // end of the element: PolyStyle

            writer.WriteEndElement(); // end of the element: Style

            writer.WriteStartElement("Style"); // start of the document: Style
            writer.WriteAttributeString("id", "yellowLineGreenPoly");
            writer.WriteElementString("name", "EVLOS");
            writer.WriteStartElement("LineStyle"); // start of the element: LineStyle
            writer.WriteElementString("color", "ff00ff00");
            writer.WriteElementString("width", "4");
            writer.WriteEndElement(); // start of the element: LineStyle

            writer.WriteStartElement("PolyStyle"); // start of the element: PolyStyle
            writer.WriteElementString("color", "ff00ff00");
            writer.WriteEndElement(); // end of the element: PolyStyle

            writer.WriteEndElement(); // end of the element: Style

            writer.WriteStartElement("Style"); // start of the document: Style
            writer.WriteAttributeString("id", "greenLineGreenPoly");
            writer.WriteElementString("name", "VLOS");
            writer.WriteStartElement("LineStyle"); // start of the element: LineStyle
            writer.WriteElementString("color", "ff00ffff");
            writer.WriteElementString("width", "4");
            writer.WriteEndElement(); // start of the element: LineStyle

            writer.WriteStartElement("PolyStyle"); // start of the element: PolyStyle
            writer.WriteElementString("color", "7f00ffff");
            writer.WriteEndElement(); // end of the element: PolyStyle

            writer.WriteEndElement(); // end of the element: Style


            writer.WriteStartElement("Style"); // start of the document: Style
            writer.WriteAttributeString("id", "blueLineGreenPoly");
            writer.WriteElementString("name", "Delivery");
            writer.WriteStartElement("LineStyle"); // start of the element: LineStyle
            writer.WriteElementString("color", "ffff0000");
            writer.WriteElementString("width", "4");
            writer.WriteEndElement(); // start of the element: LineStyle

            writer.WriteStartElement("PolyStyle"); // start of the element: PolyStyle
            writer.WriteElementString("color", "7fff0000");
            writer.WriteEndElement(); // end of the element: PolyStyle

            writer.WriteEndElement(); // end of the element: Style

            writer.WriteStartElement("Folder"); // start of the element: Folder
            writer.WriteElementString("name", "random generation points");
            writer.WriteElementString("description", "simulation of different routes spread out throught Spain");

            operations = operations.OrderBy(x => x.GetStartTime()).ToList();
            List<Point> alldiscretizedpoints = new List<Point>();
            foreach (Operation operation in operations)
            {
                // set the required information in Point from Operation: flightID, color of line, and next Lat;lon;alt
                List<Point> discretizedpoints = operation.GetDiscretizedRoute();
                for (int i = 0; i < discretizedpoints.Count - 1; i++)
                {
                    Point p = discretizedpoints[i];
                    p.SetOperation(operation.flightId);
                    switch (operation.GetRouteType())
                    {
                        case RouteType.Delivery: p.SetColor("blueLineGreenPoly"); break;
                        case RouteType.BVLOS: p.SetColor("redLineGreenPoly"); break;
                        case RouteType.VLOS: p.SetColor("greenLineGreenPoly"); break;
                        case RouteType.EVLOS: p.SetColor("yellowLineGreenPoly"); break;
                    }
                    Point pn = discretizedpoints[i + 1];
                    p.SetNextLatitude(pn.GetLatitude());
                    p.SetNextLongitude(pn.GetLongitude());
                    p.SetNextAltitude(pn.GetAltitude());
                }
                alldiscretizedpoints.AddRange(discretizedpoints);
            }

            List<Point> allsorteddiscretizedpoints = alldiscretizedpoints.OrderBy(x => x.GetTime()).ToList();
            foreach (Point dp in allsorteddiscretizedpoints)
                if (dp.GetOperation() != "")  // The last point of each flight has not been set with a FlightID or a nextLat/Lon/Alt 
                {
                    writer.WriteStartElement("Placemark"); // start of the element: Placemark
                    writer.WriteElementString("name", "flight " + dp.GetOperation());  // start of the element: name

                    writer.WriteStartElement("TimeStamp"); // start of the element: TimeStamp
                    string hour = dp.GetTime().Hour.ToString("HH:mm");
                    string time = dp.GetTime().Year + "-" + dp.GetTime().Month.ToString("d2") + "-" + dp.GetTime().Day.ToString("d2") + "T" +
                        dp.GetTime().Hour.ToString("d2") + ":" + dp.GetTime().Minute.ToString("d2") + ":" + dp.GetTime().Second.ToString("d2") + "Z";
                    writer.WriteElementString("when", time);
                    writer.WriteEndElement(); //end of the element: TimeStamp

                    writer.WriteElementString("styleUrl", dp.GetColor()); // element: StyleUrl
                    writer.WriteStartElement("LineString"); // start of the element: LineString

                    writer.WriteElementString("altitudeMode", "relativeToGround");  //  element: altitudeMode

                    writer.WriteStartElement("coordinates");  // start of the element: coordinates
                    string coordinates = dp.GetLongitude() + "," + dp.GetLatitude() + "," + dp.GetAltitude() + "  " +
                                        dp.GetNextLongitude() + "," + dp.GetNextLatitude() + "," + dp.GetNextAltitude();
                    writer.WriteString(coordinates);
                    writer.WriteEndElement(); // end of the element: coordinates

                    writer.WriteEndElement(); // end of the element: LineString
                    writer.WriteEndElement(); // end of the element: Placemark
                }

            //foreach (Operation operation in operations)
            //{               
            //    // for vlos
            //    if (operation.GetRouteType()== RouteType.VLOS)
            //    {
            //        List<Point> discretizedpoints = operation.GetDiscretizedRoute();
            //        for (int i = 0; i < discretizedpoints.Count - 1;i++ )
            //        {
            //            writer.WriteStartElement("Placemark"); // start of the element: Placemark

            //            writer.WriteElementString("name", "flight " + operation.flightId);  // start of the element: name
            //            writer.WriteStartElement("TimeStamp"); // start of the element: TimeStamp

            //            string hour = discretizedpoints[i].GetTime().Hour.ToString("HH:mm");
            //            string time = discretizedpoints[i].GetTime().Year + "-" + discretizedpoints[i].GetTime().Month.ToString("d2") + "-" + discretizedpoints[i].GetTime().Day.ToString("d2") + "T" + discretizedpoints[i].GetTime().Hour.ToString("d2") + ":" + discretizedpoints[i].GetTime().Minute.ToString("d2") + ":" + discretizedpoints[i].GetTime().Second.ToString("d2") + "Z";

            //            writer.WriteElementString("when", time);
            //            writer.WriteEndElement(); //end of the element: TimeStamp

            //            writer.WriteElementString("styleUrl", "#yellowLineGreenPoly"); // element: StyleUrl
            //            writer.WriteStartElement("LineString"); // start of the element: LineString

            //            writer.WriteElementString("altitudeMode", "relativeToGround");  //  element: altitudeMode


            //            writer.WriteStartElement("coordinates");  // start of the element: coordinates
            //            string coordinates = discretizedpoints[i].GetLongitude() + "," + discretizedpoints[i].GetLatitude() + "," + discretizedpoints[i].GetAltitude() + "  " + discretizedpoints[i + 1].GetLongitude() + "," + discretizedpoints[i + 1].GetLatitude() + "," + discretizedpoints[i+1].GetAltitude();
            //            writer.WriteString(coordinates);
            //            writer.WriteEndElement(); // end of the element: coordinates

            //            writer.WriteEndElement(); // end of the element: LineString
            //            writer.WriteEndElement(); // end of the element: Placemark

            //        }


            //    }

            //    // for evlos

            //    if (operation.GetRouteType() == RouteType.EVLOS)
            //    {
            //        List<Point> discretizedpoints = operation.GetDiscretizedRoute();
            //        for (int i = 0; i < discretizedpoints.Count - 1; i++)
            //        {
            //            writer.WriteStartElement("Placemark"); // start of the element: Placemark

            //            writer.WriteElementString("name", "flight " + operation.flightId);  // start of the element: name
            //            writer.WriteStartElement("TimeStamp"); // start of the element: TimeStamp

            //            string hour = discretizedpoints[i].GetTime().Hour.ToString("HH:mm");
            //            string time = discretizedpoints[i].GetTime().Year + "-" + discretizedpoints[i].GetTime().Month.ToString("d2") + "-" + discretizedpoints[i].GetTime().Day.ToString("d2") + "T" + discretizedpoints[i].GetTime().Hour.ToString("d2") + ":" + discretizedpoints[i].GetTime().Minute.ToString("d2") + ":" + discretizedpoints[i].GetTime().Second.ToString("d2") + "Z";

            //            writer.WriteElementString("when", time);
            //            writer.WriteEndElement(); //end of the element: TimeStamp

            //            writer.WriteElementString("styleUrl", "#greenLineGreenPoly"); // element: StyleUrl
            //            writer.WriteStartElement("LineString"); // start of the element: LineString

            //            writer.WriteElementString("altitudeMode", "relativeToGround");  //  element: altitudeMode


            //            writer.WriteStartElement("coordinates");  // start of the element: coordinates
            //            string coordinates = discretizedpoints[i].GetLongitude() + "," + discretizedpoints[i].GetLatitude() + "," + discretizedpoints[i].GetAltitude() + "  " + discretizedpoints[i + 1].GetLongitude() + "," + discretizedpoints[i + 1].GetLatitude() + "," + discretizedpoints[i+1].GetAltitude();
            //            writer.WriteString(coordinates);
            //            writer.WriteEndElement(); // end of the element: coordinates

            //            writer.WriteEndElement(); // end of the element: LineString
            //            writer.WriteEndElement(); // end of the element: Placemark

            //        }


            //    }

            //    // for bvlos

            //    if(operation.GetRouteType() == RouteType.BVLOS)
            //    {

            //        List<Point> discretizedpoints = operation.GetDiscretizedRoute();
            //        for (int i = 0; i < discretizedpoints.Count - 1; i++)
            //        {
            //            writer.WriteStartElement("Placemark"); // start of the element: Placemark

            //            writer.WriteElementString("name", "flight " + operation.flightId);  // start of the element: name
            //            writer.WriteStartElement("TimeStamp"); // start of the element: TimeStamp

            //            string hour = discretizedpoints[i].GetTime().Hour.ToString("HH:mm");
            //            string time = discretizedpoints[i].GetTime().Year + "-" + discretizedpoints[i].GetTime().Month.ToString("d2") + "-" + discretizedpoints[i].GetTime().Day.ToString("d2") + "T" + discretizedpoints[i].GetTime().Hour.ToString("d2") + ":" + discretizedpoints[i].GetTime().Minute.ToString("d2") + ":" + discretizedpoints[i].GetTime().Second.ToString("d2") + "Z";

            //            writer.WriteElementString("when", time);
            //            writer.WriteEndElement(); //end of the element: TimeStamp

            //            writer.WriteElementString("styleUrl", "#redLineGreenPoly"); // element: StyleUrl
            //            writer.WriteStartElement("LineString"); // start of the element: LineString

            //            writer.WriteElementString("altitudeMode", "relativeToGround");  //  element: altitudeMode


            //            writer.WriteStartElement("coordinates");  // start of the element: coordinates
            //            string coordinates = discretizedpoints[i].GetLongitude() + "," + discretizedpoints[i].GetLatitude() + "," + discretizedpoints[i].GetAltitude() + "  " + discretizedpoints[i + 1].GetLongitude() + "," + discretizedpoints[i + 1].GetLatitude() + "," + discretizedpoints[i+1].GetAltitude();
            //            writer.WriteString(coordinates);
            //            writer.WriteEndElement(); // end of the element: coordinates

            //            writer.WriteEndElement(); // end of the element: LineString
            //            writer.WriteEndElement(); // end of the element: Placemark

            //        }


            //    }

            //    // for delivery routes

            //    if(operation.GetRouteType() == RouteType.Delivery)
            //    {
            //        List<Point> discretizedpoints = operation.GetDiscretizedRoute();
            //        for (int i = 0; i < discretizedpoints.Count - 1; i++)
            //        {
            //            writer.WriteStartElement("Placemark"); // start of the element: Placemark

            //            writer.WriteElementString("name", "flight " + operation.flightId);  // start of the element: name
            //            writer.WriteStartElement("TimeStamp"); // start of the element: TimeStamp

            //            string hour = discretizedpoints[i].GetTime().Hour.ToString("HH:mm");
            //            string time = discretizedpoints[i].GetTime().Year + "-" + discretizedpoints[i].GetTime().Month.ToString("d2") + "-" + discretizedpoints[i].GetTime().Day.ToString("d2") + "T" + discretizedpoints[i].GetTime().Hour.ToString("d2") + ":" + discretizedpoints[i].GetTime().Minute.ToString("d2") + ":" + discretizedpoints[i].GetTime().Second.ToString("d2") + "Z";

            //            writer.WriteElementString("when", time);
            //            writer.WriteEndElement(); //end of the element: TimeStamp

            //            writer.WriteElementString("styleUrl", "#blueLineGreenPoly"); // element: StyleUrl
            //            writer.WriteStartElement("LineString"); // start of the element: LineString

            //            writer.WriteElementString("altitudeMode", "relativeToGround");  //  element: altitudeMode


            //            writer.WriteStartElement("coordinates");  // start of the element: coordinates
            //            string coordinates = discretizedpoints[i].GetLongitude() + "," + discretizedpoints[i].GetLatitude() + "," + discretizedpoints[i].GetAltitude() + "  " + discretizedpoints[i + 1].GetLongitude() + "," + discretizedpoints[i + 1].GetLatitude() + "," + discretizedpoints[i+1].GetAltitude();
            //            writer.WriteString(coordinates);
            //            writer.WriteEndElement(); // end of the element: coordinates

            //            writer.WriteEndElement(); // end of the element: LineString
            //            writer.WriteEndElement(); // end of the element: Placemark

            //        }
            //    }

            //}

            writer.WriteEndElement(); // end of the element: Folder

            writer.WriteEndElement();// end of the element: Document
            writer.WriteEndElement(); // end of the element: kml

            writer.Close();

        }

        public static void WriteCSVOperation(string filename, List<Point> route)
        {
            StreamWriter flight = new StreamWriter(filename);
            flight.Write("Time\tFlight\tLat\tLon\tAlt\n"); // header
            String f_id = route.First().GetOperation().ToString();
            foreach (Point dp in route)
            {
                string hour = dp.GetTime().Hour.ToString("HH:mm");
                string time = dp.GetTime().Year + "-" + dp.GetTime().Month.ToString("d2") + "-" + dp.GetTime().Day.ToString("d2") + "T" +
                    dp.GetTime().Hour.ToString("d2") + ":" + dp.GetTime().Minute.ToString("d2") + ":" + dp.GetTime().Second.ToString("d2") + "Z";
                flight.Write(time);

                flight.Write("\t" + f_id);

                flight.Write("\t" + dp.GetLongitude() + "\t" + dp.GetLatitude() + "\t" + dp.GetAltitude() + "\n");

            }
            flight.Close();
        }
        public static void WriteCSVOperations(string route, string date, List<Operation> operations)
        {
            StreamWriter writer = new StreamWriter(route + "_" + date + "_ALL.csv");

            writer.Write("Time\tFlight\tLon\tLat\tAlt\tOperator\n"); // header

            //operations = operations.OrderBy(x => x.StartTime).ToList();
            List<Point> alldiscretizedpoints = new List<Point>();
            foreach (Operation operation in operations)
            {
                // set the required information in Point from Operation: flightID, color of line, and next Lat;lon;alt
                List<Point> discretizedpoints = operation.GetDiscretizedRoute();
                discretizedpoints.Last().SetOperation(discretizedpoints.First().GetOperation());
                //WriteCSVOperation(route + "_" + date + "_" + operation.flightId.ToString() + ".csv", discretizedpoints);
                alldiscretizedpoints.AddRange(discretizedpoints);
            }

            List<Point> allsorteddiscretizedpoints = alldiscretizedpoints.OrderBy(x => x.GetTime()).ToList();
            foreach (Point dp in allsorteddiscretizedpoints)
            {
                //Time  Flight  Lat Lon Alt
                string hour = dp.GetTime().Hour.ToString("HH:mm");
                string time = dp.GetTime().Year + "-" + dp.GetTime().Month.ToString("d2") + "-" + dp.GetTime().Day.ToString("d2") + "T" +
                    dp.GetTime().Hour.ToString("d2") + ":" + dp.GetTime().Minute.ToString("d2") + ":" + dp.GetTime().Second.ToString("d2") + "Z";
                writer.Write(time);

                writer.Write("\t" + dp.GetOperation());

                writer.Write("\t" + dp.GetLongitude() + "\t" + dp.GetLatitude() + "\t" + dp.GetAltitude());
                //writer.Write("\t" + dp.. .operatorName );
                writer.Write("\n");
            }
            writer.Close();

        }
       
        public void WriteOperatiosInCSV(string filename, List<Operation> operations)
        {
            StreamWriter f1 = new StreamWriter(filename);

            f1.WriteLine("ICAOdroneName,DepartureTime,ArrivalTime,LatHome,LongHome,CruisingSpeed,ScanSpeed"); // file header

            foreach(Operation op in operations)
            {
                if(op.GetRouteType()==RouteType.VLOS)
                    f1.WriteLine(op.GetAircraft().GetIdentifier() + "," + op.GetStartTime().ToString("HH:mm:ss") + "," + op.GetFinalTime().ToString("HH:mm:ss") + "," + op.GetVLOSRoute().GetOriginPoint().GetLatitude() + "," + op.GetVLOSRoute().GetOriginPoint().GetLongitude() + "," + op.GetAircraft().GetCruisingSpeed() + "," + op.GetAircraft().GetScanSpeed());

                if (op.GetRouteType() == RouteType.EVLOS)
                    f1.WriteLine(op.GetAircraft().GetIdentifier() + "," + op.GetStartTime().ToString("HH:mm:ss") + "," + op.GetFinalTime().ToString("HH:mm:ss") + "," + op.GetEVLOSRoute().GetOriginPoint().GetLatitude() + "," + op.GetEVLOSRoute().GetOriginPoint().GetLongitude() + "," + op.GetAircraft().GetCruisingSpeed() + "," + op.GetAircraft().GetScanSpeed());


                if(op.GetRouteType()==RouteType.BVLOS)
                f1.WriteLine(op.GetAircraft().GetIdentifier()+","+op.GetStartTime().ToString("HH:mm:ss")+","+op.GetFinalTime().ToString("HH:mm:ss")+","+op.GetBVLOSRoute().GetOriginPoint().GetLatitude()+","+op.GetBVLOSRoute().GetOriginPoint().GetLongitude()+","+op.GetAircraft().GetCruisingSpeed()+","+op.GetAircraft().GetScanSpeed());

                if(op.GetRouteType()==RouteType.Delivery)
                 f1.WriteLine(op.GetAircraft().GetIdentifier() + "," + op.GetStartTime().ToString("HH:mm:ss") + "," + op.GetFinalTime().ToString("HH:mm:ss") + "," + op.GetDeliveryRoute().GetOriginPoint().GetLatitude() + "," + op.GetDeliveryRoute().GetOriginPoint().GetLongitude() + "," + op.GetAircraft().GetCruisingSpeed() + "," + op.GetAircraft().GetScanSpeed());
            
            
            }

            f1.Close();
        }
    }
}
