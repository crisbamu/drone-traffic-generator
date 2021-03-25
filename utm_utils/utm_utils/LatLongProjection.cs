using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace utm_utils
{
    public static class LatLongProjection
    {


        // Written by Lluís Xavier Herranz on 22/01/2017

        // This class is aimed to be an alternative of Mercator Projection so that the distance between lat lot points are more accurate.

        // the data is obtained from: http://nssdc.gsfc.nasa.gov/planetary/factsheet/earthfact.html and http://www.movable-type.co.uk/scripts/latlong.html

        private static double EarthRadius = 6371000; // in meters
        //private static double ellipticity = 0.00335;

        public static double HaversineDistance(Point firstpoint, Point secondpoint)  // this function calculates the distance between two points using thr haversine formula
        {
            double firstlatitude = firstpoint.GetLatitude() * Math.PI / 180;
            double secondlatitude = secondpoint.GetLatitude() * Math.PI / 180;

            double firstlongitude = firstpoint.GetLongitude() * Math.PI / 180;
            double secondlongitude = secondpoint.GetLongitude() * Math.PI / 180;

            double latitudedifference = secondlatitude - firstlatitude;
            double longitudedifference = secondlongitude - firstlongitude;

            double a = Math.Pow(Math.Sin(latitudedifference / 2), 2) + (Math.Cos(firstlatitude) * Math.Cos(secondlatitude) * Math.Pow(Math.Sin(longitudedifference / 2), 2));
            double c = 2 * Math.Atan2((Math.Sqrt(a)), (Math.Sqrt(1 - a)));
            double d = EarthRadius * c;
            return d;
        }

        public static double LawOfCosinesDistance(Point firstpoint, Point secondpoint) // this function calculates the distance between two points using the law of cosines
        {
            double firstlatitude = firstpoint.GetLatitude() * Math.PI / 180;
            double secondlatitude = secondpoint.GetLatitude() * Math.PI / 180;

            double firstlongitude = firstpoint.GetLongitude() * Math.PI / 180;
            double secondlongitude = secondpoint.GetLongitude() * Math.PI / 180;

            double latitudedifference = secondlatitude - firstlatitude;
            double longitudedifference = secondlongitude - firstlongitude;

            double d = Math.Acos(Math.Sin(firstlatitude) * Math.Sin(secondlatitude) + Math.Cos(firstlatitude) * Math.Cos(secondlatitude) * Math.Cos(longitudedifference)) * EarthRadius;
            return d;

        }

        public static Point DestinyPoint(Point origin, double distance, double bearing) // this function calculates the destiny point with a origin, a distance and a bearing (in radians)
        {
            double originlatitude = origin.GetLatitude() * Math.PI / 180;
            double originlongitude = origin.GetLongitude() * Math.PI / 180;

            double delta = distance / EarthRadius;

            double destlatitude = Math.Asin(Math.Sin(originlatitude) * Math.Cos(delta) + Math.Cos(originlatitude) * Math.Sin(delta) * Math.Cos(bearing));
            double destlongitude = (originlongitude + Math.Atan2(Math.Sin(bearing) * Math.Sin(delta) * Math.Cos(originlatitude), Math.Cos(delta) - Math.Sin(originlatitude) * Math.Sin(destlatitude)));
            Point newpoint = new Point(destlatitude * 180 / Math.PI, destlongitude * 180 / Math.PI);
            return newpoint;
        }

        public static double GetBearing(Point origin, Point destination) // This function gets the angle between two points in radians
        {
            double firstlatitude = origin.GetLatitude() * Math.PI / 180;
            double secondlatitude = destination.GetLatitude() * Math.PI / 180;

            double firstlongitude = origin.GetLongitude() * Math.PI / 180;
            double secondlongitude = destination.GetLongitude() * Math.PI / 180;

            double latitudedifference = secondlatitude - firstlatitude;
            double longitudedifference = secondlongitude - firstlongitude;

            double bearing = Math.Atan2((Math.Sin(longitudedifference) * Math.Cos(secondlatitude)), (Math.Cos(firstlatitude) * Math.Sin(secondlatitude) - Math.Sin(firstlatitude) * Math.Cos(secondlatitude) * Math.Cos(longitudedifference)));
            return bearing;
        }

        public static  Point IntersectionPoint(Point FirstPoint, double FirstBearing, Point SecondPoint, double SecondBearing) // returns the point where the two paths intersects
        {
            double firstlatitude = FirstPoint.GetLatitude() * Math.PI / 180;
            double secondlatitude = SecondPoint.GetLatitude() * Math.PI / 180;

            double firstlongitude = FirstPoint.GetLongitude() * Math.PI / 180;
            double secondlongitude = SecondPoint.GetLongitude() * Math.PI / 180;

            double latitudedifference = secondlatitude-firstlatitude;
            double longitudedifference = secondlongitude-firstlongitude;

            // formulas development

            double delta12 = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(latitudedifference / 2), 2) + (Math.Cos(firstlatitude) * Math.Cos(secondlatitude) * Math.Pow(Math.Sin(longitudedifference / 2), 2))));
            double thetaA = Math.Acos((Math.Sin(secondlatitude) - (Math.Sin(firstlatitude) * Math.Cos(delta12))) / (Math.Sin(delta12) * Math.Cos(firstlatitude)));
            double thetaB = Math.Acos((Math.Sin(firstlatitude) - (Math.Sin(secondlatitude) * Math.Cos(delta12))) / (Math.Sin(delta12) * Math.Cos(secondlatitude)));

            double theta12;
            double theta21;
            if(Math.Sin(secondlongitude-firstlongitude)>0)
            {
                theta12 = thetaA;
                theta21 = (2 * Math.PI) - thetaB;
            }
            else
            {
                theta12 = (2 * Math.PI) - thetaA;
                theta21 = thetaB;
            }

            double alpha1 = FirstBearing - theta12;
            double alpha2 = theta21 - SecondBearing;
            double alpha3 = Math.Acos(-Math.Cos(alpha1) * Math.Cos(alpha2) + Math.Sin(alpha1) * Math.Sin(alpha2) * Math.Cos(delta12));
            double delta13 = Math.Atan2(Math.Sin(delta12) * Math.Sin(alpha1) * Math.Sin(alpha2), Math.Cos(alpha2) + (Math.Cos(alpha1) * Math.Cos(alpha3)));

            double thirdlatitude = Math.Asin(Math.Sin((firstlatitude) * Math.Cos(delta13)) + (Math.Cos(firstlatitude) * Math.Sin(delta13) * Math.Cos(FirstBearing)));
            double Delta13 = Math.Atan2(Math.Sin(FirstBearing) * Math.Sin(delta13) * Math.Cos(firstlatitude), Math.Cos(delta13) - (Math.Sin(firstlatitude) * Math.Sin(thirdlatitude)));
            double thirdlongitude = firstlongitude + Delta13;

            Point intersectionpoint = new Point(thirdlatitude*180/Math.PI, thirdlongitude*180/Math.PI, FirstPoint.GetAltitude(), FirstPoint.GetTime()); // set as 4D with Point1 values for Alt & Time
            return intersectionpoint;
        }

        public static Point MidPoint(Point FirstPoint, Point SecondPoint)
        {
            double firstlatitude = FirstPoint.GetLatitude() * Math.PI / 180;
            double secondlatitude = SecondPoint.GetLatitude() * Math.PI / 180;

            double firstlongitude = FirstPoint.GetLongitude() * Math.PI / 180;
            double secondlongitude = SecondPoint.GetLongitude() * Math.PI / 180;

            double latitudedifference = secondlatitude - firstlatitude;
            double longitudedifference = secondlongitude - firstlongitude;
            
           // calculus
            double B_x = Math.Cos(secondlatitude) * Math.Cos(longitudedifference);
            double B_y = Math.Cos(secondlatitude) * Math.Sin(longitudedifference);
            double third_latitude = Math.Atan2(Math.Sin(firstlatitude) + Math.Sin(secondlatitude), Math.Sqrt(Math.Pow(Math.Cos(firstlatitude) + B_x, 2) + Math.Pow(B_y, 2)));
            double third_longitude = firstlongitude + Math.Atan2(B_y, Math.Cos(firstlatitude) + B_x);
            Point MidPoint = new Point(third_latitude * 180 / Math.PI, third_longitude * 180 / Math.PI);
            return MidPoint;

        }
    }
}
