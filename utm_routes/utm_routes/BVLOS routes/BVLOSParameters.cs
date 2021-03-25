using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace utm_routes.BVLOS_routes
{
   public class BVLOSParameters
    {
        // Written by Lluís Xavier Herranz on 01/25/2017

        // This class is aimed to store the bvlos parameters in only one class. 

        // Attributes

        // everything in meters except the angles, which are in degrees

        private int maxnumbergeneration;  // number of generation of scan
        private int minnumbergeneration;

        private double firstmaxdistance;  // the maximum and the minimum distance of the first BVLOS Point   
        private double firstmindistance;

        private double maxdistance; // the maximun and the minimum distance between the next BVLOS points after the first point. 
        private double mindistance;
        private double anglerange;  // It it also the angle rangein degrees

        private double rectanglemaxdistance;
        private double rectanglemindistance;  // parameters of the rectangle size

        private double maxaltitude;
        private double minaltitude;  // altitude range of the operation

        

        public BVLOSParameters(int max, int min, double a, double b, double c, double d, double e ,double f, double g, double h, double i)
        {
            this.maxnumbergeneration = max;
            this.minnumbergeneration = min;
            this.firstmaxdistance = a;
            this.firstmindistance = b;
            this.maxdistance = c;
            this.mindistance = d;
            this.anglerange = e;
            this.rectanglemaxdistance = f;
            this.rectanglemindistance = g;
            this.maxaltitude = h;
            this.minaltitude = i;
           
        }

       public BVLOSParameters()
        {

        }

       //setters and getters

       public void SetMaxNumGen(int a)
       { this.maxnumbergeneration = a; }

       public void SetMinNumGen(int a)
       { this.minnumbergeneration = a; }

       public void SetFirstMaxDistance(double a)
       { this.firstmaxdistance = a; }

       public void SetFirstMinDistance(double a)
       { this.firstmindistance = a; }

       public void SetMaxDistance(double a)
       { this.maxdistance = a; }

       public void SetMinDistance(double a)
       { this.mindistance = a; }

       public void SetAngleRange(double a)
       { this.anglerange = a; }

       public void SetRectangleMaxDistance(double a)
       { this.rectanglemaxdistance = a; }

       public void SetRectangleMinDistance(double a)
       { this.rectanglemindistance = a; }

       public void SetMaxAltitude(double a)
       { this.maxaltitude = a; }

       public void SetMinAltitude(double a)
       { this.minaltitude = a; }


        public int GetMaxNumGen()
        { return this.maxnumbergeneration; }

        public int GetMinNumGen()
        { return this.minnumbergeneration; }

        public double GetFirstMaxDist()
        { return this.firstmaxdistance; }

        public double GetFirsMinDist()
        { return this.firstmindistance; }

        public double GetMaxDist()
        { return this.maxdistance; }

        public double GetMinDist()
        { return this.mindistance; }

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

       public bool AreBVLOSParametersWellInserted()
        {
            bool well_inserted = false;
            int n = 0;
           

            if (maxnumbergeneration<minnumbergeneration|| firstmaxdistance < firstmindistance || maxdistance < mindistance || rectanglemaxdistance < rectanglemindistance || maxaltitude < minaltitude)
            {
                n++;
            }
           if(n==0)
           { 
               well_inserted = true; 
           }
           return well_inserted;
        }

 
    }
}
