using IA04.Models;
using IA04.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IA04
{
    public partial class IAForm : Form
    {
        /// <summary>
        /// A chrono to measure time elapsing and thus have stats
        /// </summary>
        static Stopwatch chrono = new Stopwatch();
        /// <summary>
        /// The dimension of the grid (it's a square) = the number of cells
        /// </summary>
        private const int GRIDSIZE = 32;
        /// <summary>
        /// The dimension in pixels of a single cell
        /// </summary>
        private const int CELLSIZE = 16;

        /// <summary>
        /// The default constructor
        /// </summary>
        public IAForm()
        {
            // Start the chrono
            chrono.Start();

            // Base initialization
            InitializeComponent();
            // Set form position to top right
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(2000, 0);
            chrono.Restart();

            // Log the chrono value
            LogChrono("Formulaire initié en ");

            // Initialize the IA
            IAService dataService = new IAService();
            Network network = dataService.LoadNetwork();

            // Log the chrono value
            LogChrono("Réseau chargé en ");

            Main();
        }

        /// <summary>
        /// THE MAIN !!
        /// </summary>
        private void Main()
        {
            HandleGrid();

            // Log the chrono value
            LogChrono("Formulaire chargé en ");
        }
        /// <summary>
        /// Log the chrono value with a custom message
        /// </summary>
        /// <param name="message"></param>
        private void LogChrono(string message)
        {
            chrono.Stop();
            Console.Write(message);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(chrono.ElapsedMilliseconds);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" ms");
            chrono.Restart();
        }

        /// <summary>
        /// Create the grid of 32x32 checkboxes (= 1024 checkboxes)
        /// </summary>
        /// <returns></returns>
        private void HandleGrid()
        {
            // 1. Create the panel
            this.UserInput.Location = new Point((this.ClientSize.Width - this.UserInput.Width) / 3 + 50, 50);
            this.UserInput.AutoSize = false;
            this.UserInput.Size = new Size(GRIDSIZE * CELLSIZE, GRIDSIZE * CELLSIZE);
            this.UserInput.BackColor = Color.White;

            // 2. Create a 32 x 32 square of checkboxes
            for (int i = 0; i < GRIDSIZE; i++)
            {
                for (int j = 0; j < GRIDSIZE; j++)
                {
                    System.Windows.Forms.CheckBox box = new System.Windows.Forms.CheckBox
                    {
                        Appearance = Appearance.Button,
                        Tag = 0,
                        Size = new Size(CELLSIZE, CELLSIZE),
                        Location = new Point(j * CELLSIZE, i * CELLSIZE),
                        AutoSize = false,
                        Margin = new Padding(0),
                        Padding = new Padding(0),
                        BackColor = Color.White,
                        ForeColor = Color.White
                    };

                    // 3. Link methods to draw
                    box.MouseDown += Box_MouseDown;
                    box.MouseUp += Box_MouseUp;
                    box.MouseMove += Box_MouseMove;

                    this.UserInput.Controls.Add(box);
                }
            }
        }

        /// <summary>
        /// Let the user draw on the grid instead of clicking the checkboxes one by one
        /// the code comes from
        /// https://www.developpez.net/forums/d655935/dotnet/developpement-windows/windows-forms/changer-propriete-checkbox-checked-passage-souris-bouton-enfonce/
        /// with (very) light changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Box_MouseMove(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.CheckBox current_checkbox = sender as System.Windows.Forms.CheckBox;
            if (!current_checkbox.ClientRectangle.Contains(e.Location))
            {
                // gives focus to the form
                this.Focus();
            }
            else if (current_checkbox != chkSrc)
            {
                if (leftMouseButtonDown)
                {
                    current_checkbox.Checked = checkAfter;
                }
            }
        }
        /// <summary>
        /// https://www.developpez.net/forums/d655935/dotnet/developpement-windows/windows-forms/changer-propriete-checkbox-checked-passage-souris-bouton-enfonce/
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Box_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                System.Windows.Forms.CheckBox current_checkbox = sender as System.Windows.Forms.CheckBox;
                leftMouseButtonDown = false;
                if (current_checkbox == chkSrc)
                {
                    current_checkbox.Checked = checkAfter;
                }
            }
            //updates the color
            foreach (System.Windows.Forms.CheckBox button in this.UserInput.Controls)
            {
                if (button.Checked)
                {
                    button.BackColor = Color.DarkBlue;
                    button.ForeColor = Color.DarkBlue;
                    button.Tag = 1;
                }
                else if (!button.Checked)
                {
                    button.BackColor = Color.White;
                    button.ForeColor = Color.White;
                    button.Tag = 0;
                }
            }
        }
        /// <summary>
        /// https://www.developpez.net/forums/d655935/dotnet/developpement-windows/windows-forms/changer-propriete-checkbox-checked-passage-souris-bouton-enfonce/
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Box_MouseDown(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.CheckBox current_checkbox = (System.Windows.Forms.CheckBox)sender;
            leftMouseButtonDown = (e.Button == MouseButtons.Left);
            checkAfter = !current_checkbox.Checked;
            chkSrc = current_checkbox;
            current_checkbox.Checked = checkAfter;
        }
        private bool leftMouseButtonDown = false;
        private bool checkAfter = false;
        private System.Windows.Forms.CheckBox chkSrc;
    }
}
