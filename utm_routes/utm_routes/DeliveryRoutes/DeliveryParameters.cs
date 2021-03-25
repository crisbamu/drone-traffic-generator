using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using utm_country;
using utm_utils;
using utm_drone;

namespace utm_routes.DeliveryRoutes
{
    public class DeliveryParameters
    {
        //Written by Lluís Xavier Herranz on 02/03/2017

        // This class is aimed to store the delivery route parameters.

        // Attributes

        private double maxdistance;
        private double maxaltitude;
        private double minaltitude;
      


        // builder

        public DeliveryParameters(double max, double maxa, double mina)
        {
            this.maxdistance = max;
            this.maxaltitude = maxa;
            this.minaltitude = mina;

        }

        public DeliveryParameters()
        {

        }

        // setters and getters

        public void SetMaxDistance(double a)
        {
            this.maxdistance = a;
        }

        public void SetMaxAltitude(double a)
        {
            this.maxaltitude = a;
        }

        public void SetMinAltitude(double a)
        {
            this.minaltitude = a;
        }

       
        public double GetMaxDistance()
        {
            return this.maxdistance;
        }

        public double GetMaxAltitude()
        {
            return this.maxaltitude;
        }

        public double GetMinAltitude()
        {
            return this.minaltitude;
        }

        public bool AreDeliveryParametersWellInserted()
        {
            bool well_inserted = false;
            int n = 0;
            
            if(n==0)
            {
                well_inserted = true;
            }
            return well_inserted;
        }

        

        
    }
}
