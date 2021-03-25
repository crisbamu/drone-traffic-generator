using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using utm_country;
using utm_utils;
using utm_drone;

namespace utm_routes.BVLOS_routes
{
    public class BVLOSPoint
    {
        // Written by Lluís Xavier Herranz on 12/12/2016

        //Attributes

        private Point Point; // this is a point where the drone does the operation.
        private Rectangle Rectangle; // this rectangle shows the operation area.


        // builders

        public BVLOSPoint()
        {

        }

        public BVLOSPoint(Point point, Rectangle rectangle)
        {

            this.Point = point;
            this.Rectangle = rectangle;
        }

        // setters and getters


        public void SetPoint(Point first)
        {
            this.Point = first;
        }

        public void SetRectangle(Rectangle first)
        {
            this.Rectangle = first;
        }

        public Point GetPoint()
        {
            return this.Point;
        }

        public Rectangle GetRectangle()
        {
            return this.Rectangle;
        }

        public BVLOSPoint GetCopy()// this function returns a copy of this bvlospoint
        {
            Point punto = this.Point.GetCopy();
            Rectangle rect = this.Rectangle.GetCopy();
            BVLOSPoint newpoint = new BVLOSPoint(punto, rect);
            return newpoint;

        }
        
    }
}
