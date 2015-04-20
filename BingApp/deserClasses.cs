using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AustinHarris.JsonRpc;
using System.Windows;
using BingApp;

public class NAVPOSLLH
{
    public int iTOW { get; set; }
    public int lat { get; set; }
    public int @long { get; set; }
    public int height { get; set; }
    public int hSML { get; set; }
    public int hAcc { get; set; }
    public int vAcc { get; set; }
}

public class RootObject
{
    public NAVPOSLLH NAV_POSLLH { get; set; }
}


namespace BingApp
{
    class deserClasses
    {
    }
}

