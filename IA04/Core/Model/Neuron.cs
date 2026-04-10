using System;
using System.Collections.Generic;
using System.Text;

namespace IACore.Model
{
    /// <summary>
    /// Neuron : neurone du réseau neuronal qui permet au réseau de "réfléchir"
    /// </summary>
    public class Neuron
    {
        /// <summary>
        /// Weigths : poids du neurone permettant de quantifier l'importance de chaque input du neuron afin de calculer l'output
        /// </summary>
        private double[] _weights;
        public double[] Weights
        {
            get { return _weights; }
            private set { _weights = value; }
        }

        /// <summary>
        /// Adjustment : valeur quantifiant l'importance du neurone dans le réseau
        /// </summary>
        private double _adjustment;
        public double Adjustment
        {
            get { return _adjustment; }
            private set { _adjustment = value; }
        }


        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="adjustment"></param>
        public Neuron(double[] weights, double adjustment)
        {
            this._weights = weights;
            this._adjustment = adjustment;
        }

    }
}
