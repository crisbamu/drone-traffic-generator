using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using utm_country;
using utm_utils;
using utm_drone;

namespace utm_routes.BVLOS_routes
{
    public class BVLOSRoute
    {
         // Written by Lluís Xavier Herranz on 12/19/2016

        // This class stores the points where a BVlos rpas will fly over

        // Attributes

        private List<BVLOSPoint> bvlospoints; // this list stores the points.
        private Point OriginPoint; // The point where the drone takes off
        private Point FirstIntermediatePoint;
        private Point SecondIntermediatePoint;
        private double altitude;
        private drone Aircraft; // the drone assigned to this route
        private List<Point> discretizedbvlosroute;

      
        static TimeGenerator timer = new TimeGenerator();
      
         // builders

        public BVLOSRoute()
        {

        }

        public BVLOSRoute(List<BVLOSPoint> list, Point origin, Point first, Point second, double alt, drone dron)
        {
            this.bvlospoints = list;
            this.OriginPoint = origin;
            this.FirstIntermediatePoint = first;
            this.SecondIntermediatePoint = second;
            this.altitude = alt;
            this.Aircraft = dron;
        }

        // setters and getters

        public void SetBVlosList(List<BVLOSPoint> list)
        {
            this.bvlospoints = list;
        }

        public void SetOriginPoint(Point origin)
        {
            this.OriginPoint = origin;
        }

        public void SetFirstIntermediate(Point first)
        {
            this.FirstIntermediatePoint = first;
        }

        public void SetSecondIntermediate(Point second)
        {
            this.SecondIntermediatePoint = second;
        }

        public void SetAltitude(double a)
        { 
            this.altitude = a; 
        }

        public void SetAircraft(drone a)
        {
            this.Aircraft = a;
        }

        public void SetDiscretizedRoute(List<Point> a)
        {
            this.discretizedbvlosroute = a;
        }

        public List<BVLOSPoint> GetBVlosList()
        {
            return this.bvlospoints;
        }

        public Point GetOriginPoint()
        {
            return this.OriginPoint;
        }

        public Point GetFirstIntermediate()
        {
            return this.FirstIntermediatePoint;
        }

        public Point GetSecondIntermediate()
        {
            return this.SecondIntermediatePoint;
        }

        public double GetAltitude()
        {
            return this.altitude;
        }

        public drone GetAircraft()
        {
            return this.Aircraft;
        }

        public List<Point> GetDiscretizedRoute()
        {
            return this.discretizedbvlosroute;
        }

        // functions
        private drone aircraft_tune(drone drone, Random rnd)
        {
            if (drone.GetMaxCruisingSpeed() <= 10)
            {
                double cruisingspeed = Math.Round((drone.GetMaxCruisingSpeed() / 1.2) * (1 + rnd.NextDouble()), 2);
                drone.SetCruisingSpeed(cruisingspeed);

                double scanspeed = Math.Round((drone.GetMaxCruisingSpeed() / 1.6) * (1 + rnd.NextDouble()), 2);
                drone.SetScanSpeed(scanspeed);

                double climbspeed = Math.Round((drone.GetMaxClimbingSpeed() / 2) * (1 + rnd.NextDouble()), 2);
                drone.SetClimbSpeed(climbspeed);

                double descendspeed = Math.Round((drone.GetMaxDescendingSpeed() / 2) * (1 + rnd.NextDouble()), 2);
                drone.SetDescendSpeed(descendspeed);
            }
            else
            {
                double cruisingspeed = Math.Round((drone.GetMaxCruisingSpeed() / 2) * (1 + rnd.NextDouble()), 2);
                drone.SetCruisingSpeed(cruisingspeed);

                double scanspeed = Math.Round((drone.GetMaxCruisingSpeed() / 2) * (1 + rnd.NextDouble()), 2);
                drone.SetScanSpeed(scanspeed);

                double climbspeed = Math.Round((drone.GetMaxClimbingSpeed() / 2) * (1 + rnd.NextDouble()), 2);
                drone.SetClimbSpeed(climbspeed);

                double descendspeed = Math.Round((drone.GetMaxDescendingSpeed() / 2) * (1 + rnd.NextDouble()), 2);
                drone.SetDescendSpeed(descendspeed);
            }
            return drone;
        }

        public BVLOSRoute CreateRoute(BVLOSParameters bvlosparameter, drone drone, Point point,int parts,Random rnd)
        {
         
            // This function creates a BVLOS route with the parameters inserted in BVLOSparameters.    
            TimeSpan time = new TimeSpan(00, 00, 00);
            DateTime randomday = DateTime.Today;

            int numberofscans = rnd.Next(bvlosparameter.GetMinNumGen(), bvlosparameter.GetMaxNumGen() + 1); // This number marks the number of scans to be performed
            double altitude = (bvlosparameter.GetMaxAltitude() - bvlosparameter.GetMinAltitude()) * rnd.NextDouble() + bvlosparameter.GetMinAltitude(); // the bvlos route altitude is created here.
            BVLOSRoute newbvlosroute = new BVLOSRoute();
            newbvlosroute.SetAltitude(altitude);
            point.SetTime(randomday + time);
            point.SetAltitude(altitude);
            newbvlosroute.SetOriginPoint(point); // the origin point is punto.

            newbvlosroute.SetAircraft(aircraft_tune(drone, rnd));

            List<BVLOSPoint> bvloslist = new List<BVLOSPoint>();

            BVLOSPoint firstbvlospoint = new BVLOSPoint();

            double angle = 2 * Math.PI * rnd.NextDouble();  // we create the angle towards the first point
            double distance = (bvlosparameter.GetFirstMaxDist() - bvlosparameter.GetFirsMinDist()) * rnd.NextDouble() + bvlosparameter.GetFirsMinDist(); // the distance from the origin point to the first point.

            // here we create the first intermediate point with a deviation angle of 30 º and in the half the distance to the first point with a deviation of +- 1 km
            double deviationangle = 15; // degrees
            deviationangle = deviationangle * Math.PI / 180;
            double intermediateangle = (angle - deviationangle + 2 * deviationangle * rnd.NextDouble());
            double deviationdistance = 1000; // meters
            double intermediatedistance = (distance / 2) - deviationdistance + 2 * deviationdistance * rnd.NextDouble();


            Point firstintermediatepoint = LatLongProjection.DestinyPoint(point, intermediatedistance, intermediateangle);
            firstintermediatepoint.SetAltitude(altitude);
            newbvlosroute.SetFirstIntermediate(firstintermediatepoint);
            //

            Point firstpoint = LatLongProjection.DestinyPoint(point, distance, angle);
            firstpoint.SetAltitude(altitude);
            firstbvlospoint.SetPoint(firstpoint);

            double d1 = (bvlosparameter.GetRectMaxDist() - bvlosparameter.GetRectMinDist()) * rnd.NextDouble() + bvlosparameter.GetRectMinDist();
            double d2 = (bvlosparameter.GetRectMaxDist() - bvlosparameter.GetRectMinDist()) * rnd.NextDouble() + bvlosparameter.GetRectMinDist();
            double rectangleangle = 2 * Math.PI * rnd.NextDouble();

            Rectangle firstrectangle = new Rectangle().CreateRectangle(firstpoint, d1, d2, rectangleangle);
            firstbvlospoint.SetRectangle(firstrectangle);

            bvloslist.Add(firstbvlospoint);

            // now we are going to create the subsecond points

            Point lastpoint = firstpoint;
            double lastangle = angle;


            for (int n = 1; n < numberofscans; n++)
            {
                BVLOSPoint newbvlospoint = new BVLOSPoint();

                double secondangle = lastangle - ((bvlosparameter.GetAngleRange() / 2) * (Math.PI) / 180) + ((bvlosparameter.GetAngleRange() * (Math.PI) / 180) * rnd.NextDouble());
                distance = (bvlosparameter.GetMaxDist() - bvlosparameter.GetMinDist()) * rnd.NextDouble() + bvlosparameter.GetMinDist();

                Point secondpoint = LatLongProjection.DestinyPoint(lastpoint, distance, secondangle);
                secondpoint.SetAltitude(altitude);
                newbvlospoint.SetPoint(secondpoint); // here we set the second point

                d1 = (bvlosparameter.GetRectMaxDist() - bvlosparameter.GetRectMinDist()) * rnd.NextDouble() + bvlosparameter.GetRectMinDist();
                d2 = (bvlosparameter.GetRectMaxDist() - bvlosparameter.GetRectMinDist()) * rnd.NextDouble() + bvlosparameter.GetRectMinDist();
                rectangleangle = 2 * Math.PI * rnd.NextDouble();

                Rectangle secondrectangle = new Rectangle().CreateRectangle(secondpoint, d1, d2, rectangleangle);
                newbvlospoint.SetRectangle(secondrectangle); // here we set the second rectangle

                bvloslist.Add(newbvlospoint);

                lastpoint = secondpoint;
                lastangle = secondangle;

            }

            newbvlosroute.SetBVlosList(bvloslist); // we recollect all the bvlos points in a list and store in the bvlos route object. 

            // here we create the second intermediate point at a deviation angle of 30º and a deviation distance of +-1km

            distance = LatLongProjection.HaversineDistance(point, lastpoint);

            angle = LatLongProjection.GetBearing(lastpoint, point);

            deviationangle = 5; // degrees
            deviationangle = deviationangle * Math.PI / 180;
            intermediateangle = (angle - deviationangle + 2 * deviationangle * rnd.NextDouble());
            deviationdistance = 1000; // meters
            intermediatedistance = (distance / 2) - deviationdistance + 2 * deviationdistance * rnd.NextDouble();

            Point secondintermediatepoint = LatLongProjection.DestinyPoint(lastpoint, intermediatedistance, intermediateangle);
            secondintermediatepoint.SetAltitude(altitude);
            newbvlosroute.SetSecondIntermediate(secondintermediatepoint);
            //

            DiscretizeBVLOSRoute(newbvlosroute, parts, rnd);

            return newbvlosroute;

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
                if (Math.Abs(final_height - actual_height) <= 0.5)
                {
                    break;
                }
            }
            return descension_path;

        }

        public Point AdvanceRouteDistance(Point origin_point, double distance, double bearing, double final_height,  drone aircraft)
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
                Point advanced_point = AdvanceRouteDistance(last_point, delta_distance, bearing, (last_point.GetAltitude() + delta_height), aircraft);
                route.Add(advanced_point);
                distance_advanced = distance_advanced + delta_distance;
                last_point = advanced_point.GetCopy();
                //if (Math.Abs(distance - distance_advanced) <= 5)
                if(LatLongProjection.HaversineDistance(last_point,final_point)<=5)
                {
                    double final_distance = LatLongProjection.HaversineDistance(last_point,final_point);
                    double final_bearing = LatLongProjection.GetBearing(last_point,final_point);
                    advanced_point = AdvanceRouteDistance(last_point, final_distance, final_bearing, final_point.GetAltitude(), aircraft);
                    route.Add(advanced_point);
                    break;
                }

            }
            return route;

        }

        public void DiscretizeBVLOSRoute(BVLOSRoute bvlosroute, int parts, Random rnd) // This function discretizes a vlos route in several parts and stores them in a list of the attribute of vlos.
        {
            if (parts < 1)
            {
                parts = 1;
            }
            List<Point> discretizedpoints = new List<Point>();

            // ascend
            List<Point> ascend_path = MakeAscension(bvlosroute.GetOriginPoint(), bvlosroute.GetAircraft(), 0, bvlosroute.GetAltitude(), parts);
            discretizedpoints.AddRange(ascend_path);
            // first straight 
           List<Point> first_intemediate_route_discretized = DiscretizeRoutePart(ascend_path[ascend_path.Count - 1], bvlosroute.GetFirstIntermediate(), parts, bvlosroute.GetAircraft());
            //List<Point> first_intemediate_route_discretized = DiscretizeRoutePart(bvlosroute.GetOriginPoint(), bvlosroute.GetFirstIntermediate(), parts, bvlosroute.GetAircraft());
            discretizedpoints.AddRange(first_intemediate_route_discretized);
            //discretization for each bvlos point
            Point lastpoint = first_intemediate_route_discretized[first_intemediate_route_discretized.Count - 1];
            foreach (BVLOSPoint scanpoint in bvlosroute.GetBVlosList())
            {
                Rectangle rect = scanpoint.GetRectangle();
                int turns = rnd.Next(7, 12);
                int best = new Rectangle().GetOptimumScan(rect, lastpoint);
                double scanwidth = new Rectangle().GetOptimumScanwidth(rect, best, turns);
                List<Point> scan = new Rectangle().MakeReducedOptimumScan(rect, scanwidth, best, bvlosroute.GetAltitude());
                scanpoint.GetRectangle().SetScan(scan);
                List<Point> intermediate_path = DiscretizeRoutePart(lastpoint, scan[0], parts, bvlosroute.GetAircraft());
                discretizedpoints.AddRange(intermediate_path);
                discretizedpoints.AddRange(scan);
                scan = rect.AddTimeToScan(scan, intermediate_path[intermediate_path.Count-1].GetTime(), bvlosroute.GetAircraft().GetScanSpeed());
                lastpoint = scan[scan.Count - 1].GetCopy();// now the actual point of the scan becomes the origin of another route
            }
            
            // from the last scan point to the second intermediate point
            List<Point> second_intermediate_route_path = DiscretizeRoutePart(lastpoint, bvlosroute.GetSecondIntermediate(), parts, bvlosroute.GetAircraft());
            discretizedpoints.AddRange(second_intermediate_route_path);
            // from the second intermediate point to the origin point
            List<Point> third_intermediate_route_path = DiscretizeRoutePart(second_intermediate_route_path[second_intermediate_route_path.Count-1 ], bvlosroute.GetOriginPoint(), parts, bvlosroute.GetAircraft());
            discretizedpoints.AddRange(third_intermediate_route_path);
            //descension path
            List<Point> descend_path = MakeDescension(third_intermediate_route_path[third_intermediate_route_path.Count - 1], bvlosroute.GetAircraft(), bvlosroute.GetAltitude(), 0, parts);
            discretizedpoints.AddRange(descend_path);
            bvlosroute.SetDiscretizedRoute(discretizedpoints);
        }

        public BVLOSRoute AddTime(BVLOSRoute bvlosroute, Random rnd, double meantime, double stdDev, TimeSpan lowerlimit, TimeSpan upperlimit) // this function adds the time to the bvlos route.
        {
            
            
            bool well_allocated = false;

            while (well_allocated == false)
            {
                BVLOSRoute copy_route = bvlosroute.GetCopy();
                TimeSpan randomtime = timer.RandomTimeGenerator(rnd, meantime, stdDev, lowerlimit, upperlimit);
                foreach (Point punto in copy_route.GetDiscretizedRoute())
                {
                    DateTime newtime = punto.GetTime() + randomtime;
                    punto.SetTime(newtime);
                }
                if (!(copy_route.GetDiscretizedRoute()[copy_route.GetDiscretizedRoute().Count - 1].GetTime() > (copy_route.GetDiscretizedRoute()[copy_route.GetDiscretizedRoute().Count - 1].GetTime().Date + upperlimit) || copy_route.GetDiscretizedRoute()[0].GetTime() < (copy_route.GetDiscretizedRoute()[0].GetTime().Date + lowerlimit)))
                {
                    well_allocated = true;
                    bvlosroute = copy_route.GetCopy();
                }
            }

            return bvlosroute;
        }

        public BVLOSRoute GetCopy() // this function returns a copy of this bvlos
        {
            List<BVLOSPoint> newlist = new List<BVLOSPoint>();
            foreach(BVLOSPoint punto in this.bvlospoints)
            {
                BVLOSPoint newpoint = new BVLOSPoint();
                newpoint = punto.GetCopy();
                newlist.Add(newpoint);
            }
            BVLOSRoute copy = new BVLOSRoute(newlist, this.OriginPoint.GetCopy(), this.FirstIntermediatePoint.GetCopy(), this.SecondIntermediatePoint.GetCopy(), this.altitude, this.Aircraft.GetCopy());

            List<Point> list = new List<Point>();
            foreach(Point punto in this.discretizedbvlosroute)
            {
                Point newpoint = new Point();
                newpoint = punto.GetCopy();
                list.Add(newpoint);
            }
            copy.SetDiscretizedRoute(list);

            return copy;

        }
    }
}
