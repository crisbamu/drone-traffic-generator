using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace utm_drone
{
   public class XMLDroneReader
    {
        // Written by Lluís Xavier Herranz on 04/17/2017

        // This class aims at reading the drones from a xml

        // functions

        public List<drone> LoadDroneList(string filename)
        {
            //String directory = AppDomain.CurrentDomain.BaseDirectory;
            //System.IO.DirectoryInfo di = new DirectoryInfo(directory + "\\Drones\\");
            List<drone> dronelist = new List<drone>();
            XmlTextReader reader = new XmlTextReader(filename);
            XmlNodeType type;

            while(reader.Read())
            {
                type = reader.NodeType;

                if(reader.Name.Equals("drone") && type== XmlNodeType.Element)
                {
                    drone newdrone = new drone();
                    while(reader.MoveToNextAttribute())
                    {
                        if(reader.Name.Equals("model"))
                        {
                            newdrone.SetModel(reader.Value);
                        }
                    }
                    // for cruising speed
                    while(reader.Name!= "max-Cruising-Speed")
                    {
                        reader.Read();
                        type = reader.NodeType;
                    }
                    if(reader.Name.Equals("max-Cruising-Speed")&&type==XmlNodeType.Element)
                    {
                        reader.Read();
                        newdrone.SetMaxCruisingSpeed(Convert.ToDouble(reader.Value));
                    }

                    while (reader.Name != "min-Cruising-Speed")
                    {
                        reader.Read();
                        type = reader.NodeType;
                    }
                    if (reader.Name.Equals("min-Cruising-Speed") && type == XmlNodeType.Element)
                    {
                        reader.Read();
                        newdrone.SetMinCruisingSpeed(Convert.ToDouble(reader.Value));
                    }
                    // for climbing speed
                    while (reader.Name != "climb-speed")
                    {
                        reader.Read();
                        type = reader.NodeType;
                    }
                    if (reader.Name.Equals("climb-speed") && type == XmlNodeType.Element)
                    {
                        reader.Read();
                        newdrone.SetMaxClimbingSpeed(Convert.ToDouble(reader.Value));
                    }
                    // for descending speed
                    while (reader.Name != "descend-speed")
                    {
                        reader.Read();
                        type = reader.NodeType;
                    }
                    if (reader.Name.Equals("descend-speed") && type == XmlNodeType.Element)
                    {
                        reader.Read();
                        newdrone.SetMaxDescendingSpeed(Convert.ToDouble(reader.Value));
                    }
                    dronelist.Add(newdrone);
                }
            }
            
            reader.Close();
            return dronelist;
        }
    }
}
