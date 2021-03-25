using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
using utm_drone;
using utm_operation;



namespace utm_operator
{
    public class Operator
    {
        //Attributes
        private string Identifier;
       
        private List<Operation> Operations;
        //private int NumberOfOperations;
        private List<drone> Fleetofdrones;
        private int NumberOfDrones;
        private OperatorType OperatorType;

        //Builder

        public Operator ()
        {   
        }

        public Operator (string id,  List<Operation> list, List<drone> fleet)
        {
            this.Identifier = id;
            this.Operations = list;
            this.Fleetofdrones = fleet;
        }

        //Setters and Getters

        public void SetIdentifier(string id)
        { this.Identifier = id; }

        public void SetOperations(List<Operation> list)
        { this.Operations = list;  }

        public void SetFleetOfDrones(List<drone> drones)
        { this.Fleetofdrones = drones; }

        public void SetOperatorType(OperatorType type)
        {
            this.OperatorType = type;
        }

        public void SetNumberOfDrones(int number)
        {
            this.NumberOfDrones = number;
        }

        public string GetIdentifier()
        { return this.Identifier; }

        public List<Operation> GetOperations()
        { return this.Operations; }

        public List<drone> GetFleetOfDrones()
        { return this.Fleetofdrones; }

        public int GetNumberOfDrones()
        {
            return this.NumberOfDrones;
        }


        public OperatorType GetOperatorType()
        { return this.OperatorType; }

       

       

       

    }
}
