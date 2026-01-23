namespace IA03
{
    partial class IA
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
            this.UserInput.Location = new System.Drawing.Point(488, 149);
            this.UserInput.Margin = new System.Windows.Forms.Padding(0);
            this.UserInput.Name = "UserInput";
            this.UserInput.Size = new System.Drawing.Size(200, 100);
            this.UserInput.TabIndex = 17;
            // 
            // IA
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(1061, 743);
            this.Controls.Add(this.UserInput);
            this.Name = "IA";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel UserInput;
    }
}

