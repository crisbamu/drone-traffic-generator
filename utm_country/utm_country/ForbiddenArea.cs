using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using utm_utils;

namespace utm_country
{
    public class ForbiddenArea
    {
        // Written by Lluís Xavier Herranz on 10/30/2016

        //Attributes
        private string name;
        private ForbiddenAreaType type;
        private List<Point> polygon;
        private Point referencePoint;
        private double radius;

        // Builders

        public ForbiddenArea()
        {
            
        }

        public ForbiddenArea(string a, ForbiddenAreaType b, List<Point> c, Point f, double d)
        {
            this.name = a;
            this.type = b;
            this.polygon = c;
            this.referencePoint = f;
            this.radius = d;
        }

        // Setters and getters

        public void SetName(string a)
        { this.name = a; }

        public void SetType(ForbiddenAreaType a)
        { this.type = a; }

        public void SetPolygon(List<Point> a)
        { this.polygon = a; }

        public void SetReferencePoint(Point a)
        { this.referencePoint = a; }

        public void SetRadius(double a)
        { this.radius = a; }

        public string GetName()
        { return this.name; }

        public ForbiddenAreaType GetAreaType()
        { return this.type; }

        public List<Point> GetPolygon()
        { return this.polygon; }

        public Point GetReferencePoint()
        { return this.referencePoint; }

        public double GetRadius()
        { return this.radius; }

    }
}
