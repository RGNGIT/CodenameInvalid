using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Config
{
    public static string proto { get; set; } = "http://";
    public static string address { get; set; } = proto + "localhost/";
    public static string apiCall { get; set; } = address + "api/";
}
