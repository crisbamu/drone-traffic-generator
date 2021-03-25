using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using utm_utils;
using System.IO;

namespace utm_country
{
    public class Country
    {
        //Written by Lluís Xavier Herranz on 10/30/2016

        // Attributes

        private CountryItem name;

        private List<CountryContour> contours;
        private List<ForbiddenArea> forbiddenareas;
        private List<ForbiddenArea> ControlledAirspace;
        private List<City> cities;
        private List<Airport> airports;
        private List<Point> deliverycenters;
        private List<ForbiddenArea> deliveryforbiddenareas;

        // working hours
        private TimeSpan Initial_time;
        private TimeSpan Final_time;
        private TimeSpan Mean_time;
        private double variance;

        // availability of forbidden flight zones
        bool ForbiddenAreasAvailability;
        bool ControlledAirspaceAvailability;
        bool CitiesAvailability;
        bool AirportsAvailability;

        // builders
        public Country()
        {

        }

        public Country(CountryItem nameofcountry ,List<CountryContour> cont, List<ForbiddenArea> forbidden, List<ForbiddenArea> airspace, List<City> city, List<Airport> air, List<Point> centers, List<ForbiddenArea> deliveryforbidden)
        {
            this.name = nameofcountry;
            this.contours = cont;
            this.forbiddenareas = forbidden;
            this.ControlledAirspace = airspace;
            this.cities = city;
            this.airports = air;
            this.deliverycenters = centers;
            this.deliveryforbiddenareas = deliveryforbidden;
        }

        public Country(TimeSpan initial, TimeSpan final, TimeSpan mean, double var)
        {
            this.Initial_time = initial;
            this.Final_time = final;
            this.Mean_time = mean;
            this.variance = var;
        }

        


        // setters and getters

        public void SetCountryName(CountryItem nameofthecountry)
        {
            this.name = nameofthecountry;
        }

        public void SetContours(List<CountryContour> a)
        {
            this.contours = a;
        }

        public void SetForbiddenAreas(List<ForbiddenArea> a)
        {
            this.forbiddenareas = a;
        }

        public void SetControlledAirspace(List<ForbiddenArea> a)
        {
            this.ControlledAirspace = a;
        }

        public void SetCities(List<City> a)
        {
            this.cities = a;
        }

        public void SetAirports(List<Airport> a)
        {
            this.airports = a;
        }

        public void SetDeliveryCenters(List<Point> a)
        {
            this.deliverycenters = a;
        }

        public void SetDeliveryForbiddenAreas(List<ForbiddenArea> a)
        {
            this.deliveryforbiddenareas = a;
        }

        public void SetInitialTime(TimeSpan initial)
        {
            this.Initial_time = initial;
        }

        public void SetFinalTime(TimeSpan final)
        {
            this.Final_time = final;
        }

        public void SetMeanTime(TimeSpan mean)
        {
            this.Mean_time = mean;
        }

        public void SetVariance(double var)
        {
            this.variance = var;
        }

        public CountryItem GetCountryName()
        {
            return this.name;
        }

        public List<CountryContour> GetContours()
        {
            return this.contours;
        }

        public List<ForbiddenArea> GetForbiddenAreas()
        {
            return this.forbiddenareas;
        }

        public List<ForbiddenArea> GetControlledAirspace()
        {
            return this.ControlledAirspace;
        }

        public List<City> GetCities()
        {
            return this.cities;
        }

        public List<Airport> GetAirports()
        {
            return this.airports;
        }

        public List<Point> GetDeliveryCenters()
        {
            return this.deliverycenters;
        }

        public List<ForbiddenArea> GetDeliveryForbiddenAreas()
        {
            return this.deliveryforbiddenareas;
        }

        public TimeSpan GetInitialTime()
        {
            return this.Initial_time;
        }

        public TimeSpan GetFinalTime()
        {
            return this.Final_time;
        }

        public TimeSpan GetMeanTime()
        {
            return this.Mean_time;
        }

        public double GetVariance()
        {
            return this.variance;
        }

        public bool GetForbiddenAreasAvailability()
        {
            return this.ForbiddenAreasAvailability;
        }

        public bool GetControlledAirspaceAvailability()
        {
            return this.ControlledAirspaceAvailability;
        }

        public bool GetCitiesAvailability()
        {
            return this.CitiesAvailability;
        }

        public bool GetAirportsAvailability()
        {
            return this.AirportsAvailability;
        }

       

        // function

        public void SetAvailability(bool forbidden_areas_availability, bool controlled_airspace_availability, bool cities_availability, bool airports_availability)
        {
            this.ForbiddenAreasAvailability= forbidden_areas_availability;
            this.ControlledAirspaceAvailability = controlled_airspace_availability;
            this.CitiesAvailability = cities_availability;
            this.AirportsAvailability = airports_availability;
        }

        public void SetTimes(TimeSpan initial, TimeSpan final, TimeSpan mean, double var)
        {
            this.Initial_time = initial;
            this.Final_time = final;
            this.Mean_time = mean;
            this.variance = var;
        }

        public CountryContour GetLandside() // this functions returns the landside from the contour of the country
        {
            CountryContour landside = new CountryContour();
            foreach(CountryContour contour in this.contours)
            {
              if(contour.GetCountryType()=="land")
              {
                  landside = contour;
                  break;
              }
            }
            return landside;
        }

        public void SetLandside(List<Point> a)
        {
            foreach (CountryContour contour in this.contours)
            {
                if (contour.GetCountryType() == "land")
                {
                    contour.SetContour(a);
                    break;
                }
            }
        }

        public List<double> GetLatitudeContourn(CountryContour contourn)
        {
            List<double> latitudes = new List<double>();
            foreach(Point punto in contourn.GetCountryContour())
            {
                latitudes.Add(punto.GetLatitude());
            }
            return latitudes;
        }

        public List<double> GetLongitudeContourn(CountryContour contourn)
        {
            List<double> longitudes = new List<double>();
            foreach (Point punto in contourn.GetCountryContour())
            {
                longitudes.Add(punto.GetLongitude());
            }
            return longitudes;
        }

        

        

    }
}
