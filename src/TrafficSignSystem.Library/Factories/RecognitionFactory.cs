using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficSignSystem.Library
{
    public static class RecognitionFactory
    {
        public static IRecognition GetRecognition(string algorithm, Parameters parameters)
        {
            string modelFile;
            if (parameters.TryGetValueByType(ParametersEnum.RF_MODEL_FILE, out modelFile))
                return new RandomForestClassifier(modelFile);
            else
                return new RandomForestClassifier();
        }
    }
}
