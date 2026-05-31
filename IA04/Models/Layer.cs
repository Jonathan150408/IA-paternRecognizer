using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA04.Models
{
    /// <summary>
    /// layer : couche du réseau neuronal contenant soit des kernels, soit des neurones
    /// </summary>
    public class Layer
    {
        /// <summary>
        /// Kernels : liste de filtres de la couche, elle est facultative car un couche peut être composée de neurones
        /// </summary>
        private List<Kernel> _kernels;
        public List<Kernel> Kernels
        {
            get { return _kernels; }
            set { _kernels = value; }
        }

        /// <summary>
        /// Neurons : liste de neurones de la couche, elle est facultative car un couche peut être composée de kernels
        /// </summary>
        private List<Neuron> _neurons;
        public List<Neuron> Neurons
        {
            get { return _neurons; }
            set { _neurons = value; }
        }

        /// <summary>
        /// Function : fonction d'activation de la couche
        /// </summary>
        private Activation _function;
        public Activation Function
        {
            get { return _function; }
            set { _function = value; }
        }

        /// <summary>
        /// constructeur pour une couche de kernels
        /// </summary>
        public Layer(Activation function, List<Kernel> kernels)
        {
            this._function = function;
            this._kernels = kernels;
        }

        /// <summary>
        /// constructeur pour une couche de neurones
        /// </summary>
        public Layer(Activation function, List<Neuron> neurons)
        {
            this._function = function;
            this._neurons = neurons;
        }
    }
}
