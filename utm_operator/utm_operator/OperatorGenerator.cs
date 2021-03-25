using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using utm_routes.BVLOS_routes;
using utm_routes.VLOS_routes;
using utm_routes.EVLOSRoutes;
using utm_routes.DeliveryRoutes;
using utm_country;
using utm_utils;
using utm_drone;
using utm_operation;
using System.IO;
using System.Xml;

namespace utm_operator
{
    public class OperatorGenerator
    {
        // Written by Lluís Xavier on 02/19/2017

        // This class is amied to generate the list of operators

        // Attributes

        private Country country;

        private VLOSParameters vlosparameters;
        private EVLOSParameters evlosparameters;
        private BVLOSParameters bvlosparameters;
        private DeliveryParameters deliveryparameters;
       
        

        // builder 

        public OperatorGenerator()
        {

        }

        public OperatorGenerator(Country con,  VLOSParameters vlos,EVLOSParameters evlos ,BVLOSParameters bvlos, DeliveryParameters deliv)
        {
            this.country = con;
            this.vlosparameters = vlos;
            this.evlosparameters = evlos;
            this.bvlosparameters = bvlos;
            this.deliveryparameters = deliv;
        }

        // functions

        public List<drone> GenerateFleetOfdrones(int numberofdrones, int startnumberofreference) // This function generate a fleet of drones
        {
            List<drone> drones = new List<drone>();
            for(int i=0;i<numberofdrones;i++)
            {
                drone newdrone = new drone();
                newdrone.SetIdentifier(Convert.ToString(startnumberofreference + i));
                drones.Add(newdrone);
            }

            return drones;
        }

        public List<Operator> GenerateOperatorsWithoutOperations(int bigoperators, int mediumoperators, int smalloperators, int startdroneidentity, int startopidentity, Random rnd)
        {
            // This function generates an operator but without the operations assigned to it. 

            List<Operator> operators = new List<Operator>();
            //OperationGenerator opgenerator = new OperationGenerator(country, citytolerance, vlosparameters, bvlosparameters, deliveryparameters);
            

            // for big operators
            for (int i = 0; i < bigoperators; i++)
            {
                int numberofdrones = rnd.Next(50, 100);
                List<drone> drones = GenerateFleetOfdrones(numberofdrones, startdroneidentity);
                string identity = Convert.ToString(startopidentity + i);
                Operator newoperator = new Operator(identity, null, drones);
                operators.Add(newoperator);
            }

            // for medium operators
            startdroneidentity = startdroneidentity + 10000;
            startopidentity = startopidentity + 1000;
            for (int i = 0; i < mediumoperators; i++)
            {
                int numberofdrones = rnd.Next(10, 50);
                List<drone> drones = GenerateFleetOfdrones(numberofdrones, startdroneidentity);
                string identity = Convert.ToString(startopidentity + i);
                Operator newoperator = new Operator(identity, null, drones);
                operators.Add(newoperator);
            }

            // for small operators
            startdroneidentity = startdroneidentity + 10000;
            startopidentity = startopidentity + 1000;
            for (int i = 0; i < smalloperators; i++)
            {
                int numberofdrones = rnd.Next(1, 10);
                List<drone> drones = GenerateFleetOfdrones(numberofdrones, startdroneidentity);
                string identity = Convert.ToString(startopidentity + i);
                Operator newoperator = new Operator(identity, null, drones);
                operators.Add(newoperator);
            }

            return operators;
        }

        public void WriteOperators(string filename,string foldername, List<Operator> operators)
        {
            // This function writes the operators in a filename provided by the user. 
            String directory = AppDomain.CurrentDomain.BaseDirectory;
            System.IO.DirectoryInfo di = new DirectoryInfo(directory + "\\" + foldername + "\\");

            if (!di.Exists)  // if it doesn't exist, create
                di.Create();

            StreamWriter writer = new StreamWriter(di+filename);
            //writer.WriteLine("opidentifier/routetype/droneidentifier/starttime/endtime/LatHome/LongHome/mavlinkfile");

            foreach (Operator op in operators)
            {
                foreach (Operation operation in op.GetOperations())
                {
                    writer.Write(op.GetIdentifier() + "\t");
                    writer.Write(operation.GetRouteType() + "\t");
                    writer.Write(operation.GetAircraft().GetIdentifier() + "\t");
                    writer.Write((operation.GetStartTime().ToString("HH:mm:ss")) + "\t");
                    writer.Write((operation.GetFinalTime().ToString("HH:mm:ss")) + "\t");
                    if(operation.GetRouteType().Equals("vlos"))
                    {
                        writer.Write(operation.GetVLOSRoute().GetOriginPoint().GetLatitude().ToString() + "\t");
                        writer.Write(operation.GetVLOSRoute().GetOriginPoint().GetLongitude().ToString() + "\t");
                    }

                    if (operation.GetRouteType().Equals("bvlos"))
                    {
                        writer.Write(operation.GetBVLOSRoute().GetOriginPoint().GetLatitude().ToString() + "\t");
                        writer.Write(operation.GetBVLOSRoute().GetOriginPoint().GetLongitude().ToString() + "\t");
                    }

                    if (operation.GetRouteType().Equals("delivery"))
                    {
                        writer.Write(operation.GetDeliveryRoute().GetOriginPoint().GetLatitude().ToString() + "\t");
                        writer.Write(operation.GetDeliveryRoute().GetOriginPoint().GetLongitude().ToString() + "\t");
                    }
                    writer.Write((operation.GetMavLinkIdentification()) + "\r\n");
                }

            }
            writer.Close();
        }



        // new functions

        public List<Operator> LoadOperators(string filename)
        {
            // This function loads the operators from a xml which has stored the operators and their operations. 
            List<Operator> listofoperators = new List<Operator>();
            XmlTextReader reader = new XmlTextReader(filename);
            XmlNodeType type;
            while (reader.Read())
            {
                type = reader.NodeType;
                if (reader.Name.Equals("operator") && type == XmlNodeType.Element)
                {
                    Operator newoperator = new Operator();
                    while (reader.MoveToNextAttribute())
                    {
                        if (reader.Name.Equals("name"))
                        {
                            newoperator.SetIdentifier(reader.Value);
                        }
                    }

                    while (reader.Name != "type-of-routes" && !(reader.Name.Equals("operator") && type == XmlNodeType.EndElement))
                    {
                        reader.Read();
                        type = reader.NodeType;
                    }
                    if (reader.Name.Equals("type-of-routes") && type == XmlNodeType.Element)
                    {
                        reader.Read();
                        if (reader.Value.Equals("VLOS"))
                        {
                            newoperator.SetOperatorType(OperatorType.VLOS);
                        }
                        if (reader.Value.Equals("EVLOS"))
                        {
                            newoperator.SetOperatorType(OperatorType.EVLOS);
                        }
                        if (reader.Value.Equals("BVLOS"))
                        {
                            newoperator.SetOperatorType(OperatorType.BVLOS);
                        }
                        if (reader.Value.Equals("VLOS/BVLOS"))
                        {
                            newoperator.SetOperatorType(OperatorType.VLOSandBVLOS);
                        }
                        if(reader.Value.Equals("VLOS/EVLOS"))
                        {
                            newoperator.SetOperatorType(OperatorType.VLOSandEVLOS);
                        }
                        if (reader.Value.Equals("EVLOS/BVLOS"))
                        {
                            newoperator.SetOperatorType(OperatorType.EVLOSandBVLOS);
                        }
                        if(reader.Value.Equals("VLOS/EVLOS/BVLOS"))
                        {
                            newoperator.SetOperatorType(OperatorType.VLOSandEVLOSandBVLOS);
                        }
                        if (reader.Value.Equals("Delivery"))
                        {
                            newoperator.SetOperatorType(OperatorType.Delivery);
                        }
                    }

                    while (reader.Name != "number-of-drones" && !(reader.Name.Equals("operator") && type == XmlNodeType.EndElement))
                    {
                        reader.Read();
                        type = reader.NodeType;
                    }
                    if (reader.Name.Equals("number-of-drones") && type == XmlNodeType.Element)
                    {
                        reader.Read();
                        newoperator.SetNumberOfDrones(Convert.ToInt32(reader.Value));
                    }

                    if (newoperator.GetNumberOfDrones() != 0 && newoperator.GetOperatorType() != 0 && newoperator.GetIdentifier() != null)
                    {
                        listofoperators.Add(newoperator);
                    }
                }
            }
            reader.Close();
            return listofoperators;
        }

        public OperationType TransformFromOperatorToOperationType(OperatorType type)
        {
            OperationType newtype = new OperationType();
            if(type==OperatorType.VLOS)
            {
                newtype = OperationType.VLOS;
            }
            if (type == OperatorType.EVLOS)
            {
                newtype = OperationType.EVLOS;
            }
            if(type==OperatorType.BVLOS)
            {
                newtype = OperationType.BVLOS;
            }
            if(type==OperatorType.VLOSandBVLOS)
            {
                newtype = OperationType.VLOSorBVLOS;
            }
            if (type == OperatorType.VLOSandEVLOS)
            {
                newtype = OperationType.VLOSorEVLOS;
            }
            if (type == OperatorType.EVLOSandBVLOS)
            {
                newtype = OperationType.EVLOSorBVLOS;
            }
            if (type == OperatorType.VLOSandEVLOSandBVLOS)
            {
                newtype = OperationType.VLOSorBVLOSorEVLOS;
            }
            if(type==OperatorType.Delivery)
            {
                newtype = OperationType.Delivery;
            }
            return newtype;
        }

        public void LoadOperations(List<Operator> operators, List<drone> dronelist, int parts,double meantime, double stdDev, TimeSpan lowerlimit, TimeSpan upperlimit, Random rnd)
        {
            OperationGenerator operation_generator = new OperationGenerator(country, vlosparameters, evlosparameters, bvlosparameters, deliveryparameters);
            int n = 23000000;
            int z = 0;
            foreach (Operator op in operators)
            {
                List<Operation> wholeoperationlist = new List<Operation>();
                List<drone> fleetofdrones = new List<drone>();
                int i = 0;
                while (i < op.GetNumberOfDrones())
                {
                    int randomdrone = rnd.Next(0, dronelist.Count);
                    drone newdrone = dronelist[randomdrone];
                    drone dronecopy = newdrone.GetCopy();
                    dronecopy.SetIdentifier(Convert.ToString(n + z + i));
                    OperatorType typeofoperator = op.GetOperatorType();
                    OperationType typeofoperation = TransformFromOperatorToOperationType(typeofoperator);
                    int numberofoperationsperdrone = rnd.Next(1, 4);
                    List<Operation> operationlist = operation_generator.GenerateListOfOperations(dronecopy, typeofoperation,numberofoperationsperdrone, parts, meantime, stdDev, lowerlimit, upperlimit, rnd);
                    fleetofdrones.Add(dronecopy);
                    wholeoperationlist.AddRange(operationlist);
                    i++;
                    
                }
                op.SetOperations(wholeoperationlist);
                op.SetFleetOfDrones(fleetofdrones);
                //z = z + 1000;
                z = z + i;
            }
          
        }

        public List<Operator> GenerateOperators(List<drone> drone_list,int big_operators, int medium_operators, int small_operators, int delivery_drones, int Operator_Identifier, int Drone_Identifier, int parts, double meantime, double stdDev, TimeSpan lowerlimit, TimeSpan upperlimit,Random rnd )
        {
            // This function generate the operators with their operations assigned. 
            List<Operator> operator_list = new List<Operator>();
            OperationGenerator operation_generator = new OperationGenerator(country,  vlosparameters,evlosparameters ,bvlosparameters, deliveryparameters);

            int n = 0;
            int x = 0;
            // for operators with large fleet of drones
            for (int i = 0; i < big_operators;i++)
            {
                Operator new_operator = new Operator();
                // new_operator's attributes
                List<drone> fleet_of_drones= new List<drone>();
                List<Operation> operation_list = new List<Operation>(); 

                new_operator.SetIdentifier(Convert.ToString(Operator_Identifier + n));
                    
                    int number_drones= rnd.Next(50, 100);

                    for (int z = 0; z < number_drones; z++)
                    {
                    int random_drone = rnd.Next(0, drone_list.Count);
                    drone selected_drone = drone_list[random_drone].GetCopy();
                    selected_drone.SetIdentifier(Convert.ToString(Drone_Identifier + x + z));
                    int operations_per_drone = rnd.Next(1, 4);
                    List<Operation> set_operations = operation_generator.GenerateListOfOperations(selected_drone, OperationType.VLOSorBVLOS, operations_per_drone,parts, meantime, stdDev, lowerlimit, upperlimit, rnd);
                    fleet_of_drones.Add(selected_drone);
                    operation_list.AddRange(set_operations);
                    }
                    new_operator.SetFleetOfDrones(fleet_of_drones);
                    new_operator.SetNumberOfDrones(number_drones);
                    new_operator.SetOperations(operation_list);
                    operator_list.Add(new_operator);
                n++;
                x = x + 1000;
            }
            
            n = n + 1000;
            // for operators with medium fleet of drones
            for (int i = 0; i < medium_operators;i++ )
            {
                Operator new_operator = new Operator();
                // new_operator's attributes
                List<drone> fleet_of_drones = new List<drone>();
                List<Operation> operation_list = new List<Operation>();

                new_operator.SetIdentifier(Convert.ToString(Operator_Identifier + n));

                int number_drones = rnd.Next(10, 50);

                for (int z = 0; z < number_drones; z++)
                {
                    int random_drone = rnd.Next(0, drone_list.Count);
                    drone selected_drone = drone_list[random_drone].GetCopy();
                    selected_drone.SetIdentifier(Convert.ToString(Drone_Identifier + x + z));
                    int operations_per_drone = rnd.Next(1, 4);
                    List<Operation> set_operations = operation_generator.GenerateListOfOperations(selected_drone, OperationType.VLOSorBVLOS, operations_per_drone, parts,meantime, stdDev, lowerlimit, upperlimit, rnd);
                    fleet_of_drones.Add(selected_drone);
                    operation_list.AddRange(set_operations);
                   
                }
                new_operator.SetFleetOfDrones(fleet_of_drones);
                new_operator.SetNumberOfDrones(number_drones);
                new_operator.SetOperations(operation_list);
                operator_list.Add(new_operator);
                n++;
                x = x + 1000;
            }

            
            n = n + 1000;
            // for operators with small fleet of drones
            for (int i = 0; i < small_operators; i++)
            {
                Operator new_operator = new Operator();
                // new_operator's attributes
                List<drone> fleet_of_drones = new List<drone>();
                List<Operation> operation_list = new List<Operation>();

                new_operator.SetIdentifier(Convert.ToString(Operator_Identifier + n));

                int number_drones = rnd.Next(1,10);

                for (int z = 0; z < number_drones; z++)
                {
                    int random_drone = rnd.Next(0, drone_list.Count);
                    drone selected_drone = drone_list[random_drone].GetCopy();
                    selected_drone.SetIdentifier(Convert.ToString(Drone_Identifier + x + z));
                    int operations_per_drone = rnd.Next(1, 4);
                    List<Operation> set_operations = operation_generator.GenerateListOfOperations(selected_drone, OperationType.VLOSorBVLOS, operations_per_drone, parts,meantime, stdDev, lowerlimit, upperlimit, rnd);
                    fleet_of_drones.Add(selected_drone);
                    operation_list.AddRange(set_operations);
                }
                new_operator.SetFleetOfDrones(fleet_of_drones);
                new_operator.SetNumberOfDrones(number_drones);
                new_operator.SetOperations(operation_list);
                operator_list.Add(new_operator);
                n++;
                x = x + 1000;
            }

            n = n + 1000;
            
            // for delivery flights
            Operator delivery_operator = new Operator();
            delivery_operator.SetIdentifier("Parcel_Company");
            // new_operator's attributes
            List<drone> delivery_fleet= new List<drone>();
            List<Operation> delivery_operations = new List<Operation>();
            for (int i = 0; i < delivery_drones; i++)
            {
                int random_drone = rnd.Next(0, drone_list.Count);
                drone selected_drone = drone_list[random_drone].GetCopy();
                selected_drone.SetIdentifier(Convert.ToString(Drone_Identifier + n + x));
                int operations_per_drone = rnd.Next(1, 4);
                List<Operation> set_operations = operation_generator.GenerateListOfOperations(selected_drone, OperationType.Delivery, operations_per_drone, parts,meantime, stdDev, lowerlimit, upperlimit, rnd);
                delivery_fleet.Add(selected_drone);
                delivery_operations.AddRange(set_operations);
                delivery_operator.SetFleetOfDrones(delivery_fleet);
                delivery_operator.SetNumberOfDrones(delivery_drones);
                delivery_operator.SetOperations(delivery_operations);
                
                n++;
               
            }
            operator_list.Add(delivery_operator);
            return operator_list;
        }


        // the following function creates a list of operators with their fleet of drones but without any operation
        public List<Operator> AssignFleetOfDrones(int big_operators, int medium_operators, int small_operators, int delivery_flights, int reference_operator_number,  Random rnd)
        {
            List<Operator> operator_list = new List<Operator>();
            int n = 0;
            // for big operators
            for (int i = 0; i < big_operators; i++ )
            {
                int number_of_drones = rnd.Next(50, 100);
                Operator new_operator = new Operator();
                new_operator.SetIdentifier(Convert.ToString(reference_operator_number + n));
                SetRandomlyOperatorType(new_operator, rnd);
                new_operator.SetNumberOfDrones(number_of_drones);
                operator_list.Add(new_operator);
                n++;
            }
            // for medium operators
            for (int i = 0; i < medium_operators; i++)
            {
                int number_of_drones = rnd.Next(10, 50);
                Operator new_operator = new Operator();
                new_operator.SetIdentifier(Convert.ToString(reference_operator_number + n));
                SetRandomlyOperatorType(new_operator, rnd);
                new_operator.SetNumberOfDrones(number_of_drones);
                operator_list.Add(new_operator);
                n++;
            }

            for (int i = 0; i < small_operators; i++)
            {
                int number_of_drones = rnd.Next(1, 10);
                Operator new_operator = new Operator();
                new_operator.SetIdentifier(Convert.ToString(reference_operator_number + n));
                SetRandomlyOperatorType(new_operator, rnd);
                new_operator.SetNumberOfDrones(number_of_drones);
                operator_list.Add(new_operator);
                n++;
            }

            
                
                Operator delivery_operator = new Operator();
                delivery_operator.SetIdentifier(Convert.ToString("parcel company"));
                delivery_operator.SetOperatorType(OperatorType.Delivery);
                delivery_operator.SetNumberOfDrones(delivery_flights);
                operator_list.Add(delivery_operator);
            
                return operator_list;

        }

        public void SetRandomlyOperatorType(Operator op, Random rnd)  // this function chooses randomly the type of operator inserted. It is not valid for delivery flights!
        {
            // This function assigns randomly a type to a operator. 
            int operator_type = rnd.Next(0, 42);
            if(operator_type<7)
            {
                op.SetOperatorType(OperatorType.BVLOS);
            }

            if (operator_type >=7 && operator_type<14)
            {
                op.SetOperatorType(OperatorType.EVLOS);
            }

            if (operator_type >= 14 && operator_type < 21)
            {
                op.SetOperatorType(OperatorType.VLOS);
            }

            if (operator_type >= 21 && operator_type < 28)
            {
                op.SetOperatorType(OperatorType.VLOSandBVLOS);
            }

            if (operator_type >= 28 && operator_type < 35)
            {
                op.SetOperatorType(OperatorType.VLOSandEVLOS);
            }

            if (operator_type >= 35 && operator_type < 42)
            {
                op.SetOperatorType(OperatorType.VLOSandEVLOSandBVLOS);
            }


        }

        
    }
}
