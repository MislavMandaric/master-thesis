using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using TrafficSignSystem.Library;

namespace TrafficSignSystem.Run
{
    public class ImagesSubOptions
    {
        [Option('d', "detection_algorithm", Required = true, HelpText = "Name of detection algorithm.")]
        public AlgorithmsEnum DetectionAlgorithm { get; set; }
        [Option('r', "recognition_algorithm", Required = true, HelpText = "Name of recognition algorithm.")]
        public AlgorithmsEnum RecognitionAlgorithm { get; set; }
        [Option('t', "test_file", Required = true, HelpText = "Path to file with testing samples.")]
        public string TestFile { get; set; }
        [Option('f', "results_file", Required = true, HelpText = "Path to file where results will be saved.")]
        public string ResultsFile { get; set; }
        [Option('c', "cascade_file", Required = true, HelpText = "Path to file with learned cascade.")]
        public string CascadeFile { get; set; }
        [Option('m', "model_file", Required = true, HelpText = "Path to file with learned model data.")]
        public string ModelFile { get; set; }
    }
}
