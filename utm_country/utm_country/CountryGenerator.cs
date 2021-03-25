using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Xml;
using utm_utils;

namespace utm_country
{
    public static class CountryGenerator
    {
        //Attributes

        static XMLAreasFormat reader = new XMLAreasFormat();

       // function

        public static Country LoadCountry(string countryname, string countryfile, string forbiddenfile, string airspacefile, string airportfile, string cityfile, string deliverycentersfile, string deliveryforbiddenfile, string workinghoursfile)
        {
            String directory = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo di = new DirectoryInfo(directory + "\\Countries\\"+countryname+"\\");
            List<CountryContour> contour = reader.ReadContoursFromXML(di+countryfile);

            CountryItem name = FindCountryName(countryname);
            List<ForbiddenArea> forbiddenareas = reader.ReadForbiddenAreasFromXML(di + forbiddenfile);
            List<ForbiddenArea> controlledairspace = reader.ReadForbiddenAreasFromXML(di + airspacefile);
            List<Airport> airports = ReadXmlAirports(di + airportfile);
            List<City> cities = ReadXmlCities(di + cityfile);
            List<Point> deliverycenters = ReadDeliveryCenters(di + deliverycentersfile);
            List<ForbiddenArea> deliveryforbiddenareas = reader.ReadForbiddenAreasFromXML(di + deliveryforbiddenfile);
            var times = ReadWorkingHours(di + workinghoursfile);
            Country newcountry = new Country(name, contour, forbiddenareas, controlledairspace, cities, airports, deliverycenters, deliveryforbiddenareas);
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
                        string[] numbers = line.Split(',');
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
                        string[] numbers = line.Split(',');
                        double latitude = Convert.ToDouble(numbers[0]);
                        double longitude = Convert.ToDouble(numbers[1]);
                        Point newpoint = new Point(latitude, longitude);
                        newcity.SetCityRefPoint(newpoint);
                    }
                    while (reader.Name != "radius")
                    {
                        reader.Read();
                        type = reader.NodeType;
                    }
                    if (reader.Name == "radius" && type == XmlNodeType.Element)
                    {
                        reader.Read();
                        newcity.SetCityRadius(Convert.ToDouble(reader.Value));
                    }
                    citylist.Add(newcity);
                }

            }

            reader.Close();
            return citylist;
        }

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
                        string[] numbers = line.Split(',');
                        double latitude = Convert.ToDouble(numbers[0]);
                        double longitude = Convert.ToDouble(numbers[1]);
                        Point newpoint = new Point(latitude, longitude);
                        if (numbers.Length == 3)
                            newpoint.SetAltitude(Convert.ToDouble(numbers[2]));
                        deliverycenters.Add(newpoint);
                    }
                }
            }
            reader.Close();
            return deliverycenters;
        }

        public static Tuple<TimeSpan, TimeSpan, TimeSpan, double> ReadWorkingHours(string filename)
        {
            TimeSpan initial_time = new TimeSpan();
            TimeSpan final_time = new TimeSpan();
            TimeSpan mean_time = new TimeSpan();
            double variance = new double();

            XmlTextReader reader = new XmlTextReader(filename);
            XmlNodeType type;
            while (reader.Read())
            {
                type = reader.NodeType;
                if (reader.Name.Equals("initial-time") && type == XmlNodeType.Element)
                {
                    reader.Read();
                    initial_time = TimeSpan.Parse(reader.Value);
                }
                if (reader.Name.Equals("final-time") && type == XmlNodeType.Element)
                {
                    reader.Read();
                    final_time = TimeSpan.Parse(reader.Value);
                }
                if (reader.Name.Equals("mean-time") && type == XmlNodeType.Element)
                {
                    reader.Read();
                    mean_time = TimeSpan.Parse(reader.Value);
                }
                if (reader.Name.Equals("standard-deviation") && type == XmlNodeType.Element)
                {
                    reader.Read();
                    variance = Convert.ToDouble(reader.Value);
                }
            }

            return new Tuple<TimeSpan, TimeSpan, TimeSpan, double>(initial_time, final_time, mean_time, variance);
        }

        
    }
}
