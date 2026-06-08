using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IA05_Form;
using System.Windows.Forms;

namespace IA05_Console
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // TODO : temporary value while waiting for an update of the json documents
            const int GRIDSIZE = 32;

            // 1. Set up variables before the form
            double[,] gridToAnalyse = new double[GRIDSIZE, GRIDSIZE];
            Dictionary<string, int> stats = new Dictionary<string, int>();
            bool wantCorrection;
            bool wantOperationDetails;

            // 2. Launch the form
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (var form = new IAForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    // 3. Get the data
                    gridToAnalyse = form.gridToAnalyse;
                    stats = form.stats;
                    wantOperationDetails = form.wantOperationDetails;
                    wantCorrection = form.wantCorrection;
                }
            }

            // 4. 

            Console.Read();
        }

        /// <summary>
        /// Displays an interactive menu.
        /// </summary>
        /// <param name="title">A facultative title to show</param>
        /// <param name="choices">A list of choices</param>
        /// <param name="topLine">The top line of the menu</param>
        /// <returns>The index of the choice selected in the list.</returns>
        static int DisplaySelectMenu(string title, List<string> choices, int topLine)
        {
            // 1. Set up variables
            int userChoice = 1;
            ConsoleKeyInfo userKey;
            Console.CursorVisible = false;
            Console.CursorTop = topLine;

            // 2. Writes the choices
            Console.WriteLine("   " + title);
            for (int i = 0; i < choices.Count; i++)
                Console.WriteLine("      " + choices[i]);

            // 3. Get an input from the user and decides
            do
            {
                // 4. Draw the arrow
                Console.SetCursorPosition(3, topLine + userChoice);
                Console.Write("->");

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
            return userChoice;
        }
    }
}
