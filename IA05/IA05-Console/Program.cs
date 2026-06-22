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
            int chosenModel = DisplayMenu("Choisissez un modèle.", previewsNames, 7, 0);
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
            Console.WriteLine("================ Final Result ================\n");
            var max = results.OrderByDescending(v => v.Value).First();
            Console.WriteLine($"{max.Key} : {max.Value}");

            // 2. Writes all the others scores
            Console.WriteLine("\n\nTous les scores : ");
            foreach (KeyValuePair<string, double> result in results)
            {
                Console.WriteLine(result.Key + " : " + result.Value.ToString());
            }

            // CORRECTION
            if (wantCorrection)
            {
                // GETTING THE VALUES
                // 1. Writes the title
                Console.WriteLine("========= Phase d'entrainement =========");

                // 2. Set up variables
                double[] expected = new double[network.Layers.Last().Neurons.Count];

                // 3. Ask for expected values
                for (int i = 0; i < network.Layers.Last().Neurons.Count; i++)
                {
                    string shape = "";
                    switch (i)
                    {
                        case 0:
                            shape = "Carré";
                            break;
                        case 1:
                            shape = "Triangle";
                            break;
                        case 2:
                            shape = "Cercle";
                            break;
                    }
                    Console.Write("Valeur attendue pour " + shape + ":\t");
                    double.TryParse(Console.ReadLine(), out expected[i]);

                    // 4. Make sure values aren't greater or lower than 0 and 1
                    if (expected[i] > 1)
                    {
                        expected[i] = 1;
                    }
                    else if (expected[i] < 0)
                    {
                        expected[i] = 0;
                    }
                }
                Console.WriteLine();

                // CORRECTING
                // 1. Correct the last layer
                network.Layers.Last().CorrectLayer(expected, network.History.Last()[0], network.History[network.History.Count - 2]);

                // NEW RESULT
                // 1. Compute the new result
                results = network.MakePrediction(gridToAnalyse);

                // 2. Write a separator and the best result
                Console.WriteLine("================ Final Result ================\n");
                max = results.OrderByDescending(v => v.Value).First();
                Console.WriteLine($"{max.Key} : {max.Value}");

                // 3. Writes all the others scores
                Console.WriteLine("\n\nTous les scores : ");
                foreach (KeyValuePair<string, double> result in results)
                {
                    Console.WriteLine(result.Key + " : " + result.Value.ToString());
                }
            }

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
        /// <param name="title">A facultative string to write</param>
        /// <param name="choices">A list of strings within the user will choose one</param>
        /// <param name="topLine">A positive integer that specifies where to write</param>
        /// <param name="defaultChoiceIndex">The default selected choice - set to 0 for "classic" behaviour</param>
        /// <returns>The index of the choice selected in the list.</returns>
        static int DisplayMenu(string title, List<string> choices, int topLine, int defaultChoiceIndex)
        {
            // 1. Set up variables
            int userChoice = 0;
            if (defaultChoiceIndex >= 0 && defaultChoiceIndex < choices.Count)
            {
                userChoice = defaultChoiceIndex;
            }
            ConsoleKeyInfo userKey;
            Console.CursorVisible = false;
            Console.CursorTop = topLine;

            // 2. Writes the choices
            Console.WriteLine(title);
            for (int i = 0; i < choices.Count; i++)
            {
                Console.WriteLine("  \t" + (i + 1).ToString() + ". " + choices[i]);
            }
            topLine++;

            // 3. Get an input from the user and decides
            do
            {
                // 4. Draw the arrow
                Console.SetCursorPosition(3, topLine + userChoice);
                Console.Write("->\t");

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
                    if (userChoice >= choices.Count)
                        userChoice = 0;
                }
                else if (userKey.Key == ConsoleKey.UpArrow)
                {
                    userChoice--;
                    // If is less than 0, set the choosed option to the last option
                    if (userChoice < 0)
                        userChoice = choices.Count - 1;
                }
                // If the user press a number (the key on the keyboard)
                else if (char.IsDigit(userKey.KeyChar))
                {
                    // a. Try to parse it
                    if (int.TryParse(userKey.KeyChar.ToString(), out userChoice))
                    {
                        // b. If valid return, else reset to 0
                        if (userChoice >= 1 && userChoice <= choices.Count)
                        {
                            return userChoice - 1;
                        }
                        else
                        {
                            userChoice = 0;
                        }
                    }
                }
            } while (userKey.Key != ConsoleKey.Enter && userKey.Key != ConsoleKey.Spacebar);

            // 8. Return the result
            return userChoice;
        }
    }
}
