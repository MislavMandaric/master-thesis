using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficSignSystem.Library
{
    public class TrainableFactory
    {
        public static ITrainable GetTrainable(string verb, Parameters parameters)
        {
            switch (verb)
            {
                case "detection":
                    string cascadeFile;
                    if (parameters.TryGetValueByType(ParametersEnum.CASCADE_FILE, out cascadeFile))
                        return new ViolaJonesDetector(cascadeFile);
                    else
                        throw new NotImplementedException();
                case "recognition":
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
