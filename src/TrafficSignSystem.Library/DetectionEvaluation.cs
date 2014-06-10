using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenCvSharp;

namespace TrafficSignSystem.Library
{
    internal class DetectionEvaluation
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

        private const double HIT_TRESHOLD = 0.5;

        private int _truePositive;
        private int _falsePositive;
        private int _falseNegative;

        private double _precision;
        private double _recall;
        private double _f1;

        private DetectionEvaluation() { }

        public void Update(IList<CvRect> systemDetections, IList<CvRect> realDetections)
        {
            Dictionary<CvRect, bool> realDetectionsHit = realDetections.ToDictionary(x => x, y => false);
            foreach (CvRect real in realDetections)
            {
                double maxCoefficient = double.MinValue;
                foreach (CvRect system in systemDetections)
                {
                    double coefficient = this.CalculateSimilarity(system, real);
                    if (coefficient > maxCoefficient)
                        maxCoefficient = coefficient;
                }
                if (maxCoefficient > HIT_TRESHOLD)
                    realDetectionsHit[real] = true;
            }
            this._truePositive += realDetectionsHit.Count(x => x.Value);
            this._falseNegative += realDetectionsHit.Count(x => !x.Value);
            this._falsePositive += systemDetections.Count - realDetectionsHit.Count(x => x.Value);
        }

        public void Update(IList<CvRect> systemDetections, IList<CvRect> realDetections, out IList<CvRect> truePositives)
        {
            Dictionary<CvRect, bool> realDetectionsHit = realDetections.ToDictionary(x => x, y => false);
            foreach (CvRect real in realDetections)
            {
                double maxCoefficient = double.MinValue;
                foreach (CvRect system in systemDetections)
                {
                    double coefficient = this.CalculateSimilarity(system, real);
                    if (coefficient > maxCoefficient)
                        maxCoefficient = coefficient;
                }
                if (maxCoefficient > HIT_TRESHOLD)
                    realDetectionsHit[real] = true;
            }
            this._truePositive += realDetectionsHit.Count(x => x.Value);
            this._falseNegative += realDetectionsHit.Count(x => !x.Value);
            this._falsePositive += systemDetections.Count - realDetectionsHit.Count(x => x.Value);
            truePositives = realDetectionsHit.Where(x => x.Value).Select(x => x.Key).ToList();
        }

        public void Calculate()
        {
            this._precision = (double)this._truePositive / (this._truePositive + this._falsePositive);
            this._recall = (double)this._truePositive / (this._truePositive + this._falseNegative);
            this._f1 = 2 * this._precision * this._recall / (this._precision + this._recall);
        }

        public void Print(string file, bool append = false)
        {
            using (StreamWriter writter = new StreamWriter(file, append))
            {
                writter.WriteLine("TP:\t\t\t{0}", this._truePositive);
                writter.WriteLine("FP:\t\t\t{0}", this._falsePositive);
                writter.WriteLine("FN:\t\t\t{0}", this._falseNegative);
                writter.WriteLine("P:\t\t\t{0}", this._precision);
                writter.WriteLine("R:\t\t\t{0}", this._recall);
                writter.WriteLine("F1:\t\t\t{0}", this._f1);
            }
        }

        private double CalculateSimilarity(CvRect system, CvRect real)
        {
            //int l = system.Left > real.Left ? system.Left : real.Left;
            //int r = system.Right < real.Right ? system.Right : real.Right;
            //int t = system.Top > real.Top ? system.Top : real.Top;
            //int b = system.Bottom < real.Bottom ? system.Bottom : real.Bottom;

            //if (l > r || t > b)
            //    return 0;

            //int intersectionArea = (r - l) * (b - t);
            if (!system.IntersectsWith(real))
                return 0;

            CvRect intersection = system.Intersect(real);

            int intersectionArea = intersection.Width * intersection.Height;
            int systemArea = system.Width * system.Height;
            int realArea = real.Width * real.Height;

            if (systemArea + realArea == intersectionArea)
                return 0;
            return (double)intersectionArea / (systemArea + realArea - intersectionArea);
        }
    }
}
