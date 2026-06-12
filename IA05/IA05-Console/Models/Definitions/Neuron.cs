using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IA05.Models
{
    /// <summary>
    /// Neuron : IA's neuron, this is what let's the neural network "think" and process data.
    /// </summary>
    public partial class Neuron
    {
        /// <summary>
        /// Id : a positive unique integer.
        /// </summary>
        [JsonPropertyName("Id")]
        public int Id { get; set; }

        /// <summary>
        /// Weigths : an array of doubles [], the neuron's values that quantifies the importance of each input.
        /// </summary>
        [JsonPropertyName("Weights")]
        public double[] Weights { get; set; }

        /// <summary>
        /// Adjustment : a double that quantifies the neuron's importance in the network.
        /// </summary>
        [JsonPropertyName("Adjustment")]
        public double Adjustment {  get; set; }

        /// <summary>
        /// Output : a double that stores the neuron's result for further uses.
        /// </summary>
        [JsonIgnore]
        public double Output { get; set; }

        /// <summary>
        /// Delta : a double that quantifies the gap between the "correct" and the current answers.
        /// </summary>
        [JsonIgnore]
        public double Delta { get; set; }

        /// <summary>
        /// Default contructor for JSONSerializer.
        /// </summary>
        public Neuron(){}
    }
}
