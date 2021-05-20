using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace utm_utils
{
    public class Point
    {
        // Written by Lluís Xavier Herranz on 10/30/2016

        // Attributes
        private double latitude;
        private double longitude;
        private double altitude;
        private DateTime time;
        private String operation;
        private String color;
        private double nextlatitude;
        private double nextlongitude;
        private double nextaltitude;

        // builder

        public Point()
        {
        }

        public Point(double a, double b)
        {
            this.latitude = a;
            this.longitude = b;
            this.altitude = 0.0;
            this.nextlatitude = 0.0;
            this.nextlongitude = 0.0;
            this.nextaltitude = 0.0;
            this.time = DateTime.Today;
            this.operation = "";
            this.color = "";
        }

        public Point(double a, double b, double c)
        {
            this.latitude = a;
            this.longitude = b;
            this.altitude = c;
            this.nextlatitude = 0.0;
            this.nextlongitude = 0.0;
            this.nextaltitude = 0.0;
            this.time = DateTime.Today; ;
            this.operation = "";
            this.color = "";
        }

        public Point(double a, double b, double c, DateTime d)
        {
            this.latitude = a;
            this.longitude = b;
            this.altitude = c;
            this.nextlatitude = 0.0;
            this.nextlongitude = 0.0;
            this.nextaltitude = 0.0;
            this.time = d;
            this.operation = "";
            this.color = "";
        }
        // Setters and Getters
        public void SetLatitude(double a)
        { 
            this.latitude = a;
        }

        public void SetLongitude(double b)
        { 
            this.longitude = b; 
        }

        public void SetAltitude(double a)
        {
            this.altitude = a;
        }
        public void SetTime(DateTime a)
        {
            this.time = a;
        }
        public void SetOperation(String op)
        {
            this.operation = op;
        }
        public void SetColor(String col)
        {
            this.color = col;
        }
        public void SetNextLatitude(double a)
        {
            this.nextlatitude = a;
        }

        public void SetNextLongitude(double b)
        {
            this.nextlongitude = b;
        }

        public void SetNextAltitude(double a)
        {
            this.nextaltitude = a;
        }
        public double GetLatitude()
        {
            return this.latitude; 
        }

        public double GetLongitude()
        { 
            return this.longitude; 
        }

        public double GetAltitude()
        {
            return this.altitude;
        }

        public DateTime GetTime()
        {
            return this.time;
        }
        public String GetOperation()
        {
            return this.operation;
        }
        public String GetColor()
        {
            return this.color;
        }
        public double GetNextLatitude()
        {
            return this.nextlatitude;
        }

        public double GetNextLongitude()
        {
            return this.nextlongitude;
        }

        public double GetNextAltitude()
        {
            return this.nextaltitude;
        }

        public Point GetCopy()
        {
            Point newpoint = new Point();
            newpoint.SetLatitude(this.latitude);
            newpoint.SetLongitude(this.longitude);
            newpoint.SetAltitude(this.altitude);
            newpoint.SetTime(this.time);
            newpoint.SetOperation(this.operation);
            newpoint.SetColor(this.color);
            newpoint.SetNextLatitude(this.nextlatitude);
            newpoint.SetNextLongitude(this.nextlongitude);
            newpoint.SetNextAltitude(this.nextaltitude);
            return newpoint;
        }
    }
}
