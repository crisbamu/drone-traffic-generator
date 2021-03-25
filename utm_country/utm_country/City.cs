using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using utm_utils;

namespace utm_country
{
    public class City
    {
        // Written on 12/11/2016

        // Attributes

        private string name;
        private Point referencePoint;
        private double radius;

        // builders

        public City()
        {

        }

        public City(string a, Point b, double c)
        {
            this.name = a;
            this.referencePoint = b;
            this.radius = c;
        }

        // setters and getters

        public void SetCityName(string a)
        { this.name = a; }

        public void SetCityRefPoint(Point a)
        { this.referencePoint = a; }

        public void SetCityRadius(double a)
        {
            this.radius = a;
        }

        public string GetCityName()
        { return this.name; }

        public Point GetCityRefPoint()
        { return this.referencePoint; }

        public double GetCityRadius()
        {
            return this.radius;
        }
    }
}
