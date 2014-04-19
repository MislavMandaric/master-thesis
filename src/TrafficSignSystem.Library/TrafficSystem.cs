using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficSignSystem.Library
{
    public class TrafficSystem
    {
        public void Run(string detectionAlgorithm, string recognitionAlgorithm, Parameters parameters)
        {

        }

        public void TestDetect(string algorithm, Parameters parameters)
        {
            string positiveFile;
            string negativeFile;
            if (parameters.TryGetValueByType(ParametersEnum.POSITIVE_FILE, out positiveFile) && parameters.TryGetValueByType(ParametersEnum.NEGATIVE_FILE, out negativeFile))
            {
                IDetection detection = DetectionFactory.GetDetection(algorithm, parameters);
                using (StreamReader reader = new StreamReader(positiveFile))
                {
                    while (!reader.EndOfStream)
                    {
                        string file = reader.ReadLine().Split(';')[0];
                        using (OpenCV.Net.Arr image = OpenCV.Net.CV.LoadImageM(file, OpenCV.Net.LoadImageFlags.Color))
                        {
                            parameters.Add(ParametersEnum.IMAGE, image);
                            OpenCV.Net.Rect[] detections = detection.Detect(parameters);
                            foreach (OpenCV.Net.Rect rectangle in detections)
                            {
                                
                            }
                        }
                    }
                }
                using (StreamReader reader = new StreamReader(negativeFile))
                {
                    while (!reader.EndOfStream)
                    {
                        string file = reader.ReadLine().Split(';')[0];
                        using (OpenCV.Net.Arr image = OpenCV.Net.CV.LoadImageM(file, OpenCV.Net.LoadImageFlags.Color))
                        {
                            parameters.Add(ParametersEnum.IMAGE, image);
                            OpenCV.Net.Rect[] detections = detection.Detect(parameters);
                        }
                    }
                }
            }
        }

        public void TestRecognize(string algorithm, Parameters parameters)
        {
            throw new NotImplementedException();
        }

        public bool Train(string algorithm, Parameters parameters)
        {
            ITrainable training = TrainableFactory.GetTrainable(algorithm, parameters);
            return training.Train(parameters);
        }
    }
}
