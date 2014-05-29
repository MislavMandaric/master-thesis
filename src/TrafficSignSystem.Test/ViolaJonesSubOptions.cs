using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace TrafficSignSystem.Test
{
    public class ViolaJonesSubOptions
    {
        [Option('c', "cascade_file", Required = true, HelpText = "Path to file with cascade.")]
        public string CascadeFile { get; set; }
        [Option('t', "test_file", Required = true, HelpText = "Path to file with test data.")]
        public string TestFile { get; set; }
        [Option('r', "results_file", Required = true, HelpText = "Path to file with results.")]
        public string ResultsFile { get; set; }
    }
}
