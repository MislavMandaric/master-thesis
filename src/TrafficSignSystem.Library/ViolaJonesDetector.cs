﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenCvSharp;

namespace TrafficSignSystem.Library
{
    internal class ViolaJonesDetector : IDetection, ITrainable, ITestable
    {
        private const string FEATURE_MODE = "BASIC";
        private const string FEATURE_TYPE = "HAAR";
        private const string BOOST_TYPE = "GAB";
        private const int BUFFER_SIZE = 1024;
        private const int WIDTH = 24;
        private const int HEIGHT = 24;
        private const int STAGES = 15;
        private const float MIN_HIT_RATE = 0.996f;
        private const float MAX_FALSE_RATE = 0.33f;
        private const float SCALE_FACTOR = 1.1f;

        private CvHaarClassifierCascade _haarCascadeClassifier;

        public ViolaJonesDetector() { }

        public ViolaJonesDetector(string haarCascadeFile)
        {
            this._haarCascadeClassifier = CvHaarClassifierCascade.FromFile(haarCascadeFile);
        }

        public CvSeq Detect(Parameters parameters)
        {
            IplImage image;
            if (!parameters.TryGetValueByType(ParametersEnum.Image, out image))
                throw new TrafficSignException("Invalid parameters.");
            using (IplImage preprocessedImage = Preprocess.ViolaJonesPreprocess(image))
            {
                CvMemStorage storage = new CvMemStorage();
                return this._haarCascadeClassifier.HaarDetectObjects(preprocessedImage, storage, SCALE_FACTOR);
            }
        }

        public void Train(Parameters parameters)
        {
            string positiveFile;
            string negativeFile;
            string vectorFile;
            string cascadeFolder;
            int totalPositive;
            int totalNegative;
            if (!(parameters.TryGetValueByType(ParametersEnum.TrainFilePositive, out positiveFile) &&
                parameters.TryGetValueByType(ParametersEnum.TrainFileNegative, out negativeFile) &&
                parameters.TryGetValueByType(ParametersEnum.VectorFile, out vectorFile) &&
                parameters.TryGetValueByType(ParametersEnum.CascadeFolder, out cascadeFolder) &&
                parameters.TryGetValueByType(ParametersEnum.TotalDataPositive, out totalPositive) &&
                parameters.TryGetValueByType(ParametersEnum.TotalDataNegative, out totalNegative)))
                throw new TrafficSignException("Invalid parameters.");
            StringBuilder builder = new StringBuilder();
            builder.Append("-vec ").Append(vectorFile)
                .Append(" -info ").Append(positiveFile)
                .Append(" -num ").Append(totalPositive)
                .Append(" -w ").Append(WIDTH)
                .Append(" -h ").Append(HEIGHT);
            using (Process process = Process.Start("opencv_createsamples.exe", builder.ToString()))
            {
                process.WaitForExit();
                if (process.ExitCode != 0)
                    throw new TrafficSignException("Generating vector file failed.");
            }
            builder.Clear();
            builder.Append("-data ").Append(cascadeFolder)
                .Append(" -vec ").Append(vectorFile)
                .Append(" -bg ").Append(negativeFile)
                .Append(" -numPos ").Append((int)(0.9f * totalPositive))
                .Append(" -numNeg ").Append(totalNegative)
                .Append(" -numStages ").Append(STAGES)
                .Append(" -precalcValBufSize ").Append(BUFFER_SIZE)
                .Append(" -precalcIdxBufSize ").Append(BUFFER_SIZE)
                .Append(" -baseFormatSave ")
                .Append(" -featureType ").Append(FEATURE_TYPE)
                .Append(" -w ").Append(WIDTH)
                .Append(" -h ").Append(HEIGHT)
                .Append(" -bt ").Append(BOOST_TYPE)
                .Append(" -minHitRate ").Append(MIN_HIT_RATE)
                .Append(" -maxFalseAlarmRate ").Append(MAX_FALSE_RATE)
                .Append(" -mode ").Append(FEATURE_MODE);
            using (Process process = Process.Start("opencv_traincascade.exe", builder.ToString()))
            {
                process.WaitForExit();
                if (process.ExitCode != 0)
                    throw new TrafficSignException("Training failed.");
            }
        }

        public void Test(Parameters parameters)
        {
            string testFile;
            string resultsFile;
            if (!(parameters.TryGetValueByType(ParametersEnum.TestFile, out testFile) &&
                parameters.TryGetValueByType(ParametersEnum.ResultsFile, out resultsFile)))
                throw new TrafficSignException("Invalid parameters.");
            string testDirectory = Directory.GetParent(testFile).FullName;
            using (StreamReader reader = new StreamReader(testFile))
            {
                while (!reader.EndOfStream)
                {
                    string[] line = reader.ReadLine().Split(' ');
                    string file = Path.Combine(testDirectory, line[0]);
                    using (IplImage image = new IplImage(file))
                    {
                        parameters[ParametersEnum.Image] = image;
                        using (CvSeq detections = this.Detect(parameters))
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

        public void Dispose()
        {
            if (this._haarCascadeClassifier != null)
                this._haarCascadeClassifier.Dispose();
        }
    }
}
