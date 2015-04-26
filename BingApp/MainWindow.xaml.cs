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

          [JsonRpcMethod]
        private void setPoints(double [] points)
         {

             Application.Current.Dispatcher.Invoke
               (new Action(() => main.setPoints(points)));

                

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

        public void setPoints(double [] points)
        {
            int i = 0;
            double p1=0, p2=0;

            foreach (var item in points)
            {
               // Console.WriteLine("{0}", item);

                if (i%2 == 0)
                {
                    p1 = item;
                }
                else
                {
                    p2 = item;

                    Pushpin pin = new DraggablePin(myMap);
                    pin.Location =new Location(p1, p2) ;
                    pin.Content = cnt++;
                    pin.ToolTip = p1.ToString() + ":" + p2.ToString();

                    // Adds the pushpin to the map.
                    myMap.Children.Add(pin);
                  //  myMap.Center = pin.Location;
                    
                    pin.MouseRightButtonDown += new MouseButtonEventHandler(pin_MouseDown);

                    Console.WriteLine("{0} {1}", p1, p2);

                }
                
                i++;

            }
        }

        public void getPoints()
        {
            string s="";

            foreach (var v in myMap.Children)
            {
                if (v is Pushpin)
                {
                    Pushpin pin = (Pushpin)v;

                    Console.WriteLine("{{{0}, {1}}}", pin.Location.Latitude, pin.Location.Longitude);
                 
                    
                    if (m_port.IsOpen)
                   
                    {


                        s = s+string.Format("{{{0}, {1}}}", pin.Location.Latitude, pin.Location.Longitude);
                        s=s+",";
                        Console.WriteLine("{0}", s);

                   


                    }

                }
            }


            Console.WriteLine("sending:");
            s = '{' + s.TrimEnd(',') + '}';
            Console.WriteLine(s);
            m_port.WriteLine(s);

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
            string s = sp.ReadLine();
            Console.WriteLine("Just read:{0}", s);
            
            var async = new JsonRpcStateAsync(rpcResultHandler, null);
            // async.JsonRpc = "{'method':'add','params':[11,2],'id':1}";
            //async.JsonRpc = "{'method':'getPoints','params':[],'id':2}";
            async.JsonRpc = s;//  "{'method':'setPoints','params':[[40,-74, 40.11, -74.11]],'id':3}";
            JsonRpcProcessor.Process(async);


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


            ToolTipService.ShowDurationProperty.OverrideMetadata(
    typeof(DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));

            rpc = new ExampleCalculatorService();

            rpc.SetMain(this);

            
            GetAllPorts();

            openPort("COM10"); //temp

            var l = new Location();
            l.Latitude = 41.0913494;
            l.Longitude = -74.1851234;

            //DraggablePin pin = new DraggablePin(myMap);
            //pin.Location = l;
           // myMap.Children.Add(pin);

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

        //connect COM Port button
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
            Console.WriteLine("I am open:{0}.", m_selectedPort);


            

            // The pushpin to add to the map.
            //Pushpin pin = new DraggablePin(myMap); //Pushpin();
            //pin.Location = l;

            //// Adds the pushpin to the map.
            //myMap.Children.Add(pin);



        }
        int cnt = 0;
        private void pin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            var p = (Pushpin)sender;

            Console.WriteLine("right {0}", p.Content);
            myMap.Children.Remove(p);
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
            // pin.
            pin.Location = pinLocation;
            pin.Content = cnt++;
            pin.ToolTip = pinLocation.ToString();
            pin.MouseRightButtonDown += new MouseButtonEventHandler(pin_MouseDown);
            
            // Adds the pushpin to the map.
            myMap.Children.Add(pin);
        }

         private void testlog_Click(object sender, RoutedEventArgs e)
         {

             var async = new JsonRpcStateAsync(rpcResultHandler, null);
            // async.JsonRpc = "{'method':'add','params':[11,2],'id':1}";
             //async.JsonRpc = "{'method':'getPoints','params':[],'id':2}";
             async.JsonRpc = "{'method':'setPoints','params':[[40,-74, 40.11, -74.11]],'id':3}";
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
