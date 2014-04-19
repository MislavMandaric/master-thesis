using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficSignSystem.Library
{
    public class DetectionFactory
    {
        public static IDetection GetDetection(string algorithm, Parameters parameters)
        {
            string haarCascadeFile;
            if (parameters.TryGetValueByType(ParametersEnum.CASCADE_FILE, out haarCascadeFile))
                return new ViolaJonesDetector(haarCascadeFile);
            else
                throw new TrafficSignException("Path to haar cascade file is required parameter.");
        }
    }
}
