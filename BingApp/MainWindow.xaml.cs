﻿using System;
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

using System.IO.Ports;
using AustinHarris.JsonRpc;


namespace BingApp
{

    public class ExampleCalculatorService : JsonRpcService
    {
        MainWindow main;

        public void SetMain(MainWindow m)
        {
            main = m;
        }

        [JsonRpcMethod]
        private double add(double l, double r)
        {
            return l + r;
        }

         [JsonRpcMethod]
        private string getPoints()
        {
          
             //calls action on main thread. 
            Application.Current.Dispatcher.Invoke
                (new Action(   () => main.getPoints()  )  );
            // Application.Current.MainWindow
            return  "cant return anything.";
        }


    }
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort m_port=new SerialPort();
        string m_selectedPort = "";
        ExampleCalculatorService rpc;
        AsyncCallback rpcResultHandler = new AsyncCallback(_ => Console.WriteLine(((JsonRpcStateAsync)_).Result));

        public void getPoints()
        {
            string s="";

            foreach (var v in myMap.Children)
            {
                if (v is Pushpin)
                {
                    Pushpin pin = (Pushpin)v;

                    Console.WriteLine("{{{0}, {1}}}", pin.Location.Latitude, pin.Location.Longitude);
                    //if (m_port.IsOpen)
                    if(true)
                    {


                        s = s+string.Format("{{{0}, {1}}}", pin.Location.Latitude, pin.Location.Longitude);
                        s=s+",";
                        Console.WriteLine("{0}", s);

                     //   m_port.WriteLine(s);


                    }

                }
            }


            Console.WriteLine("sending:");

            Console.WriteLine('{'+s.TrimEnd(',')+'}');

        }

        public List<string> GetAllPorts()
        {
            List<String> allPorts = new List<String>();
            foreach (String portName in System.IO.Ports.SerialPort.GetPortNames())
            {
                allPorts.Add(portName);
                ports.Items.Add(portName);
            }
            return allPorts;
        }

        void openPort(string comport)
        {

            m_port.PortName = comport;
            m_port.BaudRate = 38400;
            m_port.Parity = Parity.None;
            m_port.StopBits = StopBits.One;
            m_port.DataBits = 8;
            m_port.Handshake = Handshake.None;
            m_port.DataReceived += new SerialDataReceivedEventHandler(mySerialPort_DataReceived);
            m_port.Open();
        }

        private void mySerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string s = sp.ReadExisting();




            // next i want to display the data in s in a textbox. textbox1.text=s gives a cross thread exception
        }

        static void PlaceText(Map map, string text, Location location, Color fontColor, double fontSize)
        {
            System.Windows.Controls.Label label =
              new System.Windows.Controls.Label();
            label.Content = text;
            label.Foreground = new SolidColorBrush(fontColor);
            label.FontSize = fontSize;
            MapLayer.SetPosition(label, location);
            map.Children.Add(label);
            return;
        }

        public MainWindow()
        {
            InitializeComponent();

            rpc = new ExampleCalculatorService();

            rpc.SetMain(this);

            
            GetAllPorts();
            var l = new Location();
            l.Latitude = 41.0913494;
            l.Longitude = -74.1851234;

            //var l = Microsoft.Maps.MapControl.WPF.Location;

            myMap.Center = l;

            PlaceText(myMap, "home", l, Colors.Yellow, 25.0);
            
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
//            //MyCenter.Center = "37.806029,-122.407007";
       

//            string s = @"{""NAV_POSLLH"":
//                {""iTOW"":76682250,
//                ""lat"":410913510,
//                ""long"":-741851233,
//                ""height"":78657,
//                ""hSML"":112941,
//                ""hAcc"":2091,
//                ""vAcc"":6656}}";


//            //NAVPOSLLH nv=JsonConvert.DeserializeObject<NAVPOSLLH>(s);
//            RootObject r = JsonConvert.DeserializeObject<RootObject>(s);

//            Console.WriteLine(r.NAV_POSLLH.lat);

            openPort(m_selectedPort);
            m_port.WriteLine("I am alive.\n");

            

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

         private void testlog_Click(object sender, RoutedEventArgs e)
         {

             var async = new JsonRpcStateAsync(rpcResultHandler, null);
            // async.JsonRpc = "{'method':'add','params':[11,2],'id':1}";
             async.JsonRpc = "{'method':'getPoints','params':[],'id':2}";
             JsonRpcProcessor.Process(async);

             return;
             

        
         }

         private void ports_SelectionChanged(object sender, SelectionChangedEventArgs e)
         {
             ComboBox cb = (ComboBox)sender;

             Console.WriteLine("{0}", cb.SelectedValue);
             m_selectedPort = cb.SelectedValue.ToString();

         }

         private void disconnect_Click(object sender, RoutedEventArgs e)
         {
             m_port.Close();
         }


    }
}
