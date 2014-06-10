using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace TrafficSignSystem.Train
{
    public class ViolaJonesSubOptions
    {
        [Option('p', "positive_file", Required = true, HelpText = "Path to file with positive training samples.")]
        public string TrainFilePositive { get; set; }
        [Option('n', "negative_file", Required = true, HelpText = "Path to file with negative training samples.")]
        public string TrainFileNegative { get; set; }
        [Option('v', "vector_file", Required = true, HelpText = "Path to file where vector description of the positive images will be saved.")]
        public string VectorFile { get; set; }
        [Option('c', "cascade_folder", Required = true, HelpText = "Path to folder where cascade files will be saved.")]
        public string CascadeFolder { get; set; }
        [Option('s', "total_positive", Required = true, HelpText = "Total number of positive training samples.")]
        public int TotalDataPositive { get; set; }
        [Option('g', "total_negative", Required = true, HelpText = "Total number of negative training samples.")]
        public int TotalDataNegative { get; set; }
    }
}
