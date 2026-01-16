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

            //gets the activations functions
            string path = Path.Combine(AppContext.BaseDirectory, "Ressources", "functions.txt");
            string content = File.ReadAllText(path);
            string[] functions_str = content.Split('\n');
            List<Function> functions = new List<Function>();
            //parse the functions from text
            foreach (string function in functions_str)
            {
                Enum.TryParse(function, ignoreCase: true, out Function activation);
                functions.Add(activation);
            }

            // Gets the infos about the network
            path = Path.Combine(AppContext.BaseDirectory, "Ressources", "poids.txt");
            content = File.ReadAllText(path);

            
            // Split with the '+' character -> obtains the Layers
            string[] strLayers = content.Split('+');

            // Foreach layer (iLayer = index layer)
            for (int iLayer = 0; iLayer < strLayers.Length; iLayer++)
            {
                string current_layer = strLayers[iLayer];
                // Split with ';' -> gets the neurons values
                string[] strNeurons = current_layer.Split(';');

                // The current layer containing neurons
                List<Neuron> layer = new List<Neuron>(strNeurons.Length);

                // Foreach neurons
                foreach (string current_neuron in strNeurons)
                {
                    // Split the weights and adjustment
                    string[] strNeuronValues = current_neuron.Split(' ');

                    // Pré-allocation de la liste des poids (double) pour ce neurone
                    List<double> weights = new List<double>(strNeuronValues.Length - 1);

                    // Parses the string into double values
                    for (int i = 0; i < strNeuronValues.Length - 1; i++)
                    {
                        // Parses the current values, if not a value we take 0.0
                        if (!double.TryParse(strNeuronValues[i], out double parsed))
                            parsed = 0.0;
                        weights.Add(parsed);
                    }
                    // Same with the adjustment
                    if (!double.TryParse(strNeuronValues[strNeuronValues.Length - 1], out double adjustment))
                        adjustment = 0.0;

                    // Adds the new neuron to the layer
                    layer.Add(new Neuron(weights, adjustment));
                }

                // Finally adds the layer to the Network -> next layer
                Network.Add(new Layer(layer, functions[Network.Count]));
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
            //center the grid
            this.grid1.Location = new Point((this.ClientSize.Width - grid1.Width) / 2, (this.ClientSize.Height - grid1.Height) / 2);

            //create 16 checkboxes
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    this.grid1.Controls.Add(new CheckBox
                    {
                        Appearance = Appearance.Button,
                        Tag = 0,
                        Size = new Size(48, 48),
                        AutoSize = false,
                        Margin = new Padding(2),
                        BackColor = Color.White
                    });
                }
            }
            //create a "next" button
            Button done = new Button
            {
                Size = new Size(200, 100),
                Location = new Point((this.ClientSize.Width - 200) / 2, 500),
                Text = "GO",
                ForeColor = Color.White,
                Font = new Font(this.Font.FontFamily, 20, FontStyle.Italic)
            };
            this.Controls.Add(done);

            //link the click action with methods
            foreach (CheckBox checkBox in this.grid1.Controls)
            {
                checkBox.Click += CheckBox_Click;
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
            foreach (CheckBox checkBox in this.grid1.Controls)
                gridToAnalyse.Add(Convert.ToInt16(checkBox.Tag));
            WriteCalculations();

            Console.ReadLine();
            Application.Restart();
        }

        /// <summary>
        /// Checked buttons goes green else white. Maximum 4 buttons can be checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Click(object sender, EventArgs e)
        {
            CheckBox clicked = sender as CheckBox;
            int numberClicked = 0;
            foreach (CheckBox button in this.grid1.Controls)
            {
                if (button.Checked)
                {
                    button.BackColor = Color.LightGreen;
                    numberClicked++;
                    button.Tag = 1;
                }
                else if (!button.Checked)
                {
                    button.BackColor = Color.White;
                    button.Tag = 0;
                }
            }
            //Cancels the click if 4 are clicked
            if (numberClicked > 4)
            {
                clicked.Checked = false;
                clicked.BackColor = Color.White;
                clicked.Tag = 0;
            }
        }
    }
}
