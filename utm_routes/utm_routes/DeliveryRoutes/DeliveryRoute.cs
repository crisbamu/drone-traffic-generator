using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using utm_country;
using utm_utils;
using utm_drone;

namespace utm_routes.DeliveryRoutes
{
    public class DeliveryRoute
    {
        // Written by Lluís Xavier Herranz on 01/29/2016

        // This class is aimed to store the delivery routes parameters

        //Attributes

        private Point OriginPoint;
        private Point Destination;
        private double altitude;
        private drone aircraft;

        private List<Point> Route;
        private List<Point> discretizedroute;


        static TimeGenerator timer = new TimeGenerator();
        // builder

        public DeliveryRoute()
        {

        }

        public DeliveryRoute(Point origin, List<Point> route, double alt, drone air)
        {
            this.OriginPoint = origin;
            this.Destination = route.Last();
            this.Route = route;
            this.altitude = alt;
            this.aircraft = air;
        }

        // setters and getters

        public void SetOriginPoint(Point a)
        {
            this.OriginPoint = a;
        }
        public void SetDestination(Point a)
        {
            this.Destination = a;
        }

        public void SetRoute(List<Point> a)
        {
            this.Route = a;
        }

        public void SetAltitude(double a)
        {
            this.altitude = a;
        }

        public void SetAircraft(drone a)
        {
            this.aircraft = a;
        }

        public void SetDiscretizedRoute(List<Point> a)
        {
            this.discretizedroute = a;
            this.Destination = a.Last();

        }

        public Point GetOriginPoint()
        {
            return this.OriginPoint;
        }
        public Point GetDestination()
        {
            return this.Destination;
        }

        public List<Point> GetRoute()
        {
            return this.Route;
        }

        public double GetAltitude()
        {
            return this.altitude;
        }

        public drone GetAircraft()
        {
            return this.aircraft;
        }

        public List<Point> GetDiscretizedRoute()
        {
            return this.discretizedroute;
        }

        // function

        public DeliveryRoute CreateRoute(DeliveryParameters deliveryparameters,drone aircraft,  Point origin, int numberofpoints, Random rnd)
        {
            DeliveryRoute newdeliveryroute = new DeliveryRoute();
            
            List<Point> deliverypoints = new List<Point>();

            double cruisingspeed = Math.Round((aircraft.GetMaxCruisingSpeed() - aircraft.GetMinCruisingSpeed()) * rnd.NextDouble() + aircraft.GetMinCruisingSpeed(), 2);
            aircraft.SetCruisingSpeed(cruisingspeed);

            double truealtitude = origin.GetAltitude()+(deliveryparameters.GetMaxAltitude() - deliveryparameters.GetMinAltitude()) * rnd.NextDouble() + deliveryparameters.GetMinAltitude();
            newdeliveryroute.SetAltitude(truealtitude);
            newdeliveryroute.SetAircraft(aircraft);

            double truedistance = deliveryparameters.GetMaxDistance() * rnd.NextDouble();

            newdeliveryroute.SetOriginPoint(origin); // we set the origin point as the introduced one.

            double anglerange = 30; // in degrees
            anglerange = anglerange * Math.PI / 180; // in radians
            double deltadistance = truedistance / numberofpoints;
            double angle = 2 * Math.PI * rnd.NextDouble();

            Point lastpoint = origin;
            double lastangle = angle;
            for (int i = 0; i < numberofpoints; i++)
            {
                double trueangle = lastangle - anglerange / 2 + anglerange * rnd.NextDouble();
                Point nextpoint = LatLongProjection.DestinyPoint(lastpoint, deltadistance, trueangle);
                deliverypoints.Add(nextpoint);
                lastpoint = nextpoint;
                lastangle = trueangle;
            }
            newdeliveryroute.SetRoute(deliverypoints);
            newdeliveryroute.SetDestination(lastpoint); // we set the dest too
            DiscretizedDeliveryRoute(newdeliveryroute);
           
            return newdeliveryroute;
        }
        public List<Point> MakeAscension(Point launch_point, drone aircraft, double initial_height, double final_height) // discretizes the ascension path  
        {
            List<Point> ascension_path = new List<Point>();
            DateTime initial_time = launch_point.GetTime();
            DateTime actual_time = initial_time;
            TimeSpan segment_time = TimeSpan.FromSeconds((final_height - initial_height) / aircraft.GetClimbSpeed());
            double height_differencial = Math.Abs(final_height - initial_height) / segment_time.TotalSeconds * 5;
            double delta_time = 5.0;// Math.Abs(height_differencial) / aircraft.GetClimbSpeed();
            double latitude = launch_point.GetLatitude();
            double longitude = launch_point.GetLongitude();
            double actual_height = initial_height;
            TimeSpan time;
            //create the first point at initial_height
            Point new_point = new Point(latitude, longitude, initial_height, initial_time);
            ascension_path.Add(new_point);
            while (actual_height + height_differencial < final_height)
            {
                actual_height += height_differencial;
                time = TimeSpan.FromSeconds(delta_time);
                actual_time = actual_time + time;
                new_point = new Point(latitude, longitude, actual_height, actual_time);
                ascension_path.Add(new_point);
            }
            //calculate remaining time to get final_height and add this
            time = TimeSpan.FromSeconds((final_height- actual_height)/ aircraft.GetClimbSpeed());
            actual_time = actual_time + time; //Not 5 sec!!
            new_point = new Point(latitude, longitude, final_height, actual_time);
            ascension_path.Add(new_point);
            return ascension_path;

        }

        public List<Point> MakeDescension(Point land_point, drone aircraft, double initial_height, double final_height) // discretizes the ascension path  
        {
            List<Point> descension_path = new List<Point>();
            DateTime initial_time = land_point.GetTime();
            DateTime actual_time = initial_time;
            double height = Math.Abs(final_height - initial_height);
            TimeSpan segment_time = TimeSpan.FromSeconds(height / aircraft.GetDescendSpeed());
            double height_differencial = height / segment_time.TotalSeconds * 5;
            double delta_time = 5.0; // Math.Abs(height_differencial) / aircraft.GetDescendSpeed();
            double latitude = land_point.GetLatitude();
            double longitude = land_point.GetLongitude();
            double actual_height = initial_height;
            TimeSpan time;
            Point new_point;
            while (actual_height - height_differencial > final_height)
            {
                actual_height -= height_differencial;
                time = TimeSpan.FromSeconds(delta_time);
                actual_time = actual_time + time;
                new_point = new Point(latitude, longitude, actual_height, actual_time);
                descension_path.Add(new_point);
                if (Math.Abs(final_height - actual_height) <= 0.5)
                {
                    new_point.SetAltitude(final_height);
                    break;
                }
            }
            //calculate remaining time to get final_height and add this
            time = TimeSpan.FromSeconds((actual_height-final_height) / aircraft.GetDescendSpeed());
            actual_time = actual_time + time; //Not 5 sec!!
            new_point = new Point(latitude, longitude, final_height, actual_time);
            descension_path.Add(new_point);
            return descension_path;

        }
        private drone aircraft_tune(drone drone)
        {
            Random rnd = new Random();
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

        // Creates the delivery route discretized trace with a point each 5secs!!
        public void SetRadarRoute(DeliveryRoute deliveryroute, List<Point> forwardroute, List<Point> backroute)
        {
            List<Point> discretizedroute = new List<Point>();
            TimeSpan daytime = new TimeSpan(00, 00, 00);
            DateTime day = DateTime.Today;

            // ascend
            Point orig = deliveryroute.GetOriginPoint();
            orig.SetTime(day + daytime);
            List<Point> ascend_path = MakeAscension(orig, deliveryroute.GetAircraft(), orig.GetAltitude(), deliveryroute.GetAltitude());
            discretizedroute.AddRange(ascend_path);
            daytime = discretizedroute.Last().GetTime().TimeOfDay;

            // way forward
            for (int i = 0; i < forwardroute.Count - 1; i++)
            {
                double distance = LatLongProjection.HaversineDistance(forwardroute[i], forwardroute[i + 1]);
                double angle = LatLongProjection.GetBearing(forwardroute[i], forwardroute[i + 1]);
                TimeSpan segment_time = TimeSpan.FromSeconds(distance / deliveryroute.GetAircraft().GetCruisingSpeed());
                double deltadistance = distance / segment_time.TotalSeconds * 5;
                Point newpoint;
                double d = deltadistance;
                while (d <= distance)
                {
                    newpoint = LatLongProjection.DestinyPoint(forwardroute[i], d, angle);
                    newpoint.SetAltitude(deliveryroute.GetAltitude());
                    //double deltatime = deltadistance / deliveryroute.GetAircraft().GetCruisingSpeed();
                    daytime = daytime + TimeSpan.FromSeconds(5.0);//deltatime);
                    newpoint.SetTime(day + daytime);
                    discretizedroute.Add(newpoint);
                    d = d + deltadistance;
                }
                // add last route to the end this segment, with a time interval diff of 5sec
                newpoint = forwardroute[i + 1].GetCopy();
                newpoint.SetAltitude(deliveryroute.GetAltitude());
                daytime = daytime + TimeSpan.FromSeconds((d - distance) / deliveryroute.GetAircraft().GetCruisingSpeed());//deltatime);
                newpoint.SetTime(day + daytime);
                discretizedroute.Add(newpoint);
            }

            // delivery point -> the drone will hover at destination for 20" at 20m
            double delivery_alt = 20 + orig.GetAltitude();

            // TBD: check for Elevation in DEM when setting the delivery altitude!!
            List<Point> delivery = MakeDescension(discretizedroute.Last(), deliveryroute.GetAircraft(), deliveryroute.GetAltitude(), delivery_alt);
            daytime = delivery.Last().GetTime().TimeOfDay;

            // 20 secs hover are 3 more points
            Point hover = delivery.Last();
            for (int h = 1; h < 3; h++)
            {
                Point new_hover = hover.GetCopy();
                new_hover.SetAltitude(delivery_alt);
                daytime = daytime + TimeSpan.FromSeconds(5.0);//deltatime);
                new_hover.SetTime(day + daytime);
                delivery.Add(new_hover);
            }
            delivery.AddRange(MakeAscension(delivery.Last(), deliveryroute.GetAircraft(), delivery_alt, deliveryroute.GetAltitude()));
            discretizedroute.AddRange(delivery);
            daytime = daytime + TimeSpan.FromSeconds(5.0);

            // way backwards
            for (int i = 0; i < backroute.Count - 1; i++)
            {
                double distance = LatLongProjection.HaversineDistance(backroute[i], backroute[i + 1]);
                double angle = LatLongProjection.GetBearing(backroute[i], backroute[i + 1]);
                TimeSpan segment_time = TimeSpan.FromSeconds(distance / deliveryroute.GetAircraft().GetCruisingSpeed());
                double deltadistance = distance / segment_time.TotalSeconds * 5;
                Point newpoint;
                double d = deltadistance;
                while (d <= distance)
                {
                    newpoint = LatLongProjection.DestinyPoint(backroute[i], d, angle);
                    newpoint.SetAltitude(deliveryroute.GetAltitude());
                    //double deltatime = deltadistance / deliveryroute.GetAircraft().GetCruisingSpeed();
                    daytime = daytime + TimeSpan.FromSeconds(5.0); // deltatime);
                    newpoint.SetTime(day + daytime);
                    discretizedroute.Add(newpoint);
                    d = d + deltadistance;
                }
                // add last route to the end, with a time interval diff of 5sec
                newpoint = backroute[i + 1].GetCopy();
                newpoint.SetAltitude(deliveryroute.GetAltitude());
                daytime = daytime + TimeSpan.FromSeconds((d - distance) / deliveryroute.GetAircraft().GetCruisingSpeed());//deltatime);
                newpoint.SetTime(day + daytime);
                discretizedroute.Add(newpoint);
            }

            // Descend
            List<Point> descend_path = MakeDescension(discretizedroute.Last(), deliveryroute.GetAircraft(), deliveryroute.GetAltitude(), orig.GetAltitude());
            discretizedroute.AddRange(descend_path);
            //discretizedroute.Add(orig.GetCopy());

            deliveryroute.SetDiscretizedRoute(discretizedroute);
        }

        // this function discretizes a delivery route in several points and 
        // adds the return path as reverse from one way
        public void DiscretizedDeliveryRoute(DeliveryRoute deliveryroute) 
        {
            deliveryroute.SetAircraft(aircraft_tune(deliveryroute.GetAircraft()));

            Point orig = deliveryroute.GetOriginPoint();
            Point dest = deliveryroute.GetDestination(); //same as GetRoute().Last()

            // FIRST: concatennates forward + backward route 
            List<Point> forwardroute = new List<Point>();
            List<Point> backroute = new List<Point>();
            Stack<Point> returnroute = new Stack<Point>();

            forwardroute.Add(orig);// take-off point
            
            foreach (Point punto in deliveryroute.GetRoute())
            {
                forwardroute.Add(punto);
                returnroute.Push(punto);
            }
            
            while (returnroute.Count>0)
            {
                backroute.Add(returnroute.Pop());

            }
            backroute.Add(orig); // land point
            // we have 2 list of points such as fw=0,1,2,3 bw=3,2,1,0
            SetRadarRoute(deliveryroute, forwardroute, backroute);
        }

        public DeliveryRoute AddTime(DeliveryRoute route, Random rnd, double meantime, double stdDev, TimeSpan lowerlimit, TimeSpan upperlimit) // this function adds the time to the discrtized delivery route
        {
          
            
            bool well_allocated = false;
            while (well_allocated == false)
            {
                TimeSpan randomtime = timer.RandomTimeGenerator(rnd, meantime, stdDev, lowerlimit, upperlimit);
                DeliveryRoute copy_route = route.GetCopy();
                foreach (Point punto in copy_route.GetDiscretizedRoute())
                {
                    DateTime newtime = punto.GetTime() + randomtime;
                    punto.SetTime(newtime);
                }
                if (!(copy_route.GetDiscretizedRoute()[copy_route.GetDiscretizedRoute().Count - 1].GetTime() > (copy_route.GetDiscretizedRoute()[copy_route.GetDiscretizedRoute().Count - 1].GetTime().Date + upperlimit) || copy_route.GetDiscretizedRoute()[0].GetTime() < (copy_route.GetDiscretizedRoute()[0].GetTime().Date + lowerlimit)))
                {
                    well_allocated = true;
                    route = copy_route.GetCopy();
                }
            }

            return route;
        }

        public DeliveryRoute GetCopy() // Thjis functions returns a copy of this deliveryroute
        {
            

            List<Point> newroute = new List<Point>();
            foreach(Point punto in this.Route)
            {
                Point newpoint = new Point();
                newpoint = punto.GetCopy();
                newroute.Add(newpoint);
            }
            DeliveryRoute copy = new DeliveryRoute(this.OriginPoint.GetCopy(),newroute, this.altitude, this.aircraft.GetCopy());

            List<Point> newlist = new List<Point>();
            foreach(Point punto in this.discretizedroute)
            {
                Point newpoint = new Point();
                newpoint = punto.GetCopy();
                newlist.Add(newpoint);
            }
            copy.SetDiscretizedRoute(newlist);

            return copy;
        }

        }
}
