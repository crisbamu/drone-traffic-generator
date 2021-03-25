using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using utm_routes;
using utm_routes.VLOS_routes;
using utm_routes.BVLOS_routes;
using utm_routes.DeliveryRoutes;
using utm_routes.EVLOSRoutes;
using utm_country;
using utm_operator;
using utm_drone;
using System.IO;

namespace DroneSimulator
{
    /// <summary>
    /// Lógica de interacción para RouteParameters.xaml
    /// </summary>
    public partial class RouteParameters : Window
    {
        public RouteParameters()
        {
            InitializeComponent();
        }

        //statics

      
       static  String directory = AppDomain.CurrentDomain.BaseDirectory;
       static System.IO.DirectoryInfo di = new DirectoryInfo(directory + "\\Images\\");

        // attributes

        VLOSParameters vlosparameters;
        EVLOSParameters evlosparameters;
        BVLOSParameters bvlosparameters;
        DeliveryParameters deliveryparameters;

        
        

        //setters and getters

        public void SetVLOSParameters(VLOSParameters vlos)
        {
            this.vlosparameters = vlos;
        }

        public void SetEVLOSParameters(EVLOSParameters evlos)
        {
            this.evlosparameters = evlos;
        }

        public void SetBVLOSParameters(BVLOSParameters bvlos)
        {
            this.bvlosparameters = bvlos;
        }

        public void SetDeliveryParameters(DeliveryParameters delivery)
        {
            this.deliveryparameters = delivery;
        }

        public VLOSParameters GetVLOSParameters()
        {
            return this.vlosparameters;
        }

        public EVLOSParameters GetEVLOSParameters()
        {
            return this.evlosparameters;
        }

        public BVLOSParameters GetBVLOSParameters()
        {
            return this.bvlosparameters;
        }

        public DeliveryParameters GetDeliveryParameters()
        {
            return this.deliveryparameters;
        }

        // functions

        bool vlos_check = false;
        bool bvlos_check = false;
        bool evlos_check = false;
        bool delivery_check = false;

        private void Default_button_Click(object sender, RoutedEventArgs e)
        {
            vlosmaxdist.Text = "400";
            vlosmindist.Text = "200";
            vlossecondmax.Text = "300";
            vlossecondmin.Text = "100";
            vlosangle.Text = "120";
            vlosrectmax.Text = "200";
            vlosrectmin.Text = "100";
            vlosmaxalt.Text = "120";
            vlosminalt.Text = "30";
            
        }



        private void button1_Click(object sender, RoutedEventArgs e)
        {
            bvlosmaxdist.Text = "11000";
            bvlosmindist.Text = "8000";
            bvlosbetmaxdist.Text = "4000";
            bvlosbetmindist.Text = "1000";
            bvlosangle.Text = "120";
            maxrectangle.Text = "1000";
            minrectangle.Text = "700";
            bvlosmaxnum.Text = "5";
            bvlosminnum.Text = "1";
            bvlosmaxalt.Text = "120";
            bvlosminalt.Text = "30";
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            delivmaxdist.Text = "15000";
            delivmaxalt.Text = "120";
            delivminalt.Text = "30";
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            // we recollect the parameters of each route

            if (vlos_check==true && evlos_check==true && bvlos_check==true && delivery_check==true)
            {
                // for vlos routes

                double vlosmaxidist = Convert.ToDouble(vlosmaxdist.Text);
                double vlosminidist = Convert.ToDouble(vlosmindist.Text);
                double vlossecondmaxi = Convert.ToDouble(vlossecondmax.Text);
                double vlossecondmini = Convert.ToDouble(vlossecondmin.Text);
                double vlosangler = Convert.ToDouble(vlosangle.Text);
                double vlosrectmaxi = Convert.ToDouble(vlosrectmax.Text);
                double vlosrectmini = Convert.ToDouble(vlosrectmin.Text);
                double vlosmaxialt = Convert.ToDouble(vlosmaxalt.Text);
                double vlosminialt = Convert.ToDouble(vlosminalt.Text);
                vlosparameters = new VLOSParameters(vlosmaxidist, vlosminidist, vlossecondmaxi, vlossecondmini, vlosangler, vlosrectmaxi, vlosrectmini, vlosmaxialt, vlosminialt);

                // for evlos routes

                double evlosmaxidist = Convert.ToDouble(evlosmaxdist.Text);
                double evlosminidist = Convert.ToDouble(evlosmindist.Text);
                double evlossecondmaxi = Convert.ToDouble(evlossecondmax.Text);
                double evlossecondmini = Convert.ToDouble(evlossecondmin.Text);
                double evlosangler = Convert.ToDouble(evlosangle.Text);
                double evlosrectmaxi = Convert.ToDouble(evlosrectmax.Text);
                double evlosrectmini = Convert.ToDouble(evlosrectmin.Text);
                double evlosmaxialt = Convert.ToDouble(evlosmaxalt.Text);
                double evlosminialt = Convert.ToDouble(evlosminalt.Text);
                evlosparameters = new EVLOSParameters(evlosmaxidist, evlosminidist, evlossecondmaxi, evlossecondmini, evlosangler, evlosrectmaxi, evlosrectmini, evlosmaxialt, evlosminialt);

                // for bvlos routes

                double bvlosmaxidist = Convert.ToDouble(bvlosmaxdist.Text);
                double bvlosminidist = Convert.ToDouble(bvlosmindist.Text);
                double bvlosbetmaxidist = Convert.ToDouble(bvlosbetmaxdist.Text);
                double bvlosbetminidist = Convert.ToDouble(bvlosbetmindist.Text);
                double bvlosangler = Convert.ToDouble(bvlosangle.Text);
                double bvlosmaxirect = Convert.ToDouble(maxrectangle.Text);
                double bvlosminirect = Convert.ToDouble(minrectangle.Text);
                int bvlosmaxinum = Convert.ToInt32(bvlosmaxnum.Text);
                int bvlosmininum = Convert.ToInt32(bvlosminnum.Text);
                double bvlosmaxialt = Convert.ToDouble(bvlosmaxalt.Text);
                double bvlosminialt = Convert.ToDouble(bvlosminalt.Text);
                bvlosparameters = new BVLOSParameters(bvlosmaxinum, bvlosmininum, bvlosmaxidist, bvlosminidist, bvlosbetmaxidist, bvlosbetminidist, bvlosangler ,bvlosmaxirect, bvlosminirect, bvlosmaxialt, bvlosminialt);

                // for delivery routes

                double deliverymaxdist = Convert.ToDouble(delivmaxdist.Text);
                double deliverymaxalt = Convert.ToDouble(delivmaxalt.Text);
                double deliveryminalt = Convert.ToDouble(delivminalt.Text);
                deliveryparameters = new DeliveryParameters(deliverymaxdist, deliverymaxalt, deliveryminalt);
                this.Close();
            }
            else
            {
                MessageBox.Show("There are some mistakes in the parameters inserted. Please, check them out again.");
            }


        }


        private void somethingchanged(object sender, TextChangedEventArgs e)
        {
            int n = 0;
            double numbers;

            bool result = double.TryParse(vlosmaxdist.Text, out numbers);
            if (result && numbers > 0)
            { n++; }
            result = double.TryParse(vlosmindist.Text, out numbers);
            if (result && numbers > 0)
            { n++; }
            result = double.TryParse(vlossecondmax.Text, out numbers);
            if (result && numbers > 0)
            { n++; }
            result = double.TryParse(vlossecondmin.Text, out numbers);
            if (result && numbers > 0)
            { n++; }
            result = double.TryParse(vlosangle.Text, out numbers);
            if (result && numbers > 0)
            { n++; }
            result = double.TryParse(vlosrectmax.Text, out numbers);
            if (result && numbers > 0)
            { n++; }
            result = double.TryParse(vlosrectmin.Text, out numbers);
            if (result && numbers > 0)
            { n++; }
            result = double.TryParse(vlosmaxalt.Text, out numbers);
            if (result && numbers > 0)
            { n++; }
            result = double.TryParse(vlosminalt.Text, out numbers);
            if (result && numbers > 0)
            { n++; }


            if (n == 9)
            {
                vloscheck.Content = "\u2714";
                vlos_check = true;
            }
            else
            {
                vloscheck.Content = "\u2718";
                vlos_check = false;
            }

            


        }

        private void bvloschanged(object sender, TextChangedEventArgs e)
        {

            int n = 0;
            double numbers;
            int num;
            bool result = double.TryParse(bvlosmaxdist.Text, out numbers);
            if (result && numbers>0)
            { n++; }
            result = double.TryParse(bvlosmindist.Text, out numbers);
            if (result && numbers>0)
            { n++; }
            result = double.TryParse(bvlosbetmaxdist.Text, out numbers);
            if (result && numbers>0)
            { n++; }
            result = double.TryParse(bvlosbetmindist.Text, out numbers);
            if (result && numbers > 0)
            { n++; }
            result = double.TryParse(bvlosangle.Text, out numbers);
            if (result && numbers > 0)
            { n++; }
            result = double.TryParse(maxrectangle.Text, out numbers);
            if (result && numbers > 0)
            { n++; }
            result = double.TryParse(minrectangle.Text, out numbers);
            if (result && numbers > 0)
            { n++; }
            result = double.TryParse(bvlosmaxalt.Text, out numbers);
            if (result && numbers > 0)
            { n++; }
            result = double.TryParse(bvlosminalt.Text, out numbers);
            if (result && numbers > 0)
            { n++; }

            result = int.TryParse(bvlosminnum.Text, out num);
            if (result && numbers > 0)
            { n++; }

            if (n == 10)
            {
                bvloscheck.Content = "\u2714";
                bvlos_check = true;
            }
            else
            {
                bvloscheck.Content = "\u2718";
                delivery_check = false;
            }

        }

        private void deliverchanged(object sender, TextChangedEventArgs e)
        {
            int n = 0;
            double numbers;
            bool result = double.TryParse(delivmaxdist.Text, out numbers);
            if (result && numbers > 0)
            { n++; }
            result = double.TryParse(delivmaxalt.Text, out numbers);
            if (result && numbers > 0)
            { n++; }
            result = double.TryParse(delivminalt.Text, out numbers);
            if (result && numbers > 0)
            { n++; }

            if (n == 3)
            {
                deliverycheck.Content = "\u2714";
                delivery_check = true;
            }
            else
            {
                deliverycheck.Content = "\u2718";
                delivery_check = false;
            }
        }

      

        private void vlosmaxdist_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri(di + "\\VLOSfirstsegment.png");
            b.EndInit();
            vlosimage.Source = b;

        }

        private void vlossecondmaxdist_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri(di + "\\VLOSsecondsegment.png");
            b.EndInit();
            vlosimage.Source = b;
        }

        private void vlosanglerange_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri(di + "\\VLOSanglerange.png");
            b.EndInit();
            vlosimage.Source = b;
        }

        private void vlosrectangledimensions_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri(di + "\\VLOSrectangledimensions.png");
            b.EndInit();
            vlosimage.Source = b;
        }

        private void vlosheight_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri(di + "\\VLOSheight.png");
            b.EndInit();
            vlosimage.Source = b;
        }

        private void bvlosmaxdist_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri(di + "\\BVLOSfirstsegment.png");
            b.EndInit();
            bvlosimage.Source = b;
        }

        private void bvlosbetmaxdist_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri(di + "\\BVLOSbetdist.png");
            b.EndInit();
            bvlosimage.Source = b;

        }

        private void bvlosheight_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri(di + "\\VLOSheight.png");
            b.EndInit();
            bvlosimage.Source = b;
        }

        private void bvlossecondmaxdist_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void bvlosanglerange_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri(di + "\\bvlosangle.png");
            b.EndInit();
            bvlosimage.Source = b;
        }

        private void bvlosrectangledimensions_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri(di + "\\bvloscan.png");
            b.EndInit();
            bvlosimage.Source = b;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Default_button_evlos_Click(object sender, RoutedEventArgs e)
        {
            evlosmaxdist.Text = "800";
            evlosmindist.Text = "500";
            evlossecondmax.Text = "300";
            evlossecondmin.Text = "100";
            evlosangle.Text = "120";
            evlosrectmax.Text = "200";
            evlosrectmin.Text = "100";
            evlosmaxalt.Text = "120";
            evlosminalt.Text = "30";
        }

        private void evloschanged(object sender, TextChangedEventArgs e)
        {
            int n = 0;
            double numbers;

            bool result = double.TryParse(evlosmaxdist.Text, out numbers);
            if (result && numbers > 0)
            { n++; }
            result = double.TryParse(evlosmindist.Text, out numbers);
            if (result && numbers > 0)
            { n++; }
            result = double.TryParse(evlossecondmax.Text, out numbers);
            if (result && numbers > 0)
            { n++; }
            result = double.TryParse(evlossecondmin.Text, out numbers);
            if (result && numbers > 0)
            { n++; }
            result = double.TryParse(evlosangle.Text, out numbers);
            if (result && numbers > 0)
            { n++; }
            result = double.TryParse(evlosrectmax.Text, out numbers);
            if (result && numbers > 0)
            { n++; }
            result = double.TryParse(evlosrectmin.Text, out numbers);
            if (result && numbers > 0)
            { n++; }
            result = double.TryParse(evlosmaxalt.Text, out numbers);
            if (result && numbers > 0)
            { n++; }
            result = double.TryParse(evlosminalt.Text, out numbers);
            if (result && numbers > 0)
            { n++; }


            if (n == 9)
            {
                evloscheck.Content = "\u2714";
                evlos_check = true;
            }
            else
            {
                evloscheck.Content = "\u2718";
                evlos_check = false;
            }


        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void evlosmaxdist_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri(di + "\\VLOSfirstsegment.png");
            b.EndInit();
            evlosimage.Source = b;
        }

        private void evlossecondmaxdist_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri(di + "\\VLOSsecondsegment.png");
            b.EndInit();
            evlosimage.Source = b;
        }

        private void evlosanglerange_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri(di + "\\VLOSanglerange.png");
            b.EndInit();
            evlosimage.Source = b;
        }

        private void evlosrectangledimensions_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri(di + "\\VLOSrectangledimensions.png");
            b.EndInit();
            evlosimage.Source = b;
        }

        private void evlosheight_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri(di + "\\VLOSheight.png");
            b.EndInit();
            evlosimage.Source = b;
        }

        private void deliveryaltitudedown(object sender, MouseButtonEventArgs e)
        {
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri(di + "\\VLOSheight.png");
            b.EndInit();
            deliveryimage.Source = b;
        }

        

        private void delivmaxdist_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri(di + "\\delivery_parametrization.png");
            b.EndInit();
            deliveryimage.Source = b;

        }

        private void bvlosnumberareas_down(object sender, MouseButtonEventArgs e)
        {
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri(di + "\\numberofareas.png");
            b.EndInit();
            bvlosimage.Source = b;
        }
    }
}
