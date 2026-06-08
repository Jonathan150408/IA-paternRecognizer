using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IA05_Form
{
    public partial class Form1 : Form
    {
        //List<double> data = new List<double>();
        public string Results = "";
        TextBox name = new TextBox();
        Button validation = new Button();
        public Form1()
        {
            InitializeComponent();
            InitializeForm();
        }
        private void InitializeForm()
        {
            this.Controls.Add(name);
            this.Controls.Add(validation);
            validation.Click += Validation_Click;
        }

        private void Validation_Click(object sender, EventArgs e)
        {
            Results = name.Text;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
