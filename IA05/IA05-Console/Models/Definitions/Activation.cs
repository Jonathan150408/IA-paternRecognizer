using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA05.Models
{
    /// <summary>
    /// Activation : diminutif de "fonction d'activation", fonction mathématique qui permet d'interpréter les sorties des neurones/kernels en limitant leur envergure
    /// </summary>
    public partial class Activation
    {
        public enum ActivationFunction
        {
            sigmoid,
            tanh,
            softmax,
            none
        }
        /// <summary>
        /// Définit la fonction appliquée
        /// </summary>
        public ActivationFunction Function { get; set; }

        /// <summary>
        /// Contructeur par défaut
        /// </summary>
        public Activation() { }
    }
}
