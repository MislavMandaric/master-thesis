using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficSignSystem.Library
{
    public class TrainableFactory
    {
        public static ITrainable GetTrainable(string algorithm, Parameters parameters)
        {
            switch (algorithm)
            {
                case VerbsEnum.DETECTION:
                    return new ViolaJonesDetector();
                case VerbsEnum.RECOGNITION:
                    throw new NotImplementedException();
                default:
                    throw new TrafficSignException("Algorithm not supported.");
            }
        }
    }
}
