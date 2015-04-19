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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Maps.MapControl.WPF;
using DraggablePushpin;
using Newtonsoft.Json;

namespace BingApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //myMap.Loaded += (s, e) =>
            //{
            //    DraggablePin pin = new DraggablePin(myMap)
            //    {
            //        Location = myMap.Center
            //    };

            //    myMap.Children.Add(pin);
            //};
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
          //  Map.CenterProperty = "37.806029,-122.407007";
            //MyCenter.Center = "37.806029,-122.407007";
            var l=new Location();
            l.Latitude = 41.0913494;
            l.Longitude = -74.1851234;
            
            //var l = Microsoft.Maps.MapControl.WPF.Location;

            myMap.Center=l;

            string s = @"{""NAV_POSLLH"":
                {""iTOW"":76682250,
                ""lat"":410913510,
                ""long"":-741851233,
                ""height"":78657,
                ""hSML"":112941,
                ""hAcc"":2091,
                ""vAcc"":6656}}";


            //NAVPOSLLH nv=JsonConvert.DeserializeObject<NAVPOSLLH>(s);
            RootObject r = JsonConvert.DeserializeObject<RootObject>(s);

            Console.WriteLine(r.NAV_POSLLH.lat);


            

            // The pushpin to add to the map.
            //Pushpin pin = new DraggablePin(myMap); //Pushpin();
            //pin.Location = l;

            //// Adds the pushpin to the map.
            //myMap.Children.Add(pin);



        }

         private void MapWithPushpins_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Disables the default mouse double-click action.
            e.Handled = true;

            // Determin the location to place the pushpin at on the map.

            //Get the mouse click coordinates
            Point mousePosition = e.GetPosition(this);
            //Convert the mouse coordinates to a locatoin on the map
            Location pinLocation = myMap.ViewportPointToLocation(mousePosition);
            
            // The pushpin to add to the map.
            Pushpin pin = new DraggablePin(myMap);
            pin.Location = pinLocation;
            
            // Adds the pushpin to the map.
            myMap.Children.Add(pin);
        }


    }
}
