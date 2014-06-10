using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficSignSystem.Library
{
    internal static class DetectionFactory
    {
        public static IDetection GetDetection(AlgorithmsEnum algorithm, Parameters parameters)
        {
            switch (algorithm)
            {
                case AlgorithmsEnum.ViolaJones:
                    string haarCascadeFile;
                    if (parameters.TryGetValueByType(ParametersEnum.CascadeFile, out haarCascadeFile))
                        return new ViolaJonesDetector(haarCascadeFile);
                    else
                        return new ViolaJonesDetector();
                default:
                    throw new TrafficSignException("Algorithm not supported.");
            }
        }
    }
}
