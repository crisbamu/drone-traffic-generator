using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace utm_drone
{
    public class drone
    {

        //Attributes
        private string Model;
        private string TypeOfFlight;
        private string identifier;
        // Performance

        private double endurance;
        private double CruisingSpeed;
        private double ScanSpeed;
        private double MaxCruisingSpeed;
        private double MinCruisingSpeed;
        private double MaximumHeight;
        private double MaxClimbingSpeed;
        private double ClimbSpeed;
        private double MaxDescendingSpeed;
        private double DescendSpeed;
        private double MTOW;
        private double MaxTurningRatio;


        public drone()
        {
            
        }
        public drone(string tipo, string typeofflight,string id, double end, double cruisspeed,double scan, double maxcruisspeed, double mincruisspeed, double maxheight, double maxclimbingspeed, double climb, double maxdesspeed, double descend, double mtow, double maxturnratio)
        {
            this.Model = tipo;
            this.TypeOfFlight = typeofflight;
            this.identifier = id;
            this.endurance = end;
            this.CruisingSpeed = cruisspeed;
            this.ScanSpeed = scan;
            this.MaxCruisingSpeed = maxcruisspeed;
            this.MinCruisingSpeed = mincruisspeed;
            this.MaximumHeight = maxheight;
            this.MaxClimbingSpeed = maxclimbingspeed;
            this.ClimbSpeed = climb;
            this.MaxDescendingSpeed = maxdesspeed;
            this.DescendSpeed = descend;
            this.MTOW = mtow;
            this.MaxTurningRatio = maxturnratio;
        }

        //setters and getters

        public void SetModel(string tipo)
        {
            this.Model= tipo;

        }

        public void SetTypeOfFlight(string typeofflight)
        {
            this.TypeOfFlight = typeofflight;
        }

        public void SetIdentifier (string a)
        {
            this.identifier = a;
        }

        public void SetEndurance(double end)
        { 
            this.endurance = end;
        }

        public void SetCruisingSpeed(double cruispeed)
        { 
            this.CruisingSpeed = cruispeed; 
        }

        public void SetScanSpeed(double speed)
        {
            this.ScanSpeed = speed;
        }

        public void SetMaxCruisingSpeed(double maxcruispeed)
        {
            this.MaxCruisingSpeed = maxcruispeed; 
        }

        public void SetMinCruisingSpeed(double mincruispeed)
        {
            this.MinCruisingSpeed = mincruispeed; 
        }

        public void SetMaxHeight(double maxheight)
        {
            this.MaximumHeight = maxheight;
        }

        public void SetMaxClimbingSpeed(double maxclimbspeed)
        { 
            this.MaxClimbingSpeed = maxclimbspeed;
        }

        public void SetClimbSpeed(double speed)
        {
            this.ClimbSpeed = speed;
        }

        public void SetMaxDescendingSpeed(double maxdescspeed)
        {
            this.MaxDescendingSpeed = maxdescspeed;
        }

        public void SetDescendSpeed(double speed)
        {
            this.DescendSpeed = speed;
        }

        public void SetMTOW(double mtow)
        {
            this.MTOW = mtow; 
        }

        public void SetMaxTurningRatio(double maxturnratio)
        {
            this.MaxTurningRatio = maxturnratio;
        }


        public string GetModel()
        { 
            return this.Model;
        }

        public string GetTypeOfFlight()
        { 
            return this.TypeOfFlight;
        }

        public string GetIdentifier()
        { 
            return this.identifier; 
        }

        public double GetEndurance()
        { 
            return this.endurance; 
        }

        public double GetCruisingSpeed()
        { 
            return this.CruisingSpeed; 
        }

        public double GetScanSpeed()
        { 
            return this.ScanSpeed;
        }

        public double GetMaxCruisingSpeed()
        {
            return this.MaxCruisingSpeed; 
        }

        public double GetMinCruisingSpeed()
        {
            return this.MinCruisingSpeed;
        }

        public double GetMaximumHeight()
        {
            return this.MaximumHeight;
        }

        public double GetMaxClimbingSpeed()
        { 
            return this.MaxClimbingSpeed; 
        }

        public double GetClimbSpeed()
        {
            return this.ClimbSpeed;
        }

        public double GetMaxDescendingSpeed()
        { 
            return this.MaxDescendingSpeed; 
        }

        public double GetDescendSpeed()
        {
            return this.DescendSpeed;
        }

        public double GetMTOW()
        {
            return this.MTOW;
        }

        public double GetMaxTurningRatio()
        {
            return this.MaxTurningRatio;
        }

        // functions

        public drone GetCopy() // this function returns a copy of this drone
        {
            string typedrone=this.Model;
            string typeflight = this.TypeOfFlight;;
            string id=this.identifier;
        

            double end = this.endurance;
            double cruis = this.CruisingSpeed;
            double scan = this.ScanSpeed;
            double maxcruis = this.MaxCruisingSpeed;;
            double MinCruis = this.MinCruisingSpeed;
            double maxh = this.MaximumHeight;
            double maxcl = this.MaxClimbingSpeed;
            double climb = this.ClimbSpeed;
            double maxdes=this.MaxDescendingSpeed;
            double descend = this.MaxDescendingSpeed;
            double mass=this.MTOW;
            double maxtur=this.MaxTurningRatio;

            drone newdrone = new drone(typedrone, typeflight, id, end, cruis,scan, maxcruis, MinCruis, maxh, maxcl,climb, maxdes,descend, mass, maxtur);
            return newdrone;

    }

    }
}
