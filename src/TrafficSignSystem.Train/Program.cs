﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using TrafficSignSystem.Library;

namespace TrafficSignSystem.Train
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
                Parameters parameters = new Parameters();
                foreach (var property in invokedsubOptions.GetType().GetProperties())
                    parameters[property.Name] = property.GetValue(invokedsubOptions, null);
                TrafficSystem system = new TrafficSystem();
                try
                {
                    system.Train(invokedVerb, parameters);
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
