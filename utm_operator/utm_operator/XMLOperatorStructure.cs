using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using utm_operation;
using utm_drone;
using utm_utils;
using utm_routes.VLOS_routes;
using utm_routes.BVLOS_routes;
using utm_routes.DeliveryRoutes;
using utm_routes.EVLOSRoutes;
using System.Xml.Serialization;
using System.IO;
using utm_country;

namespace utm_operator
{

    public class XMLOperatorStructure
    {

        // This class is used to generate the XML files to store the operators. It is also used to read the operators from a xml file.


        public Tuple<Country, List<Operator>> GetGeneration(generation i) // Transforms the public partial class of operators into a list of operators
        {
            Country new_country = GetCountry(i.country);
            List<Operator> operator_list = new List<Operator>();
            foreach (generationOperator op in i.operators)
            {
                Operator new_operator = new Operator();
                new_operator.SetIdentifier(op.ID);
                List<drone> fleet_of_drones = GetFleetOfDrone(op);
                new_operator.SetFleetOfDrones(fleet_of_drones);
                new_operator.SetNumberOfDrones(new_operator.GetFleetOfDrones().Count);
                List<Operation> operation_list = GetListOfOperation(op, fleet_of_drones);
                new_operator.SetOperations(operation_list);
                operator_list.Add(new_operator);
            }
            return new Tuple<Country, List<Operator>>(new_country, operator_list);

        }

        public Country GetCountry(generationCountry i)
        {
            Country newcountry = new Country();
            string name = i.name;
            CountryItem name_of_the_country = FindCountryName(name);
            newcountry.SetCountryName(name_of_the_country);
            return newcountry;

        }

        public CountryItem FindCountryName(string countryname)
        {
            CountryItem name = new CountryItem();
            if (countryname.Equals("Spain"))
            {
                name = CountryItem.Spain;
            }
            return name;
        }

        public List<drone> GetFleetOfDrone(generationOperator i)
        {
            List<drone> drone_list = new List<drone>();
            foreach (generationOperatorDrone drone in i.fleetofdrones)
            {
                drone aircraft = new drone();
                aircraft.SetIdentifier(drone.ICAOcode.ToString());
                aircraft.SetModel(drone.type);
                aircraft.SetScanSpeed(Convert.ToDouble(drone.scanspeed));
                aircraft.SetCruisingSpeed(Convert.ToDouble(drone.cruisingspeed));
                aircraft.SetClimbSpeed(Convert.ToDouble(drone.climbingspeed));
                aircraft.SetDescendSpeed(Convert.ToDouble(drone.descendspeed));
                drone_list.Add(aircraft);
            }
            return drone_list;
        }

        public List<Operation> GetListOfOperation(generationOperator op, List<drone> drone_list)
        {
            List<Operation> operation_list = new List<Operation>();
            foreach (generationOperatorOperation operation in op.operations)
            {
                Operation new_operation = GetOperation(operation, drone_list);
                operation_list.Add(new_operation);
            }
            return operation_list;
        }

        public Operation GetOperation(generationOperatorOperation op, List<drone> drone_list)
        {
            Operation new_operation = new Operation("__");
            string drone_identifier = op.drone.ToString();
            drone aircraft = FindDroneInList(drone_list, drone_identifier);
            new_operation.SetAircraft(aircraft);
            DateTime departure_time = Convert.ToDateTime(op.departuretime);
            DateTime arrival_time = Convert.ToDateTime(op.arrivaltime);
            new_operation.SetStartTime(departure_time);
            new_operation.SetFinalTime(arrival_time);
            RouteType route_type = GetRouteType(op.typeofoperation.ToString());
            new_operation.SetRouteType(route_type);

            if (route_type == RouteType.VLOS)
            {
                VLOSRoute route = FillVLOSRoute(op, aircraft);
                new_operation.SetVLOSRoute(route);
            }

            if (route_type == RouteType.EVLOS)
            {
                EVLOSRoute route = FillEVLOSRoute(op, aircraft);
                new_operation.SetEVLOSRoute(route);
            }
            if (route_type == RouteType.BVLOS)
            {
                BVLOSRoute route = FillBVLOSRoute(op, aircraft);
                new_operation.SetBVLOSRoute(route);
            }
            if(route_type==RouteType.Delivery)
            {
                DeliveryRoute route = FillDeliveryRoute(op, aircraft);
                new_operation.SetDeliveryRoutes(route);
            }
            List<Point> discretized_points = GetDiscretizedList(op, new_operation.flightId);
            new_operation.SetDiscreitzedRoute(discretized_points);
            return new_operation;
        }

        public VLOSRoute FillVLOSRoute(generationOperatorOperation i, drone aircraft)
        {
            double altitude =Convert.ToDouble(i.flightaltitude);
            Point origin_point = ConvertStringToPoint(i.originpoint);
            Point first_point = ConvertStringToPoint(i.firstpoint);
            Point second_point = ConvertStringToPoint(i.secondpoint);
            // rectangle
            Point first_rectangle = ConvertStringToPoint(i.rectangle.firstrectanglepoint);
            Point second_rectangle = ConvertStringToPoint(i.rectangle.secondrectanglepoint);
            Point third_rectangle = ConvertStringToPoint(i.rectangle.thirdrectanglepoint);
            Point fourth_rectangle = ConvertStringToPoint(i.rectangle.fourthrectanglepoint);
            Rectangle rectangle = new Rectangle(first_rectangle, second_point, third_rectangle, fourth_rectangle);
            List<Point> scan = new List<Point>();
            foreach(string point in i.scan)
            {
                scan.Add(ConvertStringToPoint(point));
            }
            rectangle.SetScan(scan);
            VLOSRoute route = new VLOSRoute(origin_point, first_point,second_point,rectangle,altitude,aircraft);
            return route;
        }

        public EVLOSRoute FillEVLOSRoute(generationOperatorOperation i, drone aircraft)
        {
            double altitude = Convert.ToDouble(i.flightaltitude);
            Point origin_point = ConvertStringToPoint(i.originpoint);
            Point first_point = ConvertStringToPoint(i.firstpoint);
            Point second_point = ConvertStringToPoint(i.secondpoint);
            // rectangle
            Point first_rectangle = ConvertStringToPoint(i.rectangle.firstrectanglepoint);
            Point second_rectangle = ConvertStringToPoint(i.rectangle.secondrectanglepoint);
            Point third_rectangle = ConvertStringToPoint(i.rectangle.thirdrectanglepoint);
            Point fourth_rectangle = ConvertStringToPoint(i.rectangle.fourthrectanglepoint);
            Rectangle rectangle = new Rectangle(first_rectangle, second_point, third_rectangle, fourth_rectangle);
            List<Point> scan = new List<Point>();
            foreach (string point in i.scan)
            {
                scan.Add(ConvertStringToPoint(point));
            }
            rectangle.SetScan(scan);
            EVLOSRoute route = new EVLOSRoute(origin_point, first_point, second_point, rectangle, altitude, aircraft);
            return route;
        }

        public BVLOSRoute FillBVLOSRoute(generationOperatorOperation i, drone aircraft)
        {
            double altitude = Convert.ToDouble(i.flightaltitude);
            Point origin_point = ConvertStringToPoint(i.originpoint);
            Point first_intermediate_point = ConvertStringToPoint(i.firstintermediatepoint);
            Point second_intermediate_point = ConvertStringToPoint(i.secondintemediatepoint);
            List<BVLOSPoint> bvlospoints = new List<BVLOSPoint>();
            foreach (generationOperatorOperationBVLOSpoint bvlospoint in i.BVLOSpoints)
            {
                BVLOSPoint new_point = new BVLOSPoint();
                Point center_point = ConvertStringToPoint(bvlospoint.centerpoint);
                Point first_point = ConvertStringToPoint(bvlospoint.rectangle.firstrectanglepoint);
                Point second_point = ConvertStringToPoint(bvlospoint.rectangle.secondrectanglepoint);
                Point third_point = ConvertStringToPoint(bvlospoint.rectangle.thirdrectanglepoint);
                Point fourth_point = ConvertStringToPoint(bvlospoint.rectangle.fourthrectanglepoint);
                Rectangle rectangle = new Rectangle(first_point, second_point, third_point, fourth_point);
                List<Point> scan = new List<Point>();
                foreach(string point in bvlospoint.scan)
                {
                    scan.Add(ConvertStringToPoint(point));
                }
                rectangle.SetScan(scan);
                new_point.SetRectangle(rectangle);
                new_point.SetPoint(center_point);
                bvlospoints.Add(new_point);
            }
            BVLOSRoute route = new BVLOSRoute(bvlospoints, origin_point, first_intermediate_point, second_intermediate_point, altitude, aircraft);
            return route;
        }

        public DeliveryRoute FillDeliveryRoute(generationOperatorOperation i, drone aircraft)
        {
            double altitude = Convert.ToDouble(i.flightaltitude);
            Point origin_point = ConvertStringToPoint(i.originpoint);
            List<Point> route_points = new List<Point>();
            foreach(string point in i.routepoints)
            {
                route_points.Add(ConvertStringToPoint(point));
            }
            DeliveryRoute route = new DeliveryRoute(origin_point, route_points, altitude, aircraft);
            return route;
        }

        public List<Point> GetDiscretizedList(generationOperatorOperation op, String flight_id)
        {
            List<Point> discretized_list = new List<Point>();
            foreach (generationOperatorOperationPoint point in op.discretizedroute)
            {
                Point coordinates = ConvertStringToPoint(point.coordinates);
                double altitude = Convert.ToDouble(point.altitude);
                coordinates.SetAltitude(altitude);
                DateTime time = Convert.ToDateTime(point.time);
                coordinates.SetTime(time);
                coordinates.SetOperation(flight_id);
                discretized_list.Add(coordinates);
            }
            return discretized_list;
        }

        public RouteType GetRouteType(string type)
        {
            RouteType route_type = new RouteType();
            if(type.Equals("VLOS"))
            {
                route_type = RouteType.VLOS;
            }
            if (type.Equals("EVLOS"))
            {
                route_type = RouteType.EVLOS;
            }
            if(type.Equals("BVLOS"))
            {
                route_type = RouteType.BVLOS;
             }
            if(type.Equals("Delivery"))
            {
                route_type = RouteType.Delivery;
            }
            return route_type;
         }

        public drone FindDroneInList(List<drone> drone_list, string ICAO_code)
        {
            drone new_drone = new drone();
            foreach (drone aircraft in drone_list)
            {
                if (aircraft.GetIdentifier().Equals(ICAO_code))
                {
                    new_drone = aircraft.GetCopy();
                    break;
                }

            }
            return new_drone;
        }

        public Point ConvertStringToPoint(string information)
        {
            string[] words = information.Split(' ');
            double latitude = Convert.ToDouble(words[0]);
            double longitude = Convert.ToDouble(words[1]);
            Point new_point = new Point(latitude, longitude);
            return new_point;
        }
    }


    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class generation
    {

        private generationCountry countryField;

        private generationOperator[] operatorsField;

        /// <remarks/>
        public generationCountry country
        {
            get
            {
                return this.countryField;
            }
            set
            {
                this.countryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("operator", IsNullable = false)]
        public generationOperator[] operators
        {
            get
            {
                return this.operatorsField;
            }
            set
            {
                this.operatorsField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class generationCountry
    {

        private string nameField;

        /// <remarks/>
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class generationOperator
    {

        private generationOperatorDrone[] fleetofdronesField;

        private generationOperatorOperation[] operationsField;

        private string idField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute("fleet-of-drones")]
        [System.Xml.Serialization.XmlArrayItemAttribute("drone", IsNullable = false)]
        public generationOperatorDrone[] fleetofdrones
        {
            get
            {
                return this.fleetofdronesField;
            }
            set
            {
                this.fleetofdronesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("operation", IsNullable = false)]
        public generationOperatorOperation[] operations
        {
            get
            {
                return this.operationsField;
            }
            set
            {
                this.operationsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class generationOperatorDrone
    {

        private string typeField;

        private decimal cruisingspeedField;

        private decimal scanspeedField;

        private byte climbingspeedField;

        private byte descendspeedField;

        private uint iCAOcodeField;

        /// <remarks/>
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("cruising-speed")]
        public decimal cruisingspeed
        {
            get
            {
                return this.cruisingspeedField;
            }
            set
            {
                this.cruisingspeedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("scan-speed")]
        public decimal scanspeed
        {
            get
            {
                return this.scanspeedField;
            }
            set
            {
                this.scanspeedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("climbing-speed")]
        public byte climbingspeed
        {
            get
            {
                return this.climbingspeedField;
            }
            set
            {
                this.climbingspeedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("descend-speed")]
        public byte descendspeed
        {
            get
            {
                return this.descendspeedField;
            }
            set
            {
                this.descendspeedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("ICAO-code")]
        public uint ICAOcode
        {
            get
            {
                return this.iCAOcodeField;
            }
            set
            {
                this.iCAOcodeField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class generationOperatorOperation
    {

        private uint droneField;

        private string departuretimeField;

        private string arrivaltimeField;

        private string typeofoperationField;

        private decimal flightaltitudeField;

        private string originpointField;

        private string[] routepointsField;

        private string firstintermediatepointField;

        private generationOperatorOperationBVLOSpoint[] bVLOSpointsField;

        private string secondintemediatepointField;

        private string firstpointField;

        private string secondpointField;

        private generationOperatorOperationRectangle rectangleField;

        private string[] scanField;

        private generationOperatorOperationPoint[] discretizedrouteField;

        /// <remarks/>
        public uint drone
        {
            get
            {
                return this.droneField;
            }
            set
            {
                this.droneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("departure-time")]
        public string departuretime
        {
            get
            {
                return this.departuretimeField;
            }
            set
            {
                this.departuretimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("arrival-time")]
        public string arrivaltime
        {
            get
            {
                return this.arrivaltimeField;
            }
            set
            {
                this.arrivaltimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("type-of-operation")]
        public string typeofoperation
        {
            get
            {
                return this.typeofoperationField;
            }
            set
            {
                this.typeofoperationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("flight-altitude")]
        public decimal flightaltitude
        {
            get
            {
                return this.flightaltitudeField;
            }
            set
            {
                this.flightaltitudeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("origin-point")]
        public string originpoint
        {
            get
            {
                return this.originpointField;
            }
            set
            {
                this.originpointField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute("route-points")]
        [System.Xml.Serialization.XmlArrayItemAttribute("route-point", IsNullable = false)]
        public string[] routepoints
        {
            get
            {
                return this.routepointsField;
            }
            set
            {
                this.routepointsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("first-intermediate-point")]
        public string firstintermediatepoint
        {
            get
            {
                return this.firstintermediatepointField;
            }
            set
            {
                this.firstintermediatepointField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute("BVLOS-points")]
        [System.Xml.Serialization.XmlArrayItemAttribute("BVLOS-point", IsNullable = false)]
        public generationOperatorOperationBVLOSpoint[] BVLOSpoints
        {
            get
            {
                return this.bVLOSpointsField;
            }
            set
            {
                this.bVLOSpointsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("second-intemediate-point")]
        public string secondintemediatepoint
        {
            get
            {
                return this.secondintemediatepointField;
            }
            set
            {
                this.secondintemediatepointField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("first-point")]
        public string firstpoint
        {
            get
            {
                return this.firstpointField;
            }
            set
            {
                this.firstpointField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("second-point")]
        public string secondpoint
        {
            get
            {
                return this.secondpointField;
            }
            set
            {
                this.secondpointField = value;
            }
        }

        /// <remarks/>
        public generationOperatorOperationRectangle rectangle
        {
            get
            {
                return this.rectangleField;
            }
            set
            {
                this.rectangleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("scan-point", IsNullable = false)]
        public string[] scan
        {
            get
            {
                return this.scanField;
            }
            set
            {
                this.scanField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute("discretized-route")]
        [System.Xml.Serialization.XmlArrayItemAttribute("point", IsNullable = false)]
        public generationOperatorOperationPoint[] discretizedroute
        {
            get
            {
                return this.discretizedrouteField;
            }
            set
            {
                this.discretizedrouteField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class generationOperatorOperationBVLOSpoint
    {

        private string centerpointField;

        private generationOperatorOperationBVLOSpointRectangle rectangleField;

        private string[] scanField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("center-point")]
        public string centerpoint
        {
            get
            {
                return this.centerpointField;
            }
            set
            {
                this.centerpointField = value;
            }
        }

        /// <remarks/>
        public generationOperatorOperationBVLOSpointRectangle rectangle
        {
            get
            {
                return this.rectangleField;
            }
            set
            {
                this.rectangleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("scan-point", IsNullable = false)]
        public string[] scan
        {
            get
            {
                return this.scanField;
            }
            set
            {
                this.scanField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class generationOperatorOperationBVLOSpointRectangle
    {

        private string firstrectanglepointField;

        private string secondrectanglepointField;

        private string thirdrectanglepointField;

        private string fourthrectanglepointField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("first-rectangle-point")]
        public string firstrectanglepoint
        {
            get
            {
                return this.firstrectanglepointField;
            }
            set
            {
                this.firstrectanglepointField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("second-rectangle-point")]
        public string secondrectanglepoint
        {
            get
            {
                return this.secondrectanglepointField;
            }
            set
            {
                this.secondrectanglepointField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("third-rectangle-point")]
        public string thirdrectanglepoint
        {
            get
            {
                return this.thirdrectanglepointField;
            }
            set
            {
                this.thirdrectanglepointField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("fourth-rectangle-point")]
        public string fourthrectanglepoint
        {
            get
            {
                return this.fourthrectanglepointField;
            }
            set
            {
                this.fourthrectanglepointField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class generationOperatorOperationRectangle
    {

        private string firstrectanglepointField;

        private string secondrectanglepointField;

        private string thirdrectanglepointField;

        private string fourthrectanglepointField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("first-rectangle-point")]
        public string firstrectanglepoint
        {
            get
            {
                return this.firstrectanglepointField;
            }
            set
            {
                this.firstrectanglepointField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("second-rectangle-point")]
        public string secondrectanglepoint
        {
            get
            {
                return this.secondrectanglepointField;
            }
            set
            {
                this.secondrectanglepointField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("third-rectangle-point")]
        public string thirdrectanglepoint
        {
            get
            {
                return this.thirdrectanglepointField;
            }
            set
            {
                this.thirdrectanglepointField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("fourth-rectangle-point")]
        public string fourthrectanglepoint
        {
            get
            {
                return this.fourthrectanglepointField;
            }
            set
            {
                this.fourthrectanglepointField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class generationOperatorOperationPoint
    {

        private string coordinatesField;

        private decimal altitudeField;

        private string timeField;

        /// <remarks/>
        public string coordinates
        {
            get
            {
                return this.coordinatesField;
            }
            set
            {
                this.coordinatesField = value;
            }
        }

        /// <remarks/>
        public decimal altitude
        {
            get
            {
                return this.altitudeField;
            }
            set
            {
                this.altitudeField = value;
            }
        }

        /// <remarks/>
        public string time
        {
            get
            {
                return this.timeField;
            }
            set
            {
                this.timeField = value;
            }
        }
    }




}







