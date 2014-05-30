using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using TrafficSignSystem.Library;

namespace TrafficSignSystem.Test
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
                AlgorithmsEnum algorithm;
                Enum.TryParse<AlgorithmsEnum>(invokedVerb, out algorithm);
                Parameters parameters = new Parameters();
                foreach (var property in invokedsubOptions.GetType().GetProperties())
                {
                    ParametersEnum parameter;
                    Enum.TryParse<ParametersEnum>(property.Name, out parameter);
                    parameters[parameter] = property.GetValue(invokedsubOptions, null);
                }
                try
                {
                    TrafficSystem system = new TrafficSystem();
                    system.Test(algorithm, parameters);
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
}
