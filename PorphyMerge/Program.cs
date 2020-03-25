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
            if (!Directory.Exists(path)) return;

            //read all json-files
            var Properties = ReadFiles(Directory.GetFiles(path, "*Properties.json"));

            //convert properties into csv
            var sb = new StringBuilder();
            sb.Append(PrintHeaders(Properties.FirstOrDefault()));
            foreach (var s in Properties) sb.Append(Print(s));
            using var sv = new StreamWriter(path + "\\porphymerge.csv");
            sv.Write(sb.ToString());
        }

        /// <summary>
        /// Deserializes Files
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        static IEnumerable<KeyValuePair<string, Dictionary<string, IEnumerable<Property>>>> ReadFiles(IEnumerable<string> files)
        {
            foreach (var file in files)
                yield return new KeyValuePair<string, Dictionary<string, IEnumerable<Property>>>(file.Split('\\').Last().Split('_').First(), JsonSerializer.Deserialize(File.ReadAllText(file), typeof(Dictionary<string, IEnumerable<Property>>)) as Dictionary<string, IEnumerable<Property>>);
        }

        /// <summary>
        /// Prints a CSV Line
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static string Print(KeyValuePair<string, Dictionary<string, IEnumerable<Property>>> input)
        {
            var sb = new StringBuilder();
            sb.Append(input.Key + ";");
            if (input.Value.ContainsKey("Simulation"))
            {
                foreach (Property par in input.Value["Simulation"].OrderBy(s => !s.Name.Contains("percentage"))) sb.Append(par.Value.Split(' ').First() + ";");
                //Append oop distortion
                sb.Append(input.Value["Parameter"].FirstOrDefault().Value.Split(' ').First());
            }
            sb.Append("\n");
            return sb.ToString();
        }

        /// <summary>
        /// Prints CSV Headers
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static string PrintHeaders(KeyValuePair<string, Dictionary<string, IEnumerable<Property>>> input)
        {
            var sb = new StringBuilder();
            sb.Append("Name;");
            if (input.Value.ContainsKey("Simulation"))
            {
                foreach (Property par in input.Value["Simulation"].OrderBy(s => !s.Name.Contains("percentage"))) sb.Append(par.Name + ";");
                //Append oop distortion
                sb.Append(input.Value["Parameter"].FirstOrDefault().Name);
            }
            sb.Append("\n");
            return sb.ToString();
        }
    }
}
