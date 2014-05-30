using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace TrafficSignSystem.Run
{
    public class Options
    {
        private const string PROGRAM_NAME = "Traffic sign system";
        private const string VERSION = "1.0";
        private const string AUTOHOR_NAME = "Mislav Mandarić";
        private const int YEAR = 2014;
        private const string USAGE_TEXT = "Usage:\trun.exe [verb] [params]\nHelp:\trun.exe help\n\trun.exe [verb] help";

        [VerbOption("Video", HelpText = "Start video demonstration.")]
        public VideoSubOptions VideoVerb { get; set; }
        [VerbOption("Images", HelpText = "Start system testing.")]
        public ImagesSubOptions ImagesVerb { get; set; }

        public Options()
        {
            VideoVerb = new VideoSubOptions();
            ImagesVerb = new ImagesSubOptions();
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
