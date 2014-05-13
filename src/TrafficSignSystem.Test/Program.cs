using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficSignSystem.Library;

namespace TrafficSignSystem.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            TrafficSystem system = new TrafficSystem();
            system.TestRecognize("", null);
        }
    }
}
