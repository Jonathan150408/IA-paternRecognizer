using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IA05_Form
{
    public partial class IAForm : Form
    {
        /// <summary>
        /// The data to give to the console project
        /// </summary>
        public double[,] gridToAnalyse;
        public Dictionary<string, int> stats = new Dictionary<string, int>();
        public bool wantCorrection;
        public bool wantOperationDetails;

        /// <summary>
        /// Data used to set up the form
        /// </summary>
        int CellSize;
        private readonly uint[] GridDimensions;

        /// <summary>
        /// Form's composants
        /// </summary>
        FlowLayoutPanel correctionGroup;
        FlowLayoutPanel debugGroup;

        /// <summary>
        /// Contructor
        /// </summary>
        public IAForm(uint[] gridDimensions, int cellSize)
        {
            InitializeComponent();
            GridDimensions = gridDimensions;
            gridToAnalyse = new double[this.GridDimensions[0], this.GridDimensions[1]];
            CellSize = cellSize;
            HandleGrid();
        }

        /// <summary>
        /// Create the grid of 32x32 checkboxes (= 1024 checkboxes)
        /// </summary>
        /// <returns></returns>
        private void HandleGrid()
        {
            // GRID
            // 1. Set up the panel
            this.UserInput.AutoSize = false;
            this.UserInput.Size = new Size((int)this.GridDimensions[0] * CellSize, (int)this.GridDimensions[1] * CellSize);
            this.UserInput.BackColor = Color.White;
            // 2. Create a 32 x 32 square of checkboxes
            for (int i = 0; i < this.GridDimensions[0]; i++)
            {
                for (int j = 0; j < this.GridDimensions[1]; j++)
                {
                    CheckBox box = new CheckBox
                    {
                        Appearance = Appearance.Button,
                        Tag = 0,
                        Size = new Size(CellSize, CellSize),
                        Location = new Point(j * CellSize, i * CellSize),
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
                    // 4. Add the checkbox to the form
                    this.UserInput.Controls.Add(box);
                }
            }
            this.UserInput.Location = new Point((this.ClientSize.Width / 2 - this.UserInput.Width / 2), 50);

            // CORRECTION TOGGLE
            // 5. Create a toggle button to set the correction mode
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
            RadioButton correctionTrue = new RadioButton()
            {
                Text = "On",
                Tag = true,
                Appearance = Appearance.Button,
                ForeColor = Color.White,
                Checked = true,
                BackColor = Color.LightGreen
            };
            RadioButton correctionFalse = new RadioButton()
            {
                Text = "Off",
                Tag = false,
                Appearance = Appearance.Button,
                ForeColor = Color.White,
            };
            // 6. Add the field to the form
            this.Controls.Add(correctionGroup);
            this.Controls.Add(correctionLabel);
            correctionGroup.Controls.Add(correctionTrue);
            correctionGroup.Controls.Add(correctionFalse);
            // 7. Link the buttons with method
            foreach (RadioButton radioButton in correctionGroup.Controls)
            {
                radioButton.Click += RadioButton_Click;
            }

            // DEBUG TOGGLE
            // 8. Create a toggle button to set the correction mode
            debugGroup = new FlowLayoutPanel()
            {
                Name = "Montrer tous les calculs ?",
                ForeColor = Color.White,
                Location = new Point(50, 250)
            };
            Label debugLabel = new Label()
            {
                Text = "Montrer tous les calculs ?",
                ForeColor = Color.White,
                Location = new Point(50, 225),
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 10)
            };
            RadioButton debugTrue = new RadioButton()
            {
                Text = "On",
                Tag = true,
                Appearance = Appearance.Button,
                ForeColor = Color.White,
            };
            RadioButton debugFalse = new RadioButton()
            {
                Text = "Off",
                Tag = false,
                Appearance = Appearance.Button,
                ForeColor = Color.White,
                Checked = true,
                BackColor = Color.LightGreen
            };
            // 9. Add the field to the form
            this.Controls.Add(debugGroup);
            this.Controls.Add(debugLabel);
            debugGroup.Controls.Add(debugTrue);
            debugGroup.Controls.Add(debugFalse);
            // 10. Link the buttons with method
            foreach (RadioButton radioButton in debugGroup.Controls)
            {
                radioButton.Click += RadioButton_Click;
            }

            // SUBMIT BUTTON
            // 11. Create a submit button
            Button validation = new Button
            {
                Size = new Size(200, 100),
                Text = "GO",
                ForeColor = Color.White,
                Font = new Font(this.Font.FontFamily, 20, FontStyle.Italic),
                Cursor = Cursors.Hand,
                TabStop = true
            };
            validation.Location = new Point((this.ClientSize.Width - validation.Width) / 2, this.UserInput.Bottom + 10);
            // 12. Add the submit button
            this.Controls.Add(validation);
            validation.BringToFront();
            // 13. Link the button with method
            validation.Click += Validation_Click;
        }

        /// <summary>
        /// Locks the form, get the data and give them to the console project
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Validation_Click(object sender, EventArgs e)
        {
            // GRID
            // 1. Convert the grid into values (0 or 1)
            for (int i = 0; i < this.GridDimensions[0]; i++)
            {
                for (int j = 0; j < this.GridDimensions[1]; j++)
                {
                    this.gridToAnalyse[i, j] = Convert.ToInt16(this.UserInput.Controls[i * (int)this.GridDimensions[0] + j].Tag);
                }
            }

            // CORRECTION
            // 2. Convert the correction field
            foreach (RadioButton radioButton in correctionGroup.Controls)
            {
                if (radioButton.Checked)
                {
                    wantCorrection = Convert.ToBoolean(radioButton.Tag);
                }
            }

            // DEBUG
            // 3. Convert the debug field
            foreach (RadioButton radioButton in debugGroup.Controls)
            {
                if (radioButton.Checked)
                {
                    wantOperationDetails = Convert.ToBoolean(radioButton.Tag);
                }
            }

            // 4. Send signal and close the form
            DialogResult = DialogResult.OK;
            Close();
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
