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
                                image.Rectangle(rectangle, CvScalar.ScalarAll(255));
                                using (IplImage signImage = image.GetSubImage(rectangle))
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

        public void Test(AlgorithmsEnum algorithm, Parameters parameters)
        {
            using (ITestable testing = TestableFactory.GetTestable(algorithm, parameters))
                testing.Test(parameters);
        }

        public void Train(AlgorithmsEnum algorithm, Parameters parameters)
        {
            using (ITrainable training = TrainableFactory.GetTrainable(algorithm, parameters))
                if (!training.Train(parameters))
                    throw new TrafficSignException("Unknown error occured during training.");
        }
    }
}
