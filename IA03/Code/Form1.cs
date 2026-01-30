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
        private const int COLUMNS_NUMBER = 32;
        private const int ROWS_NUMBER = 32;
        /// <summary>
        /// The grid we'll analyse
        /// </summary>
        public double[,] gridToAnalyse = new double[ROWS_NUMBER,COLUMNS_NUMBER];
        /// <summary>
        /// The neuronal network
        /// </summary>
        private readonly List<Layer> Network;
        /// <summary>
        /// The kernels (= filters) used to analyse the grid
        /// </summary>
        private readonly List<Kernel> Kernels;

        /// <summary>
        /// The main program
        /// </summary>
        public IA()
        {
            InitializeComponent();

            //initialize the network
            Network = new List<Layer>();
            Kernels = new List<Kernel>();

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
                string[] str_allValues = str_wholeLayer[1].Split(';');

                //converts the activation function
                Enum.TryParse(str_wholeLayer[0], ignoreCase: true, out Function activation);
                tempLayer = new Layer(new List<Neuron>(), activation);

                // For a layer
                if (activation != Layer.Function.kernel)
                {
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
                    Kernel tempKernel = new Kernel(new double[str_allValues[0].Trim().Split(' ').GetLength(0), str_allValues.Length]);

                    //foreach line of values as text, we take it and...
                    for (int i = 0; i < str_allValues.Length; i++)
                    {
                        string[] str_filterValues = str_allValues[i].Trim().Split(' ');
                        //double[,] dbl_filterValues = new double[str_filterValues.Length, str_allValues.Length];

                        //...we separate it into values (as text) that we convert into doubles
                        for (int j = 0; j < str_filterValues.Length; j++)
                        {
                            double.TryParse(str_filterValues[i], out double dbl_currentValue);
                            tempKernel.filter[i, j] = dbl_currentValue;
                        }
                    }

                    this.Kernels.Add(tempKernel);
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
            this.UserInput.Location = new Point((this.ClientSize.Width - this.UserInput.Width)/3, 50);

            this.UserInput.AutoSize = true;
            this.UserInput.BackColor = Color.White;

            //create a 32x32 square of  checkboxes
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    this.UserInput.Controls.Add(new System.Windows.Forms.CheckBox
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
            foreach (System.Windows.Forms.CheckBox checkBox in this.UserInput.Controls)
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
            List<double[,]> convolutionals_Layers = new List<double[,]>();
            foreach(Kernel kernel in Kernels)
            {
                convolutionals_Layers.Add(kernel.GenerateFeatureMap(gridToAnalyse));
            }
            //writting feature maps values in the console
            foreach (double[,] map in convolutionals_Layers)
            {
                for (int i = 0; i < map.GetLength(0); i++)
                {
                    for (int j = 0; j < map.GetLength(1) - 1; j++)
                    {
                        if (map[i, j] == 0)
                        {
                            Console.Write(" ");
                        }
                        else
                            Console.Write(map[i, j].ToString());
                    }
                    Console.WriteLine("|");
                }
                Console.WriteLine("________________________");
            }

            List<double[,]> pooled_maps = MaxPooling(convolutionals_Layers);
            //writting pooled feature maps values in the console
            foreach (double[,] map in pooled_maps)
            {
                for (int i = 0; i < map.GetLength(0); i++)
                {
                    for (int j = 0; j < map.GetLength(1) - 1; j++)
                    {
                        if (map[i, j] == 0)
                        {
                            Console.Write(" ");
                        }
                        else
                            Console.Write(map[i, j].ToString());
                    }
                    Console.WriteLine("|");
                }
                Console.WriteLine("________________________");
            }
        }
        private List<double[,]> MaxPooling(List<double[,]> feature_maps)
        {
            List<double[,]> pooled_maps = new List<double[,]>();

            foreach (double[,] map in feature_maps)
            {
                double[,] temp_pooled_map = new double[(int)Math.Ceiling((double)(map.GetLength(0) / 2)), (int)Math.Ceiling((double)(map.GetLength(1) / 2))];
                for (int i = 0; i < map.GetLength(0); i += 2)
                {
                    for (int j = 0; j < map.GetLength(1) -1; j += 2)
                    {
                        try
                        {
                            temp_pooled_map[(int)Math.Ceiling((double)(i / 2)), (int)Math.Ceiling((double)(j / 2))] =
                                Math.Max(map[i, j],
                                Math.Max(map[i + 1, j],
                                Math.Max(map[i, j + 1], (map[i + 1, j + 1]))));
                        }
                        catch (Exception)
                        {
                            try
                            {
                                temp_pooled_map[(int)Math.Ceiling((double)(i / 2)), (int)Math.Ceiling((double)(j / 2))] = Math.Max(map[i, j], map[i + 1, j]);
                            }
                            catch (Exception)
                            {
                                try
                                {
                                    temp_pooled_map[(int)Math.Ceiling((double)(i / 2)), (int)Math.Ceiling((double)(j / 2))] = Math.Max(map[i, j], map[i, j + 1]);
                                }
                                catch (Exception)
                                {
                                    temp_pooled_map[(int)Math.Ceiling((double)(i / 2)), (int)Math.Ceiling((double)(j / 2))] = map[i, j];

                                }
                            }
                        }

                    }
                }
            }

            return pooled_maps;
        }
        /// <summary>
        /// Gets the values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Done_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ROWS_NUMBER; i++)
            {
                for (int j = 0; j < COLUMNS_NUMBER; j++)
                {
                    this.gridToAnalyse[i, j] = Convert.ToInt16(this.UserInput.Controls[i * 32 + j].Tag);
                }
            }

            //foreach (CheckBox checkBox in this.UserInput.Controls)
            //    gridToAnalyse.Add(Convert.ToInt16(checkBox.Tag));
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
