using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.MachineLearning;
using System.IO;

namespace TrafficSignSystem.Library
{
    public class RandomForestClassifier : IRecognition, ITrainable, ITestable
    {
        private const int WIDTH = 24;
        private const int HEIGHT = 24;

        private CvRTrees _randomForest;

        public RandomForestClassifier()
        {
            this._randomForest = new CvRTrees();
        }

        public RandomForestClassifier(string modelFile)
            : this()
        {
            this._randomForest.Load(modelFile);
        }

        public ClassesEnum Recognize(Parameters parameters)
        {
            IplImage image;
            if (!parameters.TryGetValueByType(ParametersEnum.Image, out image))
                throw new TrafficSignException("Invalid parameters.");
            using (CvMat imageFeatures = new CvMat(1, WIDTH * HEIGHT, MatrixType.F32C1))
            {
                this.SetFeaturesRow(image, imageFeatures, 0);
                return (ClassesEnum)(int)this._randomForest.Predict(imageFeatures);
            }
        }

        public void Train(Parameters parameters)
        {
            string trainFile;
            string modelFile;
            int totalData;
            if (!(parameters.TryGetValueByType(ParametersEnum.TrainFile, out trainFile) &&
                parameters.TryGetValueByType(ParametersEnum.ModelFile, out modelFile) &&
                parameters.TryGetValueByType(ParametersEnum.TotalData, out totalData)))
                throw new TrafficSignException("Invalid parameters.");
            string trainDir = Directory.GetParent(trainFile).FullName;
            using (CvMat featurseData = new CvMat(totalData, WIDTH * HEIGHT, MatrixType.F32C1))
            using (CvMat responsesData = new CvMat(totalData, 1, MatrixType.F32C1))
            {
                using (StreamReader reader = new StreamReader(trainFile))
                {
                    int row = 0;
                    while (!reader.EndOfStream)
                    {
                        string[] line = reader.ReadLine().Split(' ');
                        string file = Path.Combine(trainDir, line[0]);
                        using (IplImage image = new IplImage(file))
                        {
                            this.SetFeaturesRow(image, featurseData, row);
                        }
                        double response = double.Parse(line[1]);
                        responsesData.mSet(row, 0, response);
                        row++;
                    }
                }
                if (this._randomForest.Train(featurseData, DTreeDataLayout.RowSample, responsesData))
                    this._randomForest.Save(modelFile);
                else
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
                        ClassesEnum systemClass = this.Recognize(parameters);
                        ClassesEnum realClass = (ClassesEnum)int.Parse(line[1]);
                        RecognitionEvaluation.Instance.Update(systemClass, realClass);
                    }
                }
            }
            RecognitionEvaluation.Instance.Calculate();
            RecognitionEvaluation.Instance.Print(resultsFile);
        }

        public void Dispose()
        {
            if (this._randomForest != null)
                this._randomForest.Dispose();
        }

        private void SetFeaturesRow(IplImage image, CvMat features, int row)
        {
            using (IplImage preprocessedImage = Preprocess.RandomForestPreprocess(image, WIDTH, HEIGHT))
            {
                for (int i = 0; i < preprocessedImage.Height; i++)
                    for (int j = 0; j < preprocessedImage.Width; j++)
                        features.mSet(row, i * preprocessedImage.Width + j, preprocessedImage[i, j]);
            }
        }
    }
}
