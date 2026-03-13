using IA03;
using IA03.Code;

///22.11.2025
///Mon projet consite en : former une IA simple, réseau 16-8-2, capable de distinguer une ligne/colonne dans une grille de 4*4
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static IA03.Layer;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace IA03
{
    public partial class IA : Form
    {
        private bool wantCorrection;
        private bool wantOperationDetails;
        private const int GRIDSIZE = 32;
        private const int CELLSIZE = 16;
        private const int NUMBEROFCONVLAYERS = 2;
        private Stopwatch chrono = new Stopwatch();
        FlowLayoutPanel correctionGroup;
        FlowLayoutPanel debugGroup;
        /// <summary>
        /// The grid we'll analyse
        /// </summary> 
        public double[,] gridToAnalyse = new double[GRIDSIZE,GRIDSIZE];
        /// <summary>
        /// The neuronal network
        /// </summary>
        private List<Layer> Network;
        /// <summary>
        /// The kernels (= filters) that are used to analyse the grid, a list of lists because I use 2 convolution steps with multiple kernels
        /// </summary>
        private List<List<Kernel>> Kernels;

        /// <summary>
        /// The main program
        /// </summary>
        public IA()
        {
            chrono.Start();

            InitializeComponent();
            InitializeData();

            //writes the chrono value
            chrono.Stop();
            Console.Write("Réseau chargé en ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(chrono.ElapsedMilliseconds);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" ms");
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
            chrono.Restart();
            HandleGrid();
        }
        /// <summary>
        /// Create the grid of 32x32 checkboxes (= 1024 checkboxes)
        /// </summary>
        /// <returns></returns>
        private void HandleGrid()
        {
            // handle the grid
            this.UserInput.Location = new Point((this.ClientSize.Width - this.UserInput.Width) / 3 + 50, 50);
            this.UserInput.AutoSize = false;
            this.UserInput.Size = new Size(GRIDSIZE * CELLSIZE, GRIDSIZE * CELLSIZE);
            this.UserInput.BackColor = Color.White;

            //create a 32x32 square of checkboxes
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

                    // link methods to draw
                    box.MouseDown += CheckBox_MouseDown;
                    box.MouseUp += CheckBox_MouseUp;
                    box.MouseMove += CheckBox_MouseMove;

                    this.UserInput.Controls.Add(box);
                }
            }

            //CORRECTION MODE
            //create a toggle button to set the correction mode
            correctionGroup = new FlowLayoutPanel()
            {
                Name = "Activer la correction ?",
                ForeColor = Color.White,
                Location = new Point(50, 50)
            };
            Label correctionLabel = new Label()
            {
                Text = "Activer la correction ? (recommandé)",
                ForeColor = Color.White,
                Location = new Point(50, 25),
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 10),
            };
            System.Windows.Forms.RadioButton correctionTrue = new System.Windows.Forms.RadioButton()
            {
                Text = "On",
                Tag = true,
                Appearance = Appearance.Button,
                ForeColor= Color.White,
                Checked = true,
                BackColor = Color.LightGreen
            };
            System.Windows.Forms.RadioButton correctionFalse = new System.Windows.Forms.RadioButton()
            {
                Text = "Off",
                Tag = false,
                Appearance = Appearance.Button,
                ForeColor= Color.White,
            };
            //add the field to the form
            this.Controls.Add(correctionGroup);
            this.Controls.Add(correctionLabel);
            correctionGroup.Controls.Add(correctionTrue);
            correctionGroup.Controls.Add(correctionFalse);
            //link the buttons with method
            foreach (System.Windows.Forms.RadioButton radioButton in correctionGroup.Controls)
            {
                radioButton.Click += RadioButton_Click;
            }


            //DEBUG MODE
            //create a toggle button to set the correction mode
            debugGroup = new FlowLayoutPanel()
            {
                Name = "Montrer tous les calculs ?",
                ForeColor = Color.White,
                Location = new Point(50, 250)
            };
            Label debugLabel = new Label()
            {
                Text = "Montrer tous les calculs ?",
                ForeColor =Color.White,
                Location = new Point(50, 225),
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 10)
            };
            System.Windows.Forms.RadioButton debugTrue = new System.Windows.Forms.RadioButton()
            {
                Text = "On",
                Tag = true,
                Appearance = Appearance.Button,
                ForeColor = Color.White,
            };
            System.Windows.Forms.RadioButton debugFalse = new System.Windows.Forms.RadioButton()
            {
                Text = "Off",
                Tag = false,
                Appearance = Appearance.Button,
                ForeColor = Color.White,
                Checked = true,
                BackColor = Color.LightGreen
            };
            //add the field to the form
            this.Controls.Add(debugGroup);
            this.Controls.Add(debugLabel);
            debugGroup.Controls.Add(debugTrue);
            debugGroup.Controls.Add(debugFalse);
            //link the buttons with method
            foreach (System.Windows.Forms.RadioButton radioButton in debugGroup.Controls)
            {
                radioButton.Click += RadioButton_Click;
            }

            //create a "next" button placed under the grid
            Button done = new Button
            {
                Size = new Size(200, 100),
                Location = new Point((this.ClientSize.Width - 200) / 2 + 50, this.UserInput.Bottom + 10),
                Text = "GO",
                ForeColor = Color.White,
                Font = new Font(this.Font.FontFamily, 20, FontStyle.Italic),
                Cursor = Cursors.Hand,
                TabStop = true
            };

            this.Controls.Add(done);

            // bring "done" to front so it's not covered
            done.BringToFront();
            done.Click += Done_Click;

            //writes the chrono value
            chrono.Stop();
            Console.Write("Formulaire chargé en ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(chrono.ElapsedMilliseconds);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" ms");
        }

        /// <summary>
        /// hightlight the checked button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Click(object sender, EventArgs e)
        {
            foreach (System.Windows.Forms.RadioButton button in this.ActiveControl.Parent.Controls)
            {
                if (button.Checked)
                    button.BackColor = Color.LightGreen;
                else if (!button.Checked)
                    button.BackColor = Color.Black;
            }
        }

        /// <summary>
        /// Show the details in the console
        /// </summary>
        private void MakePrediction()
        {
            // this is a list of feature maps
            List<double[,]> convolutionals_Layers = new List<double[,]>();
            foreach(Kernel kernel in Kernels[0])
            {
                convolutionals_Layers.Add(kernel.GenerateFeatureMap(gridToAnalyse));
            }
            //pooling 1
            List<double[,]> pooled_maps = GeneralMaxPooling(convolutionals_Layers);
            //conv layer 2
            List<double[,]> maps_2 = new List<double[,]>();
            foreach (Kernel kernel_2 in this.Kernels[1])
            {
                maps_2.Add(kernel_2.RegenerateFeatureMap(pooled_maps));
            }
            //pool again
            List<double[,]> pooled_maps2 = GeneralMaxPooling(maps_2);
            //flattening
            List<double> flatten = new List<double>();
            foreach (double[,] map in pooled_maps2)
            {
                for (int i = 0; i < map.GetLength(0) - 1; i++)
                {
                    for (int j = 0; j < map.GetLength(1) - 1; j++)
                    {
                        flatten.Add(map[i, j]);
                    }
                }
            }
            //FNN
            List<double> last_layer_result = new List<double>(flatten);
            if (!wantOperationDetails)
            {
                foreach (Layer layer in this.Network)
                {
                    last_layer_result = layer.GetLayerResults(last_layer_result);
                }
            }

            //writes the operations details if wanted
            if (wantOperationDetails)
            {
                //After convolution 1
                Console.WriteLine("========= Basic feature maps level 1 rounded - not pooled - not flattered =========");
                foreach (double[,] map in convolutionals_Layers)
                {
                    for (int i = 0; i < map.GetLength(0); i++)
                    {
                        for (int j = 0; j < map.GetLength(1) - 1; j++)
                        {
                            if (map[i, j] == 0)
                            {
                                Console.Write("  ");
                            }
                            else
                            {
                                Console.Write(Math.Round(map[i, j]).ToString() + " ");
                            }
                        }
                        Console.WriteLine("|");
                    }
                    Console.WriteLine("_____________________________________________________________________________________");
                }
                //After max pooling 1
                Console.WriteLine("========= Pooled maps with max_pooling rounded =========");
                foreach (double[,] map in pooled_maps)
                {
                    for (int i = 0; i < map.GetLength(0) - 1; i++)
                    {
                        for (int j = 0; j < map.GetLength(1) - 1; j++)
                        {
                            if (map[i, j] == 0)
                            {
                                Console.Write("  ");
                            }
                            else
                            {
                                Console.Write(Math.Round(map[i, j]).ToString() + " ");
                            }
                        }
                        Console.WriteLine("|");
                    }
                    Console.WriteLine("________________________________________");
                }
                //After convolution 2
                Console.WriteLine("========= Basic feature maps level 2 rounded - not pooled - not flattened =========");
                foreach (double[,] map in maps_2)
                {
                    for (int i = 0; i < map.GetLength(0) - 1; i++)
                    {
                        for (int j = 0; j < map.GetLength(1) - 1; j++)
                        {
                            //get rid of 0
                            if (Math.Round(map[i, j]) == 0)
                            {
                                Console.Write("  ");
                            }
                            else
                            {
                                Console.Write(Math.Round(map[i, j]).ToString() + " ");
                            }
                        }
                        Console.WriteLine("|");
                    }
                    Console.WriteLine("______________________________");
                }
                //After max pooling 2
                Console.WriteLine("========= Pooled maps with max_pooling part 2 rounded =========");
                foreach (double[,] map in pooled_maps2)
                {
                    for (int i = 0; i < map.GetLength(0) - 1; i++)
                    {
                        for (int j = 0; j < map.GetLength(1) - 1; j++)
                        {
                            if (map[i, j] == 0)
                            {
                                Console.Write("  ");
                            }
                            else
                            {
                                Console.Write(Math.Round(map[i, j]).ToString() + " ");
                            }
                        }
                        Console.WriteLine("|");
                    }
                    Console.WriteLine("_____________");
                }
                //After flatten
                Console.WriteLine("========= Flattened rounded result =========");
                foreach (double[,] map in pooled_maps2)
                {
                    for (int i = 0; i < map.GetLength(0) - 1; i++)
                    {
                        for (int j = 0; j < map.GetLength(1) - 1; j++)
                        {
                            Console.Write(Math.Round(map[i, j]).ToString() + " ");
                        }
                    }
                    Console.WriteLine("|");
                }
                Console.WriteLine("========= count of flatten =========\n" + flatten.Count);
                //Every feed-forward layer
                int layer_counter = 0;
                foreach (Layer layer in this.Network)
                {
                    //calculate the result
                    last_layer_result = layer.GetLayerResults(last_layer_result);

                    //write the result in the console
                    Console.WriteLine("========= Layer {0} result =========", layer_counter);
                    foreach (double value in last_layer_result)
                    {
                        Console.WriteLine(value.ToString());
                    }
                    //update the counter
                    layer_counter++;
                }
            }

            //final decision
            int index_of_max = FindMaxValue(last_layer_result);

            //writes the chrono value
            chrono.Stop();
            Console.Write("Calculs terminés en ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(chrono.ElapsedMilliseconds);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" ms");

            //shows the final decision
            Console.WriteLine("\n========= Résultat =========\n");
            Console.Write("C'est un ");
            switch (index_of_max)
            {
                case 0:
                    Console.WriteLine("carré");
                    break;
                case 1:
                    Console.WriteLine("triangle");
                    break;
                case 2:
                    Console.WriteLine("cercle");
                    break;
                default:
                    Console.WriteLine("Erreur :C, index_of_max était : " + index_of_max);
                    break;
            }
            Console.WriteLine("\n[{0}%, {1}%, {2}%]\n", Math.Round(last_layer_result[0] * 100), Math.Round(last_layer_result[1] * 100), Math.Round(last_layer_result[2] * 100));

            //only if we want to train the network
            if (wantCorrection)
            {
                //Training phase
                Console.WriteLine("========= Phase d'entrainement =========");
                //ask for real values
                double[] expected = new double[this.Network[this.Network.Count - 1].Neurons.Count]; //create an array as long as the number of neurons in the last layer
                for (int i = 0; i < this.Network[this.Network.Count - 1].Neurons.Count; i++)
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
                }
                Console.WriteLine();

                //restart to mesure the time taken to correct the network
                chrono.Restart();
                //correct the network - only last layer
                this.Network[this.Network.Count - 1].CorrectLayer(expected, last_layer_result, flatten);
                //writes the chrono value
                chrono.Stop();
                Console.Write("Correction terminée en ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(chrono.ElapsedMilliseconds);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" ms");

                //restart to mesure the time taken to make the new calculations
                chrono.Restart();
                InitializeData();
                //writes the chrono value
                chrono.Stop();
                Console.Write("Réseau rechargé en ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(chrono.ElapsedMilliseconds);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" ms");

                //restart to mesure the time taken to calculate again
                chrono.Restart();
                //make the same prediction with the new values
                List<double> recalculated = new List<double>(this.Network[this.Network.Count - 1].GetLayerResults(flatten));
                index_of_max = FindMaxValue(recalculated);
                //writes the chrono value
                chrono.Stop();
                Console.Write("Calculs terminés en ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(chrono.ElapsedMilliseconds);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" ms");

                //shows the new result
                Console.WriteLine("\n========= Nouveau résultat =========\n");
                Console.Write("C'est un ");
                switch (index_of_max)
                {
                    case 0:
                        Console.WriteLine("carré");
                        break;
                    case 1:
                        Console.WriteLine("triangle");
                        break;
                    case 2:
                        Console.WriteLine("cercle");
                        break;
                    default:
                        Console.WriteLine("Erreur :C, index_of_max était : " + index_of_max);
                        break;
                }
                Console.WriteLine("\n[{0}%, {1}%, {2}%]\n", Math.Round(recalculated[0] * 100), Math.Round(recalculated[1] * 100), Math.Round(recalculated[2] * 100));

            }

        }

        /// <summary>
        /// find the max value and return the index
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private int FindMaxValue(List<double> values)
        {
            int index_of_max = 0;
            double max_value = values[0];

            for (int i = 1; i < values.Count; i++)
            {
                if (values[i] > max_value)
                {
                    max_value = values[i];
                    index_of_max = i;
                }
            }

            return index_of_max;
        }
        /// <summary>
        /// Takes all the feature maps and proceed to a 2x2 max pooling -> we only take the max value and the map become 4 times smaller
        /// </summary>
        /// <param name="feature_maps">A list of feature maps</param>
        /// <returns>A list of pooled feature maps</returns>
        private List<double[,]> GeneralMaxPooling(List<double[,]> feature_maps)
        {
            List<double[,]> pooled_maps = new List<double[,]>();

            foreach (double[,] map in feature_maps)
            {
                // temporary map -> 4x smaller as the original map
                double[,] temp_pooled_map = new double[(int)Math.Ceiling((double)(map.GetLength(0) / 2)), (int)Math.Ceiling((double)(map.GetLength(1) / 2))];
                for (int i = 0; i < map.GetLength(0) - 1; i += 2)
                {
                    for (int j = 0; j < map.GetLength(1) -1; j += 2)
                    {
                        List<double> temp_values_to_pool = new List<double>() { map[i, j] };
                        try
                        {
                            temp_values_to_pool.Add(map[i + 1, j]);
                        }
                        catch { }
                        try
                        {
                            temp_values_to_pool.Add(map[i, j + 1]);
                        }
                        catch { }
                        try
                        {
                            temp_values_to_pool.Add(map[i + 1, j + 1]);
                        }
                        catch { }
                        temp_pooled_map[(int)Math.Ceiling((decimal)i / 2), (int)Math.Ceiling((decimal)j / 2)] = MaxPooling(temp_values_to_pool);
                    }
                }
                pooled_maps.Add(temp_pooled_map);
            }

            return pooled_maps;
        }
        private double MaxPooling(List<double> doubles)
        {
            double max_value = 0;

            foreach (double value in doubles)
            {
                max_value = Math.Max(max_value, value);
            }

            return max_value;
        }
        /// <summary>
        /// Gets the values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Done_Click(object sender, EventArgs e)
        {
            //restart to mesure the time taken to convert the grid into values
            chrono.Restart();
            //convert the grid into values
            for (int i = 0; i < GRIDSIZE; i++)
            {
                for (int j = 0; j < GRIDSIZE; j++)
                {
                    this.gridToAnalyse[i, j] = Convert.ToInt16(this.UserInput.Controls[i * 32 + j].Tag);
                }
            }

            //convert the correction and debug fields
            //correction
            foreach (System.Windows.Forms.RadioButton radioButton in correctionGroup.Controls)
            {
                if (radioButton.Checked)
                {
                    wantCorrection = Convert.ToBoolean(radioButton.Tag);
                }
            }
            //debug
            foreach (System.Windows.Forms.RadioButton radioButton in debugGroup.Controls)
            {
                if (radioButton.Checked)
                {
                    wantOperationDetails = Convert.ToBoolean(radioButton.Tag);
                }
            }

            //writes the chrono value
            chrono.Stop();
            Console.Write("Valeurs résupérées en ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(chrono.ElapsedMilliseconds);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" ms");

            //restart the chrono to mesure the time taken for calculations
            chrono.Restart();
            //makes the calculations
            MakePrediction();

            bool wrong_answer = false;
            bool quit = false;
            ConsoleKey key = ConsoleKey.Escape;
            do
            {
                Console.WriteLine(wrong_answer ? "{0} n'est pas une réponse valide" : "Souhaitez-vous recommencer ? (O/N)", key);
                key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.O)
                {
                    wrong_answer = false;
                    Application.Restart();
                    Environment.Exit(0);
                }
                else if (key == ConsoleKey.N)
                {
                    wrong_answer = false;
                    quit = true;
                }
                else
                {
                    wrong_answer = true;
                }
            } while (!quit);
            Console.WriteLine("\nMerci d'avoir testé ce programme \\^o^/\nAppuyez sur une touche pour fermer le programme...");
            Console.ReadKey(true);
            Environment.Exit(0);
        }

        /// <summary>
        /// Initialize the networks and the kernels from the "layer_links.txt" file
        /// </summary>
        private void InitializeData()
        {
            //initialize the network
            Network = new List<Layer>();
            Kernels = new List<List<Kernel>>();
            for (int i = 0; i < NUMBEROFCONVLAYERS; i++)
            {
                Kernels.Add(new List<Kernel>());
            }

            //a link = a file = a layer/kernel, this means "foreach file that represents a layer or a kernel, do..."
            foreach (string link in File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Ressources", "layers_links.txt")).Split(';'))
            {
                //split the informations (function and values) as text for the moment
                string[] str_wholeLayer = File.ReadAllText(link.Trim()).Split('+');

                //converts the activation function
                Enum.TryParse(str_wholeLayer[0], ignoreCase: true, out Function activation);

                // For a layer
                if (activation != Layer.Function.kernel)
                {
                    //split the values (as text) -> we get an array of text like this
                    ///1 0 0 0;
                    ///1 0 0 0;
                    string[] str_allValues = str_wholeLayer[1].Split(';');

                    Layer tempLayer = new Layer(new List<Neuron>(), activation);

                    //foreach neuron as text, we convert it into values (1 text neuron = 1 line in the file)
                    foreach (string str_neuron in str_allValues)
                    {
                        string[] str_neuronValues = str_neuron.Trim().Split(' ');
                        double[] dbl_neuronValues = new double[str_neuronValues.Length - 1];

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
                else if (activation == Function.kernel)
                {
                    string[] str_kernel_lineofvalues = str_wholeLayer[1].Split(';');

                    Kernel tempKernel = new Kernel(new double[
                        str_kernel_lineofvalues.GetLength(0),                   //number of rows
                        str_kernel_lineofvalues[0].Trim().Split(' ').Length     //number of values/row
                        ]);

                    // gets the index (conv1, conv2)
                    int.TryParse(str_wholeLayer[2], out int index);

                    //foreach line of values as text, we take it and...
                    for (int i = 0; i < str_kernel_lineofvalues.Length; i++)
                    {
                        //...we separate it into values (as text) that we convert into doubles
                        string[] str_kernel_currentlinevalues = str_kernel_lineofvalues[i].Trim().Split(' ');
                        for (int j = 0; j < str_kernel_currentlinevalues.Length; j++)
                        {
                            double.TryParse(str_kernel_currentlinevalues[j], out double dbl_currentValue);
                            tempKernel.Filter[i, j] = dbl_currentValue;
                        }
                    }

                    this.Kernels[index].Add(tempKernel);
                }
            }
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
        private void CheckBox_MouseUp(object sender, MouseEventArgs e)
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
        private void CheckBox_MouseDown(object sender, MouseEventArgs e)
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
