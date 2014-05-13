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
    public class RandomForestClassifier : IRecognition
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

        public string Recognize(Parameters parameters)
        {
            CvMat image;
            if (!parameters.TryGetValueByType(ParametersEnum.IMAGE, out image))
                throw new TrafficSignException("Invalid parameters.");
            using (CvMat preprocessedImage = Preprocess.RandomForestPreprocess(image, WIDTH, HEIGHT))
            using (CvMat imageFeatures = new CvMat(1, WIDTH * HEIGHT, MatrixType.F32C1))
            {
                for (int i = 0; i < preprocessedImage.Rows; i++)
                {
                    for (int j = 0; j < preprocessedImage.Cols; j++)
                    {
                        imageFeatures.mSet(0, i * preprocessedImage.Rows + j, preprocessedImage.mGet(i, j));
                    }
                }
                return this._randomForest.Predict(imageFeatures).ToString();
            }
        }

        public bool Train(Parameters parameters)
        {
            string trainFile;
            string modelFile;
            if (!(parameters.TryGetValueByType(ParametersEnum.RF_TRAIN_FILE, out trainFile) &&
                parameters.TryGetValueByType(ParametersEnum.RF_MODEL_FILE, out modelFile)))
                throw new TrafficSignException("Invalid parameters.");
            using (CvMat data = new CvMat(420, WIDTH * HEIGHT, MatrixType.F32C1))
            using (CvMat responses = new CvMat(420, 1, MatrixType.F32C1))
            using (StreamReader reader = new StreamReader(trainFile))
            {
                int row = 0;
                while (!reader.EndOfStream)
                {
                    string[] line = reader.ReadLine().Split(';');
                    string file = line[0];
                    using (CvMat image = new CvMat(file))
                    using (CvMat preprocessedImage = Preprocess.RandomForestPreprocess(image, WIDTH, HEIGHT))
                    {
                        for (int i = 0; i < preprocessedImage.Rows; i++)
                        {
                            for (int j = 0; j < preprocessedImage.Cols; j++)
                            {
                                data.mSet(row, i * preprocessedImage.Rows + j, preprocessedImage.mGet(i, j));
                            }
                        }
                    }
                    double response = double.Parse(line[7]);
                    responses.mSet(row, 1, response);
                    row++;
                }
                if (this._randomForest.Train(data, DTreeDataLayout.RowSample, responses))
                {
                    this._randomForest.Save(modelFile);
                    return true;
                }
                else
                    return false;
            }
        }

        public void Dispose()
        {
            if (this._randomForest != null)
                this._randomForest.Dispose();
        }

        private void SetFeaturesRow(CvMat image, int row)
        {

        }
    }
}
