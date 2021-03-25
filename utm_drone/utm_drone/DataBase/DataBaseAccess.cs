using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using utm_drone;
using System.Data.OleDb;
using System.Data;

namespace utm_drone.DataBase
{
    public class DataBaseAccess
    {
        // Written by Lluís Xavier Herranz on 04/16/2017

        // This class aims at accessing to the drone database in order to obtain the characteristics of realistic drones.

        // Attributes
        static string Databasename = "DroneDataBase.mdf";  // name of the database which contains the drones.
          static OleDbConnection cnx;
        //System.Data.SqlClient.SqlConnection con;

        // function


        public int OpenDataBase() // opens the database and returns 0 if everything went well, otherwise returns -1
        {
            //con = new System.Data.SqlClient.SqlConnection();
            //con.ConnectionString = "DataSource=.\\SQLEXPRESS; AttachDbFilename =DroneDataBase.mdf;Integrated Security=True;Connect Timeout=30;User Instance=True";
            //con.Open();
            //return 0;
            try
            {
                string cnxStr = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + Databasename + ";Persist Security Info=False;";
                cnx = new OleDbConnection(cnxStr);
                cnx.Open();
                return 0;
            }
            catch (OleDbException)
            {
                return -1;
            }
        }

        public List<drone> SelectUpToVelocity( double velocity)
        {
            List<drone> dronelist = new List<drone>();
            string query = "SELECT * FROM Table WHERE Max Cruising Speed <=" + velocity;
            DataTable res = new DataTable();
            OleDbDataAdapter adp = new OleDbDataAdapter(query, cnx);
            adp.Fill(res);
            int i = 0;
            while (i < res.Rows.Count)
            {
                drone newdrone = new drone();
                string nombre = Convert.ToString(res.Rows[i]["Model"]);
                newdrone.SetModel(nombre);
                double MaxCruisingSpeed = Convert.ToDouble(res.Rows[i]["Max Cruising Speed"]);
                newdrone.SetMaxCruisingSpeed(MaxCruisingSpeed);
                double MaxClimbingSpeed = Convert.ToDouble(res.Rows[i]["Ascend Speed"]);
                newdrone.SetMaxClimbingSpeed(MaxClimbingSpeed);
                double MaxDescendSpeed = Convert.ToDouble(res.Rows[i]["Descend Speed"]);
                newdrone.SetMaxDescendingSpeed(MaxDescendSpeed);
                double Ceiling = Convert.ToDouble(res.Rows[i]["Ceiling"]);
                newdrone.SetMaxHeight(Ceiling);
                dronelist.Add(newdrone);
                i = i + 1;
            }
            return dronelist;

        }


        public void CloseDataBase()
        {
            cnx.Close();
        }

        public List<drone> GenerateDronesUpToVelocity(double velocity)
        {
            OpenDataBase();
            List<drone> dronelist = SelectUpToVelocity(velocity);
            CloseDataBase();
            return dronelist;
        }

    }
}
