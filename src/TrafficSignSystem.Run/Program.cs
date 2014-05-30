using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using TrafficSignSystem.Library;

namespace TrafficSignSystem.Run
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string invokedVerb = "";
            object invokedsubOptions = null;
            Options options = new Options();
            if (Parser.Default.ParseArgumentsStrict(args, options, (verb, subOptions) =>
            {
                invokedVerb = verb;
                invokedsubOptions = subOptions;
            }))
            {
                switch (invokedVerb)
                {
                    case "Video":
                        Run((VideoSubOptions)invokedsubOptions);
                        break;
                    case "Images":
                        Run((ImagesSubOptions)invokedsubOptions);
                        break;
                }
            }
        }

        private static void Run(VideoSubOptions videoOptions)
        {
            Parameters parameters = new Parameters();
            parameters.Add(ParametersEnum.VideoFile, videoOptions.VideoFile);
            parameters.Add(ParametersEnum.CascadeFile, videoOptions.CascadeFile);
            parameters.Add(ParametersEnum.ModelFile, videoOptions.ModelFile);
            try
            {
                TrafficSystem system = new TrafficSystem();
                system.Run(videoOptions.DetectionAlgorithm, videoOptions.RecognitionAlgorithm, parameters);
                Console.WriteLine("\nFinished succesfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine("\nError occured. Please try again.");
                Console.WriteLine(e.Message);
            }
        }

        private static void Run(ImagesSubOptions imagesOptions)
        {
            Parameters parameters = new Parameters();
            parameters.Add(ParametersEnum.TestFile, imagesOptions.TestFile);
            parameters.Add(ParametersEnum.ResultsFile, imagesOptions.ResultsFile);
            parameters.Add(ParametersEnum.CascadeFile, imagesOptions.CascadeFile);
            parameters.Add(ParametersEnum.ModelFile, imagesOptions.ModelFile);
            try
            {
                TrafficSystem system = new TrafficSystem();
                system.Test(imagesOptions.DetectionAlgorithm, imagesOptions.RecognitionAlgorithm, parameters);
                Console.WriteLine("\nFinished succesfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine("\nError occured. Please try again.");
                Console.WriteLine(e.Message);
            }
        }
    }
}
