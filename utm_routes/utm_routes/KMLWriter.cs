using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using utm_routes;
using utm_routes.VLOS_routes;
using utm_routes.BVLOS_routes;
using utm_routes.DeliveryRoutes;
using System.Xml;

namespace utm_routes
{
    public class KMLWriter
    {
        // Written by Lluís Xavier Herranz on 12/22/2016

        // This class stores a route in a kml file

        
        

        public void WriteRoute(string filename, List<BVLOSRoute> bvlos, List<VLOSRoute> vlos)
        {

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;

            XmlWriter writer = XmlWriter.Create(filename, settings);
            writer.WriteStartDocument();

            writer.WriteStartElement("kml", "http://www.opengis.net/kml/2.2");


            writer.WriteStartElement("Document"); // start of the element: Document

            int n = 0;
            foreach (BVLOSRoute bvlosroute in bvlos)
            {

                writer.WriteStartElement("Placemark"); // start of the element: Placemark

                writer.WriteElementString("name", "flight " + n);  // start of the element: name
                writer.WriteElementString("description", "bvlos flight route"); // start of the element: description

                writer.WriteStartElement("LineString"); // start of the element: LineString

                writer.WriteElementString("altitudeMode", "relativeToGround");  // start of the element: altitudeMode
                writer.WriteElementString("extrude", "1"); // start of the element: extrude

                writer.WriteStartElement("coordinates");  // start of the element: coordinates

                double altitude = bvlosroute.GetAltitude();

                writer.WriteString(bvlosroute.GetOriginPoint().GetLongitude() + "," + bvlosroute.GetOriginPoint().GetLatitude() + "," + altitude + " ");
                writer.WriteString(bvlosroute.GetFirstIntermediate().GetLongitude() + "," + bvlosroute.GetFirstIntermediate().GetLatitude() + "," + altitude + " ");

                foreach (BVLOSPoint point in bvlosroute.GetBVlosList())
                {
                    writer.WriteString(point.GetPoint().GetLongitude() + "," + point.GetPoint().GetLatitude() + "," + altitude + " ");
                }

                writer.WriteString(bvlosroute.GetSecondIntermediate().GetLongitude() + "," + bvlosroute.GetSecondIntermediate().GetLatitude() + "," + altitude + " ");
                writer.WriteString(bvlosroute.GetOriginPoint().GetLongitude() + "," + bvlosroute.GetOriginPoint().GetLatitude() + "," + altitude + " ");

                writer.WriteEndElement(); // end of the element: coordinates


                writer.WriteEndElement(); // start of the element: LineString

                writer.WriteEndElement(); // end of the element: Placemark
                n++;
            }


            foreach (VLOSRoute vlosroute in vlos)
            {

                writer.WriteStartElement("Placemark"); // start of the element: Placemark

                writer.WriteElementString("name", "flight " + n);  // start of the element: name
                writer.WriteElementString("description", " vlos flight route"); // start of the element: description

                writer.WriteStartElement("LineString"); // start of the element: LineString

                writer.WriteElementString("altitudeMode", "relativeToGround");  // start of the element: altitudeMode
                writer.WriteElementString("extrude", "1"); // start of the element: extrude

                writer.WriteStartElement("coordinates");  // start of the element: coordinates

                double altitude = vlosroute.GetAltitude();


                writer.WriteString(vlosroute.GetOriginPoint().GetLongitude() + "," + vlosroute.GetOriginPoint().GetLatitude() + "," + altitude + " ");
                writer.WriteString(vlosroute.GetFirstPoint().GetLongitude() + "," + vlosroute.GetFirstPoint().GetLatitude() + "," + altitude + " ");
                writer.WriteString(vlosroute.GetSecondPoint().GetLongitude() + "," + vlosroute.GetSecondPoint().GetLatitude() + "," + altitude + " ");
                writer.WriteString(vlosroute.GetOriginPoint().GetLongitude() + "," + vlosroute.GetOriginPoint().GetLatitude() + "," + altitude + " ");

                writer.WriteEndElement(); // end of the element: coordinates


                writer.WriteEndElement(); // start of the element: LineString

                writer.WriteEndElement(); // end of the element: Placemark
                n++;
            }

            writer.WriteEndElement(); // end of the element: Document
            writer.WriteEndElement();  // end of the element: kml



            writer.Close();


        }


        
    }
}
