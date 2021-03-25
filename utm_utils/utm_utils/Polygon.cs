using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace utm_utils
{
    public class Polygon
    {
        // Written by Lluís Xavier Herranz on 10/30/2016

        // attributes

        private List<Point> polygonlist;

        // builders
        public Polygon()
        {
            
        }

        public Polygon(List<Point> list)
        {
            this.polygonlist = list;
        }


        // functions

        // get the centroid of the polygon

        public Point GetCentroid()
        {
            // Add the first point at the end of the array.
            int NumOfElements = polygonlist.Count;
            Point[] pts = new Point[NumOfElements + 1];
            polygonlist.CopyTo(pts, 0);
            pts[NumOfElements] = polygonlist[0];

            // Find the centroid.
            double X = 0;
            double Y = 0;
            double second_factor;
            for (int i = 0; i < NumOfElements; i++)
            {
                second_factor =
                    pts[i].GetLongitude() * pts[i + 1].GetLatitude() -
                    pts[i + 1].GetLongitude() * pts[i].GetLatitude();
                X += (pts[i].GetLongitude() + pts[i + 1].GetLongitude()) * second_factor;
                Y += (pts[i].GetLatitude() + pts[i + 1].GetLatitude()) * second_factor;
            }

            // Divide by 6 times the polygon's area.
            double polygon_area = PolygonArea();
            X /= (6 * polygon_area);
            Y /= (6 * polygon_area);

            // If the values are negative, the polygon is
            // oriented counterclockwise so reverse the signs.
            if (Y < 0)
            {
                X = -X;
                Y = -Y;
            }

            return new Point(Y, X);
        }


        #region "Area Routines"
        // Return the polygon's area in "square units."
        // Add the areas of the trapezoids defined by the
        // polygon's edges dropped to the X-axis. When the
        // program considers a bottom edge of a polygon, the
        // calculation gives a negative area so the space
        // between the polygon and the axis is subtracted,
        // leaving the polygon's area. This method gives odd
        // results for non-simple polygons.
        public double PolygonArea()
        {
            // Return the absolute value of the signed area.
            // The signed area is negative if the polygon is
            // oriented clockwise.
            return Math.Abs(SignedPolygonArea());
        }

        // Return the polygon's area in "square units."
        // Add the areas of the trapezoids defined by the
        // polygon's edges dropped to the X-axis. When the
        // program considers a bottom edge of a polygon, the
        // calculation gives a negative area so the space
        // between the polygon and the axis is subtracted,
        // leaving the polygon's area. This method gives odd
        // results for non-simple polygons.
        //
        // The value will be negative if the polygon is
        // oriented clockwise.
        private double SignedPolygonArea()
        {
            // Add the first point to the end.
            int num_points = polygonlist.Count;
            Point[] pts = new Point[num_points + 1];
            polygonlist.CopyTo(pts, 0);
            pts[num_points] = polygonlist[0];

            // Get the areas.
            double area = 0;
            for (int i = 0; i < num_points; i++)
            {
                area +=
                    (pts[i + 1].GetLongitude() - pts[i].GetLongitude()) *
                    (pts[i + 1].GetLatitude() + pts[i].GetLatitude()) / 2;
            }

            // Return the result.
            return area;
        }
        #endregion // Area Routines



        public double FindMinimumDistance(Point centroid) // This functions returns the minimum distance to the centroid from a list of points
        {

            double mindistance = LatLongProjection.HaversineDistance(polygonlist[0], centroid);

            //double mindistance = Math.Sqrt(Math.Pow(polygonlist[0].GetLatitude() - centroid.GetLatitude(), 2) + (Math.Pow(polygonlist[0].GetLongitude() - centroid.GetLongitude(), 2)));
            foreach (Point punto in polygonlist)
            {
                double distance = LatLongProjection.HaversineDistance(punto, centroid);
                //double distance = Math.Sqrt(Math.Pow(punto.GetLatitude() - centroid.GetLatitude(), 2) + (Math.Pow(punto.GetLongitude() - centroid.GetLongitude(), 2)));

                if (distance < mindistance)
                    mindistance = distance;
            }

            return mindistance;

        }
    }
}
