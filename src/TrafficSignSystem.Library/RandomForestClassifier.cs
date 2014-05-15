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
        private const int ROWS = 24;
        private const int COLS = 24;

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
            using (CvMat imageFeatures = new CvMat(1, ROWS * COLS, MatrixType.F32C1))
            {
                this.SetFeaturesRow(image, imageFeatures, 0);
                return ((int)this._randomForest.Predict(imageFeatures)).ToString();
            }
        }

        public bool Train(Parameters parameters)
        {
            string trainFile;
            string modelFile;
            int totalData;
            if (!(parameters.TryGetValueByType(ParametersEnum.RF_TRAIN_FILE, out trainFile) &&
                parameters.TryGetValueByType(ParametersEnum.RF_MODEL_FILE, out modelFile) &&
                parameters.TryGetValueByType(ParametersEnum.RF_TOTAL_SAMPLES, out totalData)))
                throw new TrafficSignException("Invalid parameters.");
            string trainDir = Directory.GetParent(trainFile).FullName;
            using (CvMat featurseData = new CvMat(totalData, ROWS * COLS, MatrixType.F32C1))
            using (CvMat responsesData = new CvMat(totalData, 1, MatrixType.F32C1))
            {
                using (StreamReader reader = new StreamReader(trainFile))
                {
                    int row = 0;
                    while (!reader.EndOfStream)
                    {
                        string[] line = reader.ReadLine().Split(';');
                        string file = Path.Combine(trainDir, line[0]);
                        using (CvMat image = new CvMat(file))
                            this.SetFeaturesRow(image, featurseData, row);
                        double response = double.Parse(line[7]);
                        responsesData.mSet(row, 0, response);
                        row++;
                    }
                }
                if (this._randomForest.Train(featurseData, DTreeDataLayout.RowSample, responsesData))
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

        private void SetFeaturesRow(CvMat image, CvMat features, int row)
        {
            using (CvMat preprocessedImage = Preprocess.RandomForestPreprocess(image, ROWS, COLS))
                for (int i = 0; i < preprocessedImage.Rows; i++)
                    for (int j = 0; j < preprocessedImage.Cols; j++)
                        features.mSet(row, i * preprocessedImage.Cols + j, preprocessedImage.mGet(i, j));
        }
    }
}
