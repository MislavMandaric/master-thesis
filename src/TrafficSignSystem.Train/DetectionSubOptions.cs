using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace TrafficSignSystem.Train
{
    public class DetectionSubOptions
    {
        [Option('p', "positive_file", Required = true, HelpText = "Path to file with positive images.")]
        public string PositiveFile { get; set; }
        [Option('n', "negative_file", Required = true, HelpText = "Path to file with negative images.")]
        public string NegativeFile { get; set; }
        [Option('v', "vector_file", Required = true, HelpText = "Path to file with vector description of the positive images.")]
        public string VectorFile { get; set; }
        [Option('c', "cascade_folder", Required = true, HelpText = "Path to folder with cascade files.")]
        public string CascadeFolder { get; set; }
        [Option("total_positive", Required = true, HelpText = "Total number of positive images.")]
        public int TotalPositive { get; set; }
        [Option("total_negative", Required = true, HelpText = "Total number of negative images.")]
        public int TotalNegative { get; set; }
    }
}
