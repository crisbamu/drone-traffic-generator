using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace utm_utils
{
    public class TimeGenerator
    {
        // Written by Lluís Xavier Herranz on 1/17/2017

        // This class generates random times for the countries

        // functions

        public TimeSpan RandomTimeGenerator(Random rnd, double meantime, double stdDev, TimeSpan lowerlimit, TimeSpan upperlimit) // This function creates a random time using the Box-Muller Algorithm
        {
            bool feasible = false;
            TimeSpan time = new TimeSpan();

            while (feasible == false)
            {
                double u1 = rnd.NextDouble();
                double u2 = rnd.NextDouble();
                double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
                double randNormal = meantime + stdDev * randStdNormal; //random normal(mean,stdDev^2)
                time = TimeSpan.FromHours(randNormal);
                if (time > lowerlimit && time < upperlimit)
                {
                    feasible = true;
                }
            }
            return time;
        } 
    }
}
