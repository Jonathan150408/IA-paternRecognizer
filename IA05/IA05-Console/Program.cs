using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IA05_Form;
using System.Windows.Forms;
using IA05.Services;
using IA05.Models;

namespace IA05_Console
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //Dictionary<string, int> stats = new Dictionary<string, int>();
            List<double[,]> gridToAnalyse = new List<double[,]>();
            bool wantCorrection = false;
            bool wantOperationDetails = false;


            // INITIALIZATION AND MENU
            // 1. Get all previews
            PreviewService previewService = new PreviewService();
            List<NetworkPreview> networkPreviews = previewService.LoadPreviews();
            List<string> previewsNames = new List<string>();
            foreach (NetworkPreview networkPreview in networkPreviews)
            {
                previewsNames.Add(networkPreview.Name);
            }
            previewsNames.Add("Quitter");

            // 2. Display the menu
            Title();
            int chosenModel = DisplayMenu("Choisissez un modèle.", previewsNames, 7);
            // 3. Quit the programm if the user choose to quit
            if (chosenModel == previewsNames.LastIndexOf("Quitter"))
            {
                Environment.Exit(0);
            }
            Console.Clear();

            // 4. Load the neural network
            IAService iaService = new IAService(networkPreviews[chosenModel].Path);
            Network network = iaService.LoadNetwork();


            // FORM
            if (networkPreviews[chosenModel].NeedForm)
            {
                // 1. Launch the form
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                using (var form = new IAForm(networkPreviews[chosenModel].GridDimensions))
                {
                    // 2. Get the form's data
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        wantOperationDetails = form.wantOperationDetails;
                        wantCorrection = form.wantCorrection;
                        gridToAnalyse.Add(form.gridToAnalyse);
                    }
                }
            }


            // CALCULATIONS
            Dictionary<string, double> results = network.MakePrediction(gridToAnalyse);

            // DEBUG
            if (wantOperationDetails)
            {
                // 1. Writes the steps
                foreach (List<double[,]> maps in network.History)
                {
                    WriteStep(maps);
                }
            }

            // FINAL RESULT

            // 1. Write a separator and the best result
            Console.WriteLine("================ Final Result ================\n" + results.Max(v => v.Key) + " : " + results.Max(v => v.Value).ToString());

            // 2. Writes all the others scores
            Console.WriteLine("\n\nTous les scores : ");
            foreach (KeyValuePair<string, double> result in results)
            {
                Console.WriteLine(result.Key + " : " + result.Value.ToString());
            }

            // CORRECTION


            Console.Read();
        }

        /// <summary>
        /// Format the maps into values and write them down in the console.
        /// </summary>
        /// <param name="maps"></param>
        static void WriteStep(List<double[,]> maps)
        {
            // 1. Take each map
            foreach (double[,] map  in maps)
            {
                // 2. Add a separator
                for (int i = 0; i < map.GetLength(0) * 3 - 2; i++)
                {
                    Console.Write("-");
                }
                Console.WriteLine();

                // 3. Browse the map
                for (int i = 0; i < map.GetLength(0) - 1; i++)
                {
                    for (int j = 0; j < map.GetLength(1) - 1; j++)
                    {
                        // 4. Write the cell's value unless it's 0
                        if (Math.Round(map[i, j]) == 0)
                        {
                            Console.Write("   ");
                        }

                        else if (map[i, j] > 0)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write(" " + Math.Round(map[i, j]).ToString() + " ");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else if (map[i, j] < 0)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.Write(Math.Round(map[i, j]).ToString() + " ");
                            Console.ForegroundColor = ConsoleColor.White;
                        }

                    }
                    Console.WriteLine("|");
                }

                // 5. Add a separator
                for (int i = 0;i < map.GetLength(0) * 3 - 2; i++)
                {
                    Console.Write("-");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Writes a title
        /// </summary>
        static void Title()
        {
            Console.SetCursorPosition(15, 2);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.SetCursorPosition(15, 3);
            Console.WriteLine("║                - IAManager -                 ║");
            Console.SetCursorPosition(15, 4);
            Console.WriteLine("║          Réalisé par Jonathan Junod          ║");
            Console.SetCursorPosition(15, 5);
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");
        }

        /// <summary>
        /// Displays an interactive menu.
        /// </summary>
        /// <param name="title">A facultative title to show</param>
        /// <param name="choices">A list of choices</param>
        /// <param name="topLine">The top line of the menu</param>
        /// <returns>The index of the choice selected in the list.</returns>
        static int DisplayMenu(string title, List<string> choices, int topLine)
        {
            // 1. Set up variables
            int userChoice = 1;
            ConsoleKeyInfo userKey;
            Console.CursorVisible = false;
            Console.CursorTop = topLine;

            // 2. Writes the choices
            Console.WriteLine(title);
            for (int i = 0; i < choices.Count; i++)
                Console.WriteLine("  \t" + (i + 1).ToString() + ". " + choices[i]);

            // 3. Get an input from the user and decides
            do
            {
                // 4. Draw the arrow
                Console.SetCursorPosition(3, topLine + userChoice);
                Console.Write("->\t" + userChoice + ". " + choices[userChoice - 1]);

                // 5. Get the user's input
                userKey = Console.ReadKey(true);

                // 6. Deletes the previous arrow
                Console.SetCursorPosition(3, topLine + userChoice);
                Console.Write("  ");

                // 7. Choose what to do with the user's input
                if (userKey.Key == ConsoleKey.DownArrow)
                {
                    userChoice++;
                    // If exceed the number of choices, reset to the min choice
                    if (userChoice > choices.Count)
                        userChoice = 1;
                }
                else if (userKey.Key == ConsoleKey.UpArrow)
                {
                    userChoice--;
                    // If is less than 1, set the choosed option to the last option
                    if (userChoice < 1)
                        userChoice = choices.Count;
                }
                else if (char.IsDigit(userKey.KeyChar))
                    return userKey.KeyChar - '0';
            } while (userKey.Key != ConsoleKey.Enter && userKey.Key != ConsoleKey.Spacebar);

            // 8. Return the result
            return userChoice - 1;
        }
    }
}
