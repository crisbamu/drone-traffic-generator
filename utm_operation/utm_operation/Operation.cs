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
        static int next_id = 1000;
        public String flightId;
        public string operatorName;

        private string MavLinkidentification;

        // builders
        public Operation(String operatorName)
        {
            this.flightId = operatorName.Substring(0,2).ToUpper()+(next_id++).ToString();
        }

        public Operation(String name,DateTime start, DateTime end, drone air, RouteType route, VLOSRoute vlos,EVLOSRoute evlos ,BVLOSRoute bvlos, DeliveryRoute deliv, string mavlink)
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
            this.operatorName = name;
            this.flightId = name.Substring(0, 2).ToUpper() + (next_id++).ToString();

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
            string operatorName= this.operatorName;
            Operation newoperation = new Operation(operatorName, starttime, finaltime, air, type, vlos, evlos, bvlos, deliver, mavlink);
            return newoperation;


    }

        public int CompareTo(DateTime dep)
        {
            if (this.StartTime == dep)
                return 0;
            return this.StartTime.CompareTo(dep);
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
        public List<Point> GetRoute()
        {
            List<Point> discretizedroute = new List<Point>();
            if (routeType == RouteType.VLOS)
            {
                discretizedroute = this.vlosroute.GetDiscretizedRoute();
            }
            if (routeType == RouteType.EVLOS)
            {
                discretizedroute = this.evlosroute.GetDiscretizedRoute();
            }
            if (routeType == RouteType.BVLOS)
            {
                discretizedroute = this.bvlosroute.GetDiscretizedRoute();
            }
            if (routeType == RouteType.Delivery)
            {
                discretizedroute = this.deliveryroute.GetRoute();
            }
            return discretizedroute;
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
