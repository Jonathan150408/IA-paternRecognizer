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
            Console.WriteLine("Hello World");

            //utiliser le form
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var form = new IAForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    Console.WriteLine(form);
                }
            }

            Console.Read();
        }
    }
}
