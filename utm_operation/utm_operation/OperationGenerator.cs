using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using utm_routes;
using utm_routes.BVLOS_routes;
using utm_routes.VLOS_routes;
using utm_routes.EVLOSRoutes;
using utm_routes.DeliveryRoutes;
using utm_country;
using utm_utils;
using utm_drone;
using System.Xml;

namespace utm_operation
{
    public class OperationGenerator
    {
        // Written by Lluís Xavier on 02/19/2017

        // This class is amied to generate the list of operators

        //Attributes

        private Country country;

        // parameters of the routes
        private VLOSParameters vlosparameters;
        private EVLOSParameters evlosparameters;
        private BVLOSParameters bvlosparameters;
        private DeliveryParameters deliveryparameters;

        // builder

        public OperationGenerator(Country con,  VLOSParameters vlos, EVLOSParameters evlos, BVLOSParameters bvlos, DeliveryParameters deliv)
        {
            this.country = con;
            this.vlosparameters = vlos;
            this.evlosparameters = evlos;
            this.bvlosparameters = bvlos;
            this.deliveryparameters = deliv;
        }
        //attributes
        

        // functions

        public Operation GenerateOperation(drone drone, OperationType typeofoperation,int parts ,Random rnd)
        {
             RouteGenerator routegenerator = new RouteGenerator(country, vlosparameters, evlosparameters, bvlosparameters, deliveryparameters);
            
            Operation newoperation = new Operation();
            if(typeofoperation==OperationType.VLOS)
            {
                VLOSRoute route = routegenerator.GenerateVLOSRoute(drone,parts,rnd);
                newoperation.SetVLOSRoute(route);
                newoperation.SetRouteType(RouteType.VLOS);
                newoperation.SetAircraft(drone);
            }
            if (typeofoperation == OperationType.EVLOS)
            {
                EVLOSRoute route = routegenerator.GenerateEVLOSRoute(drone, parts, rnd);
                newoperation.SetEVLOSRoute(route);
                newoperation.SetRouteType(RouteType.EVLOS);
                newoperation.SetAircraft(drone);
            }
            if(typeofoperation==OperationType.BVLOS)
            {
                BVLOSRoute route = routegenerator.GenerateBVLOSRoute(drone, parts,rnd);
                newoperation.SetBVLOSRoute(route);
                newoperation.SetRouteType(RouteType.BVLOS);
                newoperation.SetAircraft(drone);
            }
            if(typeofoperation==OperationType.VLOSorBVLOS)
            {
                int randomnumber = rnd.Next(0, 10); // decide whether is a vlos or a bvlos route by random number
                if(randomnumber<5) // in case of vlos route, random number is smaller than 5
                {
                    VLOSRoute route = routegenerator.GenerateVLOSRoute(drone, parts,rnd);
                    newoperation.SetVLOSRoute(route);
                    newoperation.SetRouteType(RouteType.VLOS);
                    newoperation.SetAircraft(drone);
                }
                if(randomnumber>=5) // otherwise, a bvlos route is created when the random number is bigger or equal than 5
                {
                    BVLOSRoute route = routegenerator.GenerateBVLOSRoute(drone, parts,rnd);
                    newoperation.SetBVLOSRoute(route);
                    newoperation.SetRouteType(RouteType.BVLOS);
                    newoperation.SetAircraft(drone);
                }
            }

            if(typeofoperation==OperationType.VLOSorEVLOS)
            {
                int randomnumber = rnd.Next(0, 10); // decide whether is a VLOS or a EVLOS operation.
                if(randomnumber<5) // in this case, the route is a VLOS
                {
                    VLOSRoute route = routegenerator.GenerateVLOSRoute(drone, parts, rnd);
                    newoperation.SetVLOSRoute(route);
                    newoperation.SetRouteType(RouteType.VLOS);
                    newoperation.SetAircraft(drone);
                }
                if(randomnumber>=5)
                {
                    EVLOSRoute route = routegenerator.GenerateEVLOSRoute(drone, parts, rnd);
                    newoperation.SetEVLOSRoute(route);
                    newoperation.SetRouteType(RouteType.EVLOS);
                    newoperation.SetAircraft(drone);
                }
            }

            if (typeofoperation == OperationType.EVLOSorBVLOS)
            {
                int randomnumber = rnd.Next(0, 10); // decide whether is a VLOS or a EVLOS operation.
                if (randomnumber < 5) // in this case, the route is a VLOS
                {
                    EVLOSRoute route = routegenerator.GenerateEVLOSRoute(drone, parts, rnd);
                    newoperation.SetEVLOSRoute(route);
                    newoperation.SetRouteType(RouteType.EVLOS);
                    newoperation.SetAircraft(drone);
                }
                if (randomnumber >= 5)
                {
                    BVLOSRoute route = routegenerator.GenerateBVLOSRoute(drone, parts, rnd);
                    newoperation.SetBVLOSRoute(route);
                    newoperation.SetRouteType(RouteType.BVLOS);
                    newoperation.SetAircraft(drone);
                }
            }

            if(typeofoperation==OperationType.VLOSorBVLOSorEVLOS)
            {
                int randomnumber = rnd.Next(0, 9); // decide whether is a VLOS, BVLOS or a EVLOS. Since there are three possibilities, the randomnumber goes from 0 to 9 in order to divide by 3.
                if (randomnumber < 3) // in this case, the route is a VLOS
                {
                    VLOSRoute route = routegenerator.GenerateVLOSRoute(drone, parts, rnd);
                    newoperation.SetVLOSRoute(route);
                    newoperation.SetRouteType(RouteType.VLOS);
                    newoperation.SetAircraft(drone);
                }
                if (randomnumber >= 3 || randomnumber<=5)
                {
                    EVLOSRoute route = routegenerator.GenerateEVLOSRoute(drone, parts, rnd);
                    newoperation.SetEVLOSRoute(route);
                    newoperation.SetRouteType(RouteType.EVLOS);
                    newoperation.SetAircraft(drone);
                }
                if (randomnumber >5) // in this case, the route is a VLOS
                {
                    BVLOSRoute route = routegenerator.GenerateBVLOSRoute(drone, parts, rnd);
                    newoperation.SetBVLOSRoute(route);
                    newoperation.SetRouteType(RouteType.BVLOS);
                    newoperation.SetAircraft(drone);
                }
            }

            if (typeofoperation==OperationType.Delivery)
            {
                DeliveryRoute route = routegenerator.GenerateDeliveryRoute(drone, rnd);
                newoperation.SetDeliveryRoutes(route);
                newoperation.SetRouteType(RouteType.Delivery);
                newoperation.SetAircraft(drone);
            }
            return newoperation;
        }
         
        public void AddTimeToRoute(Operation operation, Random rnd, double meantime, double stdDev, TimeSpan lowerlimit, TimeSpan upperlimit)
        {
            if(operation.GetRouteType()==RouteType.VLOS)
            {
                VLOSRoute route = operation.GetVLOSRoute();
               route= route.AddTime(route, rnd, meantime, stdDev, lowerlimit, upperlimit);
                operation.SetVLOSRoute(route);
                operation.SetStartTime(route.GetDiscretizedRoute()[0].GetTime());
                operation.SetFinalTime(route.GetDiscretizedRoute()[route.GetDiscretizedRoute().Count - 1].GetTime());
            }
            if (operation.GetRouteType() == RouteType.EVLOS)
            {
                EVLOSRoute route = operation.GetEVLOSRoute();
               route= route.AddTime(route, rnd, meantime, stdDev, lowerlimit, upperlimit);
                operation.SetEVLOSRoute(route);
                operation.SetStartTime(route.GetDiscretizedRoute()[0].GetTime());
                operation.SetFinalTime(route.GetDiscretizedRoute()[route.GetDiscretizedRoute().Count - 1].GetTime());
            }
            if(operation.GetRouteType()==RouteType.BVLOS)
            {
                BVLOSRoute route = operation.GetBVLOSRoute();
                //TimeSpan bvlosupperlimit = upperlimit - new TimeSpan(2, 00, 00);
                route = route.AddTime(route, rnd, meantime, stdDev, lowerlimit, upperlimit);
                operation.SetBVLOSRoute(route);
                operation.SetStartTime(route.GetDiscretizedRoute()[0].GetTime());
                operation.SetFinalTime(route.GetDiscretizedRoute()[route.GetDiscretizedRoute().Count - 1].GetTime());
            }
            if(operation.GetRouteType()==RouteType.Delivery)
            {
                DeliveryRoute route = operation.GetDeliveryRoute();
                route = route.AddTime(route, rnd, meantime, stdDev, lowerlimit, upperlimit);
                operation.SetDeliveryRoutes(route);
                operation.SetStartTime(route.GetDiscretizedRoute()[0].GetTime());
                operation.SetFinalTime(route.GetDiscretizedRoute()[route.GetDiscretizedRoute().Count - 1].GetTime());
            }
        }
        
         public bool IsRouteFeasible(Operation op, List<Operation> operations)
        {
            DateTime starttime = op.GetStartTime();
            DateTime finaltime = op.GetFinalTime(); ;
            bool goodroute = false;
            int n = 0;

            foreach(Operation oper in operations)
            {
                DateTime startop = oper.GetStartTime();
                DateTime finalop = oper.GetFinalTime();

                if(!(finaltime<startop || starttime>finalop))
                {
                    n++;
                    break;
                }
                
            }

            if(n==0)
            {
                goodroute = true;
            }

            return goodroute;

        }

        public List<Operation> GenerateListOfOperations(drone aircraft, OperationType typeofoperation, int numberofoperations,int parts ,double meantime, double stdDev, TimeSpan lowerlimit, TimeSpan upperlimit, Random rnd)
        {
            List<Operation> operationlist = new List<Operation>();
            int i = 0;
            while (i < numberofoperations)
            {
                int attemp = 0;
                bool goodroute = false;
                Operation newoperation = GenerateOperation(aircraft, typeofoperation, parts,rnd);
                Operation operationcopy = newoperation.GetCopy();
                while (goodroute == false && attemp<=20)
                {
                    AddTimeToRoute(operationcopy, rnd, meantime, stdDev, lowerlimit, upperlimit);
                    goodroute = IsRouteFeasible(operationcopy, operationlist);
                    if(goodroute==true)
                    {
                        newoperation = operationcopy.GetCopy();
                        operationlist.Add(newoperation);
                    }
                    else
                    {
                        operationcopy = newoperation.GetCopy();
                        attemp++;
                    }
                    
                }
                i++;
            }
            return operationlist;
        }

        

        
    }
}
