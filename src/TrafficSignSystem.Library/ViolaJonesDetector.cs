using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficSignSystem.Library
{
    public class ViolaJonesDetector : IDetection
    {
        private const string CASCADE_MODE = "ALL";
        private const string FEATURE_TYPE = "LBP";
        private const int BUFFER_SIZE = 1024;
        private const int WIDTH = 24;
        private const int HEIGHT = 24;
        private const int STAGES = 20;
        private const float MIN_HIT_RATE = 0.995f;
        private const float MAX_FALSE_RATE = 0.4f;
        private const float SCALE_FACTOR = 1.2f;

        private string _haarCascadeFile;
        private OpenCV.Net.HaarClassifierCascade _haarCascadeClassifier;

        public OpenCV.Net.HaarClassifierCascade HaarCascadeClassifier
        {
            get
            {
                if (_haarCascadeClassifier == null)
                    _haarCascadeClassifier = OpenCV.Net.HaarClassifierCascade.Load(_haarCascadeFile);
                return _haarCascadeClassifier;
            }
        }

        public ViolaJonesDetector() { }

        public ViolaJonesDetector(string haarCascadeFile)
        {
            _haarCascadeFile = haarCascadeFile;
        }

        public OpenCV.Net.Seq Detect(Parameters parameters)
        {
            OpenCV.Net.Arr image;
            if (parameters.TryGetValueByType(ParametersEnum.IMAGE, out image))
            {
                OpenCV.Net.Arr grayscaleImage = null;
                OpenCV.Net.CV.CvtColor(image, grayscaleImage, OpenCV.Net.ColorConversion.Bgr2Gray);
                OpenCV.Net.CV.EqualizeHist(grayscaleImage, grayscaleImage);
                using (OpenCV.Net.MemStorage storage = new OpenCV.Net.MemStorage())
                using (OpenCV.Net.Seq sequence = this.HaarCascadeClassifier.DetectObjects(grayscaleImage, storage, SCALE_FACTOR))
                    return sequence;
            }
            else
                return null;
        }

        public bool Train(Parameters parameters)
        {
            string positiveFile;
            string negativeFile;
            string vectorFile;
            string cascadeFolder;
            int totalPositive;
            int totalNegative;
            if (parameters.TryGetValueByType(ParametersEnum.POSITIVE_FILE, out positiveFile) && parameters.TryGetValueByType(ParametersEnum.NEGATIVE_FILE, out negativeFile) &&
                parameters.TryGetValueByType(ParametersEnum.VECTOR_FILE, out vectorFile) && parameters.TryGetValueByType(ParametersEnum.CASCADE_FOLDER, out cascadeFolder) &&
                parameters.TryGetValueByType(ParametersEnum.TOTAL_POSITIVE, out totalPositive) && parameters.TryGetValueByType(ParametersEnum.TOTAL_NEGATIVE, out totalNegative))
            {
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
                    .Append(" -numPos ").Append(0.9f * totalPositive)
                    .Append(" -numNeg ").Append(totalNegative)
                    .Append(" -numStages ").Append(STAGES)
                    .Append(" -precalcValBufSize ").Append(BUFFER_SIZE)
                    .Append(" -precalcIdxBufSize ").Append(BUFFER_SIZE)
                    .Append(" -featureType ").Append(FEATURE_TYPE)
                    .Append(" -w ").Append(WIDTH)
                    .Append(" -h ").Append(HEIGHT)
                    .Append(" -minHitRate ").Append(MIN_HIT_RATE)
                    .Append(" -maxFalseAlarmRate ").Append(MAX_FALSE_RATE)
                    .Append(" -mode ").Append(CASCADE_MODE);
                using (Process process = Process.Start("opencv_traincascade.exe", builder.ToString()))
                {
                    process.WaitForExit();
                    if (process.ExitCode != 0)
                        return false;
                }
                return true;
            }
            else
                return false;
        }
    }
}
