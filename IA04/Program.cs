using IA04.Models;
using IA04.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA04
{
    internal class Program
    {
        /// <summary>
        /// A chrono to measure time elapsing and thus have stats
        /// </summary>
        static Stopwatch chrono = new Stopwatch();

        /// <summary>
        /// The MAIN
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Start the chrono
            chrono.Start();

            // Initialize the IA
            IAService dataService = new IAService();
            Network network = dataService.LoadNetwork();

            // Log the chrono value
            LogChrono("Réseau chargé en ");

            
        }

        /// <summary>
        /// Log the chrono value with a custom message
        /// </summary>
        /// <param name="message"></param>
        static void LogChrono(string message)
        {
            chrono.Stop();
            Console.Write(message);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(chrono.ElapsedMilliseconds);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" ms");
        }
    }
}
