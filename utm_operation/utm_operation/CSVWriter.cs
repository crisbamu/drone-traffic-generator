using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace utm_operation
{
    public class CSVWriter
    {
        // Written by Lluís Xavier Herranz on 03/16/2017

        // This class is aimed to write operations performance in csv files.

        // functions

        public void WriteOperatiosInCSV(string filename, List<Operation> operations)
        {
            StreamWriter f1 = new StreamWriter(filename);

            f1.WriteLine("ICAOdroneName,DepartureTime,ArrivalTime,LatHome,LongHome,CruisingSpeed,ScanSpeed"); // file header

            foreach(Operation op in operations)
            {
                if(op.GetRouteType()==RouteType.VLOS)
                    f1.WriteLine(op.GetAircraft().GetIdentifier() + "," + op.GetStartTime().ToString("HH:mm:ss") + "," + op.GetFinalTime().ToString("HH:mm:ss") + "," + op.GetVLOSRoute().GetOriginPoint().GetLatitude() + "," + op.GetVLOSRoute().GetOriginPoint().GetLongitude() + "," + op.GetAircraft().GetCruisingSpeed() + "," + op.GetAircraft().GetScanSpeed());

                if (op.GetRouteType() == RouteType.EVLOS)
                    f1.WriteLine(op.GetAircraft().GetIdentifier() + "," + op.GetStartTime().ToString("HH:mm:ss") + "," + op.GetFinalTime().ToString("HH:mm:ss") + "," + op.GetEVLOSRoute().GetOriginPoint().GetLatitude() + "," + op.GetEVLOSRoute().GetOriginPoint().GetLongitude() + "," + op.GetAircraft().GetCruisingSpeed() + "," + op.GetAircraft().GetScanSpeed());


                if(op.GetRouteType()==RouteType.BVLOS)
                f1.WriteLine(op.GetAircraft().GetIdentifier()+","+op.GetStartTime().ToString("HH:mm:ss")+","+op.GetFinalTime().ToString("HH:mm:ss")+","+op.GetBVLOSRoute().GetOriginPoint().GetLatitude()+","+op.GetBVLOSRoute().GetOriginPoint().GetLongitude()+","+op.GetAircraft().GetCruisingSpeed()+","+op.GetAircraft().GetScanSpeed());

                if(op.GetRouteType()==RouteType.Delivery)
                 f1.WriteLine(op.GetAircraft().GetIdentifier() + "," + op.GetStartTime().ToString("HH:mm:ss") + "," + op.GetFinalTime().ToString("HH:mm:ss") + "," + op.GetDeliveryRoute().GetOriginPoint().GetLatitude() + "," + op.GetDeliveryRoute().GetOriginPoint().GetLongitude() + "," + op.GetAircraft().GetCruisingSpeed() + "," + op.GetAircraft().GetScanSpeed());
            
            
            }

            f1.Close();
        }
    }
}
