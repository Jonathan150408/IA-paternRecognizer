using IA03;
///22.11.2025
///Mon projet consite en : former une IA simple, réseau 16-8-2, capable de distinguer une ligne/colonne dans une grille de 4*4
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static IA03.Layer;

namespace IA03
{
    public partial class IA : Form
    {
        /// <summary>
        /// The grid we'll analyse
        /// </summary>
        public List<double> gridToAnalyse = new List<double>();
        /// <summary>
        /// The neuronal network
        /// </summary>
        private readonly List<Layer> Network;

        /// <summary>
        /// The main program
        /// </summary>
        public IA()
        {
            InitializeComponent();

            //initialize the network
            Network = new List<Layer>();

            //
            //Import part
            //

            //a link = a file = a layer, this means "foreach file that represents a layer, do..."
            foreach (string link in File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Ressources", "layers_links.txt")).Split(';'))
            {
                Layer tempLayer;

                //split the function and the neurons (as text for the moment)
                string[] str_wholeLayer = File.ReadAllText(link.Trim()).Split('+');

                //split the neurons (as text) -> we get an array of text neurons
                string[] str_allValues = str_wholeLayer[2].Split(';');

                //converts the activation function
                Enum.TryParse(str_wholeLayer[0], ignoreCase: true, out Function activation);
                Enum.TryParse(str_wholeLayer[1], ignoreCase: true, out Layer.Type type);
                tempLayer = new Layer(new List<Neuron>(), activation, type);

                //foreach neuron as text, we convert it into values (1 text neuron = 1 line in the file)
                foreach (string str_neuron in str_allValues)
                {
                    string[] str_neuronValues = str_neuron.Trim().Split(' ');
                    double[] dbl_neuronValues = new double[str_neuronValues.Length - 1]; //give the lenght to optimise the RAM usage

                    //foreach value in the text, we convert it to double and assign it to the neuron
                    for (int i = 0; i < str_neuronValues.Length - 1; i++)
                    {
                        double.TryParse(str_neuronValues[i], out double dbl_currentValue);
                        dbl_neuronValues[i] = dbl_currentValue;
                    }
                    //parse the adjutement (last value)
                    double.TryParse(str_neuronValues[str_neuronValues.Length - 1], out double dbl_adjustement);

                    tempLayer.Neurons.Add(new Neuron(dbl_neuronValues, dbl_adjustement));
                }
                Network.Add(tempLayer);
            }
        }
        /// <summary>
        /// Loading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            // Set form position to top right
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(2000, 0);

            HandleGrid();
        }
        /// <summary>
        /// Create a grid and 16 checkboxes
        /// </summary>
        /// <returns></returns>
        private void HandleGrid()
        {
            this.UserInput.Location = new Point((this.ClientSize.Width - this.UserInput.Width)/3, 50);

            this.UserInput.AutoSize = true;
            this.UserInput.BackColor = Color.White;

            //create a 32x32 square of  checkboxes
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    this.UserInput.Controls.Add(new CheckBox
                    {
                        Appearance = Appearance.Button,
                        Tag = 0,
                        Size = new Size(16, 16),
                        Location = new Point(j * 16, i * 16),
                        AutoSize = false,
                        Margin = new Padding(0),
                        Padding = new Padding(0),
                        BackColor = Color.White,
                        ForeColor = Color.White
                    });
                }
            }
            //create a "next" button
            Button done = new Button
            {
                Size = new Size(200, 100),
                Location = new Point((this.ClientSize.Width - 200) / 2, this.UserInput.Height + this.UserInput.Location.Y + 10),
                Text = "GO",
                ForeColor = Color.White,
                Font = new Font(this.Font.FontFamily, 20, FontStyle.Italic)
            };
            this.Controls.Add(done);

            //link the click action with methods
            foreach (CheckBox checkBox in this.UserInput.Controls)
            {
                checkBox.MouseDown += CheckBox_MouseDown;
                checkBox.MouseUp += CheckBox_MouseUp;
                checkBox.MouseMove += CheckBox_MouseMove;
            }
            done.Click += Done_Click;
        }

        /// <summary>
        /// Show the details in the console
        /// </summary>
        private void WriteCalculations()
        {
            //confirm the neurons values in console for layer 1
            string lay1 = "";
            foreach(Neuron neuron in Network[0].Neurons)
            {
                foreach (double dbl in neuron.Weights)
                {
                    lay1 += dbl.ToString();
                    lay1 += " | ";
                }
                lay1 += neuron.Adjustment.ToString() + "\n";
            }
            Console.WriteLine(lay1);

            //confirm the neurons values in console for layer 2
            string lay2 = "";
            foreach (Neuron neuron in Network[1].Neurons)
            {
                foreach (double dbl in neuron.Weights)
                {
                    lay2 += dbl.ToString();
                    lay2 += " | ";
                }
                lay2 += neuron.Adjustment.ToString() + "\n";
            }
            Console.WriteLine(lay2);

            //results from layer 1
            int count = 0;
            List<double> r1 = new List<double>();
            foreach (double dbl in Network[0].GetLayerResults(gridToAnalyse))
                r1.Add(dbl);

            foreach (double result in r1)
            {
                if (count == 4)
                    Console.WriteLine();
                Console.WriteLine(result);
                count ++;
            }
            Console.Write("---------------------------------\nFinal results :\n");

            //final results
            //values with sigmoid
            List<double> r2 = new List<double>();
            foreach (double dbl in Network[1].GetLayerResults(r1))
                r2.Add(dbl);
            foreach (double result in r2)
            {
                Console.WriteLine(result);
            }
            //conclusion
            Console.WriteLine("---------------------------------\nConclusion :");
            if (r2[0] > r2[1])
                Console.WriteLine("Ligne horizontale");
            else if (r2[1] > r2[0])
                Console.WriteLine("Ligne verticale");
            else
                Console.WriteLine("Indéterminable");
        }
        /// <summary>
        /// Gets the values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Done_Click(object sender, EventArgs e)
        {
            foreach (CheckBox checkBox in this.UserInput.Controls)
                gridToAnalyse.Add(Convert.ToInt16(checkBox.Tag));
            WriteCalculations();

            Console.ReadLine();
            Application.Restart();
        }

        /// <summary>
        /// permit to draw on the grid instead of clicking the checkboxes one by one
        /// the code comes from
        /// https://www.developpez.net/forums/d655935/dotnet/developpement-windows/windows-forms/changer-propriete-checkbox-checked-passage-souris-bouton-enfonce/
        /// with (very) light changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_MouseMove(object sender, MouseEventArgs e)
        {
            CheckBox current_checkbox = sender as CheckBox;
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
        private void CheckBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                CheckBox current_checkbox = sender as CheckBox;
                leftMouseButtonDown = false;
                if (current_checkbox == chkSrc)
                {
                    current_checkbox.Checked = checkAfter;
                }
            }
            //updates the color
            foreach (CheckBox button in this.UserInput.Controls)
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
        private void CheckBox_MouseDown(object sender, MouseEventArgs e)
        {
            CheckBox current_checkbox = (CheckBox)sender;
            leftMouseButtonDown = (e.Button == MouseButtons.Left);
            checkAfter = !current_checkbox.Checked;
            chkSrc = current_checkbox;
            current_checkbox.Checked = checkAfter;
        }
        private bool leftMouseButtonDown = false;
        private bool checkAfter = false;
        private CheckBox chkSrc;

    }
}
