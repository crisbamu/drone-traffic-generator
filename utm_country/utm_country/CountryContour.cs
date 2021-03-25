using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using utm_utils;

namespace utm_country
{
    public class CountryContour
    {
        // Written by Lluís Xavier Herranz on 12/9/2016

        // This class stores the polygon which forms the country contour.

        // Attributes

        private List<Point> contour;
        private string type;


        // builders

        public CountryContour()
        {

        }

        public CountryContour(List<Point> points, string a)
        {
            this.contour = points;
            this.type = a;

        }

        // setters and getters

        public void SetContour(List<Point> points)
        { this.contour = points; }

        public void SetCountryType(string a)
        {
            this.type = a;
        }

        public List<Point> GetCountryContour()
        { return this.contour; }

        public string GetCountryType()
        { return this.type; }



        
    
    }
}
