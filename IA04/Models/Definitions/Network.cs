using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IA04.Models
{
    /// <summary>
    /// Network : représente l'entièreté du réseau neuronal
    /// </summary>
    public partial class Network
    {
        /// <summary>
        /// Cette liste contient toutes les couches du réseau neuronal
        /// </summary>
        [JsonPropertyName("Layers")]
        public List<Layer> Layers { get; set; }

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public Network(){ }
    }
}
