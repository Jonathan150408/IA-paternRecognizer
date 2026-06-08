using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IA04.Models
{
    /// <summary>
    /// layer : couche du réseau neuronal contenant soit des kernels, soit des neurones
    /// </summary>
    public partial class Layer
    {
        /// <summary>
        /// Permet de connaitre la position de la couche dans le réseau.
        /// Les couches sont utilisées dans l'ordre croissant de leurs ids.
        /// </summary>
        [JsonPropertyName("Id")]
        public int Id { get; set; }

        /// <summary>
        /// layerType : enum permettant de classer les couches en catégories, la catégorie définit le comportement de la couche (layer)
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum layerType
        {
            convolutive,
            full
        }
        /// <summary>
        /// Permet de déterminer si il s'agit d'une couche de neurones "classiques" ou de kernels
        /// convolutive correspond à une couche de kernels
        /// full correspond à une couche de neurones "classiques"
        /// </summary>
        [JsonPropertyName("Type")]
        public layerType Type { get; set; }

        /// <summary>
        /// Kernels : liste de filtres de la couche, elle est facultative car un couche peut être composée de neurones.
        /// </summary>
        [JsonPropertyName("Kernels")]
        public List<Kernel> Kernels { get; set; }

        /// <summary>
        /// Neurons : liste de neurones de la couche, elle est facultative car un couche peut être composée de kernels.
        /// </summary>
        [JsonPropertyName("Neurons")]
        public List<Neuron> Neurons { get; set; }

        /// <summary>
        /// ActivationFunction : enum spécifiant le nom de la fonction d'activation utilisée sur cette couche.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ActivationFunction
        {
            sigmoid,
            tanh,
            softmax,
            none
        }
        [JsonPropertyName("Function")]
        /// <summary>
        /// Function : fonction d'activation de la couche.
        /// </summary>
        public ActivationFunction Function { get; set; }

        /// <summary>
        /// Contructeur par défaut
        /// </summary>
        public Layer() { }
    }
}
