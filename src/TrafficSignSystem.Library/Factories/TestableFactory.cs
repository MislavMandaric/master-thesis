﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficSignSystem.Library
{
    public static class TestableFactory
    {
        public static ITestable GetTestable(AlgorithmsEnum algorithm, Parameters parameters)
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
                        return new RandomForestClassifier(modelFile);
                    else
                        return new RandomForestClassifier();
                default:
                    throw new TrafficSignException("Algorithm not supported.");
            }
        }
    }
}
