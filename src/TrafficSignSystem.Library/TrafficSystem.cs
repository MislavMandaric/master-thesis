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
                    using (IplImage frame = videoCapture.RetrieveFrame())
                    using (CvMat image = Cv.GetMat(frame))
                    {
                        parameters[ParametersEnum.Image] = image;
                        using (CvSeq detections = detection.Detect(parameters))
                        {
                            for (int i = 0; i < detections.Total; i++)
                            {
                                CvRect rectangle = (CvRect)detections.GetSeqElem<CvRect>(i);
                                image.Rectangle(rectangle, CvScalar.ScalarAll(255));
                                using (CvMat signImage = Cv.GetMat(frame.GetSubImage(rectangle)))
                                {
                                    parameters[ParametersEnum.Image] = signImage;
                                    ClassesEnum recognizedClass = recognition.Recognize(parameters);
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

        public void TestDetect(AlgorithmsEnum algorithm, Parameters parameters)
        {
            string testFile;
            string resultsFile;
            if (!(parameters.TryGetValueByType(ParametersEnum.TestFile, out testFile) &&
                parameters.TryGetValueByType(ParametersEnum.ResultsFile, out resultsFile)))
                throw new TrafficSignException("Invalid parameters.");
            string testDirectory = Directory.GetParent(testFile).FullName;
            using (IDetection detection = DetectionFactory.GetDetection(algorithm, parameters))
            using (StreamReader reader = new StreamReader(testFile))
            {
                while (!reader.EndOfStream)
                {
                    string[] line = reader.ReadLine().Split(' ');
                    string file = Path.Combine(testDirectory, line[0]);
                    using (CvMat image = new CvMat(file))
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
                            DetectionEvaluation.Instance.Update(systemDetections, realDetections);
                        }
                    }
                }
            }
            DetectionEvaluation.Instance.Calculate();
            DetectionEvaluation.Instance.Print(resultsFile);
        }

        public void TestRecognize(AlgorithmsEnum algorithm, Parameters parameters)
        {
            string testFile;
            string resultsFile;
            if (!(parameters.TryGetValueByType(ParametersEnum.TestFile, out testFile) &&
                parameters.TryGetValueByType(ParametersEnum.ResultsFile, out resultsFile)))
                throw new TrafficSignException("Invalid parameters.");
            string testDirectory = Directory.GetParent(testFile).FullName;
            using (IRecognition recognition = RecognitionFactory.GetRecognition(algorithm, parameters))
            using (StreamReader reader = new StreamReader(testFile))
            {
                while (!reader.EndOfStream)
                {
                    string[] line = reader.ReadLine().Split(' ');
                    string file = Path.Combine(testDirectory, line[0]);
                    using (CvMat image = new CvMat(file))
                    {
                        parameters[ParametersEnum.Image] = image;
                        ClassesEnum systemClass = recognition.Recognize(parameters);
                        ClassesEnum realClass = (ClassesEnum)int.Parse(line[1]);
                        RecognitionEvaluation.Instance.Update(systemClass, realClass);
                    }
                }
            }
            RecognitionEvaluation.Instance.Calculate();
            RecognitionEvaluation.Instance.Print(resultsFile);
        }

        public void Train(AlgorithmsEnum algorithm, Parameters parameters)
        {
            using (ITrainable training = TrainableFactory.GetTrainable(algorithm, parameters))
                if (!training.Train(parameters))
                    throw new TrafficSignException("Unknown error occured during training.");
        }
    }
}
