using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace utm_utils
{
    public class Rectangle
    {
        // Written by Lluís Xavier Herranz Bareas on 11/25/2016

        // This class define the 4 points that form a rectangle

        // Attributes

        private Point First;
        private Point Second;
        private Point Third;
        private Point Fourth;

        private List<Point> Scan;

        // builders

        public Rectangle()
        {

        }

        public Rectangle(Point a, Point b, Point c, Point d)
        {
            this.First = a;
            this.Second = b;
            this.Third = c;
            this.Fourth = d;
        }

        // setters and getters

        public void SetFirst(Point a)
        {
            this.First = a;
        }

        public void SetSecond(Point a)
        {
            this.Second = a;
        }

        public void SetThird(Point a)
        {
            this.Third = a;
        }

        public void SetFourth(Point a)
        {
            this.Fourth = a;
        }

        public void SetScan(List<Point> a)
        {
            this.Scan = a;
        }
  

        public Point GetFirst()
        { return this.First; }

        public Point GetSecond()
        { return this.Second; }

        public Point GetThird()
        { return this.Third; }

        public Point GetFourth()
        { return this.Fourth; }

        public List<Point> GetScan()
        {
            return this.Scan;
        }

        // functions

        public Rectangle CreateRectangle(Point centerpoint, double d1, double d2, double angle) // This function creates a rectangle
        {    
            double d = Math.Sqrt(Math.Pow(d1 / 2, 2) + Math.Pow(d2 / 2, 2));

            Point firstrectanglepoint = LatLongProjection.DestinyPoint(centerpoint, d, angle);

            double alpha = 2 * Math.Atan(d2 / d1);
            angle = angle + alpha;

            Point secondrectanglepoint = LatLongProjection.DestinyPoint(centerpoint, d, angle);

            double beta = 2 * Math.Atan(d1 / d2);
            angle = angle + beta;

            Point thirdrectanglepoint = LatLongProjection.DestinyPoint(centerpoint, d, angle);

            angle = angle + alpha;

            Point fourthrectanglepoint = LatLongProjection.DestinyPoint(centerpoint, d, angle);

            Rectangle newrectangle = new Rectangle(firstrectanglepoint, secondrectanglepoint, thirdrectanglepoint, fourthrectanglepoint); // here we create the rectangle
            

            return newrectangle;
        }

        public List<Point> MakeScan(Rectangle rect, double scanwidth) // This function creates a scan with the rectangle as a parameter
        {
            List<Point> scanpoints = new List<Point>();

            double distancey = LatLongProjection.HaversineDistance(rect.GetSecond(), rect.GetThird());
            double angley = LatLongProjection.GetBearing(rect.GetSecond(), rect.GetThird());
            double deltadistancey = scanwidth;

            double distancex = LatLongProjection.HaversineDistance(rect.GetFirst(), rect.GetSecond());
            double anglex = LatLongProjection.GetBearing(rect.GetFirst(), rect.GetSecond());
            double deltadistancex = distancex / 10;

            Point lastpoint = rect.GetFirst();
            double lastangle = anglex;

            double dy = 0;
            while (dy <= distancey)
            {

                double d = 0;
                
                while (d <= distancex)
                {
                    Point newpoint = LatLongProjection.DestinyPoint(lastpoint, deltadistancex, lastangle);
                    scanpoints.Add(newpoint);
                    d = d + deltadistancex;
                    lastpoint = newpoint;
                }

                lastpoint = LatLongProjection.DestinyPoint(lastpoint, deltadistancey, angley);
                
                lastangle = lastangle + Math.PI;
                dy = dy + deltadistancey;

                if(!(dy>distancey))
                scanpoints.Add(lastpoint);
            }
               
            return scanpoints;

        }

        public int GetOptimumScan(Rectangle rect, Point punto) // this functions finds the minimum distance from the Point punto to the points which form the rectangle
        {
            double shortestdistance = LatLongProjection.HaversineDistance(punto, rect.GetFirst());
            int best = 1;

           double  newdistance = LatLongProjection.HaversineDistance(punto, rect.GetSecond());
           if (newdistance < shortestdistance)
           {
               shortestdistance = newdistance;
               best = 2;
           }

           newdistance = LatLongProjection.HaversineDistance(punto, rect.GetThird());
           if (newdistance < shortestdistance)
           {
               shortestdistance = newdistance;
               best = 3;
           }

           newdistance = LatLongProjection.HaversineDistance(punto, rect.GetFourth());
           if (newdistance < shortestdistance)
           {
               shortestdistance = newdistance;
               best = 4;
           }

           return best;
            

        }

        public List<Point> MakeReducedOptimumScan(Rectangle rect, double scanwidth, int closest, double height)
        {
            List<Point> scanpoints = new List<Point>();

            double distancey = 0;
            double angley = 0;
            double deltadistancey = 0;

            double distancex = 0;
            double anglex = 0;
            double deltadistancex = 0;

            Point lastpoint = new Point();
            double lastangle = 0;

            if (closest == 1)
            {
                distancey = LatLongProjection.HaversineDistance(rect.GetSecond(), rect.GetThird());
                angley = LatLongProjection.GetBearing(rect.GetSecond(), rect.GetThird());
                deltadistancey = scanwidth;

                distancex = LatLongProjection.HaversineDistance(rect.GetFirst(), rect.GetSecond());
                anglex = LatLongProjection.GetBearing(rect.GetFirst(), rect.GetSecond());
                deltadistancex = distancex / 10;

                lastpoint = rect.GetFirst();
                lastpoint.SetAltitude(height);
                lastangle = anglex;
            }

            if (closest == 2)
            {
                distancey = LatLongProjection.HaversineDistance(rect.GetThird(), rect.GetFourth());
                angley = LatLongProjection.GetBearing(rect.GetThird(), rect.GetFourth());
                deltadistancey = scanwidth;

                distancex = LatLongProjection.HaversineDistance(rect.GetSecond(), rect.GetThird());
                anglex = LatLongProjection.GetBearing(rect.GetSecond(), rect.GetThird());
                deltadistancex = distancex / 10;

                lastpoint = rect.GetFirst();
                lastpoint.SetAltitude(height);
                lastangle = anglex;
            }

            if (closest == 3)
            {
                distancey = LatLongProjection.HaversineDistance(rect.GetFourth(), rect.GetFirst());
                angley = LatLongProjection.GetBearing(rect.GetFourth(), rect.GetFirst());
                deltadistancey = scanwidth;

                distancex = LatLongProjection.HaversineDistance(rect.GetThird(), rect.GetFourth());
                anglex = LatLongProjection.GetBearing(rect.GetThird(), rect.GetFourth());
                deltadistancex = distancex / 10;

                lastpoint = rect.GetFirst();
                lastpoint.SetAltitude(height);
                lastangle = anglex;
            }

            if (closest == 4)
            {
                distancey = LatLongProjection.HaversineDistance(rect.GetFirst(), rect.GetSecond());
                angley = LatLongProjection.GetBearing(rect.GetFirst(), rect.GetSecond());
                deltadistancey = scanwidth;

                distancex = LatLongProjection.HaversineDistance(rect.GetFourth(), rect.GetFirst());
                anglex = LatLongProjection.GetBearing(rect.GetFourth(), rect.GetFirst());
                deltadistancex = distancex / 10;

                lastpoint = rect.GetFirst();
                lastpoint.SetAltitude(height);
                lastangle = anglex;
            }

            scanpoints.Add(lastpoint);
            double dy = 0;
            while (dy < distancey)
            {
                Point newpoint = LatLongProjection.DestinyPoint(lastpoint, distancex, lastangle);
                newpoint.SetAltitude(height);
                scanpoints.Add(newpoint);
                lastpoint = newpoint.GetCopy();

                lastpoint = LatLongProjection.DestinyPoint(lastpoint, deltadistancey, angley);
                lastpoint.SetAltitude(height);
                lastangle = lastangle + Math.PI;
                dy = dy + deltadistancey;

                if (!(dy >= distancey))
                    scanpoints.Add(lastpoint);
            }


            return scanpoints;

        }

        public List<Point>MakeOptimumScan(Rectangle rect,double scanwidth, int closest)
        {
            List<Point> scanpoints = new List<Point>();

            double distancey=0;
            double angley=0;
            double deltadistancey=0;

            double distancex=0;
            double anglex=0;
            double deltadistancex=0;

            Point lastpoint = new Point();
            double lastangle =0;

            if (closest == 1)
            {
                distancey = LatLongProjection.HaversineDistance(rect.GetSecond(), rect.GetThird());
                angley = LatLongProjection.GetBearing(rect.GetSecond(), rect.GetThird());
                deltadistancey = scanwidth;

                distancex = LatLongProjection.HaversineDistance(rect.GetFirst(), rect.GetSecond());
                anglex = LatLongProjection.GetBearing(rect.GetFirst(), rect.GetSecond());
                deltadistancex = distancex / 10;

                lastpoint = rect.GetFirst();
                lastangle = anglex;
            }

            if (closest == 2)
            {
                distancey = LatLongProjection.HaversineDistance(rect.GetThird(), rect.GetFourth());
                angley = LatLongProjection.GetBearing(rect.GetThird(), rect.GetFourth());
                deltadistancey = scanwidth;

                distancex = LatLongProjection.HaversineDistance(rect.GetSecond(), rect.GetThird());
                anglex = LatLongProjection.GetBearing(rect.GetSecond(), rect.GetThird());
                deltadistancex = distancex / 10;

                lastpoint = rect.GetFirst();
                lastangle = anglex;
            }

            if (closest == 3)
            {
                distancey = LatLongProjection.HaversineDistance(rect.GetFourth(), rect.GetFirst());
                angley = LatLongProjection.GetBearing(rect.GetFourth(), rect.GetFirst());
                deltadistancey = scanwidth;

                distancex = LatLongProjection.HaversineDistance(rect.GetThird(), rect.GetFourth());
                anglex = LatLongProjection.GetBearing(rect.GetThird(), rect.GetFourth());
                deltadistancex = distancex / 10;

                lastpoint = rect.GetFirst();
                lastangle = anglex;
            }

            if (closest == 4)
            {
                distancey = LatLongProjection.HaversineDistance(rect.GetFirst(), rect.GetSecond());
                angley = LatLongProjection.GetBearing(rect.GetFirst(), rect.GetSecond());
                deltadistancey = scanwidth;

                distancex = LatLongProjection.HaversineDistance(rect.GetFourth(), rect.GetFirst());
                anglex = LatLongProjection.GetBearing(rect.GetFourth(), rect.GetFirst());
                deltadistancex = distancex / 10;

                lastpoint = rect.GetFirst();
                lastangle = anglex;
            }


            scanpoints.Add(lastpoint);
            double dy = 0;
            while (dy < distancey)
            {

                double d = 0;

                while (d < distancex)
                {
                    Point newpoint = LatLongProjection.DestinyPoint(lastpoint, deltadistancex, lastangle);
                    scanpoints.Add(newpoint);
                    d = d + deltadistancex;
                    lastpoint = newpoint;
                }

                lastpoint = LatLongProjection.DestinyPoint(lastpoint, deltadistancey, angley);

                lastangle = lastangle + Math.PI;
                dy = dy + deltadistancey;

                if (!(dy >= distancey))
                scanpoints.Add(lastpoint);
            }
            

            return scanpoints;

        }



        public List<Point> AddTimeToScan(List<Point> scan, DateTime actual_time, double velocity) // This function creates a scan with the rectangle as a parameter
        {      
            int i = 0;
            DateTime time = actual_time;
            scan[0].SetTime(time);

            while(i<scan.Count-1)
            {
                double delta_distance = LatLongProjection.HaversineDistance(scan[i], scan[i + 1]);
                double height_distance = scan[i + 1].GetAltitude() - scan[i].GetAltitude();
                double total_distance = Math.Sqrt(Math.Pow(delta_distance,2)+Math.Pow(height_distance,2));
                double deltatime = total_distance / velocity;
                time = time + TimeSpan.FromSeconds(deltatime);
                scan[i + 1].SetTime(time);
                i++;
            }
            return scan;
        }

        public double GetOptimumScanwidth(Rectangle rect,int best, int turns)
        {
            double scanwidth = 0;

            if (best == 1)
                scanwidth = LatLongProjection.HaversineDistance(rect.GetSecond(), rect.GetThird()) / turns;

            if (best == 2)
                scanwidth = LatLongProjection.HaversineDistance(rect.GetThird(), rect.GetFourth()) / turns;

            if (best == 3)
                scanwidth = LatLongProjection.HaversineDistance(rect.GetFourth(), rect.GetFirst()) / turns;

            if (best == 4)
                scanwidth = LatLongProjection.HaversineDistance(rect.GetFirst(), rect.GetSecond()) / turns;

            return scanwidth;
        }
        public Rectangle GetCopy() // this function returns a copy of this rectangle
        {
            
            Point f = this.First.GetCopy();
            Point s = this.Second.GetCopy();
            Point t = this.Third.GetCopy();
            Point fo = this.Fourth.GetCopy();
            List<Point> scanlist = new List<Point>();
            foreach (Point punto in this.Scan)
            {
                Point newpoint = punto.GetCopy();
                scanlist.Add(newpoint);
            }

            Rectangle newrectangle = new Rectangle(f,s,t,fo);
            newrectangle.SetScan(scanlist);
            return newrectangle;
        }
    }
}
