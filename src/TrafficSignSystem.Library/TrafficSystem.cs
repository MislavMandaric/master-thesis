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
                        parameters[ParametersEnum.IMAGE] = image;
                        using (CvSeq detections = detection.Detect(parameters))
                        {
                            for (int i = 0; i < detections.Total; i++)
                            {
                                CvRect rectangle = (CvRect)detections.GetSeqElem<CvRect>(i);
                                image.Rectangle(rectangle, CvScalar.ScalarAll(255));
                                using (CvMat signImage = Cv.GetMat(frame.GetSubImage(rectangle)))
                                {
                                    parameters[ParametersEnum.IMAGE] = signImage;
                                    string recognizedClass = recognition.Recognize(parameters);
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

        public void TestDetect(string algorithm, Parameters parameters)
        {
            string samplesFile;
            string resultsFile;
            if (!(parameters.TryGetValueByType(ParametersEnum.VJ_TEST_FILE, out samplesFile) &&
                parameters.TryGetValueByType(ParametersEnum.VJ_RESULTS_FILE, out resultsFile)))
                throw new TrafficSignException("Invalid parameters.");
            string samplesDir = Directory.GetParent(samplesFile).FullName;
            using (IDetection detection = DetectionFactory.GetDetection(algorithm, parameters))
            using (StreamReader reader = new StreamReader(samplesFile))
            {
                while (!reader.EndOfStream)
                {
                    string[] line = reader.ReadLine().Split(';');
                    string file = Path.Combine(samplesDir, line[0]);
                    using (CvMat image = new CvMat(file))
                    {
                        parameters[ParametersEnum.IMAGE] = image;
                        using (CvSeq detections = detection.Detect(parameters))
                        {
                            IList<CvRect> systemDetections = new List<CvRect>();
                            for (int i = 0; i < detections.Total; i++)
                                systemDetections.Add((CvRect)detections.GetSeqElem<CvRect>(i));
                            // TODO: Dohvatiti listu stvarnih rectangleova iz filea
                            IList<CvRect> realDetections = new List<CvRect>();
                            DetectionEvaluation.Instance.Update(systemDetections, realDetections);
                        }
                    }
                }
            }
            DetectionEvaluation.Instance.Calculate();
            DetectionEvaluation.Instance.Print(resultsFile);
        }

        public void TestRecognize(string algorithm, Parameters parameters)
        {
            string samplesFile;
            string resultsFile;
            if (!(parameters.TryGetValueByType(ParametersEnum.RF_TEST_FILE, out samplesFile) &&
                parameters.TryGetValueByType(ParametersEnum.RF_RESULTS_FILE, out resultsFile)))
                throw new TrafficSignException("Invalid parameters.");
            string samplesDir = Directory.GetParent(samplesFile).FullName;
            using (IRecognition recognition = RecognitionFactory.GetRecognition(algorithm, parameters))
            using (StreamReader reader = new StreamReader(samplesFile))
            {
                while (!reader.EndOfStream)
                {
                    string[] line = reader.ReadLine().Split(';');
                    string file = Path.Combine(samplesDir, line[0]);
                    using (CvMat image = new CvMat(file))
                    {
                        parameters[ParametersEnum.IMAGE] = image;
                        string systemClass = recognition.Recognize(parameters);
                        string realClass = line[7];
                        RecognitionEvaluation.Instance.Update(systemClass, realClass);
                    }
                }
            }
            RecognitionEvaluation.Instance.Calculate();
            RecognitionEvaluation.Instance.Print(resultsFile);
        }

        public void Train(string type, string algorithm, Parameters parameters)
        {
            using (ITrainable training = TrainableFactory.GetTrainable(type, algorithm, parameters))
                if (!training.Train(parameters))
                    throw new TrafficSignException("Unknown error occured during training.");
        }
    }
}
