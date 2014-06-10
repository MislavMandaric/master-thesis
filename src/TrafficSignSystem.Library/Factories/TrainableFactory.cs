using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficSignSystem.Library
{
    internal static class TrainableFactory
    {
        public static ITrainable GetTrainable(AlgorithmsEnum algorithm, Parameters parameters)
        {
            switch (algorithm)
            {
                case AlgorithmsEnum.ViolaJones:
                    string haarCascadeFile;
                    if (parameters.TryGetValueByType(ParametersEnum.CascadeFile, out haarCascadeFile))
                        return new ViolaJonesDetector(haarCascadeFile);
                    else
                        return new ViolaJonesDetector();
                case AlgorithmsEnum.RandomForests:
                    string modelFile;
                    if (parameters.TryGetValueByType(ParametersEnum.ModelFile, out modelFile))
                        return new RandomForestsClassifier(modelFile);
                    else
                        return new RandomForestsClassifier();
                default:
                    throw new TrafficSignException("Algorithm not supported.");
            }
        }
    }
}
