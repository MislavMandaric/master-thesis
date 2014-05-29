using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace TrafficSignSystem.Train
{
    public class RandomForestsSubOptions
    {
        [Option('m', "model_file", Required = true, HelpText = "Path to file with model.")]
        public string ModelFile { get; set; }
        [Option('t', "train_file", Required = true, HelpText = "Path to file with train data.")]
        public string TrainFile { get; set; }
        [Option('s', "total_samples", Required = true, HelpText = "Total number of samples.")]
        public int TotalData { get; set; }
    }
}
