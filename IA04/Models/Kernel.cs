using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IA04.Models
{
    /// <summary>
    /// Kernel : filtre permettant de générer une feature map
    /// </summary>
    public class Kernel
    {
        /// <summary>
        /// Permet d'ordonner les neurones de la couche
        /// </summary>
        [JsonPropertyName("Id")]
        public int Id { get; set; }

        /// <summary>
        /// Filter : un tableau de valeurs permettant d'extraire les features
        /// </summary>
        [JsonPropertyName("Filter")]
        public double[][] Filter { get; set; }

        /// <summary>
        /// Contructeur par défaut
        /// </summary>
        public Kernel() { }
    }
}
