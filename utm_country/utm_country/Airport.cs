using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using utm_utils;

namespace utm_country
{
    public class Airport
    {
        // Written by Lluís Xavier Herranz on 10/30/2016

        // Attributes
        private string name;
        private string type;
        private string IcaoName;
        private bool military;
        private bool heliport;
        private Point referencePoint;
        private double radius;

        // builder

        public Airport()
        {
            this.name = null;
            this.type = null;
            this.IcaoName = null;
            this.military = false;
            this.heliport = false;
            this.referencePoint = null;

            this.radius = 0;
        }

        public Airport(string a, string b, string icaoname, bool army, bool heli, Point refpoint, double r)
        {
            this.name = a;
            this.type = b;
            this.IcaoName = icaoname;
            this.military = army;
            this.heliport = heli;
            this.referencePoint = refpoint;
            this.radius = r;
        }

        //setters

        public void SetName(string a)
        { this.name = a; }

        public void SetAirportType(string a)
        { this.type = a; }

        public void SetIcaoName(string a)
        { this.IcaoName = a; }

        public void SetMilitary(bool a)
        { this.military = a; }

        public void SetHeliport(bool a)
        { this.heliport = a; }

        public void SetRefPoint(Point a)
        { this.referencePoint = a; }


        public void SetRadius(double a)
        { this.radius = a; }

        // getters

        public string GetName()
        { return this.name; }

        public string GetAirportType()
        { return this.type; }

        public string GetIcaoName()
        { return this.IcaoName; }

        public bool GetMilitary()
        { return this.military; }

        public bool GetHeliport()
        { return this.heliport; }

        public Point GetRefPoint()
        { return this.referencePoint; }

        public double GetRadius()
        { return this.radius; }


    }
}
