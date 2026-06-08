namespace IA05_Form
{
    partial class IAForm
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.UserInput = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // UserInput
            // 
            this.UserInput.Location = new System.Drawing.Point(411, 64);
            this.UserInput.Name = "UserInput";
            this.UserInput.Size = new System.Drawing.Size(200, 100);
            this.UserInput.TabIndex = 0;
            // 
            // IAForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.Controls.Add(this.UserInput);
            this.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.Name = "IAForm";
            this.Text = "IAForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel UserInput;
    }
}

