using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace TrafficSignSystem.Test
{
    public class RandomForestsSubOptions
    {
        [Option('m', "model_file", Required = true, HelpText = "Path to file with learned model data.")]
        public string ModelFile { get; set; }
        [Option('t', "test_file", Required = true, HelpText = "Path to file with testing samples.")]
        public string TestFile { get; set; }
        [Option('r', "results_file", Required = true, HelpText = "Path to file where results will be saved.")]
        public string ResultsFile { get; set; }
    }
}
