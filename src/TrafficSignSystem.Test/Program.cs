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
            Parameters parameters = new Parameters();
            parameters[ParametersEnum.CascadeFile] = @"C:\opencv\sources\data\haarcascades\haarcascade_frontalface_alt.xml";
            parameters[ParametersEnum.TestFile] = @"C:\Users\Mislav\Documents\diplomski\test\positive.txt";
            parameters[ParametersEnum.ResultsFile] = @"C:\Users\Mislav\Desktop\results.txt";
            system.TestDetect(AlgorithmsEnum.ViolaJones, parameters);
        }
    }
}
