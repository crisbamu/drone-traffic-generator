using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using utm_country;
using utm_utils;
using utm_drone;
using utm_routes.DeliveryRoutes;
using utm_routes.VLOS_routes;
using utm_routes.BVLOS_routes;
using utm_routes.EVLOSRoutes;  

namespace utm_routes
{
   public static class InPolygon
    {
        // attributes

     

        // functions

        

        //public Point centroid { get; set; }
        // this is a test function

        /// <summary>
        /// Determines if the given point is inside the polygon
        /// </summary>
        /// <param name="polygon">the vertices of polygon</param>
        /// <param name="testPoint">the given point</param>
        /// <returns>true if the point is inside the polygon; otherwise, false</returns>
        public static bool IsPointInPolygon(Point testPoint, List<Point> polygon) 
        {
            bool result = false;

            int last = polygon.Count() - 1;
            if ((polygon[0].GetLatitude() < testPoint.GetLatitude() && polygon[last].GetLatitude() >= testPoint.GetLatitude()) || (polygon[last].GetLatitude() < testPoint.GetLatitude() && polygon[0].GetLatitude() >= testPoint.GetLatitude()))
            {
                if (polygon[0].GetLongitude() + (testPoint.GetLatitude() - polygon[0].GetLatitude()) / (polygon[last].GetLatitude() - polygon[0].GetLatitude()) * (polygon[last].GetLongitude() - polygon[0].GetLongitude()) < testPoint.GetLongitude())
                {
                    result = !result;
                }
            }
            for (int i = 1; i < polygon.Count(); i++)
            {
                if ((polygon[i].GetLatitude() < testPoint.GetLatitude() && polygon[i-1].GetLatitude() >= testPoint.GetLatitude()) || (polygon[i-1].GetLatitude() < testPoint.GetLatitude() && polygon[i].GetLatitude() >= testPoint.GetLatitude()))
                {
                    if (polygon[i].GetLongitude() + (testPoint.GetLatitude() - polygon[i].GetLatitude()) / (polygon[i-1].GetLatitude() - polygon[i].GetLatitude()) * (polygon[i-1].GetLongitude() - polygon[i].GetLongitude()) < testPoint.GetLongitude())
                    {
                        result = !result;
                    }
                }
            }
            return result;
        }

        public static List<Point> InPolygonList(List<Point> points, List<Point> polygon)  // this functions returns a list of the points which are inside the polygon.
        {
            
            List<Point> inpoints = new List<Point>();

            Polygon fpolygon = new Polygon(polygon);
            Point Centroid = fpolygon.GetCentroid();
            double mindistance = fpolygon.FindMinimumDistance(Centroid);


            foreach (Point puntos in points)
            {
                bool inside = false;
                double distance = LatLongProjection.HaversineDistance(puntos, Centroid);


                if (distance <= mindistance)
                {
                    inside = true;
                }
                else
                {
                    inside = InPolygon.IsPointInPolygon(puntos, polygon);
                }
                if (inside == true)
                    inpoints.Add(puntos);

            }

            return inpoints;

        }

        public static bool IsPointInForbiddenAreas(Point testpoint, List<ForbiddenArea> forbiddenareas) // this function returns true if the point is inside any forbidden area on the list.
        {
            bool inside = false;
           
            foreach(ForbiddenArea area in forbiddenareas)
            {
                if (area.GetAreaType() == ForbiddenAreaType.points)
                {
                    bool test = IsPointInPolygon(testpoint, area.GetPolygon());
                    if (test == true)
                    {
                        inside = true;
                        break;
                    }
                }

                if(area.GetAreaType()== ForbiddenAreaType.circle)
                {
                    double distance = LatLongProjection.HaversineDistance(testpoint, area.GetReferencePoint());
                    if(distance<=area.GetRadius())
                    {
                        inside = true;
                        break;
                    }
                
                }
            }
            return inside;
        }

        public static bool IsPointInCities(Point testpoint, List<City> cities) // this function returns true if the point is inside the securitymargin from any city on the list.
        {
            bool inside = false;
         
            foreach(City city in cities)
            {
                double distance = LatLongProjection.HaversineDistance(testpoint, city.GetCityRefPoint());
                if(distance<=(city.GetCityRadius()*1852))
                {
                    inside = true;
                    break;
                }
            }

            return inside;
        }

        public static bool IsPointInAirports(Point testpoint, List<Airport> airports) // this function returns true if the testpoint is inside the security distance of any airport on the city.
        {
            bool inside = false;
           
            foreach(Airport airport in airports)
            {
                double distance = LatLongProjection.HaversineDistance(testpoint, airport.GetRefPoint());
                if(distance<=airport.GetRadius())
                {
                    inside = true;
                    break;
                }
            }
            return inside;
        }


        public static bool IsPointFeasible(Point testpoint, Country country)// this functions returns true if the point is in the country contour and is out all forbidden areas
        {
            if (!IsPointInPolygon(testpoint, country.GetLandside().GetCountryContour()))
                return false;

            if (country.GetForbiddenAreasAvailability() && IsPointInForbiddenAreas(testpoint, country.GetForbiddenAreas()))
                return false;

            if (country.GetControlledAirspaceAvailability() && IsPointInForbiddenAreas(testpoint, country.GetControlledAirspace()))
                return false;

            if (country.GetCitiesAvailability() && IsPointInCities(testpoint, country.GetCities()))
                return false;

            if (country.GetAirportsAvailability() && IsPointInAirports(testpoint, country.GetAirports()))
                return false;

            return true;
        }

        public static bool IsVLOSRouteFeasible(VLOSRoute route, Country country)
        {
            
            bool goodroute = false;
            List<ForbiddenArea> wholeforbiddenareas = new List<ForbiddenArea>(country.GetForbiddenAreas());
            wholeforbiddenareas.AddRange(country.GetControlledAirspace());
            bool goodpoint;
            int x = 0;
            foreach (Point punto in route.GetDiscretizedRoute())
            {
                goodpoint = IsPointFeasible(punto, country);
                if (!goodpoint)
                {
                    x++;
                    break;
                }
            }

            if (x == 0)
            {
                goodroute = true;
            }

            return goodroute;
        }

        public static bool IsEVLOSRouteFeasible(EVLOSRoute route, Country country)
        {
            bool goodroute = false;
            List<ForbiddenArea> wholeforbiddenareas = new List<ForbiddenArea>(country.GetForbiddenAreas());
            wholeforbiddenareas.AddRange(country.GetControlledAirspace());
            bool goodpoint;
            int x = 0;
            foreach (Point punto in route.GetDiscretizedRoute())
            {
                goodpoint = IsPointFeasible(punto, country);
                if (!goodpoint)
                {
                    x++;
                    break;
                }
            }

            if (x == 0)
            {
                goodroute = true;
            }

            return goodroute;
        }

        public static bool IsBVLOSRouteFeasible(BVLOSRoute route, Country country)
        {
            bool goodroute = false;
            List<ForbiddenArea> wholeforbiddenareas = new List<ForbiddenArea>(country.GetForbiddenAreas());
            wholeforbiddenareas.AddRange(country.GetControlledAirspace());
            bool goodpoint;
            int x = 0;
            foreach (Point punto in route.GetDiscretizedRoute())
            {
                goodpoint = IsPointFeasible(punto, country);
                if (!goodpoint)
                {
                    x++;
                    break;
                }
            }

            if (x == 0)
            {
                goodroute = true;
            }

            return goodroute;
        }

        


        //
        public static List<Point> OutFrobiddenAreas(List<Point> points, List<ForbiddenArea> arealist) // This functions returns the points which are out of the forbidden areas.
        {
            List<Point> list = new List<Point>();
            

            foreach (Point punto in points)
            {

                int n = 0;
                bool inside = false;
                foreach (ForbiddenArea area in arealist)
                {
                    if (inside == false)
                    {

                        ForbiddenAreaType Areatype = area.GetAreaType();
                        if (Areatype == ForbiddenAreaType.circle)
                        {


                            double distance = LatLongProjection.HaversineDistance(punto, area.GetReferencePoint());

                            if (distance <= (area.GetRadius() * 1852)) // the radius is in NM and we need it in meters
                            {
                                inside = true;
                                n = n + 1;
                                break;
                            }
                        }

                        if (Areatype == ForbiddenAreaType.points)
                        {

                            inside = InPolygon.IsPointInPolygon(punto, area.GetPolygon());

                            if (inside == true)
                            {
                                n = n + 1;
                                break;
                            }

                        }
                    }
                }

                if (inside == false)
                {
                    list.Add(punto);
                }

            }
            return list;
        }

        public static List<Point> OutAirports(List<Point> points, List<Airport> aiportlist) // this function returns the list of points out the airports
        {
            List<Point> inpoints = new List<Point>();

            foreach (Point punto in points)
            {
                bool inside = false;

                foreach (Airport aeropuerto in aiportlist)
                {
                    if (inside == false)
                    {


                        double distance = LatLongProjection.HaversineDistance(punto, aeropuerto.GetRefPoint());

                        if (distance <= aeropuerto.GetRadius() * 1000) // the radius is in km and we need it in meters
                            inside = true;
                    }
                }
                if (inside == false)
                    inpoints.Add(punto);
            }

            return inpoints;
        }

        public static List<Point> OutCities(List<Point> points, List<City> citylist, double tolerance) // This function returns the list of points out the cities
        {
            List<Point> inpoints = new List<Point>();

            foreach (Point punto in points)
            {
                bool inside = false;

                foreach (City ciudad in citylist)
                {
                    if (inside == false)
                    {


                        double distance = LatLongProjection.HaversineDistance(punto, ciudad.GetCityRefPoint());

                        if (distance <= tolerance) // the tolerance must be in meters
                            inside = true;
                    }

                }
                if (inside == false)
                    inpoints.Add(punto);
            }

            return inpoints;

        }

        public static List<Point> FilterList(List<Point> points, List<Point> contour, List<ForbiddenArea> forbiddenlist, List<Airport> airportlist, List<City> citylist, Point centroid, double tolerance) // This function summarizes all the previous function adn filter every point for each polygon
        {
            List<Point> inpoint = new List<Point>();

            inpoint = InPolygon.InPolygonList(points, contour); // filter by contour
            inpoint = InPolygon.OutFrobiddenAreas(inpoint, forbiddenlist); //filter by forbidden areas
            inpoint = InPolygon.OutAirports(inpoint, airportlist);// filter by airports
            inpoint = InPolygon.OutCities(inpoint, citylist, tolerance); // filter by cities
            return inpoint;
        }

        public static Point FilterPoint(Point point, List<Point> contour, List<ForbiddenArea> forbiddenlist, List<Airport> airportlist, List<City> citylist, double radius, Point centroid, double tolerance) // This function summarizes all the previous function adn filter every point for each polygon
        {
            List<Point> inpoint = new List<Point>();
            inpoint.Add(point);

            inpoint = InPolygon.InPolygonList(inpoint, contour); // filter by contour
            inpoint = InPolygon.OutFrobiddenAreas(inpoint, forbiddenlist); //filter by forbidden areas
            inpoint = InPolygon.OutAirports(inpoint, airportlist);// filter by airports
            inpoint = InPolygon.OutCities(inpoint, citylist, tolerance); // filter by cities


            Point punto = new Point();

            if (inpoint.Count == 1)
            {
                punto = inpoint[0];
            }
            else
            {
                punto = null;
            }
            return punto;
        }


        public static bool IsInForbiddenPlaces(Point point, List<Point> contour, List<ForbiddenArea> forbiddenlist, List<Airport> airportlist, List<City> citylist, double tolerance) // This function says if the point is inside the polygon and out of forbidden areas. True if it is inside, otherwise is zero.
        {
            bool inside = false;
            List<Point> inpoint = new List<Point>();
            inpoint.Add(point);


            inpoint = InPolygon.InPolygonList(inpoint, contour); // filter by contour
            inpoint = InPolygon.OutFrobiddenAreas(inpoint, forbiddenlist); //filter by forbidden areas
            inpoint = InPolygon.OutAirports(inpoint, airportlist);// filter by airports
            inpoint = InPolygon.OutCities(inpoint, citylist, tolerance); // filter by cities

            if (inpoint.Count == 1)
            {
                inside = false;
            }
            else
            {
                inside = true;

            }
            return inside;
        }

        public static bool PointFilter(Point point, List<Point> contour, List<ForbiddenArea> forbidden) // this function indicates if a point of a delivery route is inside the forbidden places. If it is true, the point is inside the contour and out the forbidden areas, otherwise is false.
        {
            bool goodpoint = false;
            bool insidecontour = IsPointInPolygon(point, contour);
            bool isoutforbiddenareas = IsOutForbiddenAreas(point, forbidden);

            if (insidecontour == true && isoutforbiddenareas == true)
            {
                goodpoint = true;
            }
            return goodpoint;
        }

        public static bool IsOutForbiddenAreas(Point point, List<ForbiddenArea> forbiddenareas) // this functions says if the point is outside the forbidden areas. It is true if it is outside, otherwise is false.
        {
            int n = 0;
            

            foreach (ForbiddenArea area in forbiddenareas)
            {
                if (area.GetAreaType() == ForbiddenAreaType.circle)
                {
                    double distance = LatLongProjection.HaversineDistance(point, area.GetReferencePoint());

                    if (distance <= area.GetRadius() * 1852) // the radius is in NM and we do the operations in meters
                    {
                        n++;
                        break;
                    }

                }

                if (area.GetAreaType() == ForbiddenAreaType.points)
                {
                    bool isinside = IsPointInPolygon(point, area.GetPolygon());
                    if (isinside == true)
                    {
                        n++;
                        break;
                    }
                }
            }

            bool goodpoint = false;
            if (n == 0)
            {
                goodpoint = true;
            }
            return goodpoint;

        }

        public static bool DeliveryRouteFilter(DeliveryRoute route, List<Point> contour, List<ForbiddenArea> forbidden)
        {
            if (!PointFilter(route.GetOriginPoint(), contour, forbidden))
                return false;

            foreach (Point punto in route.GetDiscretizedRoute())
            {
                if (!PointFilter(punto, contour, forbidden))
                    return false;
            }
            return true;
        }
    }
}
