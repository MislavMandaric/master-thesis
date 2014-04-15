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
        private const string AUTOHOR_NAME = "Mislav Mandarić";
        private const int YEAR = 2014;
        private const string USAGE_TEXT = "Usage:\ttrain.exe [verb] [params]\nHelp:\ttrain.exe help\n\ttrain.exe [verb] help";

        [VerbOption("detection", HelpText = "Start detector training.")]
        public DetectionSubOptions DetectionVerb { get; set; }
        [VerbOption("recognition", HelpText = "Start recognition training.")]
        public RecognitionSubOptions RecognitionVerb { get; set; }

        public Options()
        {
            DetectionVerb = new DetectionSubOptions();
            RecognitionVerb = new RecognitionSubOptions();
        }

        [HelpVerbOption]
        public string GetUsage(string verb)
        {
            HelpText help = HelpText.AutoBuild(this, verb);
            help.Heading = new HeadingInfo(PROGRAM_NAME);
            help.Copyright = new CopyrightInfo(AUTOHOR_NAME, YEAR);
            help.AddPreOptionsLine(USAGE_TEXT);
            return help;
        }
    }
}
