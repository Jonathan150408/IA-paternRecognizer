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
        /// L'historique des résultats du réseau neuronal
        /// </summary>
        public List<object> history { get; set; }

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public Network()
        {
            history = new List<object>();
        }
    }
}
