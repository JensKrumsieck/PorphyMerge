using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace PorphyMerge
{
    class Program
    {
        static void Main(string[] args)
        {
            string path;
            if (args.Length == 0)
            {
                Console.WriteLine("Enter Path:");
                path = Console.ReadLine();
            }
            else path = args[0];
            if (!Directory.Exists(path)) Environment.Exit(1);

            //read all json-files
            var Properties = ReadFiles(Directory.GetFiles(path, "*Properties.json"));

            //convert properties into csv
            var sb = new StringBuilder();
            sb.Append(Print(Properties.FirstOrDefault(), true));
            foreach (var s in Properties) sb.Append(Print(s));
            using var sv = new StreamWriter(path + "\\porphymerge.csv");
            sv.Write(sb.ToString());
        }

        /// <summary>
        /// Deserializes Files
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        static IEnumerable<KeyValuePair<string, Dictionary<string, IEnumerable<Property>>>> ReadFiles(IEnumerable<string> files) =>
            from file in files
            select new KeyValuePair<string, Dictionary<string, IEnumerable<Property>>>
                (file.Split('\\').Last().Split('_').First(),
                JsonSerializer.Deserialize(File.ReadAllText(file), typeof(Dictionary<string, IEnumerable<Property>>))
                    as Dictionary<string, IEnumerable<Property>>);


        /// <summary>
        /// Prints Property Value
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static string PrintValue(Property input) => $"{input.Value.Split(' ').First()};";

        /// <summary>
        /// Prints Property Header
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static string PrintHeader(Property input) => $"{input.Name};";

        /// <summary>
        /// Prints a CSV Line
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static string Print(KeyValuePair<string, Dictionary<string, IEnumerable<Property>>> input, bool headers = false)
        {
            var output = $"{input.Key};";
            if (input.Value.ContainsKey("Simulation"))
            {
                foreach (Property par in input.Value["Simulation"].OrderBy(s => !s.Name.Contains("percentage"))) output += headers ? PrintHeader(par) : PrintValue(par);
                //Append oop distortion
                var oop = input.Value["Parameter"].FirstOrDefault();
                output += headers ? PrintHeader(oop) : PrintValue(oop); 
            }
            output += "\n";
            return output;
        }
    }
}
