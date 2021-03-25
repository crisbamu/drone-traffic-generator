using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using utm_utils;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace utm_country
{
    public class XMLAreasFormat
    {
        public List<CountryContour> ReadContoursFromXML(String filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(country));

            // Declare an object variable of the type to be deserialized.
            country i;

            using (Stream reader = new FileStream(filename, FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                i = (country)serializer.Deserialize(reader);
            }

            return GetCountryContours(i);
           
        }


        public List<CountryContour> GetCountryContours(country i)
        {
            List<CountryContour> contours = new List<CountryContour>();
           foreach(countryContour cont in i.contour)
           {
               CountryContour contour = GetCountryContour(cont);
               contours.Add(contour);
           }

           
            return contours;
        }


        public CountryContour GetCountryContour(countryContour i)
        {
            CountryContour contour = new CountryContour();
            List<Point> contourpoints = new List<Point>();
           foreach(string point in i.coordinates)
           {
               Point coordinate = ConvertStringToPoint(point);
               contourpoints.Add(coordinate);
           }

           string type = i.type;
           if(type.Equals("Land"))
           {
               contour.SetCountryType("land");
           }
           if (type.Equals("Island"))
           {
               contour.SetCountryType("island");
           }

           contour.SetContour(contourpoints);
           return contour;
        }

        public Point ConvertStringToPoint(string information)
        {
            string[] words = information.Split(' ', ',');
            double latitude = Convert.ToDouble(words[0]);
            double longitude = Convert.ToDouble(words[1]);
            Point new_point = new Point(latitude, longitude);
            if (words.Length == 3)
            {
                double altitude = Convert.ToDouble(words[2]);
                new_point.SetAltitude(altitude);
            }
            return new_point;
        }

        public Point ConvertCoordinatesToPoint(string information)
        {
            string[] words = information.Split(' ', ',');
            double latitude = Convert.ToDouble(words[1]);
            double longitude = Convert.ToDouble(words[2]);
            Point new_point = new Point(latitude, longitude);
            return new_point;
        }

        // for contours

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class country
        {

            private countryContour[] contourField;

            private string nameField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("contour")]
            public countryContour[] contour
            {
                get
                {
                    return this.contourField;
                }
                set
                {
                    this.contourField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string name
            {
                get
                {
                    return this.nameField;
                }
                set
                {
                    this.nameField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class countryContour
        {

            private string[] pointField;

            private string[] coordinatesField;

            private string typeField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("point")]
            public string[] point
            {
                get
                {
                    return this.pointField;
                }
                set
                {
                    this.pointField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayItemAttribute("point", IsNullable = false)]
            public string[] coordinates
            {
                get
                {
                    return this.coordinatesField;
                }
                set
                {
                    this.coordinatesField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string type
            {
                get
                {
                    return this.typeField;
                }
                set
                {
                    this.typeField = value;
                }
            }
        }



        // for forbidden areas

        public List<ForbiddenArea> ReadForbiddenAreasFromXML(String filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(prohibitedareas));

            // Declare an object variable of the type to be deserialized.
            prohibitedareas i;

            using (Stream reader = new FileStream(filename, FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                i = (prohibitedareas)serializer.Deserialize(reader);
            }

            return GetListOfForbiddenAreas(i);

        }

        public List<ForbiddenArea> GetListOfForbiddenAreas(prohibitedareas i)
        {
            List<ForbiddenArea> listofforbiddenareas = new List<ForbiddenArea>();
            foreach(prohibitedareasPolygon polygons in i.polygons)
            {
                ForbiddenArea forbiddenarea = GetForbiddenArea(polygons);
                listofforbiddenareas.Add(forbiddenarea);
            }
            return listofforbiddenareas;
        }

        public ForbiddenArea GetForbiddenArea(prohibitedareasPolygon i)
        {
            ForbiddenArea forbiddenarea = new ForbiddenArea();
            string nameofthearea = i.name;
            forbiddenarea.SetName(nameofthearea);
            if(i.type.Equals("points"))
            {
                forbiddenarea.SetType(ForbiddenAreaType.points);
                List<Point> points = new List<Point>();
                foreach(string punto in i.point)
                {
                    Point areapoint = ConvertStringToPoint(punto);
                    points.Add(areapoint);
                }
                forbiddenarea.SetPolygon(points);
            }

            else if (i.type.Equals("circle"))
            {
                forbiddenarea.SetType(ForbiddenAreaType.circle);
                Point reference_point = ConvertStringToPoint(i.referencepoint);
                forbiddenarea.SetReferencePoint(reference_point);
                double radius = Convert.ToDouble(i.radius);
                forbiddenarea.SetRadius(radius);
            }

            return forbiddenarea;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute("prohibited-areas", Namespace = "", IsNullable = false)]
        public partial class prohibitedareas
        {

            private prohibitedareasPolygon[] polygonsField;

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayItemAttribute("polygon", IsNullable = false)]
            public prohibitedareasPolygon[] polygons
            {
                get
                {
                    return this.polygonsField;
                }
                set
                {
                    this.polygonsField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class prohibitedareasPolygon
        {

            private string referencepointField;

            private decimal radiusField;

            private bool radiusFieldSpecified;

            private string[] pointField;

            private string nameField;

            private string typeField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("reference-point")]
            public string referencepoint
            {
                get
                {
                    return this.referencepointField;
                }
                set
                {
                    this.referencepointField = value;
                }
            }

            /// <remarks/>
            public decimal radius
            {
                get
                {
                    return this.radiusField;
                }
                set
                {
                    this.radiusField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool radiusSpecified
            {
                get
                {
                    return this.radiusFieldSpecified;
                }
                set
                {
                    this.radiusFieldSpecified = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("point")]
            public string[] point
            {
                get
                {
                    return this.pointField;
                }
                set
                {
                    this.pointField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string name
            {
                get
                {
                    return this.nameField;
                }
                set
                {
                    this.nameField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string type
            {
                get
                {
                    return this.typeField;
                }
                set
                {
                    this.typeField = value;
                }
            }
        }





    }
}
