using IA05.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA05_Console.Models
{
    public partial class TrainingTemplate
    {
        Stopwatch chrono = new Stopwatch();

        /// <summary>
        /// Train the network with the template
        /// </summary>
        /// <param name="network"></param>
        public void Run(Network network)
        {
            // FIRST RESULTS
            chrono.Restart();

            // 1. Set up variables
            List<double[,]> FakeInput = new List<double[,]>();
            FakeInput.Add(this.FakeUserInput);

            // 2. Make a prediction
            Dictionary<string, double> results = network.MakePrediction(FakeInput);

            // 3. Show the result
            WriteResult(results);

            // Log the chrono
            LogChrono("Calculs terminés");

            // CORRECTION
            chrono.Restart();

            // 1. Writes the title
            Console.WriteLine("\n========= Phase d'entrainement =========");

            // 2. Correct the network
            network.Layers.Last().CorrectLayer(this.ExpectedResult, network.History.Last()[0], network.History[network.History.Count - 2]);

            // Log the chrono
            LogChrono("Correction faite");

            // NEW RESULTS
            chrono.Restart();

            // 1. Compute the new result
            results.Clear();
            results = network.MakePrediction(FakeInput);

            // 2. Show the new result
            WriteResult(results);

            // Log the chrono
            LogChrono("Révision des calculs finie");
        }

        /// <summary>
        /// Shows the results in the console
        /// </summary>
        /// <param name="results"></param>
        private void WriteResult(Dictionary<string, double> results)
        {
            // 1. Write a separator and the best result
            Console.WriteLine("\n================ Résultat ================\n");
            var max = results.OrderByDescending(v => v.Value).First();
            Console.WriteLine($"C'est {max.Key}");
            // 3. Writes all the others scores
            Console.WriteLine("\n\nTous les scores : ");
            foreach (KeyValuePair<string, double> result in results.OrderByDescending(v => v.Value))
            {
                Console.WriteLine(result.Key + " : " + Math.Round(result.Value * 100).ToString() + "%");
            }
            Console.WriteLine();
        }
        private void LogChrono(string logged)
        {
            //writes the chrono value
            chrono.Stop();
            Console.Write($"{logged} en ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(chrono.ElapsedMilliseconds);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" ms");
        }

    }
}
