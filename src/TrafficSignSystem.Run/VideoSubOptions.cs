using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using TrafficSignSystem.Library;

namespace TrafficSignSystem.Run
{
    public class VideoSubOptions
    {
        [Option('d', "detection_algorithm", Required = true, HelpText = "Name of detection algorithm.")]
        public AlgorithmsEnum DetectionAlgorithm { get; set; }
        [Option('r', "recognition_algorithm", Required = true, HelpText = "Name of recognition algorithm.")]
        public AlgorithmsEnum RecognitionAlgorithm { get; set; }
        [Option('v', "video_file", Required = true, HelpText = "Path to video file.")]
        public string VideoFile { get; set; }
        [Option('c', "cascade_file", Required = true, HelpText = "Path to file with learned cascade.")]
        public string CascadeFile { get; set; }
        [Option('m', "model_file", Required = true, HelpText = "Path to file with learned model data.")]
        public string ModelFile { get; set; }
    }
}
