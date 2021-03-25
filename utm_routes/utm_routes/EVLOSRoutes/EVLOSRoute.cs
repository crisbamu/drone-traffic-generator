using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using utm_country;
using utm_utils;
using utm_drone;

namespace utm_routes.EVLOSRoutes
{
   public class EVLOSRoute
    {
        // Written by Lluís Xavier Herranz on 05/22/2017

        // This class aims at storing and creating EVLOS routes

        //Attributes
        private Point OriginPoint;  // These are the point where the drone will fly over 
        private Point FirstPoint;
        private Point SecondPoint;
        private Rectangle rectangle; // This the rectangle where te drone will do a scan
        private double altitude; // the height above the ground
        private drone aircraft; // the drone which flies the route
        private List<Point> discretizedvlosroute; // the vlos route discretized in several points

   

        static TimeGenerator timer = new TimeGenerator();
        public EVLOSRoute()
        {

        }

        public EVLOSRoute(Point origin, Point first, Point second, Rectangle rec, double alt, drone air)
        {
            this.OriginPoint = origin;
            this.FirstPoint = first;
            this.SecondPoint = second;
            this.rectangle = rec;
            this.altitude = alt;
            this.aircraft = air;
        }

        // setters and getters

        public void SetOrigin(Point a)
        { this.OriginPoint = a; }

        public void SetFirst(Point a)
        { this.FirstPoint = a; }

        public void SetSecond(Point a)
        { this.SecondPoint = a; }

        public void SetRectangle(Rectangle a)
        { this.rectangle = a; }

        public void SetAltitude(double a)
        { this.altitude = a; }

        public void SetAircraft(drone a)
        { this.aircraft = a; }

        public void SetDiscretizedRoute(List<Point> a)
        { this.discretizedvlosroute = a; }


        public Point GetOriginPoint()
        { return this.OriginPoint; }

        public Point GetFirstPoint()
        { return this.FirstPoint; }

        public Point GetSecondPoint()
        { return this.SecondPoint; }

        public Rectangle GetRectangle()
        { return this.rectangle; }

        public double GetAltitude()
        { return this.altitude; }

        public drone GetAircraft()
        { return this.aircraft; }

        public List<Point> GetDiscretizedRoute()
        { return this.discretizedvlosroute; }


        // functions

        public EVLOSRoute CreateRoute(EVLOSParameters parameters, drone drone, Point originpoint, int parts, Random rnd) // This function creates a VLOS route for the entered point
        {
            EVLOSRoute vlosflight = new EVLOSRoute();
            DateTime randomday = DateTime.Today;
            TimeSpan time = new TimeSpan(0, 0, 0);
            originpoint.SetTime(randomday + time);


            double altitude = (parameters.GetMaxAltitude() - parameters.GetMinAltitude()) * rnd.NextDouble() + parameters.GetMinAltitude(); // here a random altitude between the intervals is created.
            originpoint.SetAltitude(altitude);

            double cruisingspeed = Math.Round((drone.GetMaxCruisingSpeed() - drone.GetMinCruisingSpeed()) * rnd.NextDouble() + drone.GetMinCruisingSpeed(), 2);
            drone.SetCruisingSpeed(cruisingspeed);

            double scanspeed = Math.Round((5 - 1) * rnd.NextDouble() + 1, 2);
            drone.SetScanSpeed(scanspeed);

            double climbspeed = Math.Round((drone.GetMaxClimbingSpeed() / 2) * (1 + rnd.NextDouble()), 2);
            drone.SetClimbSpeed(climbspeed);

            double descendspeed = Math.Round((drone.GetMaxDescendingSpeed() / 2) * (1 + rnd.NextDouble()), 2);
            drone.SetDescendSpeed(descendspeed);
            // here we create a new point at a random distance and a random angle. The distance interval goes from firstmindistance m to firstmaxdistance m. The angle goes from 0 to 2 pi radians

            double longest = parameters.GetFirstMaxDist();
            double shortest = parameters.GetFirsMinDist();
            double distance = (longest - shortest) * rnd.NextDouble() + shortest;
            double angle = 2 * Math.PI * rnd.NextDouble();

            Point firstpoint = LatLongProjection.DestinyPoint(originpoint, distance, angle);
            firstpoint.SetAltitude(altitude);


            // Now we are going to create the second point at a shorter distance and angle between the range of the angle selected from the original angle

            double newangle = angle - ((parameters.GetAngleRange() / 2) * (Math.PI) / 180) + (parameters.GetAngleRange() * (Math.PI) / 180) * rnd.NextDouble();

            // The second distance will be from secondmindistance  to secondmaxdistance 
            longest = parameters.GetSecondMaxDist();
            shortest = parameters.GetSecondMinDist();

            double seconddistance = (longest - shortest) * rnd.NextDouble() + shortest;

            Point secondpoint = LatLongProjection.DestinyPoint(firstpoint, seconddistance, newangle);
            secondpoint.SetAltitude(altitude);

            // Now, we have to generate the rectangle by defining the d1 and the d2 distance.
            double rectangle = rnd.NextDouble() * 2 * Math.PI;

            //// the d1 and d2 go from rectanglemaxdistance m to rectanglemindistance m 

            double d1 = parameters.GetRectMinDist() + ((parameters.GetRectMaxDist() - parameters.GetRectMinDist()) * rnd.NextDouble());
            double d2 = parameters.GetRectMinDist() + ((parameters.GetRectMaxDist() - parameters.GetRectMinDist()) * rnd.NextDouble());

            Rectangle newrectangle = new Rectangle().CreateRectangle(secondpoint, d1, d2, rectangle);

            vlosflight = new EVLOSRoute(originpoint, firstpoint, secondpoint, newrectangle, altitude, drone); // here we create the whole vlos route.

            DiscretizeEVLOSRoute(vlosflight, parts, rnd); // the new vlosflight is discretized and the points are stored.

            return vlosflight;

        }

        public List<Point> MakeAscension(Point launch_point, drone aircraft, double initial_height, double final_height, int parts) // discretizes the ascension path  
        {
            List<Point> ascension_path = new List<Point>();
            DateTime initial_time = launch_point.GetTime();
            DateTime actual_time = initial_time;
            if (parts < 1)
            {
                parts = 1;
            }
            double height_differencial = (final_height - initial_height) / parts;
            double delta_time = Math.Abs(height_differencial) / aircraft.GetClimbSpeed();
            double latitude = launch_point.GetLatitude();
            double longitude = launch_point.GetLongitude();
            double actual_height = initial_height;
            //create the first point at initial_height
            Point new_point = new Point(latitude, longitude, initial_height, initial_time);
            ascension_path.Add(new_point);
            while (actual_height < final_height)
            {
                actual_height = actual_height + height_differencial;
                TimeSpan time = TimeSpan.FromSeconds(delta_time);
                actual_time = actual_time + time;
                new_point = new Point(latitude, longitude, actual_height, actual_time);
                ascension_path.Add(new_point);
                if (Math.Abs(final_height - actual_height) <= 0.5)
                {
                    break;
                }
            }
            return ascension_path;

        }

        public List<Point> MakeDescension(Point land_point, drone aircraft, double initial_height, double final_height, int parts) // discretizes the ascension path  
        {
            List<Point> descension_path = new List<Point>();
            DateTime initial_time = land_point.GetTime();
            DateTime actual_time = initial_time;
            if (parts < 1)
            {
                parts = 1;
            }
            double height_differencial = (final_height - initial_height) / parts;
            double delta_time = Math.Abs(height_differencial) / aircraft.GetDescendSpeed();
            double latitude = land_point.GetLatitude();
            double longitude = land_point.GetLongitude();
            double actual_height = initial_height;
            //create the first point at initial_height
            Point new_point = new Point(latitude, longitude, initial_height, initial_time);
            descension_path.Add(new_point);
            while (actual_height > final_height)
            {
                actual_height = actual_height + height_differencial;
                TimeSpan time = TimeSpan.FromSeconds(delta_time);
                actual_time = actual_time + time;
                new_point = new Point(latitude, longitude, actual_height, actual_time);
                descension_path.Add(new_point);
                if (Math.Abs(final_height - actual_height) <= 2)
                {
                    break;
                }
            }
            return descension_path;

        }

        public Point AdvanceRouteDistance(Point origin_point, double distance, double bearing, double final_height, drone aircraft)
        {
            Point advanced_point = LatLongProjection.DestinyPoint(origin_point, distance, bearing);
            advanced_point.SetAltitude(final_height);
            double delta_height = final_height - origin_point.GetAltitude();
            double total_distance = Math.Sqrt(Math.Pow(distance, 2) + Math.Pow(delta_height, 2));
            DateTime actual_time = origin_point.GetTime();
            double delta_time = total_distance / aircraft.GetCruisingSpeed();
            advanced_point.SetTime(actual_time + TimeSpan.FromSeconds(delta_time));
            return advanced_point;
        }

        public Point AdvanceScanDistance(Point origin_point, double distance, double bearing, double final_height, drone aircraft)
        {
            Point advanced_point = LatLongProjection.DestinyPoint(origin_point, distance, bearing);
            advanced_point.SetAltitude(final_height);
            double delta_height = final_height - origin_point.GetAltitude();
            double total_distance = Math.Sqrt(Math.Pow(distance, 2) + Math.Pow(delta_height, 2));
            DateTime actual_time = origin_point.GetTime();
            double delta_time = total_distance / aircraft.GetScanSpeed();
            advanced_point.SetTime(actual_time + TimeSpan.FromSeconds(delta_time));
            return advanced_point;
        }

        public List<Point> DiscretizeRoutePart(Point origin_point, Point final_point, int parts, drone aircraft)
        {
            List<Point> route = new List<Point>();
            double distance = LatLongProjection.HaversineDistance(origin_point, final_point);
            double bearing = LatLongProjection.GetBearing(origin_point, final_point);
            double delta_height = (final_point.GetAltitude() - origin_point.GetAltitude()) / parts;
            double delta_distance = distance / parts;
            double distance_advanced = 0;
            Point last_point = origin_point.GetCopy();
            while (distance_advanced < distance)
            {
                Point advanced_point = AdvanceRouteDistance(last_point, delta_distance, bearing, (last_point.GetAltitude() + delta_height),  aircraft);
                route.Add(advanced_point);
                distance_advanced = distance_advanced + delta_distance;
                last_point = advanced_point.GetCopy();
                //if (Math.Abs(distance - distance_advanced) <= 0.5)
                //{
                //    break;
                //}
                if (LatLongProjection.HaversineDistance(last_point, final_point) <= 1)
                {
                    double final_distance = LatLongProjection.HaversineDistance(last_point, final_point);
                    double final_bearing = LatLongProjection.GetBearing(last_point, final_point);
                    advanced_point = AdvanceRouteDistance(last_point, final_distance, final_bearing, final_point.GetAltitude(), aircraft);
                    route.Add(advanced_point);
                    break;
                }

            }
            return route;

        }

        public void DiscretizeEVLOSRoute(EVLOSRoute evlosroute, int parts, Random rnd) // This function discretizes a vlos route in several parts and stores them in a list of the attribute of vlos.
        {
            if (parts < 1)
            {
                parts = 1;
            }
            List<Point> discretizedpoints = new List<Point>();

            // ascend
            List<Point> ascend_path = MakeAscension(evlosroute.GetOriginPoint(), evlosroute.GetAircraft(), 0, evlosroute.GetAltitude(), parts);
            discretizedpoints.AddRange(ascend_path);
            // first straight 
            List<Point> first_route_discretized = DiscretizeRoutePart(ascend_path[ascend_path.Count-1], evlosroute.GetFirstPoint(), parts, evlosroute.GetAircraft());
           // List<Point> first_route_discretized = DiscretizeRoutePart(evlosroute.GetOriginPoint(), evlosroute.GetFirstPoint(), parts, evlosroute.GetAircraft());
            discretizedpoints.AddRange(first_route_discretized);
            //discretization of the rectangle

            Rectangle rect = evlosroute.GetRectangle();

            int best = new Rectangle().GetOptimumScan(rect, first_route_discretized[first_route_discretized.Count - 1]);
            int turns = rnd.Next(7, 12);
            double scanwidth = new Rectangle().GetOptimumScanwidth(rect, best, turns);

            List<Point> scan = rect.MakeReducedOptimumScan(rect, scanwidth, best, evlosroute.GetAltitude());

            // second straight
            List<Point> second_route_discretized = DiscretizeRoutePart(first_route_discretized[first_route_discretized.Count - 1], scan[0], parts, evlosroute.GetAircraft());
            scan = rect.AddTimeToScan(scan, second_route_discretized[second_route_discretized.Count - 1].GetTime(), evlosroute.GetAircraft().GetScanSpeed());
            evlosroute.GetRectangle().SetScan(scan); // set the scan points to the vlosroute
            discretizedpoints.AddRange(second_route_discretized);
            discretizedpoints.AddRange(scan);
            // third straight
            List<Point> third_route_discretized = DiscretizeRoutePart(scan[scan.Count - 1], evlosroute.GetOriginPoint(), parts, evlosroute.GetAircraft());
            discretizedpoints.AddRange(third_route_discretized);
            //land path
            List<Point> descend_path = MakeDescension(third_route_discretized[third_route_discretized.Count - 1], evlosroute.GetAircraft(), evlosroute.GetAltitude(), 0, parts);
            discretizedpoints.AddRange(descend_path);
            evlosroute.SetDiscretizedRoute(discretizedpoints);
        }

        public EVLOSRoute AddTime(EVLOSRoute evlosroute, Random rnd, double meantime, double stdDev, TimeSpan lowerlimit, TimeSpan upperlimit) // this function adds the time to a vlos route.
        {
            
            
            bool well_allocated = false;
            while (well_allocated == false)
            {
                TimeSpan randomtime = timer.RandomTimeGenerator(rnd, meantime, stdDev, lowerlimit, upperlimit);
                EVLOSRoute copy_route = evlosroute.GetCopy();
                foreach (Point punto in copy_route.GetDiscretizedRoute())
                {

                    DateTime newtime = punto.GetTime() + randomtime;
                    punto.SetTime(newtime);
                }
                if (!(copy_route.GetDiscretizedRoute()[copy_route.GetDiscretizedRoute().Count - 1].GetTime() > (copy_route.GetDiscretizedRoute()[copy_route.GetDiscretizedRoute().Count - 1].GetTime().Date + upperlimit) || copy_route.GetDiscretizedRoute()[0].GetTime() < (copy_route.GetDiscretizedRoute()[0].GetTime().Date + lowerlimit)))
                {
                    well_allocated = true;
                    evlosroute = copy_route.GetCopy();
                }
            }

            return evlosroute;
        }

        public EVLOSRoute GetCopy() // this function creates a copy of the route
        {


            Point origin = this.GetOriginPoint().GetCopy();
            Point first = this.GetFirstPoint().GetCopy();
            Point second = this.GetSecondPoint().GetCopy();
            Rectangle rect = this.GetRectangle().GetCopy();
            double alt = this.GetAltitude();
            drone air = this.GetAircraft().GetCopy();

            List<Point> newlist = new List<Point>();
            foreach (Point punto in this.discretizedvlosroute)
            {
                Point newpoint = punto.GetCopy();
                newlist.Add(newpoint);
            }
            //List<Point> dis = this.GetDiscretizedRoute();
            EVLOSRoute copy = new EVLOSRoute(origin, first, second, rect, altitude, air);

            copy.SetDiscretizedRoute(newlist);

            return copy;
        }

       
    }
}
