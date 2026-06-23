using IA05.Models;
using IA05.Services;
using IA05_Console.Services;
using IA05_Form;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IA05_Console
{
    internal class Program
    {
        /// <summary>
        /// Determines wether the neural network is ready or need to be loaded
        /// </summary>
        static bool initialized = false;


        /// <summary>
        /// A service that loads the previews
        /// </summary>
        static PreviewService previewService;
        /// <summary>
        /// A service that loads the chosen model
        /// </summary>
        static IAService iaService;
        /// <summary>
        /// A service that set the files up if needed before running the IA
        /// </summary>
        static FileSetupService fileSetupService;


        /// <summary>
        /// All the previews of the awailables networks
        /// </summary>
        static List<NetworkPreview> networkPreviews;
        /// <summary>
        /// The list of awailables networks's names
        /// </summary>
        static List<string> previewsNames;
        /// <summary>
        /// The neural network itself
        /// </summary>
        static Network network;

        /// <summary>
        /// A chrono used to get the loading time
        /// </summary>
        static Stopwatch chrono = new Stopwatch();

        /// <summary>
        /// The number of trials for this session
        /// </summary>
        static int numberOfTrials = 0;
        /// <summary>
        /// The number of correct answers
        /// </summary>
        static int numberOfCorrect = 0;
        /// <summary>
        /// The numbers of each shape guessed | Square - Triangle - Circle
        /// </summary>
        static int[] guessDistribution = new int[] { 0, 0, 0};

        [STAThread]
        static void Main(string[] args)
        {
            ///////////////////////////////////////////////////////////////////////////////////////////
            // Start the chrono
            chrono.Restart();

            // ENVIRONMENT SETUP
            fileSetupService = new FileSetupService();
            fileSetupService.Setup();

            // Log the chrono
            LogChrono("Vérification des fichiers et setup fait");

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Start the chrono
            chrono.Restart();

            // INITIALIZATION
            // 1. Set up variables
            List<double[,]> gridToAnalyse = new List<double[,]>();
            bool wantCorrection = false;
            bool wantOperationDetails = false;

            // 2. One-time form setup
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 3. Get all previews
            if (!initialized)
            {
                previewService = new PreviewService();
                networkPreviews = previewService.LoadPreviews();
                previewsNames = new List<string>();
                foreach (NetworkPreview networkPreview in networkPreviews)
                {
                    previewsNames.Add(networkPreview.Name);
                }
                previewsNames.Add("Quitter");
            }

            // Log the chrono
            LogChrono("Chargement des previews réalisés");

            ///////////////////////////////////////////////////////////////////////////////////////////
            //No chrono at the beginning because of the user's input -> handled in the method
            // MENU
            // 1. Display the menu
            Title(6);
            int chosenModel = DisplayMenu("Choisissez un modèle.", previewsNames, 10, 0);
            // 2. Quit the programm if the user choose to quit
            if (chosenModel == previewsNames.LastIndexOf("Quitter"))
            {
                Environment.Exit(417);
            }
            ClearConsoleArea(100, previewsNames.Count + 16, 4);

            // Start the chrono
            chrono.Restart();

            // 3. Load the neural network
            if (!initialized)
            {
                iaService = new IAService(networkPreviews[chosenModel].Path);
                network = iaService.LoadNetwork();
            }

            // Log the chrono
            LogChrono("Réseau chargé");

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Chrono is handled inside the method
            // 1. Handle the following steps
            HandleNetwork(
                chosenModel: chosenModel,
                wantOperationDetails: wantOperationDetails,
                wantCorrection: wantCorrection,
                gridToAnalyse: gridToAnalyse
                );
        }

        /// <summary>
        /// Handle the calculations, this is splited from main because this part carries every step of the computing process without needing to get/initialize data.
        /// If this is placed ni main, we'll have a conflict with the saving process.
        /// </summary>
        /// <param name="chosenModel"></param>
        /// <param name="wantOperationDetails"></param>
        /// <param name="wantCorrection"></param>
        /// <param name="gridToAnalyse"></param>
        static void HandleNetwork(int chosenModel, bool wantOperationDetails, bool wantCorrection, List<double[,]> gridToAnalyse)
        {
            ///////////////////////////////////////////////////////////////////////////////////////////
            // Start the chrono
            chrono.Restart();

            // FORM
            if (networkPreviews[chosenModel].NeedForm)
            {
                // 1. Reset the grid in case of restart
                gridToAnalyse.Clear();

                // 2. Launch the form
                using (var form = new IAForm(networkPreviews[chosenModel].GridDimensions))
                {
                    // Log the chrono
                    LogChrono("Formulaire chargé");

                    form.Activate();
                    // 3. Get the form's data
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        wantOperationDetails = form.wantOperationDetails;
                        wantCorrection = form.wantCorrection;
                        gridToAnalyse.Add(form.gridToAnalyse);
                    }
                }
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Start the chrono
            chrono.Restart();

            // CALCULATIONS
            Dictionary<string, double> results = network.MakePrediction(gridToAnalyse);

            // Log the chrono
            LogChrono("Calculs terminés");

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Start the chrono
            chrono.Restart();

            // DEBUG
            if (wantOperationDetails)
            {
                // 1. Writes the steps
                foreach (List<double[,]> maps in network.History)
                {
                    WriteStep(maps);
                }
            }

            // Log the chrono
            LogChrono("Calculs rédigés");

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Start the chrono
            chrono.Restart();

            // FINAL RESULT

            // 1. Write the results
            WriteResult(results);

            // Log the chrono
            LogChrono("Résultats affichés");

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Start the chrono
            chrono.Restart();

            // CORRECTION
            if (wantCorrection)
            {
                // GETTING THE VALUES
                // 1. Writes the title
                Console.WriteLine("\n========= Phase d'entrainement =========");

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

                    // pause the chrono
                    chrono.Stop();

                    double.TryParse(Console.ReadLine(), out expected[i]);

                    // unpause the chrono
                    chrono.Start();

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

                // 5. Increments the counters
                numberOfTrials++;
                if (results.Values.ToList().IndexOf(results.Values.Max()) == Array.IndexOf(expected, expected.Max()))
                {
                    numberOfCorrect++;
                }
                guessDistribution[results.Values.ToList().IndexOf(results.Values.Max())]++;

                // Log the chrono
                LogChrono("Valeurs correctes reçues en");

                ///////////////////////////////////////////////////////////////////////////////////////////
                // Start the chrono
                chrono.Restart();

                // CORRECTING
                // 1. Correct the last layer
                network.Layers.Last().CorrectLayer(expected, network.History.Last()[0], network.History[network.History.Count - 2]);

                // Log the chrono
                LogChrono("Correction effectuée");

                ///////////////////////////////////////////////////////////////////////////////////////////
                // Start the chrono
                chrono.Restart();

                // NEW RESULT
                // 1. Compute the new result
                results = network.MakePrediction(gridToAnalyse);

                // Log the chrono
                LogChrono("Calculs revisités");

                ///////////////////////////////////////////////////////////////////////////////////////////
                // Start the chrono
                chrono.Restart();

                // 2. Show the results
                WriteResult(results);

                // Log the chrono
                LogChrono("Nouveaux résultats écrits");
            }

            // RESTART
            // 1. Set up the variables
            bool wrongAnswer = false;
            bool quit = false;
            ConsoleKey key = ConsoleKey.Escape;

            // 2. Get the user's key
            do
            {
                Console.WriteLine(wrongAnswer ? "{0} n'est pas une réponse valide" : "Souhaitez-vous recommencer ? (O/N)", key);
                key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.O)
                {
                    wrongAnswer = false;
                    quit = false;
                }
                else if (key == ConsoleKey.N)
                {
                    wrongAnswer = false;
                    quit = true;
                }
                else
                {
                    wrongAnswer = true;
                }
            } while (wrongAnswer);

            // 3. Restart or quit
            if (quit)
            {
                // a. Merge the last infos
                network.TotalOfGuesses += numberOfTrials;
                network.TotalOfCorrectAnswers += numberOfCorrect;
                for (int i = 0; i < network.TotalGuessDistribution.GetLength(0); i++)
                {
                    network.TotalGuessDistribution[i] += guessDistribution[i];
                }
                // b. Save the network
                iaService.SaveNetwork(network);
                fileSetupService.Commit();
                // c. Writes a small message
                GoodbyeMessage();
                // d. Waits for a user input
                Console.ReadKey(true);
                // e. Close the program
                Environment.Exit(0);
            }
            else
            {
                initialized = true;
                HandleNetwork(
                    chosenModel: chosenModel,
                    wantOperationDetails: wantOperationDetails,
                    wantCorrection: wantCorrection,
                    gridToAnalyse: gridToAnalyse
                    );
            }
        }

        /// <summary>
        /// Shows the results in the console
        /// </summary>
        /// <param name="results"></param>
        static void WriteResult(Dictionary<string, double> results)
        {
            // 1. Write a separator and the best result
            Console.WriteLine("\n================ Résultat ================\n");
            var max = results.OrderByDescending(v => v.Value).First();
            Console.WriteLine($"C'est un {max.Key}");
            // 3. Writes all the others scores
            Console.WriteLine("\n\nTous les scores : ");
            foreach (KeyValuePair<string, double> result in results.OrderByDescending(v => v.Value))
            {
                Console.WriteLine(result.Key + " : " + Math.Round(result.Value * 100).ToString() + "%");
            }
            Console.WriteLine();
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
        static void Title(int topLine)
        {
            // 1. Start the chrono
            chrono.Restart();

            // 2. Writes the title
            Console.SetCursorPosition(15, topLine);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.SetCursorPosition(15, topLine + 1);
            Console.WriteLine("║                - IAManager -                 ║");
            Console.SetCursorPosition(15, topLine + 2);
            Console.WriteLine("║          Réalisé par Jonathan Junod          ║");
            Console.SetCursorPosition(15, topLine + 3);
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            // Log the chrono
            Console.SetCursorPosition(0, 2);
            LogChrono("Titre affiché");
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
            // Start the chrono
            chrono.Restart();

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

            // Log the chrono
            Console.SetCursorPosition(0, 3);
            LogChrono("Menu dessiné");

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

        /// <summary>
        /// Stops the chrono and display it's value with a short message
        /// </summary>
        /// <param name="logged"></param>
        static void LogChrono(string logged)
        {
            //writes the chrono value
            chrono.Stop();
            Console.Write($"{logged} en ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(chrono.ElapsedMilliseconds);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" ms");
        }

        /// <summary>
        /// Clear a console area
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        static void ClearConsoleArea(int width, int height, int topPosition)
        {
            // 1. Set the cursor position
            Console.SetCursorPosition(0, topPosition);

            // 2. Write empty chars
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Console.Write(' ');
                }
                Console.WriteLine();
            }

            // 3. Set the cursor position
            Console.SetCursorPosition(0, topPosition);
        }

        static void GoodbyeMessage()
        {
            // 1. Basics stats
            Console.WriteLine("\n\n>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>\nAu revoir ^^\n");
            Console.WriteLine("Nombre d'essais pour cette session : {0}", numberOfTrials);
            Console.WriteLine("Taux de réponses correctes pour cette session : {0}%", Math.Round((double)numberOfCorrect / (double)numberOfTrials * 100));
            Console.WriteLine("\nNombre d'essais en tout : {0}", network.TotalOfGuesses);
            Console.WriteLine("Taux de réponses correctes en tout : {0}%", Math.Round((double)network.TotalOfCorrectAnswers / (double)network.TotalOfGuesses * 100));

            // 2. Advanced stats
            Console.WriteLine("\n\nRépartition des réponses de cette session :");
            for (int i = 0; i < guessDistribution.GetLength(0); i++)
            {
                Console.WriteLine(
                    "\t" +
                    network.Layers.Last().Results[i].ToString() +
                    " : " +
                    Math.Round((double)guessDistribution[i] / (double)numberOfTrials * 100) +
                    "%"
                    );
            }
            Console.WriteLine("\nRépartition des réponses de en tout :");
            for (int i = 0; i < network.TotalGuessDistribution.GetLength(0); i++)
            {
                Console.WriteLine(
                    "\t" +
                    network.Layers.Last().Results[i].ToString() +
                    " : " +
                    Math.Round((double)network.TotalGuessDistribution[i] / (double)network.TotalOfGuesses*100) +
                    "%"
                    );
            }

            // 3. Kind message ^^
            Console.WriteLine("\nMerci d'avoir testé ce programme \\^o^/\nAppuyez sur une touche pour fermer le programme...");
        }
    }
}
