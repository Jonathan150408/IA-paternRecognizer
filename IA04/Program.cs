using IA04.Models;
using IA04.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA04
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IAService dataService = new IAService();
            Console.WriteLine(dataService.GetPath());
            Network network = dataService.LoadNetwork();
            Console.ReadLine();
        }
    }
}
