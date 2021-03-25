using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using utm_drone;
using utm_routes;
using utm_routes.VLOS_routes;
using utm_routes.BVLOS_routes;
using utm_routes.EVLOSRoutes;
using utm_routes.DeliveryRoutes;
using System.IO;
using System.Xml;
using utm_utils;


namespace utm_operation
{
    public class Operation
    {
        // Attributes
        private DateTime StartTime;
        private DateTime FinalTime;
        private drone aircraft;
        private RouteType routeType; 
        private VLOSRoute vlosroute;
        private EVLOSRoute evlosroute;
        private BVLOSRoute bvlosroute;
        private DeliveryRoute deliveryroute;
        static Random randomSeed = new Random();
        public int flightId;

        private string MavLinkidentification;

        // builders
        public Operation()
        {
            this.flightId = randomSeed.Next();
        }


        public Operation(DateTime start, DateTime end, drone air, RouteType route)
        {
            this.StartTime = start;
            this.FinalTime = end;
            this.aircraft = air;
            this.routeType = route;
            this.flightId = randomSeed.Next();

        }

        public Operation(DateTime start, DateTime end, drone air, RouteType route, VLOSRoute vlos,EVLOSRoute evlos ,BVLOSRoute bvlos, DeliveryRoute deliv, string mavlink)
        {
            this.StartTime = start;
            this.FinalTime = end;
            this.aircraft = air;
            this.routeType = route;
            this.vlosroute = vlos;
            this.evlosroute = evlos;
            this.bvlosroute = bvlos;
            this.deliveryroute = deliv;
            this.MavLinkidentification = mavlink;
            this.flightId = randomSeed.Next();
        }

        // setters and getters

        public void SetStartTime(DateTime a)
        {
            this.StartTime = a;
        }

        public void SetFinalTime(DateTime a)
        {
            this.FinalTime = a;
        }

        public void SetAircraft(drone a)
        {
            this.aircraft = a;
        }

        public void SetRouteType(RouteType route)
        {
            this.routeType = route;
        }

        public void SetVLOSRoute(VLOSRoute a)
        {
            this.vlosroute = a;
        }

        public void SetEVLOSRoute(EVLOSRoute a)
        {
            this.evlosroute = a;
        }

        public void SetBVLOSRoute(BVLOSRoute a)
        {
            this.bvlosroute = a;
        }

        public void SetDeliveryRoutes(DeliveryRoute a)
        {
            this.deliveryroute = a;
        }

        public void SetMavLinkIdentification(string a)
        {
            this.MavLinkidentification = a;
        }

        public DateTime GetStartTime()
        {
            return this.StartTime;
        }

        public DateTime GetFinalTime()
        {
            return this.FinalTime;
        }

        public drone GetAircraft()
        {
            return this.aircraft;
        }

        public RouteType GetRouteType()
        {
            return this.routeType;
        }
        
        public VLOSRoute GetVLOSRoute()
        {
            return this.vlosroute;
        }

        public EVLOSRoute GetEVLOSRoute()
        {
            return this.evlosroute;
        }

        public BVLOSRoute GetBVLOSRoute()
        {
            return this.bvlosroute;
        }
        
        public DeliveryRoute GetDeliveryRoute()
        {
            return this.deliveryroute;
        }

        public string GetMavLinkIdentification()
        {
            return this.MavLinkidentification;
        }

        public Operation GetCopy()
        {
           
            DateTime starttime = this.StartTime;
            DateTime finaltime = this.FinalTime;
            drone air = this.aircraft.GetCopy();
            RouteType type = this.routeType;
            VLOSRoute vlos=null;
            EVLOSRoute evlos = null;
            BVLOSRoute bvlos=null;
            DeliveryRoute deliver=null;
            if(this.routeType == RouteType.VLOS)
            {
                 vlos = this.vlosroute.GetCopy();
                 evlos = null;
                 bvlos = null;
                 deliver = null;
            }

            if (this.routeType == RouteType.BVLOS)
            {
                bvlos = this.bvlosroute.GetCopy();
                vlos = null;
                evlos = null;
                deliver = null;
            }
            if (this.routeType == RouteType.EVLOS)
            {
                evlos = this.evlosroute.GetCopy();
                vlos = null;
                bvlos = null;
                deliver = null;
            }

            if (this.routeType == RouteType.Delivery)
            {
                bvlos = null;
                vlos = null;
                evlos = null;
                deliver = this.deliveryroute.GetCopy();
            }

            string mavlink = this.MavLinkidentification;
            Operation newoperation = new Operation(starttime, finaltime, air, type, vlos, evlos, bvlos, deliver, mavlink);
            return newoperation;


    }

        public int CompareTo(DateTime dep)
        {
            if (this.StartTime == dep)
                return 0;
            return this.StartTime.CompareTo(dep);
        }

        public List<Operation> AssignOperations(List<VLOSRoute> vlos, List<EVLOSRoute> evlos ,List<BVLOSRoute> bvlos, List<DeliveryRoute> delivery)
        {
            List<Operation> operations = new List<Operation>();
            int n = 0;
            foreach(VLOSRoute route in vlos)
            {
                Operation newoperation = new Operation();
                drone aircraft = route.GetAircraft();
                aircraft.SetIdentifier(Convert.ToString(23000000 + n));
                newoperation.SetAircraft(aircraft);
                newoperation.SetStartTime(route.GetDiscretizedRoute()[0].GetTime());
                newoperation.SetFinalTime(route.GetDiscretizedRoute()[route.GetDiscretizedRoute().Count - 1].GetTime());
                newoperation.SetVLOSRoute(route);
                newoperation.SetEVLOSRoute(null);
                newoperation.SetBVLOSRoute(null);
                newoperation.SetDeliveryRoutes(null);
                operations.Add(newoperation);
                n++;

            }
            foreach(EVLOSRoute route in evlos)
            {
                Operation newoperation = new Operation();
                drone aircraft = route.GetAircraft();
                aircraft.SetIdentifier(Convert.ToString(23000000 + n));
                newoperation.SetAircraft(aircraft);
                newoperation.SetStartTime(route.GetDiscretizedRoute()[0].GetTime());
                newoperation.SetFinalTime(route.GetDiscretizedRoute()[route.GetDiscretizedRoute().Count - 1].GetTime());
                newoperation.SetVLOSRoute(null);
                newoperation.SetEVLOSRoute(route);
                newoperation.SetBVLOSRoute(null);
                newoperation.SetDeliveryRoutes(null);
                operations.Add(newoperation);
                n++;  
            }

            foreach(BVLOSRoute route in bvlos)
            {
                Operation newoperation = new Operation();
                drone aircraft = route.GetAircraft();
                aircraft.SetIdentifier(Convert.ToString(23000000 + n));
                newoperation.SetAircraft(aircraft);
                newoperation.SetStartTime(route.GetDiscretizedRoute()[0].GetTime());
                newoperation.SetFinalTime(route.GetDiscretizedRoute()[route.GetDiscretizedRoute().Count - 1].GetTime());
                newoperation.SetVLOSRoute(null);
                newoperation.SetEVLOSRoute(null);
                newoperation.SetBVLOSRoute(route);
                newoperation.SetDeliveryRoutes(null);
                operations.Add(newoperation);
                n++;
            }

            foreach(DeliveryRoute route in delivery)
            {
                Operation newoperation = new Operation();
                drone aircraft = route.GetAircraft();
                aircraft.SetIdentifier(Convert.ToString(23000000 + n));
                newoperation.SetAircraft(aircraft);
                newoperation.SetStartTime(route.GetDiscretizedRoute()[0].GetTime());
                newoperation.SetFinalTime(route.GetDiscretizedRoute()[route.GetDiscretizedRoute().Count - 1].GetTime());
                newoperation.SetVLOSRoute(null);
                newoperation.SetEVLOSRoute(null);
                newoperation.SetBVLOSRoute(null);
                newoperation.SetDeliveryRoutes(route);
                operations.Add(newoperation);
                n++;


            }

            return operations;
          
        }

        

        public void WriteKMLOperations(string filename, List<Operation> operations)
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

            operations = operations.OrderBy(x => x.StartTime).ToList();
            List<Point> alldiscretizedpoints = new List<Point>();
            foreach (Operation operation in operations)
            {
                // set the required information in Point from Operation: flightID, color of line, and next Lat;lon;alt
                List<Point> discretizedpoints = operation.GetDiscretizedRoute();
                for (int i = 0; i <  discretizedpoints.Count - 1; i++)
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
                if (dp.GetOperation()!=-1)  // The last point of each flight has not been set with a FlightID or a nextLat/Lon/Alt 
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

        public void WriteCSVOperation(string filename, List<Point> route)
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
        public void WriteCSVOperations(string filename, List<Operation> operations)
        {
            StreamWriter writer = new StreamWriter(filename);

            writer.Write("Time\tFlight\tLon\tLat\tAlt\n"); // header
            
            //operations = operations.OrderBy(x => x.StartTime).ToList();
            List<Point> alldiscretizedpoints = new List<Point>();
            foreach (Operation operation in operations)
            {
                // set the required information in Point from Operation: flightID, color of line, and next Lat;lon;alt
                List<Point> discretizedpoints = operation.GetDiscretizedRoute();
                discretizedpoints.Last().SetOperation(discretizedpoints.First().GetOperation());
                WriteCSVOperation("ResultFlights/" + operation.flightId.ToString() + ".csv", discretizedpoints);
                alldiscretizedpoints.AddRange(discretizedpoints);
            }

            List<Point> allsorteddiscretizedpoints = alldiscretizedpoints.OrderBy(x => x.GetTime()).ToList();
            foreach (Point dp in allsorteddiscretizedpoints) {
                //Time  Flight  Lat Lon Alt
                string hour = dp.GetTime().Hour.ToString("HH:mm");
                string time = dp.GetTime().Year + "-" + dp.GetTime().Month.ToString("d2") + "-" + dp.GetTime().Day.ToString("d2") + "T" +
                    dp.GetTime().Hour.ToString("d2") + ":" + dp.GetTime().Minute.ToString("d2") + ":" + dp.GetTime().Second.ToString("d2") + "Z";
                writer.Write(time);

                writer.Write("\t"+ dp.GetOperation());

                writer.Write("\t" + dp.GetLongitude() + "\t" + dp.GetLatitude() + "\t" + dp.GetAltitude() + "\n");
            }
            writer.Close();

        }
        public void SetDiscreitzedRoute(List<Point> points)
        {
            if (routeType == RouteType.VLOS)
            {
                this.vlosroute.SetDiscretizedRoute(points);
            }
            if (routeType == RouteType.EVLOS)
            {
                this.evlosroute.SetDiscretizedRoute(points);
            }
            if (routeType == RouteType.BVLOS)
            {
                this.bvlosroute.SetDiscretizedRoute(points);
            }
            if (routeType == RouteType.Delivery)
            {
                this.deliveryroute.SetDiscretizedRoute(points);
            }
        }

        public List<Point> GetDiscretizedRoute()
        {
            List<Point> discretizedroute = new List<Point>();
            if(routeType==RouteType.VLOS)
            {
                discretizedroute= this.vlosroute.GetDiscretizedRoute();
            }
            if (routeType == RouteType.EVLOS)
            {
                discretizedroute = this.evlosroute.GetDiscretizedRoute();
            }
            if(routeType==RouteType.BVLOS)
            {
                discretizedroute = this.bvlosroute.GetDiscretizedRoute();
            }
            if(routeType==RouteType.Delivery)
            {
                discretizedroute = this.deliveryroute.GetDiscretizedRoute();
            }
            return discretizedroute;
        }

        public double GetMaximumDistanceWRTPoint(Point reference_point, Operation operation)
        {
            
            List<Point> discretized_route = operation.GetDiscretizedRoute();
            double maximum_distance = 0;
            foreach(Point punto in discretized_route)
            {
                double distance = LatLongProjection.HaversineDistance(reference_point, punto);
                if(distance>maximum_distance)
                {
                    maximum_distance = distance;
                }
            }
            return maximum_distance;

        }
    }
}
