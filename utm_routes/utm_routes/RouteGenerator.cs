using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using utm_utils;
using utm_country;
using utm_routes;
using utm_routes.BVLOS_routes;
using utm_routes.VLOS_routes;
using utm_routes.EVLOSRoutes;
using utm_routes.DeliveryRoutes;
using utm_drone;
using System.Xml;


namespace utm_routes
{
    public class RouteGenerator
    {
        // Written by Lluís Xavier Herranz on 02/08/2017

        //Attributes

        private Country country;

        // parameters of the routes
        private VLOSParameters vlosparameters;
        private EVLOSParameters evlosparameters;
        private BVLOSParameters bvlosparameters;
        private DeliveryParameters deliveryparameters;
        



        // builder

        public RouteGenerator(Country c, VLOSParameters vlosparam,EVLOSParameters evlosparam ,BVLOSParameters bvlosparam, DeliveryParameters deliver)
        {
            this.country = c;
            this.vlosparameters = vlosparam;
            this.evlosparameters = evlosparam;
            this.bvlosparameters = bvlosparam;
            this.deliveryparameters = deliver;
            
        }

        public RouteGenerator()
        {

        }

        public void SetCountry(Country a)
        {
            this.country = a;
        }

       

        public void SetVLOSParameters(VLOSParameters param)
        { this.vlosparameters = param; }

        public void SetEVLOSParameters(EVLOSParameters param)
        {
            this.evlosparameters = param;
        }

        public void SetBVLOSParameters(BVLOSParameters param)
        {
            this.bvlosparameters = param;
        }

        public void SetDeliveryParameters(DeliveryParameters param)
        { this.deliveryparameters = param; }

        public Tuple<double, double, double, double> FindMaximumPoints(CountryContour contour)  // This function finds the extreme points of a contour
        {
            double northest = contour.GetCountryContour()[0].GetLatitude();
            double southest = contour.GetCountryContour()[0].GetLatitude();
            double eastest = contour.GetCountryContour()[0].GetLongitude();
            double westest = contour.GetCountryContour()[0].GetLongitude();

            foreach (Point punto in contour.GetCountryContour())
            {
                if (punto.GetLatitude() > northest)
                    northest = punto.GetLatitude();

                if (punto.GetLatitude() < southest)
                    southest = punto.GetLatitude();

                if (punto.GetLongitude() > eastest)
                    eastest = punto.GetLongitude();

                if (punto.GetLongitude() < westest)
                    westest = punto.GetLongitude();
            }

            return new Tuple<double, double, double, double>(northest, southest, eastest, westest);

        }

        // for vlos

        public VLOSRoute GenerateVLOSRoute(drone drone, int parts,Random rnd)
        {
            //List<ForbiddenArea> wholeforbiddenareas = new List<ForbiddenArea>(country.GetForbiddenAreas());
            //wholeforbiddenareas.AddRange(country.GetControlledAirspace());

            var recolect = FindMaximumPoints(country.GetLandside());
            double northest = recolect.Item1;
            double southest = recolect.Item2;
            double eastest = recolect.Item3;
            double westest = recolect.Item4;

           
            VLOSRoute goodroute = new VLOSRoute();

            bool checkroute = false;
            while(checkroute==false)
            {
                Point newpoint = new Point();
                double latitude = (northest - southest) * rnd.NextDouble() + southest;
                double longitude = (eastest - westest) * rnd.NextDouble() + westest;
                newpoint = new Point(latitude, longitude);
                VLOSRoute preliminarroute = new VLOSRoute().CreateRoute(vlosparameters, drone, newpoint, parts, rnd);
                checkroute = InPolygon.IsVLOSRouteFeasible(preliminarroute, country);

                if(checkroute==true)
                {
                    goodroute = preliminarroute.GetCopy();
                }
            }
            return goodroute;
        }

        // for EVLOS

        public EVLOSRoute GenerateEVLOSRoute(drone aircraft, int parts, Random rnd)
        {
            List<ForbiddenArea> wholeforbiddenareas = new List<ForbiddenArea>(country.GetForbiddenAreas());
            wholeforbiddenareas.AddRange(country.GetControlledAirspace());

            var recolect = FindMaximumPoints(country.GetLandside());
            double northest = recolect.Item1;
            double southest = recolect.Item2;
            double eastest = recolect.Item3;
            double westest = recolect.Item4;

            
            EVLOSRoute goodroute = new EVLOSRoute();

            bool checkroute = false;
            while (checkroute == false)
            {
                Point newpoint = new Point();
                double latitude = (northest - southest) * rnd.NextDouble() + southest;
                double longitude = (eastest - westest) * rnd.NextDouble() + westest;
                newpoint = new Point(latitude, longitude);
                EVLOSRoute preliminarroute = new EVLOSRoute().CreateRoute(evlosparameters, aircraft, newpoint, parts, rnd);
                checkroute = InPolygon.IsEVLOSRouteFeasible(preliminarroute, country);

                if (checkroute == true)
                {
                    goodroute = preliminarroute.GetCopy();
                }
            }
            return goodroute;

        }
            
            
            



        // for bvlos

        public BVLOSRoute GenerateBVLOSRoute(drone drone, int parts,Random rnd)
        {
            List<ForbiddenArea> wholeforbiddenareas = new List<ForbiddenArea>(country.GetForbiddenAreas());
            wholeforbiddenareas.AddRange(country.GetControlledAirspace());

            var recolect = FindMaximumPoints(country.GetLandside());
            double northest = recolect.Item1;
            double southest = recolect.Item2;
            double eastest = recolect.Item3;
            double westest = recolect.Item4;

            
            BVLOSRoute goodroute = new BVLOSRoute();
            

            bool checkroute = false;
            while (checkroute == false)
            {
                Point newpoint = new Point();
                double latitude = (northest - southest) * rnd.NextDouble() + southest;//41.50481676853676;// 
                double longitude =  (eastest - westest) * rnd.NextDouble() + westest; //1.902405718909779;//
                newpoint = new Point(latitude, longitude);
                BVLOSRoute preliminarroute = new BVLOSRoute().CreateRoute(bvlosparameters, drone, newpoint, parts,rnd);
                checkroute = InPolygon.IsBVLOSRouteFeasible(preliminarroute, country);

                if (checkroute == true)
                {
                    goodroute = preliminarroute.GetCopy();
                }
            }
            return goodroute;
        }

        

        // for delivery routes

        public DeliveryRoute GenerateDeliveryRoute(drone aircraft, Random rnd)
        {
            List<ForbiddenArea> extendedforbiddenareas = country.GetForbiddenAreas();
            extendedforbiddenareas.AddRange(country.GetDeliveryForbiddenAreas());
            List<Point> contour = country.GetLandside().GetCountryContour();
            DeliveryRoute deliveryroute = null;

            bool goodroute = false;
            while (goodroute == false)
            {           
                int n = rnd.Next(0, country.GetDeliveryCenters().Count); // here we choose which delivery center is where the drone takes off
                int numberofmidpoints = rnd.Next(1, 6); // the number of mid points until the drone reaches the delivery zone.
                deliveryroute = new DeliveryRoute().CreateRoute(deliveryparameters, aircraft, country.GetDeliveryCenters()[n], numberofmidpoints, rnd);

                if (InPolygon.DeliveryRouteFilter(deliveryroute, contour, extendedforbiddenareas))
                {
                    goodroute = true;
                    deliveryroute.DiscretizeDeliveryRoute();

                }
                //else
                //{
                //   del deliveryroute;
                //}
            }
            return deliveryroute;
        }

        // this function loads the parameters stored in a xml
        public Tuple<VLOSParameters, EVLOSParameters, BVLOSParameters, DeliveryParameters> LoadRouteParameters(string foldername)
        {
            VLOSParameters vlosparameters = new VLOSParameters();
            EVLOSParameters evlosparameters = new EVLOSParameters();
            BVLOSParameters bvlosparameters = new BVLOSParameters();
            DeliveryParameters deliveryparameters = new DeliveryParameters();

            XmlTextReader reader = new XmlTextReader(foldername);
            XmlNodeType type;
            while (reader.Read())
            {
                type = reader.NodeType;
                if(reader.Name.Equals("parameter") && type == XmlNodeType.Element)
                {
                    while(reader.MoveToNextAttribute())
                    {
                        if(reader.Name.Equals("type"))
                        {
                            if(reader.Value.Equals("VLOS"))
                            {
                                while(!(reader.Name.Equals("parameter") && type == XmlNodeType.EndElement))
                                {
                                    if(reader.Name.Equals("first-max-distance") && type==XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        vlosparameters.SetFirstMaxDist(Convert.ToDouble(reader.Value));
                                    }
                                    if (reader.Name.Equals("first-min-distance") && type == XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        vlosparameters.SetFirstMinDist(Convert.ToDouble(reader.Value));
                                    }
                                    if (reader.Name.Equals("second-max-distance") && type == XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        vlosparameters.SetSecondMaxDist(Convert.ToDouble(reader.Value));
                                    }
                                    if (reader.Name.Equals("second-min-distance") && type == XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        vlosparameters.SetSecondMinDist(Convert.ToDouble(reader.Value));
                                    }
                                    if (reader.Name.Equals("angle-range") && type == XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        vlosparameters.SetAngleRange(Convert.ToDouble(reader.Value));
                                    }
                                    if (reader.Name.Equals("rectangle-max-distance") && type == XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        vlosparameters.SetRectangleMaxDist(Convert.ToDouble(reader.Value));
                                    }
                                    if (reader.Name.Equals("rectangle-min-distance") && type == XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        vlosparameters.SetRectangleMinDist(Convert.ToDouble(reader.Value));
                                    }
                                    if (reader.Name.Equals("flight-max-altitude") && type == XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        vlosparameters.SetMaximumAltitude(Convert.ToDouble(reader.Value));
                                    }
                                    if (reader.Name.Equals("flight-min-altitude") && type == XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        vlosparameters.SetMinimumAltitude(Convert.ToDouble(reader.Value));
                                    }
                                    reader.Read();
                                    type = reader.NodeType;
                                }
                            }

                            if (reader.Value.Equals("EVLOS"))
                            {
                                while (!(reader.Name.Equals("parameter") && type == XmlNodeType.EndElement))
                                {
                                    if (reader.Name.Equals("first-max-distance") && type == XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        evlosparameters.SetFirstMaxDist(Convert.ToDouble(reader.Value));
                                    }
                                    if (reader.Name.Equals("first-min-distance") && type == XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        evlosparameters.SetFirstMinDist(Convert.ToDouble(reader.Value));
                                    }
                                    if (reader.Name.Equals("second-max-distance") && type == XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        evlosparameters.SetSecondMaxDist(Convert.ToDouble(reader.Value));
                                    }
                                    if (reader.Name.Equals("second-min-distance") && type == XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        evlosparameters.SetSecondMinDist(Convert.ToDouble(reader.Value));
                                    }
                                    if (reader.Name.Equals("angle-range") && type == XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        evlosparameters.SetAngleRange(Convert.ToDouble(reader.Value));
                                    }
                                    if (reader.Name.Equals("rectangle-max-distance") && type == XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        evlosparameters.SetRectangleMaxDist(Convert.ToDouble(reader.Value));
                                    }
                                    if (reader.Name.Equals("rectangle-min-distance") && type == XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        evlosparameters.SetRectangleMinDist(Convert.ToDouble(reader.Value));
                                    }
                                    if (reader.Name.Equals("flight-max-altitude") && type == XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        evlosparameters.SetMaximumAltitude(Convert.ToDouble(reader.Value));
                                    }
                                    if (reader.Name.Equals("flight-min-altitude") && type == XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        evlosparameters.SetMinimumAltitude(Convert.ToDouble(reader.Value));
                                    }
                                    reader.Read();
                                    type = reader.NodeType;
                                }

                            }

                            if (reader.Value.Equals("BVLOS"))
                            {
                                while (!(reader.Name.Equals("parameter") && type == XmlNodeType.EndElement))
                                {
                                    if (reader.Name.Equals("max-number-of-operations") && type == XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        bvlosparameters.SetMaxNumGen(Convert.ToInt32(reader.Value));
                                    }
                                    if (reader.Name.Equals("min-number-of-operations") && type == XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        bvlosparameters.SetMinNumGen(Convert.ToInt32(reader.Value));
                                    }
                                    if (reader.Name.Equals("first-max-distance") && type == XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        bvlosparameters.SetFirstMaxDistance(Convert.ToDouble(reader.Value));
                                    }
                                    if (reader.Name.Equals("first-min-distance") && type == XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        bvlosparameters.SetFirstMinDistance(Convert.ToDouble(reader.Value));
                                    }
                                    if (reader.Name.Equals("max-distance-between-operations") && type == XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        bvlosparameters.SetMaxDistance(Convert.ToDouble(reader.Value));
                                    }
                                    if (reader.Name.Equals("min-distance-between-operations") && type == XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        bvlosparameters.SetMinDistance(Convert.ToDouble(reader.Value));
                                    }
                                    if (reader.Name.Equals("angle-range-between-operations") && type == XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        bvlosparameters.SetAngleRange(Convert.ToDouble(reader.Value));
                                    }
                                    if (reader.Name.Equals("rectangle-max-distance") && type == XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        bvlosparameters.SetRectangleMaxDistance(Convert.ToDouble(reader.Value));
                                    }
                                    if (reader.Name.Equals("rectangle-min-distance") && type == XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        bvlosparameters.SetRectangleMinDistance(Convert.ToDouble(reader.Value));
                                    }
                                    if (reader.Name.Equals("flight-max-altitude") && type == XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        bvlosparameters.SetMaxAltitude(Convert.ToDouble(reader.Value));
                                    }
                                    if (reader.Name.Equals("flight-min-altitude") && type == XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        bvlosparameters.SetMinAltitude(Convert.ToDouble(reader.Value));
                                    }
                                    reader.Read();
                                    type = reader.NodeType;
                                }
                            }

                            if (reader.Value.Equals("Delivery"))
                            {
                                while (!(reader.Name.Equals("parameter") && type == XmlNodeType.EndElement))
                                {
                                    if (reader.Name.Equals("max-distance-of-operation") && type == XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        deliveryparameters.SetMaxDistance(Convert.ToDouble(reader.Value));
                                    }
                                    if (reader.Name.Equals("flight-max-altitude") && type == XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        deliveryparameters.SetMaxAltitude(Convert.ToDouble(reader.Value));
                                    }
                                    if (reader.Name.Equals("flight-min-altitude") && type == XmlNodeType.Element)
                                    {
                                        reader.Read();
                                        deliveryparameters.SetMinAltitude(Convert.ToDouble(reader.Value));
                                    }
                                    reader.Read();
                                    type = reader.NodeType;
                                }

                            }
                        }
                    }
                }
                
            }
            
            reader.Close();
                return new Tuple<VLOSParameters, EVLOSParameters, BVLOSParameters, DeliveryParameters>(vlosparameters, evlosparameters, bvlosparameters, deliveryparameters);
          
        }

        // this function checks if the parameters are well inserted or if some of them miss an element.
        public bool AreTheRouteParametersWellLoaded(VLOSParameters vlosparameters, EVLOSParameters evlosparameters, BVLOSParameters bvlosparameters, DeliveryParameters deliveryparameters)
        {
            bool well_inserted = false;
            if(vlosparameters.AreVLOSParameterWellInserted()==true && evlosparameters.AreEVLOSParameterWellInserted()==true && bvlosparameters.AreBVLOSParametersWellInserted()==true && deliveryparameters.AreDeliveryParametersWellInserted()==true)
            {
                well_inserted = true;
            }
            return well_inserted;

        }
 
    }
}
