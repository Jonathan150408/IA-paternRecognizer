using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA04.Models
{
    /// <summary>
    /// Network : représente l'entièreté du réseau neuronal
    /// </summary>
    public class Network
    {
        /// <summary>
        /// Cette liste contient toutes les couches du réseau neuronal
        /// </summary>
        public List<Layer> Layers { get; set; }

        /// <summary>
        /// Le constructeur
        /// </summary>
        /// <param name="layers"></param>
        public Network(List<Layer> layers)
        {
            Layers = layers;
        }
    }
}
