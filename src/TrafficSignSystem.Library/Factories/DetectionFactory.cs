using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficSignSystem.Library
{
    public static class DetectionFactory
    {
        public static IDetection GetDetection(string algorithm, Parameters parameters)
        {
            string haarCascadeFile;
            if (parameters.TryGetValueByType(ParametersEnum.VJ_CASCADE_FILE, out haarCascadeFile))
                return new ViolaJonesDetector(haarCascadeFile);
            else
                return new ViolaJonesDetector();
        }
    }
}
