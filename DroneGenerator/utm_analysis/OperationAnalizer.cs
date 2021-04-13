using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using utm_country;
using utm_drone;
using utm_operation;
using utm_operator;
using utm_routes;
using utm_routes.BVLOS_routes;
using utm_routes.DeliveryRoutes;
using utm_routes.VLOS_routes;
using utm_utils;
using System.IO;
using System.Xml;


namespace utm_analysis
{

    public static class OperationAnalizer
    {
        //Written by Lluís Xavier Herranz on 04/21/2017

        // This class aims at analysing the operations in search of conflicts, busy areas and operation proximity to determined zones

        //Attributes
        public static int MAX_SAFE_DISTANCE = 20; // min distance set to 20 meters in horizontal
        public static int MAX_SAFE_ALT = 5; // min distance set to 5 meters in vertical
        static OperatorGenerator operator_generator = new OperatorGenerator();


        // functions
        public static void SetSafeDistance(float scale)
        {
            MAX_SAFE_DISTANCE = Convert.ToInt32(MAX_SAFE_DISTANCE * scale);
            MAX_SAFE_ALT = Convert.ToInt32(MAX_SAFE_ALT * scale);
            Console.WriteLine("Conflict Distance Volume is {0} m radius and {1} m altidude", MAX_SAFE_DISTANCE, MAX_SAFE_ALT);
        }

        // this function works out the minimum distance between two operations by using the Haversine algorithm.
        public static double FindMinimumDistanceBetweenOperations(Operation a, Operation b)
        {
            double minimum_distance = LatLongProjection.HaversineDistance(a.GetDiscretizedRoute()[0], b.GetDiscretizedRoute()[0]);
            foreach (Point punto in a.GetDiscretizedRoute())
            {
                foreach (Point bpoint in b.GetDiscretizedRoute())
                {
                    double distance = LatLongProjection.HaversineDistance(punto, bpoint);
                    if (distance < minimum_distance)
                    {
                        minimum_distance = distance;
                    }
                }
            }
            return minimum_distance;
        }

        // This function calculates the closest equidistant point between two operations.
        public static Point FindMidPointBetween2Operations(Operation a, Operation b)
        {
            double minimum_distance = LatLongProjection.HaversineDistance(a.GetDiscretizedRoute()[0], b.GetDiscretizedRoute()[0]);
            Point mid_point = new Point();
            foreach (Point punto in a.GetDiscretizedRoute())
            {
                foreach (Point bpoint in b.GetDiscretizedRoute())
                {
                    double distance = LatLongProjection.HaversineDistance(punto, bpoint);
                    if (distance < minimum_distance)
                    {
                        minimum_distance = distance;
                        mid_point = LatLongProjection.MidPoint(punto, bpoint);
                    }
                }
            }
            return mid_point;
        }

        // this function calculates the maximum distance from a point inserted by the user (punto) with respect to the two inserted operations (Operation a and Operation b).
        // This distance is useful to work out the radius of the busy area.
        public static double FindMaximumDistanceFromAPoint(Operation a, Operation b, Point punto)
        {
            double maximum_distance = 0;
            foreach (Point apoint in a.GetDiscretizedRoute())
            {
                double distance = LatLongProjection.HaversineDistance(apoint, punto);
                if (distance > maximum_distance)
                {
                    maximum_distance = distance;
                }
            }

            foreach (Point bpoint in b.GetDiscretizedRoute())
            {
                double distance = LatLongProjection.HaversineDistance(bpoint, punto);
                if (distance > maximum_distance)
                {
                    maximum_distance = distance;
                }
            }

            return maximum_distance;

        }

        /* function changed by CBM:
         *      calculate conflicts also delivery-to-delivery
         */
        public static List<Conflict> FindTemporalConflicts(List<Operator> operatorlist)
        {
            List<Operation> operationlist = OperatorWriter.GetListOfOperations(operatorlist);
            List<Conflict> ConflictPointsList = new List<Conflict>();
            for (int i = 0; i < operationlist.Count - 1; i++)
            {
                int n = i + 1;
                while (n < operationlist.Count)
                {
                    if (true) //(!(operationlist[i].GetRouteType() == RouteType.Delivery && operationlist[n].GetRouteType() == RouteType.Delivery))
                    {
                        var result = GetTemporalConflict(operationlist[i], operationlist[n]);
                        bool conflict = result.Item1;
                        if (conflict == true)
                        {
                            ConflictPointsList.Add(result.Item2);
                        }
                    }
                    n++;
                }
            }
            return ConflictPointsList;
        }

        /* this function os never called!! */
        public static List<Conflict> FindAtemporalConflicts(List<Operator> operatorlist)
        {

            List<Operation> operationlist = OperatorWriter.GetListOfOperations(operatorlist);
            List<Conflict> ConflictPointsList = new List<Conflict>();
            for (int i = 0; i < operationlist.Count - 1; i++)
            {
                int n = i + 1;
                while (n < operationlist.Count)
                {
                    var dist = GetAtemporalConflict(operationlist[i], operationlist[n]);
                    bool conflict = dist.Item1;
                    if (conflict == true)
                    {
                        ConflictPointsList.AddRange(dist.Item2);
                    }
                    n++;
                }
            }
            return ConflictPointsList;
        }
        public static double CalculateMinDist(Point P1, Point P2, double d12, double d1i, Point P3, Point P4, double d34, double d3i)
        {
            double min_dist = 0.0;
            double v1 = d12 / P2.GetTime().Subtract(P1.GetTime()).TotalSeconds;
            double v2 = d34 / P4.GetTime().Subtract(P3.GetTime()).TotalSeconds;
            double t1i = d1i / v1;
            double t3i = d3i / v2;
            double d1_t3i = v1 * t3i;
            double d3_t1i = v2 * t1i;
            min_dist = Math.Min(Math.Abs(d1i-d1_t3i), Math.Abs(d3i-d3_t1i));

            return min_dist;
        }
        public static Tuple<bool, Conflict> GetConflict(Point FirstPoint, Point SecondPoint, Point ThirdPoint, Point FourthPoint)
        {
            Conflict conflict_info = new Conflict();
            // Easy way - valid also for parallel trajectories or hover routes
            double dist1to3 = Math.Abs(LatLongProjection.HaversineDistance(FirstPoint, ThirdPoint));
            double alt1to3 = Math.Abs(FirstPoint.GetAltitude() - ThirdPoint.GetAltitude());
            if ((dist1to3 < MAX_SAFE_DISTANCE) &&
                (alt1to3 < MAX_SAFE_ALT ))
            {
                double min_dist = Math.Sqrt(dist1to3*dist1to3 + alt1to3*alt1to3);
                conflict_info.SetCpa(FirstPoint, min_dist);            
                return new Tuple<bool, Conflict>(true, conflict_info);
            }
            else if (LatLongProjection.HaversineDistance(FirstPoint, SecondPoint) == 0 || LatLongProjection.HaversineDistance(ThirdPoint, FourthPoint)==0)
            {   //any one of the aircraft is hovering or vertical mov
                return new Tuple<bool, Conflict>(false, conflict_info);
            }
            bool conflict = false;
            // local parameters
            double first_distance = LatLongProjection.HaversineDistance(FirstPoint, SecondPoint);
            double first_bearing = LatLongProjection.GetBearing(FirstPoint, SecondPoint);
            double second_distance = LatLongProjection.HaversineDistance(ThirdPoint, FourthPoint);
            double second_bearing = LatLongProjection.GetBearing(ThirdPoint, FourthPoint);
            // intersection parameters
            Point intersection_point = LatLongProjection.IntersectionPoint(FirstPoint, first_bearing, ThirdPoint, second_bearing);
            double first_intersection_distance = LatLongProjection.HaversineDistance(FirstPoint, intersection_point);
            double second_intersection_distance = LatLongProjection.HaversineDistance(ThirdPoint, intersection_point);
            // check the intersection
            if(first_intersection_distance<=first_distance && second_intersection_distance<=second_distance)
            {
                conflict = true;
                double min_dist = CalculateMinDist(FirstPoint, SecondPoint, first_distance, first_intersection_distance, 
                                                     ThirdPoint, FourthPoint, second_distance, second_intersection_distance);
                conflict_info.SetCpa (intersection_point, min_dist);
            }

            return new Tuple<bool, Conflict>(conflict, conflict_info);
        }

        public static bool ArePotentialBusyAreas(Operation first_operation, Operation second_operation, double distance)
        {
            bool potential = false;
            double first_distance = first_operation.GetMaximumDistanceWRTPoint(first_operation.GetDiscretizedRoute()[0], first_operation);
            double second_distance = second_operation.GetMaximumDistanceWRTPoint(second_operation.GetDiscretizedRoute()[0], second_operation);
            double operation_distance = LatLongProjection.HaversineDistance(first_operation.GetDiscretizedRoute()[0], second_operation.GetDiscretizedRoute()[0]);
            if (operation_distance <= (first_distance + second_distance+distance))
            {
                potential = true;
            }
            return potential;
        }

        public static bool ArePotentialConflictiveRoutes(Operation first_operation, Operation second_operation)
        {
            bool potential = false;

            double first_distance = first_operation.GetMaximumDistanceWRTPoint(first_operation.GetDiscretizedRoute()[0], first_operation);
            double second_distance = second_operation.GetMaximumDistanceWRTPoint(second_operation.GetDiscretizedRoute()[0], second_operation);
            double operation_distance = LatLongProjection.HaversineDistance(first_operation.GetDiscretizedRoute()[0], second_operation.GetDiscretizedRoute()[0]);
            if(operation_distance<=(first_distance+second_distance))
            {
                potential = true;
            }

            return potential;
        }

        public static bool AreTemporalPotentialConflictiveRoutes(Operation first_operation, Operation second_operation)
        {
            bool potential = false;
            DateTime first_initial_time = first_operation.GetStartTime();
            DateTime first_final_time = first_operation.GetFinalTime();
            DateTime second_initial_time = second_operation.GetStartTime();
            DateTime second_final_time = second_operation.GetFinalTime();
            if(!(first_final_time<second_initial_time || first_initial_time>second_final_time))
            {
                potential = true;
            }
            return potential;
        }

        public static Tuple<bool, Conflict> GetTemporalConflict(Operation first_operation, Operation second_operation)
        {
            bool conflict = false;
            Conflict ConflictPoint = new Conflict();
            List<Point> firstdiscretizedroute = first_operation.GetDiscretizedRoute();
            List<Point> seconddiscretizedroute = second_operation.GetDiscretizedRoute();
            bool temporal_conflict = AreTemporalPotentialConflictiveRoutes(first_operation, second_operation);
            if (temporal_conflict == true)
            {
                bool spacial_conflict = ArePotentialConflictiveRoutes(first_operation, second_operation);
                if (spacial_conflict == true)
                {
                    for (int i = 0; i < firstdiscretizedroute.Count - 1; i++)
                    {
                        Point FirstPoint = firstdiscretizedroute[i];
                        Point SecondPoint = firstdiscretizedroute[i + 1];
                        for (int n = 0; n < seconddiscretizedroute.Count - 1; n++)
                        {
                            Point ThirdPoint = seconddiscretizedroute[n];
                            Point FourthPoint = seconddiscretizedroute[n + 1];
                            var result = GetConflict(FirstPoint, SecondPoint, ThirdPoint, FourthPoint);
                            bool is_conflict = result.Item1;
                            if (is_conflict == true)
                            {
                                result.Item2.SetOp1(first_operation);
                                result.Item2.SetOp2(second_operation);
                                ConflictPoint=result.Item2;
                                conflict = true;
                                return new Tuple<bool, Conflict>(conflict, ConflictPoint);
                            }

                        }
                    }
                }
            }
            return new Tuple<bool, Conflict>(conflict, ConflictPoint);
        }

        public static Tuple<bool,List<Conflict>> GetAtemporalConflict(Operation firstoperation, Operation secondoperation) // this functions looks for conflicts between two operation without taking into account the time
        {
            bool conflict = false;
            List<Conflict> ConflictPoints = new List<Conflict>();
            List<Point> firstdiscretizedroute = firstoperation.GetDiscretizedRoute();
            List<Point> seconddiscretizedroute = secondoperation.GetDiscretizedRoute();
            bool potential = ArePotentialConflictiveRoutes(firstoperation, secondoperation);
            if (potential == true)
            {
                for (int i = 0; i < firstdiscretizedroute.Count - 1; i++)
                {
                    Point FirstPoint = firstdiscretizedroute[i];
                    Point SecondPoint = firstdiscretizedroute[i + 1];
                    for (int n = 0; n < seconddiscretizedroute.Count - 1; n++)
                    {
                        Point ThirdPoint = seconddiscretizedroute[n];
                        Point FourthPoint = seconddiscretizedroute[n + 1];
                        var result = GetConflict(FirstPoint, SecondPoint, ThirdPoint, FourthPoint);
                        bool is_conflict = result.Item1;
                        if(is_conflict==true)
                        {
                            result.Item2.SetOp1(firstoperation);
                            result.Item2.SetOp2(secondoperation);
                            ConflictPoints.Add(result.Item2);
                            conflict = true;
                        }
                        
                    }

                }
            }


            return new Tuple<bool, List<Conflict>>(conflict, ConflictPoints);
        }

        public static Tuple<List<double>, List<double>> GetScheduleOperation(List<Operator> operators, TimeSpan initialtime, TimeSpan finaltime)
        {
            List<double> x = new List<double>();
            List<double> y = new List<double>();

            List<Operation> operations = OperatorWriter.GetListOfOperations(operators);

            double deltatime = 60; // in seconds
            TimeSpan dt = TimeSpan.FromSeconds(deltatime);

            TimeSpan actualtime = initialtime;


            while (actualtime < finaltime)
            {
                double actime = actualtime.TotalHours;
                double numofoperations = 0;

                foreach (Operation op in operations)
                {
                    DateTime today = op.GetStartTime().Date;
                    DateTime actual = today + actualtime;


                    if (actual >= op.GetStartTime() && actual <= op.GetFinalTime())
                    {
                        numofoperations++;
                    }
                }

                x.Add(actime);
                y.Add(numofoperations);
                actualtime = actualtime + dt;
            }

            return new Tuple<List<double>, List<double>>(x, y);


        }

        public static Tuple<double, double, double,double, double> GetDistanceStatistics(List<Operator> operators)
        {
            List<Operation> operations = OperatorWriter.GetListOfOperations(operators);
            

            double meandistance = 0;

            int vlosop = 0;
            int evlosop = 0;
            int bvlosop = 0;
            int delivop = 0;

            double vlosmeandistance = 0;
            double evlosmeandistance = 0;
            double bvlosmeandistance = 0;
            double deliverymeandistance = 0;

            foreach (Operation op in operations)
            {

                if (op.GetRouteType() == RouteType.VLOS)
                {
                    vlosop++;
                    double distance = 0;

                    for (int i = 0; i < op.GetVLOSRoute().GetDiscretizedRoute().Count - 1; i++)
                    {
                        distance = distance + LatLongProjection.HaversineDistance(op.GetVLOSRoute().GetDiscretizedRoute()[i], op.GetVLOSRoute().GetDiscretizedRoute()[i + 1]);

                    }
                    vlosmeandistance = vlosmeandistance + distance;
                }

                if (op.GetRouteType() == RouteType.EVLOS)
                {
                    evlosop++;
                    double distance = 0;

                    for (int i = 0; i < op.GetEVLOSRoute().GetDiscretizedRoute().Count - 1; i++)
                    {
                        distance = distance + LatLongProjection.HaversineDistance(op.GetEVLOSRoute().GetDiscretizedRoute()[i], op.GetEVLOSRoute().GetDiscretizedRoute()[i + 1]);

                    }
                    evlosmeandistance = evlosmeandistance + distance;
                }

                if (op.GetRouteType() == RouteType.BVLOS)
                {
                    bvlosop++;
                    double distance = 0;

                    for (int i = 0; i < op.GetBVLOSRoute().GetDiscretizedRoute().Count - 1; i++)
                    {
                        distance = distance + LatLongProjection.HaversineDistance(op.GetBVLOSRoute().GetDiscretizedRoute()[i], op.GetBVLOSRoute().GetDiscretizedRoute()[i + 1]);

                    }
                    bvlosmeandistance = bvlosmeandistance + distance;
                }

                if (op.GetRouteType() == RouteType.Delivery)
                {
                    delivop++;
                    double distance = 0;

                    for (int i = 0; i < op.GetDeliveryRoute().GetDiscretizedRoute().Count - 1; i++)
                    {
                        distance = distance + LatLongProjection.HaversineDistance(op.GetDeliveryRoute().GetDiscretizedRoute()[i], op.GetDeliveryRoute().GetDiscretizedRoute()[i + 1]);

                    }
                    deliverymeandistance = deliverymeandistance + distance;
                }



            }

            if (!(vlosop == 0 && evlosop == 0 && bvlosop == 0 && delivop == 0))
                meandistance = (vlosmeandistance + evlosmeandistance + bvlosmeandistance + deliverymeandistance) / (vlosop + bvlosop + evlosop + delivop);


            if (vlosop != 0)
            {
                vlosmeandistance = vlosmeandistance / vlosop;
            }

            if (evlosop != 0)
            {
                evlosmeandistance = evlosmeandistance / evlosop;
            }


            if (bvlosop != 0)
            {
                bvlosmeandistance = bvlosmeandistance / bvlosop;
            }

            if (delivop != 0)
            {
                deliverymeandistance = deliverymeandistance / delivop;
            }



            return new Tuple<double, double, double,double, double>(Math.Round(meandistance,2),Math.Round(vlosmeandistance,2),Math.Round(evlosmeandistance,2),Math.Round(bvlosmeandistance,2), Math.Round(deliverymeandistance,2));
        }

        public static Tuple<double, double, double,double, double> GetTimeStatistics(List<Operator> operators)
        {
            List<Operation> operations = OperatorWriter.GetListOfOperations(operators);
            double meantime = 0;
            double vlostime = 0;
            double evlostime = 0;
            double bvlostime = 0;
            double deliverytime = 0;

            int vlosop = 0;
            int evlosop = 0;
            int bvlosop = 0;
            int deliveryops = 0;

            foreach (Operation op in operations)
            {
                if (op.GetRouteType() == RouteType.VLOS)
                {
                    vlosop++;
                    TimeSpan missiontime = op.GetFinalTime() - op.GetStartTime();
                    vlostime = vlostime + missiontime.TotalSeconds;

                }

                if (op.GetRouteType() == RouteType.EVLOS)
                {
                    evlosop++;
                    TimeSpan missiontime = op.GetFinalTime() - op.GetStartTime();
                    evlostime = evlostime + missiontime.TotalSeconds;

                }

                if (op.GetRouteType() == RouteType.BVLOS)
                {
                    bvlosop++;
                    TimeSpan missiontime = op.GetFinalTime() - op.GetStartTime();
                    bvlostime = bvlostime + missiontime.TotalSeconds;

                }

                if (op.GetRouteType() == RouteType.Delivery)
                {
                    deliveryops++;
                    TimeSpan missiontime = op.GetFinalTime() - op.GetStartTime();
                    deliverytime = deliverytime + missiontime.TotalSeconds;

                }

            }


            if (!(vlosop == 0 && evlosop == 0 && bvlosop == 0 && deliveryops == 0))
                meantime = (vlostime + evlostime + bvlostime + deliverytime) / (vlosop + bvlosop + evlosop + deliveryops);


            if (vlosop != 0)
            {
                vlostime = vlostime / vlosop;
            }

            if (evlosop != 0)
            {
                evlostime = evlostime / evlosop;
            }


            if (bvlosop != 0)
            {
                bvlostime = bvlostime / bvlosop;
            }

            if (deliveryops != 0)
            {
                deliverytime = deliverytime / deliveryops;
            }

            return new Tuple<double, double, double,double, double>(Math.Round(meantime/60,2),Math.Round((vlostime/60),2),Math.Round((evlostime/60),2) ,Math.Round((bvlostime/60),2), Math.Round((deliverytime/60),2));
        }

        public static Tuple<int, int,int , int> GetOperationsStatistics(List<Operator> operators)
        {
            List<Operation> operations = OperatorWriter.GetListOfOperations(operators);
            int vlosop = 0;
            int evlosop = 0;
            int bvlosop = 0;
            int deliveryop = 0;

            foreach (Operation op in operations)
            {
                if (op.GetRouteType() == RouteType.VLOS)
                {
                    vlosop++;
                }

                if (op.GetRouteType() == RouteType.EVLOS)
                {
                    evlosop++;
                }

                if (op.GetRouteType() == RouteType.BVLOS)
                {
                    bvlosop++;
                }

                if (op.GetRouteType() == RouteType.Delivery)
                {
                    deliveryop++;
                }
            }

            return new Tuple<int, int, int,int>(vlosop, evlosop,bvlosop, deliveryop);
        }

        // Whats the differences with the function in AnalysisWriter???=> Use that, not this one!!
        public static void WriteConflicts(List<Conflict> list, string filename)
        {
            // out=open(folder+'Conflicts_'+day+'.csv', 'w')
            // out.write('Time,Lon,Lat,Alt,CPA,flight1,flight2\n')

            StreamWriter writer = new StreamWriter(filename);

            list = list.OrderBy(x => x.GetCpa().GetTime()).ToList();
            writer.WriteLine("Time,Lon,Lat,Alt,CPA,flight1,flight2");
            foreach (Conflict punto in list)
            {
                string hour = punto.GetCpa().GetTime().Hour.ToString("HH:mm");
                string time = punto.GetCpa().GetTime().Year + "-" + punto.GetCpa().GetTime().Month.ToString("d2") + "-" + punto.GetCpa().GetTime().Day.ToString("d2") + "T" +
                    punto.GetCpa().GetTime().Hour.ToString("d2") + ":" + punto.GetCpa().GetTime().Minute.ToString("d2") + ":" + punto.GetCpa().GetTime().Second.ToString("d2") + "Z";
                writer.Write(time);

                writer.Write(","+punto.GetCpa().GetLongitude() + "," + punto.GetCpa().GetLatitude() + "," + punto.GetCpa().GetAltitude());

                writer.Write("," + punto.GetCpaDist() + "," + punto.GetOp1().flightId + "," + punto.GetOp2().flightId+"\n");
            }
            writer.Close();
        }

        public static List<Operation> FindBusyAreas(List<Operator> operator_list) // find busy areas where several operations are concentrated
        {
            List<Operation> conflictoperation = new List<Operation>();
            List<Operation> operation_list = OperatorWriter.GetListOfOperations(operator_list);
            for(int i=0; i<operation_list.Count-1;i++)
            {
                int n = i + 1;
                int z = 0;
                if (operation_list[i].GetRouteType() != RouteType.Delivery)
                {
                    while (n < operation_list.Count)
                    {

                        bool busy = ArePotentialConflictiveRoutes(operation_list[i], operation_list[n]);
                        if (busy == true)
                        {
                            z++;
                        }

                        if (z >= 2)
                        {

                            conflictoperation.Add(operation_list[i]);
                            break;
                        }
                        n++;
                    }
                }
                
            }

            return conflictoperation;
        }

        public static List<Tuple<Operation, Operation, Point,double>> FindBusyAreasUnderARadius(List<Operator> operator_list, double radius, Country country)
        {
            List<Operation> whole_operation_list = OperatorWriter.GetListOfOperations(operator_list);
            List<Tuple<Operation, Operation, Point, double>> conflict_operations = new List<Tuple<Operation, Operation, Point, double>>();
            for (int i = 0; i < whole_operation_list.Count - 1; i++)
            {
                int n = i + 1;
                
                while (n < whole_operation_list.Count)
                {
                    bool temporal_conflictive = AreTemporalPotentialConflictiveRoutes(whole_operation_list[i], whole_operation_list[n]);
                    if (temporal_conflictive == true)
                    {
                        
                        bool delivery_operations = false;
                        if (whole_operation_list[i].GetRouteType() == RouteType.Delivery && whole_operation_list[n].GetRouteType() == RouteType.Delivery)
                        {
                            delivery_operations = true;
                        }
                        if (delivery_operations == false)
                        {
                            bool potentialroutes = ArePotentialBusyAreas(whole_operation_list[i], whole_operation_list[n], radius);
                            if (potentialroutes == true)
                            {
                                double minimum_distance = FindMinimumDistanceBetweenOperations(whole_operation_list[i], whole_operation_list[n]);
                                if (minimum_distance <= radius)
                                {
                                    Point mid_point = FindMidPointBetween2Operations(whole_operation_list[i], whole_operation_list[n]);
                                    
                                    bool inside_country = InPolygon.IsPointInPolygon(mid_point, country.GetLandside().GetCountryContour());
                                    if(inside_country==true)
                                    {
                                        double max_distance = FindMaximumDistanceFromAPoint(whole_operation_list[i], whole_operation_list[n], mid_point);
                                        Tuple<Operation, Operation, Point, double> conflicts = new Tuple<Operation, Operation, Point, double>(whole_operation_list[i], whole_operation_list[n], mid_point, max_distance);
                                        conflict_operations.Add(conflicts);
                                    }
                                    
                                }
                            }
                        }
                    }
                    n++;
              
                }
            }
            return conflict_operations;

        }

        public static void ExportDataToCSV(List<Operator> operator_list, string filename)
        {
            StreamWriter f1 = new StreamWriter(filename);
            var distance = GetDistanceStatistics(operator_list);
            var times = GetTimeStatistics(operator_list);
            var operations = GetOperationsStatistics(operator_list);
            f1.WriteLine(" ,VLOS operations,EVLOS operations, BVLOS operations, Delivery Operations"); // file header
            f1.WriteLine("Distances (meters),"+distance.Item1.ToString()+","+distance.Item2.ToString()+","+distance.Item3.ToString()+","+distance.Item4.ToString());
            f1.WriteLine("Times (minutes)," + times.Item1.ToString() + "," + times.Item2.ToString() + "," + times.Item3.ToString() + "," + times.Item4.ToString());
            f1.WriteLine("Operations," + operations.Item1.ToString() + "," + operations.Item2.ToString() + "," + operations.Item3.ToString() + "," + operations.Item4.ToString());
            f1.Close();
        }

        public static void ExportDataToXML(List<Operator> operator_list, string filename)
        {
            var distances = GetDistanceStatistics(operator_list);
            var times = GetTimeStatistics(operator_list);
            var operations = GetOperationsStatistics(operator_list);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            XmlWriter writer = XmlWriter.Create(filename, settings);
            writer.WriteStartDocument();
            writer.WriteStartElement("analysis"); // start of the element: analysis
            writer.WriteStartElement("VLOS-operations"); // start of the element: VLOS-operations
            writer.WriteElementString("mean-distance", distances.Item1.ToString());
            writer.WriteElementString("mean-time", times.Item1.ToString());
            writer.WriteElementString("number-of-operations", operations.Item1.ToString());
            writer.WriteEndElement(); // end of the element: VLOS-operations
            writer.WriteStartElement("EVLOS-operations"); // start of the element: EVLOS-operations
            writer.WriteElementString("mean-distance", distances.Item2.ToString());
            writer.WriteElementString("mean-time", times.Item2.ToString());
            writer.WriteElementString("number-of-operations", operations.Item2.ToString());
            writer.WriteEndElement(); // end of the element: EVLOS-operations
            writer.WriteStartElement("BVLOS-operations"); // start of the element: BVLOS-operations
            writer.WriteElementString("mean-distance", distances.Item3.ToString());
            writer.WriteElementString("mean-time", times.Item3.ToString());
            writer.WriteElementString("number-of-operations", operations.Item3.ToString());
            writer.WriteEndElement(); // end of the element: BVLOS-operations
            writer.WriteStartElement("delivery-operations"); // start of the element: delivery-operations
            writer.WriteElementString("mean-distance", distances.Item4.ToString());
            writer.WriteElementString("mean-time", times.Item4.ToString());
            writer.WriteElementString("number-of-operations", operations.Item4.ToString());
            writer.WriteEndElement(); // end of the element: delivery-operations
            writer.WriteEndElement(); // end of the element: analysis
            writer.Close();
        }
        
    }
}
