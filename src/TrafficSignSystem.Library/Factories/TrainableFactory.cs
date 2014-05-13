using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficSignSystem.Library
{
    public static class TrainableFactory
    {
        public static ITrainable GetTrainable(string type, string algorithm, Parameters parameters)
        {
            switch (type)
            {
                case AlgorithmsEnum.DETECTION:
                    return DetectionFactory.GetDetection(algorithm, parameters);
                case AlgorithmsEnum.RECOGNITION:
                    return RecognitionFactory.GetRecognition(algorithm, parameters);
                default:
                    throw new TrafficSignException("Algorithm not supported.");
            }
        }
    }
}
