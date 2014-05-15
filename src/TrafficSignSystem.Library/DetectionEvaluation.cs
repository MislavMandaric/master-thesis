using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenCvSharp;

namespace TrafficSignSystem.Library
{
    public class DetectionEvaluation
    {
        private static DetectionEvaluation _instance;

        public static DetectionEvaluation Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DetectionEvaluation();
                return _instance;
            }
        }

        private int totalData;

        private int _truePositive;
        private int _falsePositive;
        private int _falseNegative;

        private double _precision;
        private double _response;
        private double _f1;

        private DetectionEvaluation() { }

        public void Update(IList<CvRect> systemDetections, IList<CvRect> realDetections)
        {
            totalData++;
            // TODO: Definirati kada je detekcija TP, FP, FN
        }

        public void Calculate()
        {
            this._precision = (double)this._truePositive / (this._truePositive + this._falsePositive);
            this._response = (double)this._truePositive / (this._truePositive + this._falseNegative);
            this._f1 = 2 * this._precision * this._response / (this._precision + this._response);
        }

        public void Print(string file)
        {
            using (StreamWriter writter = new StreamWriter(file))
            {
                writter.WriteLine("TP:\t{0}", this._truePositive);
                writter.WriteLine("FP:\t{0}", this._falsePositive);
                writter.WriteLine("FN:\t{0}", this._falseNegative);
                writter.WriteLine("P:\t{0}", this._precision);
                writter.WriteLine("R:\t{0}", this._response);
                writter.WriteLine("F1:\t{0}", this._f1);
            }
        }
    }
}
