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

        public void Train(string algorithm, Parameters parameters)
        {
            ITrainable training = TrainableFactory.GetTrainable(algorithm, parameters);
            training.Train(parameters);
        }
    }
}
