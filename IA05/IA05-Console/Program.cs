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

            

            Console.Read();
        }
    }
}
