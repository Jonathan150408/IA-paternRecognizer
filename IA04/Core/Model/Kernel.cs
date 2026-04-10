using System;
using System.Collections.Generic;
using System.Text;

namespace IACore.Model
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
