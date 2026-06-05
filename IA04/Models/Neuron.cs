using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IA04.Models
{
    /// <summary>
    /// Neuron : neurone du réseau neuronal qui permet au réseau de "réfléchir"
    /// </summary>
    public class Neuron
    {
        /// <summary>
        /// Permet d'ordonner les neurones de la couche
        /// </summary>
        [JsonPropertyName("Id")]
        public int Id { get; set; }

        /// <summary>
        /// Weigths : poids du neurone permettant de quantifier l'importance de chaque input du neuron afin de calculer l'output
        /// </summary>
        [JsonPropertyName("Weights")]
        public double[] Weights { get; set; }

        /// <summary>
        /// Adjustment : valeur quantifiant l'importance du neurone dans le réseau
        /// </summary>
        [JsonPropertyName("Adjustment")]
        public double Adjustment {  get; set; }

        /// <summary>
        /// Contructeur par défaut
        /// </summary>
        public Neuron(){}
    }
}
