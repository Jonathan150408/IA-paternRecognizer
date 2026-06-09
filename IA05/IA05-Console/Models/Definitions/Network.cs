using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IA05.Models
{
    /// <summary>
    /// Network : représente l'entièreté du réseau neuronal.
    /// </summary>
    public partial class Network
    {
        /// <summary>
        /// Layers : The list of all layers in the neural network.
        /// </summary>
        [JsonPropertyName("Layers")]
        public List<Layer> Layers { get; set; }

        /// <summary>
        /// Schema : A list of string that defines what steps the Network will follow.
        /// </summary>
        [JsonPropertyName("Schema")]
        public List<string> Schema { get; set; }

        /// <summary>
        /// History : A list of results that the IA calculated.
        /// </summary>
        public List<List<double[,]>> History;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Network()
        {
            History = new List<List<double[,]>>();
        }
    }
}
