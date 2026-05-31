using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA04.Models
{
    /// <summary>
    /// Kernel : filtre permettant de générer une feature map
    /// </summary>
    public class Kernel
    {
        /// <summary>
        /// Filter : un tableau de valeurs permettant d'extraire les features
        /// </summary>
        private double[,] _filter;
        public double[,] Filter
        {
            get { return _filter; }
            private set { _filter = value; }
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        public Kernel(double[,] values)
        {
            _filter = values;
        }
    }
}
