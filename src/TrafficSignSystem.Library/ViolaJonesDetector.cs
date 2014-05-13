using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace TrafficSignSystem.Library
{
    public class ViolaJonesDetector : IDetection
    {
        private const string FEATURE_MODE = "BASIC";
        private const string FEATURE_TYPE = "HAAR";
        private const string BOOST_TYPE = "GAB";
        private const int BUFFER_SIZE = 1024;
        private const int WIDTH = 24;
        private const int HEIGHT = 24;
        private const int STAGES = 10;
        private const float MIN_HIT_RATE = 0.995f;
        private const float MAX_FALSE_RATE = 0.4f;
        private const float SCALE_FACTOR = 1.1f;

        private CvHaarClassifierCascade _haarCascadeClassifier;

        public ViolaJonesDetector() { }

        public ViolaJonesDetector(string haarCascadeFile)
        {
            this._haarCascadeClassifier = CvHaarClassifierCascade.FromFile(haarCascadeFile);
        }

        public CvSeq Detect(Parameters parameters)
        {
            CvMat image;
            if (!parameters.TryGetValueByType(ParametersEnum.IMAGE, out image))
                throw new TrafficSignException("Invalid parameters.");
            using (CvMat preprocessedImage = Preprocess.ViolaJonesPreprocess(image))
            {
                CvMemStorage storage = new CvMemStorage();
                return this._haarCascadeClassifier.HaarDetectObjects(preprocessedImage, storage, SCALE_FACTOR);
            }
        }

        public bool Train(Parameters parameters)
        {
            string positiveFile;
            string negativeFile;
            string vectorFile;
            string cascadeFolder;
            int totalPositive;
            int totalNegative;
            if (!(parameters.TryGetValueByType(ParametersEnum.VJ_POSITIVE_FILE, out positiveFile) &&
                parameters.TryGetValueByType(ParametersEnum.VJ_NEGATIVE_FILE, out negativeFile) &&
                parameters.TryGetValueByType(ParametersEnum.VJ_VECTOR_FILE, out vectorFile) &&
                parameters.TryGetValueByType(ParametersEnum.VJ_CASCADE_FOLDER, out cascadeFolder) &&
                parameters.TryGetValueByType(ParametersEnum.TOTAL_POSITIVE, out totalPositive) &&
                parameters.TryGetValueByType(ParametersEnum.TOTAL_NEGATIVE, out totalNegative)))
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
                    return false;
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
                    return false;
            }
            return true;
        }

        public void Dispose()
        {
            if (this._haarCascadeClassifier != null)
                this._haarCascadeClassifier.Dispose();
        }
    }
}
