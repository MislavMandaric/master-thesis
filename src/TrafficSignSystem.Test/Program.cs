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
            parameters[ParametersEnum.ModelFile] = args[1];
            parameters[ParametersEnum.TestFile] = args[2];
            parameters[ParametersEnum.ResultsFile] = args[3];
            system.TestRecognize(AlgorithmsEnum.RandomForests, parameters);
        }
    }
}
