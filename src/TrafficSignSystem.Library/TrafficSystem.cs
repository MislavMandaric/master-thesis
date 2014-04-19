using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficSignSystem.Library
{
    public class TrafficSystem
    {
        public void Run()
        {

        }

        public bool Train(string algorithm, Parameters parameters)
        {
            ITrainable training = TrainableFactory.GetTrainable(algorithm, parameters);
            return training.Train(parameters);
        }
    }
}
