using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TrafficSignSystem.Library
{
    public class RecognitionEvaluation
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

        private int totalData;
        private IDictionary<string, bool> _classes;

        private IDictionary<string, int> _truePositives;
        private IDictionary<string, int> _trueNegatives;
        private IDictionary<string, int> _falsePositives;
        private IDictionary<string, int> _falseNegatives;
        private int _truePositive;
        private int _trueNegative;
        private int _falsePositive;
        private int _falseNegative;

        private IDictionary<string, double> _precisions;
        private IDictionary<string, double> _responses;
        private IDictionary<string, double> _macroF1s;
        private double _precision;
        private double _response;
        private double _macroF1;
        private double _microF1;

        private RecognitionEvaluation()
        {
            this._classes = new Dictionary<string, bool>();

            this._truePositives = new Dictionary<string, int>();
            this._trueNegatives = new Dictionary<string, int>();
            this._falsePositives = new Dictionary<string, int>();
            this._falseNegatives = new Dictionary<string, int>();

            this._precisions = new Dictionary<string, double>();
            this._responses = new Dictionary<string, double>();
            this._macroF1s = new Dictionary<string, double>();
        }

        public void Update(string systemClass, string realClass)
        {
            totalData++;
            this._classes[realClass] = true;
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
            foreach (string key in this._classes.Keys)
            {
                this._trueNegatives[key] = totalData -
                    this.GetFromDictionary(this._truePositives, key) -
                    this.GetFromDictionary(this._falsePositives, key) -
                    this.GetFromDictionary(this._falseNegatives, key);

                this._precisions[key] = (double)this.GetFromDictionary(this._truePositives, key) /
                    (this.GetFromDictionary(this._truePositives, key) + this.GetFromDictionary(this._falsePositives, key));
                this._responses[key] = (double)this.GetFromDictionary(this._truePositives, key) /
                    (this.GetFromDictionary(this._truePositives, key) + this.GetFromDictionary(this._falseNegatives, key));

                this._macroF1s[key] = 2 * this.GetFromDictionary(this._precisions, key) * this.GetFromDictionary(this._responses, key) /
                    (this.GetFromDictionary(this._precisions, key) + this.GetFromDictionary(this._responses, key));
            }
            this._truePositive = this._truePositives.Sum(x => x.Value);
            this._trueNegative = this._trueNegatives.Sum(x => x.Value);
            this._falsePositive = this._falsePositives.Sum(x => x.Value);
            this._falseNegative = this._falseNegatives.Sum(x => x.Value);

            this._precision = (double)this._truePositive / (this._truePositive + this._falsePositive);
            this._response = (double)this._truePositive / (this._truePositive + this._falseNegative);

            this._macroF1 = this._macroF1s.Sum(x => x.Value) / this._macroF1s.Count;
            this._microF1 = 2 * this._precision * this._response / (this._precision + this._response);
        }

        public void Print(string file)
        {
            using (StreamWriter writter = new StreamWriter(file))
            {
                foreach (string key in this._classes.Keys)
                {
                    writter.WriteLine(key);
                    writter.WriteLine("TP:\t{0}", this.GetFromDictionary(this._truePositives, key));
                    writter.WriteLine("TN:\t{0}", this.GetFromDictionary(this._trueNegatives, key));
                    writter.WriteLine("FP:\t{0}", this.GetFromDictionary(this._falsePositives, key));
                    writter.WriteLine("FN:\t{0}", this.GetFromDictionary(this._falseNegatives, key));
                    writter.WriteLine("P:\t{0}", this.GetFromDictionary(this._precisions, key));
                    writter.WriteLine("R:\t{0}", this.GetFromDictionary(this._responses, key));
                    writter.WriteLine("F1 macro:\t{0}", this.GetFromDictionary(this._macroF1s, key));
                    writter.WriteLine();
                }
                writter.WriteLine("TP:\t{0}", this._truePositive);
                writter.WriteLine("TN:\t{0}", this._trueNegative);
                writter.WriteLine("FP:\t{0}", this._falsePositive);
                writter.WriteLine("FN:\t{0}", this._falseNegative);
                writter.WriteLine("P:\t{0}", this._precision);
                writter.WriteLine("R:\t{0}", this._response);
                writter.WriteLine("F1 macro:\t{0}", this._macroF1);
                writter.WriteLine("F1 micro:\t{0}", this._microF1);
            }
        }

        private void UpdateDictionary(IDictionary<string, int> dict, string key)
        {
            if (dict.ContainsKey(key))
                dict[key] = dict[key]++;
            else
                dict[key] = 1;
        }

        private int GetFromDictionary(IDictionary<string, int> dict, string key)
        {
            if (dict.ContainsKey(key))
                return dict[key];
            else
                return 0;
        }

        private double GetFromDictionary(IDictionary<string, double> dict, string key)
        {
            if (dict.ContainsKey(key))
                return dict[key];
            else
                return 0;
        }
    }
}
