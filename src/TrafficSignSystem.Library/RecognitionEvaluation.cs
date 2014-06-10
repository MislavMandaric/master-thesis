using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TrafficSignSystem.Library
{
    internal class RecognitionEvaluation
    {
        private static RecognitionEvaluation _instance;

        public static RecognitionEvaluation Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new RecognitionEvaluation();
                return _instance;
            }
        }

        private int _totalData;

        private IDictionary<ClassesEnum, int> _truePositives;
        private IDictionary<ClassesEnum, int> _trueNegatives;
        private IDictionary<ClassesEnum, int> _falsePositives;
        private IDictionary<ClassesEnum, int> _falseNegatives;
        private int _truePositive;
        private int _trueNegative;
        private int _falsePositive;
        private int _falseNegative;

        private IDictionary<ClassesEnum, double> _precisions;
        private IDictionary<ClassesEnum, double> _recalls;
        private IDictionary<ClassesEnum, double> _macroF1s;
        private double _precision;
        private double _recall;
        private double _macroF1;
        private double _microF1;

        private RecognitionEvaluation()
        {
            this._truePositives = new Dictionary<ClassesEnum, int>();
            this._trueNegatives = new Dictionary<ClassesEnum, int>();
            this._falsePositives = new Dictionary<ClassesEnum, int>();
            this._falseNegatives = new Dictionary<ClassesEnum, int>();

            this._precisions = new Dictionary<ClassesEnum, double>();
            this._recalls = new Dictionary<ClassesEnum, double>();
            this._macroF1s = new Dictionary<ClassesEnum, double>();
        }

        public void Update(ClassesEnum systemClass, ClassesEnum realClass)
        {
            _totalData++;
            if (systemClass == realClass)
                this.UpdateDictionary(this._truePositives, realClass);
            else
            {
                this.UpdateDictionary(this._falsePositives, systemClass);
                this.UpdateDictionary(this._falseNegatives, realClass);
            }
        }

        public void Calculate()
        {
            foreach (ClassesEnum key in Enum.GetValues(typeof(ClassesEnum)))
            {
                this._trueNegatives[key] = _totalData -
                    this.GetFromDictionary(this._truePositives, key) -
                    this.GetFromDictionary(this._falsePositives, key) -
                    this.GetFromDictionary(this._falseNegatives, key);

                this._precisions[key] = (double)this.GetFromDictionary(this._truePositives, key) /
                    (this.GetFromDictionary(this._truePositives, key) + this.GetFromDictionary(this._falsePositives, key));
                this._recalls[key] = (double)this.GetFromDictionary(this._truePositives, key) /
                    (this.GetFromDictionary(this._truePositives, key) + this.GetFromDictionary(this._falseNegatives, key));

                this._macroF1s[key] = 2 * this.GetFromDictionary(this._precisions, key) * this.GetFromDictionary(this._recalls, key) /
                    (this.GetFromDictionary(this._precisions, key) + this.GetFromDictionary(this._recalls, key));
            }
            this._truePositive = this._truePositives.Sum(x => x.Value);
            this._trueNegative = this._trueNegatives.Sum(x => x.Value);
            this._falsePositive = this._falsePositives.Sum(x => x.Value);
            this._falseNegative = this._falseNegatives.Sum(x => x.Value);

            this._precision = (double)this._truePositive / (this._truePositive + this._falsePositive);
            this._recall = (double)this._truePositive / (this._truePositive + this._falseNegative);

            this._macroF1 = this._macroF1s.Sum(x => x.Value) / this._macroF1s.Count;
            this._microF1 = 2 * this._precision * this._recall / (this._precision + this._recall);
        }

        public void Print(string file, bool append = false)
        {
            using (StreamWriter writter = new StreamWriter(file, append))
            {
                foreach (ClassesEnum key in Enum.GetValues(typeof(ClassesEnum)))
                {
                    writter.WriteLine(key);
                    writter.WriteLine("TP:\t\t\t{0}", this.GetFromDictionary(this._truePositives, key));
                    writter.WriteLine("TN:\t\t\t{0}", this.GetFromDictionary(this._trueNegatives, key));
                    writter.WriteLine("FP:\t\t\t{0}", this.GetFromDictionary(this._falsePositives, key));
                    writter.WriteLine("FN:\t\t\t{0}", this.GetFromDictionary(this._falseNegatives, key));
                    writter.WriteLine("P:\t\t\t{0}", this.GetFromDictionary(this._precisions, key));
                    writter.WriteLine("R:\t\t\t{0}", this.GetFromDictionary(this._recalls, key));
                    writter.WriteLine("MF1:\t\t{0}", this.GetFromDictionary(this._macroF1s, key));
                    writter.WriteLine();
                }
                writter.WriteLine("TP:\t\t\t{0}", this._truePositive);
                writter.WriteLine("TN:\t\t\t{0}", this._trueNegative);
                writter.WriteLine("FP:\t\t\t{0}", this._falsePositive);
                writter.WriteLine("FN:\t\t\t{0}", this._falseNegative);
                writter.WriteLine("P:\t\t\t{0}", this._precision);
                writter.WriteLine("R:\t\t\t{0}", this._recall);
                writter.WriteLine("MF1:\t\t{0}", this._macroF1);
                writter.WriteLine("mF1:\t\t{0}", this._microF1);
            }
        }

        private void UpdateDictionary(IDictionary<ClassesEnum, int> dict, ClassesEnum key)
        {
            if (dict.ContainsKey(key))
                dict[key] = dict[key] + 1;
            else
                dict[key] = 1;
        }

        private T GetFromDictionary<T>(IDictionary<ClassesEnum, T> dict, ClassesEnum key)
        {
            if (dict.ContainsKey(key))
                return dict[key];
            else
                return default(T);
        }
    }
}
