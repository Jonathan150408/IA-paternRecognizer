using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace IAConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //boucle qui permet de recommencer/relancer le programme
            ConsoleKey keyUserInput;
            do
            {
                //charger les data -> Core
                //si besoin, lancer un form -> Form
                //calculer la réponse -> Core
                //afficher la réponse -> Console
                //demander la correction -> Console
                //corriger -> Core
                //recalculer la réponse -> Core
                //afficher la réponse -> Console

                //demander si souhaite recommencer
                Console.WriteLine("Souhaitez vous recommencer ?");
                keyUserInput = Console.ReadKey(true).Key;
            } while (keyUserInput != ConsoleKey.Escape && keyUserInput != ConsoleKey.N);

            //quitter le programme
            Console.WriteLine("\nMerci d'avoir testé ce programme \\^o^/\nAppuyez sur une touche pour fermer le programme...");
            Console.Read();
            Environment.Exit(0);
        }
    }
}
