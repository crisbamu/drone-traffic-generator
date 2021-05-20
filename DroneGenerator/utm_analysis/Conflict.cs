using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using utm_country;
using utm_utils;
using utm_operation;

namespace utm_analysis
{
    public class Conflict
    {
        // Written by CBM 19-June-2020

        // Attributes
        private Operation op1;
        private Operation op2;
        private Point cpa; 
        private double cpa_dist;
        private double alt;
        // Point attributes are
        // double latitude, longitude, altitude
        // DateTime time

        // builder
        public Conflict()
        {
        }


        public Conflict(Operation op1, Operation op2, double a, double b, double c, DateTime d)
        {
            this.op1 = op1;
            this.op2 = op2;
            this.cpa = new Point(a, b, c, d);
            this.cpa_dist = 0;
            this.alt = c;
        }
        // Getters & Setters
        public Operation GetOp1()
        {
            return this.op1;
        }

        public Operation GetOp2()
        {
            return this.op2 ;
        }

        public Point GetCpa()
        {
            return this.cpa ;
        }

        public double GetAlt()
        {
            return this.alt;
        }

        public double GetCpaDist()
        {
            return this.cpa_dist;
        }
        public void SetOp1(Operation b)
        {
            this.op1 = b;
        }

        public void SetOp2(Operation b)
        {
            this.op2 = b;
        }

        public void SetCpa(Point b, double dist=0.0, double alt=0.0)
        {
            this.cpa = new Point (b.GetLatitude(), b.GetLongitude(), b.GetAltitude(), b.GetTime());
            this.cpa_dist = dist;
            if (alt != 0)
                this.alt = b.GetAltitude();
            else
                this.alt = alt;
        }

        public Conflict GetCopy()
        {
            Conflict newconflict = new Conflict(this.op1, this.op2, 
                this.cpa.GetLatitude(), this.cpa.GetLongitude(), this.cpa.GetAltitude(), this.cpa.GetTime());

            return newconflict;
        }
    }
}
