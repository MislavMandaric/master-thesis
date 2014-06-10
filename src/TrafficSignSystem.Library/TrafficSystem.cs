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

        public void Run(AlgorithmsEnum detectionAlgorithm, AlgorithmsEnum recognitionAlgorithm, Parameters parameters)
        {
            string videoFile;
            if (!parameters.TryGetValueByType(ParametersEnum.VideoFile, out videoFile))
                throw new TrafficSignException("Invalid parameters.");
            using (IDetection detection = DetectionFactory.GetDetection(detectionAlgorithm, parameters))
            using (IRecognition recognition = RecognitionFactory.GetRecognition(recognitionAlgorithm, parameters))
            using (CvCapture videoCapture = CvCapture.FromFile(videoFile))
            using (CvWindow window = new CvWindow("Traffic sign system"))
            {
                if (videoCapture == null)
                    throw new TrafficSignException("Could not open video file.");
                while (videoCapture.GrabFrame() > 0)
                {
                    using (IplImage image = videoCapture.RetrieveFrame())
                    {
                        parameters[ParametersEnum.Image] = image;
                        using (CvSeq detections = detection.Detect(parameters))
                        {
                            for (int i = 0; i < detections.Total; i++)
                            {
                                CvRect rectangle = (CvRect)detections.GetSeqElem<CvRect>(i);
                                image.Rectangle(rectangle, new CvScalar(255, 0, 0));
                                using (IplImage signImage = image.GetSubImage(rectangle))
                                {
                                    parameters[ParametersEnum.Image] = signImage;
                                    ClassesEnum recognizedClass = recognition.Recognize(parameters);
                                    image.PutText(recognizedClass.ToString(), rectangle.TopLeft, new CvFont(FontFace.HersheyComplexSmall, 1, 1), new CvScalar(0, 0, 0));
                                    Console.WriteLine(recognizedClass);
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

        public void Test(AlgorithmsEnum detectionAlgorithm, AlgorithmsEnum recognitionAlgorithm, Parameters parameters)
        {
            string testFile;
            string resultsFile;
            if (!(parameters.TryGetValueByType(ParametersEnum.TestFile, out testFile) &&
                parameters.TryGetValueByType(ParametersEnum.ResultsFile, out resultsFile)))
                throw new TrafficSignException("Invalid parameters.");
            string testDirectory = Directory.GetParent(testFile).FullName;
            using (IDetection detection = DetectionFactory.GetDetection(detectionAlgorithm, parameters))
            using (IRecognition recognition = RecognitionFactory.GetRecognition(recognitionAlgorithm, parameters))
            using (StreamReader reader = new StreamReader(testFile))
            {
                while (!reader.EndOfStream)
                {
                    string[] line = reader.ReadLine().Split(' ');
                    string file = Path.Combine(testDirectory, line[0]);
                    using (IplImage image = new IplImage(file))
                    {
                        parameters[ParametersEnum.Image] = image;
                        using (CvSeq detections = detection.Detect(parameters))
                        {
                            IList<CvRect> systemDetections = new List<CvRect>();
                            for (int i = 0; i < detections.Total; i++)
                                systemDetections.Add((CvRect)detections.GetSeqElem<CvRect>(i));
                            IList<CvRect> realDetections = new List<CvRect>();
                            int numOfSigns = int.Parse(line[1]);
                            for (int i = 0; i < numOfSigns; i++)
                            {
                                int x = int.Parse(line[i * 4 + 2]);
                                int y = int.Parse(line[i * 4 + 3]);
                                int w = int.Parse(line[i * 4 + 4]);
                                int h = int.Parse(line[i * 4 + 5]);
                                realDetections.Add(new CvRect(x, y, w, h));
                            }
                            IList<CvRect> truePositives;
                            DetectionEvaluation.Instance.Update(systemDetections, realDetections, out truePositives);
                            foreach(CvRect rectangle in truePositives)
                            {
                                using (IplImage signImage = image.GetSubImage(rectangle))
                                {
                                    parameters[ParametersEnum.Image] = signImage;
                                    ClassesEnum systemClass = recognition.Recognize(parameters);
                                    ClassesEnum realClass = (ClassesEnum)int.Parse(line[line.Length - 1]);
                                    RecognitionEvaluation.Instance.Update(systemClass, realClass);
                                }
                            }
                        }
                    }
                }
            }
            DetectionEvaluation.Instance.Calculate();
            RecognitionEvaluation.Instance.Calculate();
            DetectionEvaluation.Instance.Print(resultsFile);
            RecognitionEvaluation.Instance.Print(resultsFile, true);
        }

        public void Test(AlgorithmsEnum algorithm, Parameters parameters)
        {
            using (ITestable testing = TestableFactory.GetTestable(algorithm, parameters))
            {
                testing.Test(parameters);
            }
        }

        public void Train(AlgorithmsEnum algorithm, Parameters parameters)
        {
            using (ITrainable training = TrainableFactory.GetTrainable(algorithm, parameters))
            {
                training.Train(parameters);
            }
        }
    }
}
