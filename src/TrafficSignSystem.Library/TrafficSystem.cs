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
            string videoFile;
            if (!parameters.TryGetValueByType(ParametersEnum.VIDEO_FILE, out videoFile))
                throw new TrafficSignException("Invalid parameters.");
            IDetection detection = DetectionFactory.GetDetection(detectionAlgorithm, parameters);
            IRecognition recognition = RecognitionFactory.GetRecognition(recognitionAlgorithm, parameters);
            using (OpenCV.Net.Capture videoCapture = OpenCV.Net.Capture.CreateFileCapture(videoFile))
            using (OpenCV.Net.NamedWindow window = new OpenCV.Net.NamedWindow("Traffic sign system"))
            {
                if (videoCapture.IsClosed)
                    throw new TrafficSignException("Could not open video file.");
                while (videoCapture.GrabFrame())
                {
                    using (OpenCV.Net.IplImage image = videoCapture.RetrieveFrame())
                    {
                        parameters[ParametersEnum.IMAGE] = image;
                        OpenCV.Net.Rect[] detections = detection.Detect(parameters);
                        foreach (OpenCV.Net.Rect rectangle in detections)
                        {
                            OpenCV.Net.CV.Rectangle(image, rectangle, OpenCV.Net.Scalar.Rgb(0, 255, 0));
                            using (OpenCV.Net.IplImage signImage = image.GetSubRect(rectangle))
                            {
                                parameters[ParametersEnum.IMAGE] = signImage;
                                string recognizedClass = recognition.Recognize(parameters);
                            }
                        }
                        window.ShowImage(image);
                    }
                    if (OpenCV.Net.CV.WaitKey(1) == 27)
                        break;
                }
            }
        }

        public void TestDetect(string algorithm, Parameters parameters)
        {
            string positiveFile;
            string negativeFile;
            string resultsFile;
            if (!(parameters.TryGetValueByType(ParametersEnum.POSITIVE_FILE, out positiveFile) &&
                parameters.TryGetValueByType(ParametersEnum.NEGATIVE_FILE, out negativeFile) &&
                parameters.TryGetValueByType(ParametersEnum.RESULTS_FILE, out resultsFile)))
                throw new TrafficSignException("Invalid parameters.");
            int truePositive = 0;
            int trueNegative = 0;
            int falsePositive = 0;
            int falseNegative = 0;
            IDetection detection = DetectionFactory.GetDetection(algorithm, parameters);
            using (StreamReader reader = new StreamReader(positiveFile))
            {
                while (!reader.EndOfStream)
                {
                    string file = reader.ReadLine().Split(';')[0];
                    using (OpenCV.Net.Arr image = OpenCV.Net.CV.LoadImageM(file, OpenCV.Net.LoadImageFlags.Color))
                    {
                        parameters[ParametersEnum.IMAGE] = image;
                        OpenCV.Net.Rect[] detections = detection.Detect(parameters);
                        if (detections.Length == 0)
                            falseNegative++;
                        else if (detections.Length == 1)
                            truePositive++;
                        else if (detections.Length >= 2)
                        {
                            truePositive++;
                            falsePositive = detections.Length - 1;
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
                        parameters[ParametersEnum.IMAGE] = image;
                        OpenCV.Net.Rect[] detections = detection.Detect(parameters);
                        if (detections.Length == 0)
                            trueNegative++;
                        else if (detections.Length >= 1)
                            falsePositive = detections.Length;
                    }
                }
            }
            using (StreamWriter writer = new StreamWriter(resultsFile))
            {
                writer.WriteLine(";+;-");
                writer.WriteLine("+;{0};{1}", truePositive, falsePositive);
                writer.WriteLine("-;{0};{1}", falseNegative, trueNegative);
            }
        }

        public void TestRecognize(string algorithm, Parameters parameters)
        {
            throw new NotImplementedException();
        }

        public void Train(string algorithm, Parameters parameters)
        {
            ITrainable training = TrainableFactory.GetTrainable(algorithm, parameters);
            if (!training.Train(parameters))
                throw new TrafficSignException("Unknown error occured during training.");
        }
    }
}
