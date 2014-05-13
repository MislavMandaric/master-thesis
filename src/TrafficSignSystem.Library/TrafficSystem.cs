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
            //string positiveFile;
            //string negativeFile;
            //string resultsFile;
            //if (!(parameters.TryGetValueByType(ParametersEnum.POSITIVE_FILE, out positiveFile) &&
            //    parameters.TryGetValueByType(ParametersEnum.NEGATIVE_FILE, out negativeFile) &&
            //    parameters.TryGetValueByType(ParametersEnum.RESULTS_FILE, out resultsFile)))
            //    throw new TrafficSignException("Invalid parameters.");
            //int truePositive = 0;
            //int trueNegative = 0;
            //int falsePositive = 0;
            //int falseNegative = 0;
            //IDetection detection = DetectionFactory.GetDetection(algorithm, parameters);
            //using (StreamReader reader = new StreamReader(positiveFile))
            //{
            //    while (!reader.EndOfStream)
            //    {
            //        string file = reader.ReadLine().Split(' ')[0];
            //        using (IplImage image = new IplImage(file))
            //        {
            //            parameters[ParametersEnum.IMAGE] = image;
            //            CvSeq detections = detection.Detect(parameters);
            //            if (detections.Total == 0)
            //                falseNegative++;
            //            else if (detections.Total == 1)
            //                truePositive++;
            //            else if (detections.Total >= 2)
            //            {
            //                truePositive++;
            //                falsePositive = detections.Total - 1;
            //            }
            //        }
            //    }
            //}
            //using (StreamReader reader = new StreamReader(negativeFile))
            //{
            //    while (!reader.EndOfStream)
            //    {
            //        string file = reader.ReadLine().Split(' ')[0];
            //        using (IplImage image = new IplImage(file))
            //        {
            //            parameters[ParametersEnum.IMAGE] = image;
            //            CvSeq detections = detection.Detect(parameters);
            //            if (detections.Total == 0)
            //                trueNegative++;
            //            else if (detections.Total >= 1)
            //                falsePositive = detections.Total;
            //        }
            //    }
            //}
            //using (StreamWriter writer = new StreamWriter(resultsFile))
            //{
            //    writer.WriteLine(";+;-");
            //    writer.WriteLine("+;{0};{1}", truePositive, falsePositive);
            //    writer.WriteLine("-;{0};{1}", falseNegative, trueNegative);
            //}
        }

        public void TestRecognize(string algorithm, Parameters parameters)
        {
            RandomForestClassifier c = new RandomForestClassifier(@"C:\Users\Mislav\Dropbox\My Documents\FER\Diplomski\SPEED_LIMITS_SIGNS\MASTIF\images\train\model\model.txt");
            Parameters p = new Parameters();

            using (StreamReader reader = new StreamReader(@"C:\Users\Mislav\Dropbox\My Documents\FER\Diplomski\SPEED_LIMITS_SIGNS\MASTIF\images\test\samples.txt"))
            {
                int br = 0;
                while (!reader.EndOfStream)
                {
                    string[] line = reader.ReadLine().Split(';');
                    string file = line[0];
                    string clasa = line[7];
                    p[ParametersEnum.IMAGE] = new CvMat(file);
                    string a = c.Recognize(p);
                    if (a.Equals(clasa))
                        br++;
                    Console.WriteLine(a + " " + clasa);
                }
                Console.WriteLine(br / 172.0f);
            }
        }

        public void Train(string type, string algorithm, Parameters parameters)
        {
            ITrainable training = TrainableFactory.GetTrainable(type, algorithm, parameters);
            if (!training.Train(parameters))
                throw new TrafficSignException("Unknown error occured during training.");
        }
    }
}
