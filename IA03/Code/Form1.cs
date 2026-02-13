using IA03;
using IA03.Code;

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
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static IA03.Layer;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace IA03
{
    public partial class IA : Form
    {
        private const int GRIDSIZE = 32;
        private const int CELLSIZE = 16;
        private const int NUMBEROFCONVLAYERS = 2;
        /// <summary>
        /// The grid we'll analyse
        /// </summary> 
        public double[,] gridToAnalyse = new double[GRIDSIZE,GRIDSIZE];
        /// <summary>
        /// The neuronal network
        /// </summary>
        private readonly List<Layer> Network;
        /// <summary>
        /// The kernels (= filters) that are used to analyse the grid, a list of lists because I use 2 convolution steps with multiple kernels
        /// </summary>
        private readonly List<List<Kernel>> Kernels;

        /// <summary>
        /// The main program
        /// </summary>
        public IA()
        {
            InitializeComponent();

            //initialize the network
            Network = new List<Layer>();
            Kernels = new List<List<Kernel>>();
            for (int i = 0; i < NUMBEROFCONVLAYERS; i++)
            {
                Kernels.Add(new List<Kernel>());
            }

            //
            // Import part
            //

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
            // handle the grid
            this.UserInput.Location = new Point((this.ClientSize.Width - this.UserInput.Width) / 3, 50);
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

            //create a "next" button placed sous la grille
            Button done = new Button
            {
                Size = new Size(200, 100),
                Location = new Point((this.ClientSize.Width - 200) / 2, this.UserInput.Bottom + 10),
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
            //writting feature maps values in the console
            Console.WriteLine("========= Basic feature maps level 1 - not pooled - not flattered =========");
            foreach (double[,] map in convolutionals_Layers)
            {
                for (int i = 0; i < map.GetLength(0); i++)
                {
                    for (int j = 0; j < map.GetLength(1) - 1; j++)
                    {
                        if (map[i, j] == 0)
                        {
                            Console.Write("   ");
                        }
                        else if (map[i, j].ToString().Length == 1)
                        {
                            Console.Write(" " + map[i, j].ToString() + " ");
                        }
                        else
                        {
                            Console.Write(map[i, j].ToString() + " ");
                        }
                    }
                    Console.WriteLine("|");
                }
                Console.WriteLine("______________________________________________________________");
            }

            //conv layer 2
            Console.WriteLine("========= Basic feature maps level 2 - not pooled - not flattered =========");
            List<double[,]> maps_2 = new List<double[,]>();
            foreach (Kernel kernel_2 in this.Kernels[1])
            {
                maps_2.Add(kernel_2.RegenerateFeatureMap(convolutionals_Layers));
            }
            foreach (double[,] map in maps_2)
            {
                for (int i = 0; i < map.GetLength(0) - 1; i++)
                {
                    for (int j = 0; j < map.GetLength(1) - 1; j++)
                    {
                        if (map[i, j] == 0)
                        {
                            Console.Write("   ");
                        }
                        else if (map[i, j].ToString().Length == 1)
                        {
                            Console.Write(" " + map[i, j].ToString() + " ");
                        }
                        else
                        {
                            Console.Write(map[i, j].ToString() + " ");
                        }
                    }
                    Console.WriteLine("|");
                }
                Console.WriteLine("______________________________________________________________");
            }

            List<double[,]> pooled_maps = GeneralMaxPooling(maps_2);
            //pooled feature maps
            Console.WriteLine("========= Pooled maps with max_pooling =========");
            foreach (double[,] map in pooled_maps)
            {
                for (int i = 0; i < map.GetLength(0) - 1; i++)
                {
                    for (int j = 0; j < map.GetLength(1) - 1; j++)
                    {
                        //get rid of the non-valuable informations - ONLY VISUALLY
                        if (map[i, j] < 38)
                        {
                            Console.Write("   ");
                        }
                        else if (map[i, j].ToString().Length == 1)
                        {
                            Console.Write(" " + map[i, j].ToString() + " ");
                        }
                        else
                        {
                            Console.Write(map[i, j].ToString() + " ");
                        }
                    }
                    Console.WriteLine("|");
                }
                Console.WriteLine("____________________________________");
            }

            // checking what the flattered result looks like
            Console.WriteLine("========= Flattered result =========");
            foreach (double[,] map in pooled_maps)
            {
                for(int i = 0;i < map.GetLength(0) - 1; i++)
                {
                    for(int j = 0; j < map.GetLength(1) -1; j++)
                    {
                        Console.Write(map[(int)i, j].ToString());
                    }
                }
                Console.WriteLine("|");
            }

            //pool again
            Console.WriteLine("========= Pooled pooled maps with max_pooling =========");
            List<double[,]> pooled_pooled_maps = GeneralMaxPooling(pooled_maps);
            foreach (double[,] map in pooled_pooled_maps)
            {
                for (int i = 0; i < map.GetLength(0) - 1; i++)
                {
                    for (int j = 0; j < map.GetLength(1) - 1; j++)
                    {
                        //get rid of the non-valuable informations - ONLY VISUALLY
                        if (map[i, j] == 0)
                        {
                            Console.Write("   ");
                        }
                        else if (map[i, j].ToString().Length == 1)
                        {
                            Console.Write(" " + map[i, j].ToString() + " ");
                        }
                        else
                        {
                            Console.Write(map[i, j].ToString() + " ");
                        }
                    }
                    Console.WriteLine("|");
                }
                Console.WriteLine("____________________________________");
            }
            Console.WriteLine("========= Flattered result again =========");
            foreach (double[,] map in pooled_pooled_maps)
            {
                for (int i = 0; i < map.GetLength(0) - 1; i++)
                {
                    for (int j = 0; j < map.GetLength(1) - 1; j++)
                    {
                        Console.Write(map[(int)i, j].ToString());
                    }
                }
                Console.WriteLine("|");
            }

            // count the number of intresting values (The bigger is, the more interesting is)
            Console.WriteLine("========= Count of interesting features =========");
            List<double> list = new List<double>();
            foreach (double[,] map in pooled_pooled_maps)
            {
                double temp_counter20 = 0;
                double temp_counter38 = 0;
                foreach (double value  in map)
                {
                    //why 37 ?
                    // because a full line represents 37
                    if (value > 37)
                    {
                        temp_counter38++;
                    }
                    //why 20 ?
                    // because it is less than 37 but still big enought to not have like ~100 values
                    else if (value > 20)
                    {
                        temp_counter20++;
                    }
                }
                list.Add(temp_counter20);
                list.Add(temp_counter38);
            }
            foreach (double value in list)
            {
                Console.Write(value.ToString() + " ");
            }


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
            for (int i = 0; i < GRIDSIZE; i++)
            {
                for (int j = 0; j < GRIDSIZE; j++)
                {
                    this.gridToAnalyse[i, j] = Convert.ToInt16(this.UserInput.Controls[i * 32 + j].Tag);
                }
            }
            MakePrediction();
            ConsoleKey key = Console.ReadKey().Key;

            if (key == ConsoleKey.Enter)
            {
                Application.Restart();
            }
            else
            {
                Environment.Exit(0);
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
