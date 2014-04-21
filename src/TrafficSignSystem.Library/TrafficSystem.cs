using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace TrafficSignSystem.Library
{
    public class TrafficSystem
    {
        private const int ESC = 27;

        public void Run(string detectionAlgorithm, string recognitionAlgorithm, Parameters parameters)
        {
            string videoFile;
            if (!parameters.TryGetValueByType(ParametersEnum.VIDEO_FILE, out videoFile))
                throw new TrafficSignException("Invalid parameters.");
            IDetection detection = DetectionFactory.GetDetection(detectionAlgorithm, parameters);
            IRecognition recognition = RecognitionFactory.GetRecognition(recognitionAlgorithm, parameters);
            using (CvCapture videoCapture = CvCapture.FromFile(videoFile))
            using (CvWindow window = new CvWindow("Traffic sign system"))
            {
                if (videoCapture == null)
                    throw new TrafficSignException("Could not open video file.");
                while (videoCapture.GrabFrame() > 0)
                {
                    using (IplImage image = videoCapture.RetrieveFrame())
                    {
                        parameters[ParametersEnum.IMAGE] = image;
                        using (CvSeq detections = detection.Detect(parameters))
                        {
                            for (int i = 0; i < detections.Total; i++)
                            {
                                CvRect rectangle = (CvRect)detections.GetSeqElem<CvRect>(i);
                                image.Rectangle(rectangle, CvScalar.ScalarAll(255));
                                using (IplImage signImage = image.GetSubImage(rectangle))
                                {
                                    parameters[ParametersEnum.IMAGE] = signImage;
                                    string recognizedClass = recognition.Recognize(parameters);
                                }
                            }
                        }
                        window.ShowImage(image);
                    }
                    if (Cv.WaitKey(1) == ESC)
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
                    using (IplImage image = new IplImage(file))
                    {
                        parameters[ParametersEnum.IMAGE] = image;
                        CvSeq detections = detection.Detect(parameters);
                        if (detections.Total == 0)
                            falseNegative++;
                        else if (detections.Total == 1)
                            truePositive++;
                        else if (detections.Total >= 2)
                        {
                            truePositive++;
                            falsePositive = detections.Total - 1;
                        }
                    }
                }
            }
            using (StreamReader reader = new StreamReader(negativeFile))
            {
                while (!reader.EndOfStream)
                {
                    string file = reader.ReadLine().Split(';')[0];
                    using (IplImage image = new IplImage(file))
                    {
                        parameters[ParametersEnum.IMAGE] = image;
                        CvSeq detections = detection.Detect(parameters);
                        if (detections.Total == 0)
                            trueNegative++;
                        else if (detections.Total >= 1)
                            falsePositive = detections.Total;
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
