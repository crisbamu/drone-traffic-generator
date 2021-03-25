using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using utm_utils;
using utm_routes;
using utm_routes.VLOS_routes;
using utm_routes.BVLOS_routes;
using utm_routes.DeliveryRoutes;
using utm_routes.EVLOSRoutes;
using utm_drone;
using System.IO;

namespace utm_operation
{
    public class MavLinkWriter
    {
        // Written by Lluís Xavier Herranz on 02/22/2017

        // This class writes files in MavLink Protocol

        // function

        static string takeoffline = "{0}\t0\t3\t22\t0.000000\t0.000000\t0.000000\t0.000000\t{1}\t{2}\t{3}\t1";
        static string waypoint = "{0}\t0\t3\t16\t0.000000\t0.000000\t0.000000\t0.000000\t{1}\t{2}\t{3}\t1";
        static string changespeed = "{0}\t0\t3\t178\t1.000000\t{1}\t0.000000\t0.000000\t0.000000\t0.000000\t0.000000\t1";
        static string landline = "{0}\t0\t3\t21\t0.000000\t0.000000\t0.000000\t0.000000\t{1}\t{2}\t{3}\t1";

        public void DeleteElementsinFile(string foldername)
        {
           // String directory = AppDomain.CurrentDomain.BaseDirectory;
            System.IO.DirectoryInfo di = new DirectoryInfo(foldername);

            if (di.Exists)
            {

                foreach (FileInfo file2 in di.GetFiles())
                {
                    file2.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
        }

        public void WriteMavLinkFile(string filename, String foldername, Operation op)
        {

            //String directory = AppDomain.CurrentDomain.BaseDirectory;
            System.IO.DirectoryInfo di = new DirectoryInfo(foldername);
           
            if (!di.Exists)  // if it doesn't exist, create
                di.Create();

            // StreamWriter writer = new StreamWriter(di + filename);
            StreamWriter writer = new StreamWriter(di+"\\"+filename);
            writer.WriteLine("QGC WPL 110");
            
            if(op.GetRouteType()==RouteType.VLOS)
            {
                VLOSRoute route = op.GetVLOSRoute(); // the take off point
                WriteVLOSMavlink(writer, route);
            }

            if (op.GetRouteType() == RouteType.EVLOS)
            {
                EVLOSRoute route = op.GetEVLOSRoute(); // the take off point
                WriteEVLOSMavlink(writer, route);
            }

            if (op.GetRouteType()==RouteType.BVLOS)
            {
                BVLOSRoute route = op.GetBVLOSRoute();
                WriteBVLOSMavLink(writer, route);
               
            }
            if (op.GetRouteType()==RouteType.Delivery)
            {
                DeliveryRoute route = op.GetDeliveryRoute();
                WriteDeliveryMavlink(writer, route);
                
            }

            writer.Close();

        
        }

        public void WriteVLOSMavlink(StreamWriter writer, VLOSRoute route)
        {

            int n = 0;
            Point takeoffpoint = route.GetOriginPoint();
            writer.WriteLine(waypoint, n, takeoffpoint.GetLatitude(), takeoffpoint.GetLongitude(), route.GetAltitude());
            n++;
            writer.WriteLine(takeoffline, n, takeoffpoint.GetLatitude(), takeoffpoint.GetLongitude(), route.GetAltitude());
            n++;
            writer.WriteLine(changespeed, n, route.GetAircraft().GetCruisingSpeed());
            n++;
            Point punto = route.GetFirstPoint();  // write the first point
            writer.WriteLine(waypoint, n, punto.GetLatitude(), punto.GetLongitude(), route.GetAltitude());
            n++;
            for (int i = 0; i < route.GetRectangle().GetScan().Count; i++)
            {
                writer.WriteLine(waypoint, n, route.GetRectangle().GetScan()[i].GetLatitude(), route.GetRectangle().GetScan()[i].GetLongitude(), route.GetAltitude());
                n++;
                if (i == 0)
                {
                    writer.WriteLine(changespeed, n, route.GetAircraft().GetScanSpeed());
                    n++;
                }
            }
            writer.WriteLine(changespeed, n, route.GetAircraft().GetCruisingSpeed());
            n++;
            Point land = route.GetOriginPoint(); // the land point
            writer.WriteLine(landline, n, land.GetLatitude(), land.GetLongitude(), route.GetAltitude());
        }

        public void WriteEVLOSMavlink(StreamWriter writer, EVLOSRoute route)
        {

            int n = 0;
            Point takeoffpoint = route.GetOriginPoint();
            writer.WriteLine(waypoint, n, takeoffpoint.GetLatitude(), takeoffpoint.GetLongitude(), route.GetAltitude());
            n++;
            writer.WriteLine(takeoffline, n, takeoffpoint.GetLatitude(), takeoffpoint.GetLongitude(), route.GetAltitude());
            n++;
            writer.WriteLine(changespeed, n, route.GetAircraft().GetCruisingSpeed());
            n++;
            Point punto = route.GetFirstPoint();  // write the first point
            writer.WriteLine(waypoint, n, punto.GetLatitude(), punto.GetLongitude(), route.GetAltitude());
            n++;
            for (int i = 0; i < route.GetRectangle().GetScan().Count; i++)
            {
                writer.WriteLine(waypoint, n, route.GetRectangle().GetScan()[i].GetLatitude(), route.GetRectangle().GetScan()[i].GetLongitude(), route.GetAltitude());
                n++;
                if (i == 0)
                {
                    writer.WriteLine(changespeed, n, route.GetAircraft().GetScanSpeed());
                    n++;
                }
            }
            writer.WriteLine(changespeed, n, route.GetAircraft().GetCruisingSpeed());
            n++;
            Point land = route.GetOriginPoint(); // the land point
            writer.WriteLine(landline, n, land.GetLatitude(), land.GetLongitude(), route.GetAltitude());
        }

        public void WriteBVLOSMavLink(StreamWriter writer, BVLOSRoute route)
        {
            
            int n = 0;
            Point takeoffpoint = route.GetOriginPoint();
            writer.WriteLine(waypoint, n, takeoffpoint.GetLatitude(), takeoffpoint.GetLongitude(), route.GetAltitude());
            n++;
            writer.WriteLine(takeoffline, n, takeoffpoint.GetLatitude(), takeoffpoint.GetLongitude(), route.GetAltitude());
            n++;
            writer.WriteLine(changespeed, n, route.GetAircraft().GetCruisingSpeed());
            n++;
            Point first = route.GetFirstIntermediate();
            writer.WriteLine(waypoint, n, first.GetLatitude(), first.GetLongitude(), route.GetAltitude());
            n++;
            foreach (BVLOSPoint punto in route.GetBVlosList())
            {
                for (int i = 0; i < punto.GetRectangle().GetScan().Count; i++)
                {
                    writer.WriteLine(waypoint, n, punto.GetRectangle().GetScan()[i].GetLatitude(), punto.GetRectangle().GetScan()[i].GetLongitude(), route.GetAltitude());
                    n++;
                    if (i == 0)
                    {
                        writer.WriteLine(changespeed, n, route.GetAircraft().GetCruisingSpeed());
                        n++;
                    }
                }
                writer.WriteLine(changespeed, n, route.GetAircraft().GetCruisingSpeed());
                n++;
            }

            Point second = route.GetSecondIntermediate();
            writer.WriteLine(waypoint, n, second.GetLatitude(), second.GetLongitude(), route.GetAltitude());
            n++;
            Point land = route.GetOriginPoint();
            writer.WriteLine(landline, n, land.GetLatitude(), land.GetLongitude(), route.GetAltitude());
        }

        public void WriteDeliveryMavlink(StreamWriter writer, DeliveryRoute route)
        {
            
            int n = 0;
            Point takeoffpoint = route.GetOriginPoint();
            writer.WriteLine(waypoint, n, takeoffpoint.GetLatitude(), takeoffpoint.GetLongitude(), route.GetAltitude());
            n++;
            writer.WriteLine(takeoffline, n, takeoffpoint.GetLatitude(), takeoffpoint.GetLongitude(), route.GetAltitude());
            n++;
            writer.WriteLine(changespeed, n, route.GetAircraft().GetCruisingSpeed());
            n++;
            Stack<Point> inverseroute = new Stack<Point>();
            foreach (Point punto in route.GetRoute())
            {
                writer.WriteLine(waypoint, n, punto.GetLatitude(), punto.GetLongitude(), route.GetAltitude());
                n++;
                inverseroute.Push(punto);
            }
            Point repeated = inverseroute.Pop(); // the last point is repeated.

            while (inverseroute.Count > 0)
            {
                Point punto = inverseroute.Pop();
                writer.WriteLine(waypoint, n, punto.GetLatitude(), punto.GetLongitude(), route.GetAltitude());
                n++;
            }

            Point land = route.GetOriginPoint();
            writer.WriteLine(landline, n, land.GetLatitude(), land.GetLongitude(), route.GetAltitude());

        }


    }
}
