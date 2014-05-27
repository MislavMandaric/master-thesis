using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace TrafficSignSystem.Train
{
    public class Options
    {
        private const string PROGRAM_NAME = "Train";
        private const string VERSION = "1.0";
        private const string AUTOHOR_NAME = "Mislav Mandarić";
        private const int YEAR = 2014;
        private const string USAGE_TEXT = "Usage:\ttrain.exe [verb] [params]\nHelp:\ttrain.exe help\n\ttrain.exe [verb] help";

        [VerbOption("ViolaJones", HelpText = "Start Viola-Jones training.")]
        public ViolaJonesSubOptions ViolaJonesVerb { get; set; }
        [VerbOption("RandomForests", HelpText = "Start Random Forests training.")]
        public RandomForestsSubOptions RandomForestsVerb { get; set; }

        public Options()
        {
            ViolaJonesVerb = new ViolaJonesSubOptions();
            RandomForestsVerb = new RandomForestsSubOptions();
        }

        [HelpVerbOption]
        public string GetUsage(string verb)
        {
            HelpText help = HelpText.AutoBuild(this, verb);
            help.Heading = new HeadingInfo(PROGRAM_NAME, VERSION);
            help.Copyright = new CopyrightInfo(AUTOHOR_NAME, YEAR);
            help.AddPreOptionsLine(USAGE_TEXT);
            return help;
        }
    }
}
