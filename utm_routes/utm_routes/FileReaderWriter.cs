using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using utm_country;
using utm_utils;
using utm_drone;
using System.IO;
using System.Xml;
using utm_routes.BVLOS_routes;
using utm_routes.VLOS_routes;
using utm_routes.DeliveryRoutes;



namespace utm_routes
{
    public static class FileReaderWriter
    {
        // Written by Lluís Xavier Herranz on 11/24/2016

        // This class reads a list of points as an input and writes its content in a txt file



        // function that reads the country contour

        public static List<CountryContour> ReadNewOneContour(string foldername)
        {
            XmlTextReader reader = new XmlTextReader(foldername);
            XmlNodeType type;

            List<CountryContour> countrylist = new List<CountryContour>();

            while (reader.Read())
            {
                type = reader.NodeType;



                if (reader.Name == "coordinates" && type == XmlNodeType.Element)
                {
                    CountryContour newcontour = new CountryContour();
                    List<Point> contourpoints = new List<Point>();

                    while (reader.MoveToNextAttribute())
                    {
                        if (reader.Name == "type")
                        {
                            newcontour.SetCountryType(reader.Value);
                        }
                    }


                    reader.Read();
                    string text = reader.Value;
                    string[] stringSeparators = new string[] { "\r\n" };
                    string[] words = text.Split(stringSeparators, StringSplitOptions.None);

                    int i = 0;

                    while (i < words.Length)
                    {
                        if (words[i].Length != 0)
                        {
                            string[] numbers = words[i].Split(' ');
                            double latitude = Convert.ToDouble(numbers[0]);
                            double longitude = Convert.ToDouble(numbers[1]);

                            Point newpoint = new Point(latitude, longitude);


                            contourpoints.Add(newpoint);
                        }
                        i++;

                    }
                    newcontour.SetContour(contourpoints);
                    countrylist.Add(newcontour);
                }

            }
            reader.Close();
            return countrylist;
        }


        public static List<CountryContour> ReadNewContour(string foldername)
        {
            XmlTextReader reader = new XmlTextReader(foldername);
            XmlNodeType type;

            List<CountryContour> countrylist = new List<CountryContour>();

            while (reader.Read())
            {
                type = reader.NodeType;



                if (reader.Name == "coordinates" && type == XmlNodeType.Element)
                {
                    CountryContour newcontour = new CountryContour();
                    List<Point> contourpoints = new List<Point>();

                    while (reader.MoveToNextAttribute())
                    {
                        if (reader.Name == "type")
                        {
                            newcontour.SetCountryType(reader.Value);
                        }
                    }

                  
                    reader.Read();
                    string text = reader.Value;
                    string[] stringSeparators = new string[] { "\r\n" };
                    string[] words = text.Split(stringSeparators, StringSplitOptions.None);
                    
                    int i = 0;

                    while (i < words.Length)
                    {
                        if (words[i].Length != 0)
                        {
                            string[] numbers = words[i].Split(' ');
                            double latitude = Convert.ToDouble(numbers[0]);
                            double longitude = Convert.ToDouble(numbers[1]);
                            
                            Point newpoint = new Point(latitude, longitude);
                            if (numbers.Length == 3)
                            {
                                double altitude = Convert.ToDouble(numbers[2]);
                                newpoint.SetAltitude(altitude);
                            }

                            contourpoints.Add(newpoint);
                        }
                        i++;

                    }
                    newcontour.SetContour(contourpoints);
                    countrylist.Add(newcontour);
                }

            }
            reader.Close();
            return countrylist;
        }


        public static List<CountryContour> ReadContour(string foldername)
        {
            XmlTextReader reader = new XmlTextReader(foldername);
            XmlNodeType type;

            List<CountryContour> countrylist = new List<CountryContour>();

            while (reader.Read())
            {
                type = reader.NodeType;



                if (reader.Name == "coordinates" && type != XmlNodeType.EndElement)
                {
                    CountryContour newcontour = new CountryContour();


                    while (reader.MoveToNextAttribute())
                    {
                        if (reader.Name == "type")
                        {
                            newcontour.SetCountryType(reader.Value);
                        }
                    }

                    List<Point> contourpoints = new List<Point>();
                    reader.Read();
                    string text = reader.Value;
                    char[] delimitersChars = { ' ' };
                    string[] words = text.Split(delimitersChars);
                    int i = 0;

                    while (i < words.Length)
                    {
                        string[] numbers = words[i].Split(',');
                        double latitude = Convert.ToDouble(numbers[1]);
                        double longitude = Convert.ToDouble(numbers[0]);

                        Point newpoint = new Point(latitude, longitude);


                        contourpoints.Add(newpoint);



                        i++;
                    }

                    newcontour.SetContour(contourpoints);

                    countrylist.Add(newcontour);


                }

            }
            reader.Close();
            return countrylist;
        }

        public static Tuple<TimeSpan, TimeSpan, TimeSpan, double> ReadWorkingHours(string filename)
        {
            TimeSpan initial_time = new TimeSpan();
            TimeSpan final_time = new TimeSpan();
            TimeSpan mean_time = new TimeSpan();
            double variance = new double();

            XmlTextReader reader = new XmlTextReader(filename);
            XmlNodeType type;
            while(reader.Read())
            {
                type = reader.NodeType;
                if(reader.Name.Equals("initial-time") && type==XmlNodeType.Element)
                {
                    reader.Read();
                    initial_time = TimeSpan.Parse(reader.Value);
                }
                if(reader.Name.Equals("final-time") && type==XmlNodeType.Element)
                {
                    reader.Read();
                     final_time = TimeSpan.Parse(reader.Value);
                }
                if(reader.Name.Equals("mean-time") && type==XmlNodeType.Element)
                {
                    reader.Read();
                    mean_time = TimeSpan.Parse(reader.Value);
                }
                if(reader.Name.Equals("variance") && type==XmlNodeType.Element)
                {
                    reader.Read();
                    variance = Convert.ToDouble(reader.Value);
                }
            }

            return new Tuple<TimeSpan, TimeSpan, TimeSpan, double>(initial_time, final_time, mean_time, variance);
        }

        public static List<ForbiddenArea> ReadXmlForbidden(string filename)  // This function reads the xml where the forbidden areas are stored.
        {
            List<ForbiddenArea> ForbiddenAreaList = new List<ForbiddenArea>();
            XmlTextReader reader = new XmlTextReader(filename);
            XmlNodeType type;

            while (reader.Read())
            {
                type = reader.NodeType;

                if (type == XmlNodeType.Element)
                {
                    if (reader.Name == "polygon")
                    {
                        ForbiddenArea newarea = new ForbiddenArea();
                        while (reader.MoveToNextAttribute())
                        {
                            if (reader.Name == "name")
                                newarea.SetName(reader.Value);

                            if (reader.Name == "type")
                            {
                                if(reader.Value.Equals("points"))
                                {
                                    newarea.SetType(ForbiddenAreaType.points);
                                }
                                if (reader.Value.Equals("circle"))
                                {
                                    newarea.SetType(ForbiddenAreaType.circle);
                                }
                                

                                if (reader.Value == "circle")
                                {
                                    while (reader.Name != "reference-point")
                                    { reader.Read(); }

                                    if (reader.Name == "reference-point")
                                    {
                                        reader.Read();
                                        string text = reader.Value;
                                        string[] words = text.Split(' ');
                                        double latitude = Convert.ToDouble(words[0]);
                                        double longitude = Convert.ToDouble(words[1]);
                                        Point newpoint = new Point(latitude, longitude);
                                        newarea.SetReferencePoint(newpoint);
                                    }

                                    while (reader.Name != "radius")
                                    { reader.Read(); }

                                    if (reader.Name == "radius")
                                    {
                                        reader.Read();
                                        double radius = Convert.ToDouble(reader.Value);
                                        newarea.SetRadius(radius);
                                    }

                                    ForbiddenAreaList.Add(newarea);
                                }

                                if (reader.Value == "points")
                                {
                                    List<Point> newlist = new List<Point>();
                                    while (reader.Name != "point")
                                    { reader.Read(); }

                                    while (reader.Name != "polygon")
                                    {

                                        if (reader.Name == "point" && type != XmlNodeType.EndElement)
                                        {
                                            reader.Read();
                                            string text = reader.Value;
                                            string[] words = text.Split(' ');
                                            double latitude = Convert.ToDouble(words[0]);
                                            double longitude = Convert.ToDouble(words[1]);
                                            Point newpoint = new Point(latitude, longitude);
                                            newlist.Add(newpoint);
                                        }
                                        reader.Read();
                                        type = reader.NodeType;
                                    }
                                    Point firstpoint = new Point(newlist[0].GetLatitude(), newlist[0].GetLongitude());   // we end the polygon list with the first point in order to close the polygon since the last point of each polygon is not the point where the polygon starts.
                                    newlist.Add(firstpoint);
                                    newarea.SetPolygon(newlist);

                                }

                                ForbiddenAreaList.Add(newarea);

                            }
                        }


                    }
                }
            }
            reader.Close();
            return ForbiddenAreaList;

        }

        public static List<ForbiddenArea> ReadAirspaceAreas(string filename) // this function reads the xml where the airspace are stored and returns a list of the forbidden areas
        {
            List<ForbiddenArea> forbiddenlist = new List<ForbiddenArea>();
            XmlTextReader reader = new XmlTextReader(filename);
            XmlNodeType type;

            while (reader.Read())
            {
                type = reader.NodeType;

                if (reader.Name == "polygon" && type == XmlNodeType.Element)
                {
                    ForbiddenArea newarea = new ForbiddenArea();
                    newarea.SetType(ForbiddenAreaType.points);
                    while (reader.MoveToNextAttribute())
                    {
                        if (reader.Name == "name")
                            newarea.SetName(reader.Value);

                    }
                    while(reader.Name!="coordinates")
                    { reader.Read(); }

                    if (reader.Name == "coordinates" && type == XmlNodeType.Element)
                    {
                        List<Point> contourpoints = new List<Point>();
                        reader.Read();
                        string text = reader.Value;
                        char[] delimitersChars = { ' ' };
                        string[] words = text.Split(delimitersChars);
                        int i = 0;

                        while (i < words.Length)
                        {
                            string[] numbers = words[i].Split(',');
                            double latitude = Convert.ToDouble(numbers[1]);
                            double longitude = Convert.ToDouble(numbers[0]);

                            Point newpoint = new Point(latitude, longitude);


                            contourpoints.Add(newpoint);



                            i++;
                        }
                        newarea.SetPolygon(contourpoints);
                        forbiddenlist.Add(newarea);
                    }

                }
 
            }
            reader.Close();
            return forbiddenlist;
        }

       
        public static List<City> ReadXmlCities(string filename)
        {
            List<City> citylist = new List<City>();
            XmlTextReader reader = new XmlTextReader(filename);
            XmlNodeType type;

            while (reader.Read())
            {
                type = reader.NodeType;

                if (reader.Name == "city" && type == XmlNodeType.Element)
                {
                    City newcity = new City();
                    while (reader.MoveToNextAttribute())
                    {
                        if (reader.Name == "name")
                            newcity.SetCityName(reader.Value);
                    }

                    while (reader.Name != "coordinates")
                    {
                        reader.Read();
                        type = reader.NodeType;
                    }

                    if (reader.Name == "coordinates" && type == XmlNodeType.Element)
                    {
                        reader.Read();
                        string line = reader.Value;
                        string[] numbers = line.Split(' ');
                        double latitude = Convert.ToDouble(numbers[0]);
                        double longitude = Convert.ToDouble(numbers[1]);
                        Point newpoint = new Point(latitude, longitude);
                        newcity.SetCityRefPoint(newpoint);
                    }
                    citylist.Add(newcity);
                }

            }

            reader.Close();
            return citylist;
        }

        public static List<Airport> ReadXmlAirports(string filename)  // this function reads the xml where the airport data is stored
        {
            List<Airport> airportlist = new List<Airport>();
            XmlTextReader reader = new XmlTextReader(filename);
            XmlNodeType type;

            while (reader.Read())
            {
                type = reader.NodeType;

                if ((reader.Name == "airport") && type == XmlNodeType.Element)
                {
                    Airport newairport = new Airport();
                    while (reader.MoveToNextAttribute())
                    {
                        if (reader.Name == "name")
                            newairport.SetName(reader.Value);

                        if (reader.Name == "icao-name")
                            newairport.SetIcaoName(reader.Value);

                        if (reader.Name == "type")
                            newairport.SetAirportType(reader.Value);

                        if (reader.Name == "military")
                            newairport.SetMilitary(Convert.ToBoolean(reader.Value));

                        if (reader.Name == "heliport")
                            newairport.SetHeliport(Convert.ToBoolean(reader.Value));
                    }

                    while (reader.Name != "reference-point")
                    {
                        reader.Read();
                        type = reader.NodeType;
                    }

                    if ((reader.Name == "reference-point") && type == XmlNodeType.Element)
                    {
                        reader.Read();
                        string line = reader.Value;
                        string[] numbers = line.Split(' ');
                        double latitude = Convert.ToDouble(numbers[0]);
                        double longitude = Convert.ToDouble(numbers[1]);
                        Point newpoint = new Point(latitude, longitude);
                        newairport.SetRefPoint(newpoint);
                    }

                    while (reader.Name != "radius")
                    {
                        reader.Read();
                        type = reader.NodeType;
                    }

                    if ((reader.Name == "radius") && type == XmlNodeType.Element)
                    {
                        reader.Read();
                        newairport.SetRadius(Convert.ToDouble(reader.Value));
                    }

                    airportlist.Add(newairport);
                }
            }
            reader.Close();
            return airportlist;
        }

        // NOT USED: instead it is used the XMLAreaFormats.cs from utm_country
        public static List<Point> ReadDeliveryCenters(string filename) // This class reads the xml where the delivery centers are stored
        {
            List<Point> deliverycenters = new List<Point>();
            XmlTextReader reader = new XmlTextReader(filename);
            XmlNodeType type;

            while (reader.Read())
            {
                type = reader.NodeType;

                if (reader.Name == "center" && type == XmlNodeType.Element)
                {
                    while (reader.Name != "coordinates")
                    {
                        reader.Read();
                        type = reader.NodeType;
                    }

                    if (reader.Name == "coordinates" && type == XmlNodeType.Element)
                    {
                        reader.Read();
                        string line = reader.Value;
                        string[] numbers = line.Split(' ');
                        double latitude = Convert.ToDouble(numbers[0]);
                        double longitude = Convert.ToDouble(numbers[1]);
                        Point newpoint = new Point(latitude, longitude);
                        if (numbers.Length == 3)
                        {
                            double altitude = Convert.ToDouble(numbers[2]);
                            newpoint.SetAltitude(altitude);
                        }
                        deliverycenters.Add(newpoint);
                    }
                }
            }
            reader.Close();
            return deliverycenters;
        }

        public static List<ForbiddenArea> ReadDeliveryForbiddenAreas(string filename)
        {
            List<ForbiddenArea> ForbiddenAreaList = new List<ForbiddenArea>();
            XmlTextReader reader = new XmlTextReader(filename);
            XmlNodeType type;

            while (reader.Read())
            {
                type = reader.NodeType;

                if (type == XmlNodeType.Element)
                {
                    if (reader.Name == "polygon")
                    {
                        ForbiddenArea newarea = new ForbiddenArea();
                        List<Point> points = new List<Point>();
                        while (reader.MoveToNextAttribute())
                        {
                            if (reader.Name == "name")
                                newarea.SetName(reader.Value);

                            if (reader.Name == "type")
                                if (reader.Value.Equals("points"))
                                {
                                    newarea.SetType(ForbiddenAreaType.points);
                                }
                            if (reader.Value.Equals("circle"))
                            {
                                newarea.SetType(ForbiddenAreaType.circle);
                            }
                        }

                        while (reader.Name != "polygon")
                        {

                            if (reader.Name == "point" && type == XmlNodeType.Element)
                            {
                                reader.Read();
                                string text = reader.Value;
                                string[] words = text.Split(' ');
                                double latitude = Convert.ToDouble(words[0]);
                                double longitude = Convert.ToDouble(words[1]);
                                Point newpoint = new Point(latitude, longitude);
                                points.Add(newpoint);
                            }

                            reader.Read();
                            type = reader.NodeType;
                        }
                        Point firstpoint = new Point(points[0].GetLatitude(), points[0].GetLongitude()); // we end the polygon list with the first point in order to close the polygon since the last point of each polygon is not the point where the polygon starts.
                        points.Add(firstpoint);
                        newarea.SetPolygon(points);
                        ForbiddenAreaList.Add(newarea);
                    }

                }
            }
            reader.Close();
            return ForbiddenAreaList;
        }

        

        public static Country LoadCountry(string countryname,string countryfile, string forbiddenfile,string airspacefile, string airportfile, string cityfile, string deliverycentersfile, string deliveryforbiddenfile, string workinghoursfile) // this function reads all the areas.
        {
            String directory = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo di = new DirectoryInfo(directory + "\\Countries\\"+countryname+"\\");
            List<CountryContour> country = ReadNewContour(di+countryfile);
            CountryItem name = FindCountryName(countryname);
            List<ForbiddenArea> forbiddenareas = ReadXmlForbidden(di+forbiddenfile);
            List<ForbiddenArea> controlledairspace = ReadAirspaceAreas(di+airspacefile);
            List<Airport> airports = ReadXmlAirports(di+airportfile);
            List<City> cities = ReadXmlCities(di+cityfile);
            List<Point> deliverycenters = ReadDeliveryCenters(di+deliverycentersfile);
            List<ForbiddenArea> deliveryforbiddenareas = ReadDeliveryForbiddenAreas(di+deliveryforbiddenfile);
            var times = ReadWorkingHours(di + workinghoursfile);
            Country newcountry = new Country(name, country, forbiddenareas, controlledairspace, cities, airports, deliverycenters, deliveryforbiddenareas);
            newcountry.SetTimes(times.Item1, times.Item2, times.Item3, times.Item4);
            return newcountry;

        }


        public static CountryItem FindCountryName(string countryname)
        {
            CountryItem name = new CountryItem();
            if (countryname.Equals("Spain"))
            {
                name = CountryItem.Spain;
            }
            return name;
        }
        


        // writers
        public static void WriteIn(string filename, List<Point> list)  // This function reads a list of points and writes it in a file.
        {
            StreamWriter f1 = new StreamWriter(filename);

            foreach (Point points in list)
            {
                string line = points.GetLatitude() + " " + points.GetLongitude();
                f1.WriteLine(line);
            }
            f1.Close();
        }

        public static void ContourWriter(string filename, List<CountryContour> contour) // This function reads a list of the points which form the contour of the country and writes in a file. 
        {
            StreamWriter f1 = new StreamWriter(filename);

            foreach (CountryContour polygon in contour)
            {
                f1.WriteLine("new polygon type:" + polygon.GetCountryType());
                foreach (Point punto in polygon.GetCountryContour())
                {
                    string line = punto.GetLatitude() + " " + punto.GetLongitude();
                    f1.WriteLine(line);
                }
                f1.WriteLine("\r\n");
            }
            f1.Close();
        }

        public static void VlosWriteIn(string filename, List<VLOSRoute> list)     // this functions writes the vlos points in a file
        {

            StreamWriter f1 = new StreamWriter(filename);
            foreach (VLOSRoute point in list)
            {
                string line = point.GetOriginPoint().GetLatitude() + " " + point.GetOriginPoint().GetLongitude();
                f1.WriteLine(line);
                line = point.GetFirstPoint().GetLatitude() + " " + point.GetFirstPoint().GetLongitude();
                f1.WriteLine(line);
                line = point.GetSecondPoint().GetLatitude() + " " + point.GetSecondPoint().GetLongitude();
                f1.WriteLine(line);
                line = point.GetRectangle().GetFirst().GetLatitude() + " " + point.GetRectangle().GetFirst().GetLongitude();
                f1.WriteLine(line);
                line = point.GetRectangle().GetSecond().GetLatitude() + " " + point.GetRectangle().GetSecond().GetLongitude();
                f1.WriteLine(line);
                line = point.GetRectangle().GetThird().GetLatitude() + " " + point.GetRectangle().GetThird().GetLongitude();
                f1.WriteLine(line);
                line = point.GetRectangle().GetFourth().GetLatitude() + " " + point.GetRectangle().GetFourth().GetLongitude();
                f1.WriteLine(line);
                line = point.GetRectangle().GetFirst().GetLatitude() + " " + point.GetRectangle().GetFirst().GetLongitude();
                f1.WriteLine(line + "\r\n");

            }

            f1.Close();
        }

        public static void BVlosWritein(List<BVLOSRoute> bvloslist, string filename) // This function reads the bvlos points from a list and writes it down in a file
        {
            StreamWriter f1 = new StreamWriter(filename);
            foreach (BVLOSRoute bvlosroute in bvloslist)
            {

                string line = bvlosroute.GetOriginPoint().GetLatitude() + " " + bvlosroute.GetOriginPoint().GetLongitude(); // origin point 
                f1.WriteLine(line);
                line = bvlosroute.GetFirstIntermediate().GetLatitude() + " " + bvlosroute.GetFirstIntermediate().GetLongitude(); // first intermediate point
                f1.WriteLine(line);
                f1.WriteLine(Convert.ToString(bvlosroute.GetBVlosList().Count));

                foreach (BVLOSPoint bvlos in bvlosroute.GetBVlosList())
                {

                    line = bvlos.GetPoint().GetLatitude() + " " + bvlos.GetPoint().GetLongitude(); // the point
                    f1.WriteLine(line);

                    line = bvlos.GetRectangle().GetFirst().GetLatitude() + " " + bvlos.GetRectangle().GetFirst().GetLongitude(); // the rectangle
                    f1.WriteLine(line);
                    line = bvlos.GetRectangle().GetSecond().GetLatitude() + " " + bvlos.GetRectangle().GetSecond().GetLongitude();
                    f1.WriteLine(line);
                    line = bvlos.GetRectangle().GetThird().GetLatitude() + " " + bvlos.GetRectangle().GetThird().GetLongitude();
                    f1.WriteLine(line);
                    line = bvlos.GetRectangle().GetFourth().GetLatitude() + " " + bvlos.GetRectangle().GetFourth().GetLongitude();
                    f1.WriteLine(line);

                }


                line = bvlosroute.GetSecondIntermediate().GetLatitude() + " " + bvlosroute.GetSecondIntermediate().GetLongitude(); // second intermediate point
                f1.WriteLine(line + "\r\n");

            }
            f1.Close();
        }

        public static void ForbiddenWritein(string filename, List<ForbiddenArea> list)
        {

            StreamWriter f1 = new StreamWriter(filename);
            foreach (ForbiddenArea area in list)
            {

                f1.WriteLine(area.GetName());
                f1.WriteLine(area.GetAreaType());

                if (area.GetAreaType() == ForbiddenAreaType.circle)
                {
                    string line = area.GetReferencePoint().GetLatitude() + " " + area.GetReferencePoint().GetLongitude();
                    f1.WriteLine(line);
                    f1.WriteLine(area.GetRadius() + "\r\n");
                }

                if (area.GetAreaType() == ForbiddenAreaType.points)
                {
                    foreach (Point point in area.GetPolygon())
                    {
                        string line = point.GetLatitude() + " " + point.GetLongitude();
                        f1.WriteLine(line);
                    }
                    f1.WriteLine("\r\n");
                }

            }
            f1.Close();
        }

        public static void AirportWriter(string filename, List<Airport> airportlist) // This function reads a list of airports and writes it in a file. 
        {
            StreamWriter f1 = new StreamWriter(filename);

            foreach (Airport airport in airportlist)
            {
                f1.WriteLine("name: " + airport.GetName());
                f1.WriteLine("ICAO-name: " + airport.GetIcaoName());
                f1.WriteLine("type: " + airport.GetAirportType());
                f1.WriteLine("Military: " + airport.GetMilitary());
                f1.WriteLine("Heliport: " + airport.GetHeliport());
                f1.WriteLine("reference point: " + airport.GetRefPoint().GetLatitude() + " " + airport.GetRefPoint().GetLongitude());
                f1.WriteLine("radius(km): " + airport.GetRadius());
                f1.WriteLine("\r\n");
            }
            f1.Close();
        }

        public static void AirportWriter2(string filename, List<Airport> airportlist)
        {

            StreamWriter f1 = new StreamWriter(filename);
            foreach (Airport airport in airportlist)
            {
                f1.WriteLine(airport.GetName());
                f1.WriteLine(airport.GetRefPoint().GetLatitude() + " " + airport.GetRefPoint().GetLongitude());
                f1.WriteLine(airport.GetRadius());
                f1.WriteLine("\n");
            }

            f1.Close();
        }

        public static void CityWriter(string filename, List<City> citylist) // This function reads the a lst of the city and writes it in a file.
        {
            StreamWriter f1 = new StreamWriter(filename);

            foreach (City ciudad in citylist)
            {
                f1.WriteLine(ciudad.GetCityName());
                f1.WriteLine(ciudad.GetCityRefPoint().GetLatitude() + " " + ciudad.GetCityRefPoint().GetLongitude());
                f1.WriteLine("\r\n");
            }
            f1.Close();

        }

        public static int CheckForbiddenArea(List<ForbiddenArea> forbiddenlist)
        {
            int n=0;
            foreach(ForbiddenArea area in forbiddenlist)
            {
                if (area.GetPolygon()[0].GetLatitude() == area.GetPolygon()[area.GetPolygon().Count - 1].GetLatitude() && area.GetPolygon()[0].GetLongitude() == area.GetPolygon()[area.GetPolygon().Count - 1].GetLongitude())
                {
                    n++;
                }
            }
            return n;
        }

        public static bool DeleteFile(string filename,string foldername) // says whether the file is deleted or not
        {
            bool done=false;
            //String directory = AppDomain.CurrentDomain.BaseDirectory;
            //System.IO.DirectoryInfo di = new DirectoryInfo(directory + "\\" + foldername + "\\");
            System.IO.DirectoryInfo di = new DirectoryInfo(foldername);

            if (di.Exists)  // if it doesn't exist, create
            {
                File.Delete(di +"\\"+ filename);
                done=true;
            }
                
            return done;
        }


       

        


    }
}
