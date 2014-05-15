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
            parameters[ParametersEnum.RF_MODEL_FILE] = args[1];
            parameters[ParametersEnum.RF_TEST_FILE] = args[2];
            parameters[ParametersEnum.RF_RESULTS_FILE] = args[3];
            system.TestRecognize(AlgorithmsEnum.RANDOM_FOREST, parameters);
        }
    }
}
