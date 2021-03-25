using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace utm_routes.EVLOSRoutes
{
   public class EVLOSParameters
    {
         // Written by Lluís Xavier Herranz on 05/22/2017

        // This class is aimed to store all the values for making EVLOS routes and carry them in only one object.

        // attributes

        // everything in meters except the angles

        private double firstmaxdistance;  // the maximum and the minimum distance of the first VLOS Point   
        private double firstmindistance;

        private double secondmaxdistance; // the maximun and the minimum distance of the second VLOS point. It it also the angle range
        private double secondmindistance;
        private double anglerange;

        private double rectanglemaxdistance;
        private double rectanglemindistance;

        private double maxaltitude;
        private double minaltitude;

       


        // builder

        public EVLOSParameters(double a, double b, double c, double d, double e, double f, double g, double h, double i)
        {
            this.firstmaxdistance = a;
            this.firstmindistance = b;
            this.secondmaxdistance = c;
            this.secondmindistance = d;
            this.anglerange = e;
            this.rectanglemaxdistance = f;
            this.rectanglemindistance = g;
            this.maxaltitude = h;
            this.minaltitude = i;

        }

       public EVLOSParameters()
        {

        }

        // setters and getters


       public void SetFirstMaxDist(double a)
       { this.firstmaxdistance = a; }

       public void SetFirstMinDist(double a)
       { this.firstmindistance = a; }

       public void SetSecondMaxDist(double a)
       { this.secondmaxdistance = a; }

       public void SetSecondMinDist(double a)
       { this.secondmindistance = a; }

       public void SetAngleRange(double a)
       { this.anglerange = a; }

       public void SetRectangleMaxDist(double a)
       { this.rectanglemaxdistance = a; }

       public void SetRectangleMinDist(double a)
       { this.rectanglemindistance = a; }

       public void SetMaximumAltitude(double a)
       { this.maxaltitude = a; }

       public void SetMinimumAltitude(double a)
       { this.minaltitude = a; }

        public double GetFirstMaxDist()
        { return this.firstmaxdistance; }

        public double GetFirsMinDist()
        { return this.firstmindistance; }

        public double GetSecondMaxDist()
        { return this.secondmaxdistance; }

        public double GetSecondMinDist()
        { return this.secondmindistance; }

        public double GetAngleRange()
        { return this.anglerange; }

        public double GetRectMaxDist()
        { return this.rectanglemaxdistance; }

        public double GetRectMinDist()
        { return this.rectanglemindistance; }

        public double GetMaxAltitude()
        { return this.maxaltitude; }

        public double GetMinAltitude()
        { return this.minaltitude; }

        public bool AreEVLOSParameterWellInserted()
        {
            bool well_inserted = false;
            int n = 0;
           

            if (firstmaxdistance < firstmindistance || secondmaxdistance < secondmindistance || rectanglemaxdistance<rectanglemindistance || maxaltitude<minaltitude)
            {
                n++;
            }
           
            if (n == 0)
            {
                well_inserted = true;
            }
            return well_inserted;
        }
    }
}
