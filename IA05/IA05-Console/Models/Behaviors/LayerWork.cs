using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;

namespace IA04.Models
{
    public partial class Layer
    {
        private Activation functionHandler = new Activation();

        /// ----------------------------------------------------------------------------------------------
        /// METHODS FOR A LAYER OF NEURONS
        /// ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Calculate the result for each neuron and apply the activation function.
        /// </summary>
        /// <param name="inputs">The raw values to process</param>
        /// <returns>Returns a list of doubles.</returns>
        public List<double> GetLayerResults(List<double> inputs)
        {
            // 1. Check the layer's type
            if (this.Type != layerType.full)
            {
                throw new InvalidOperationException("Seule les couches feed-forward effectuer cette operation. Cette couche est de type " + this.Type.ToString());
            }

            // 2. Set up the variables
            List<double> results = new List<double>(this.Neurons.Count);
            int index = 0;

            // 3. Calculate every neurons result
            foreach (Neuron neuron in this.Neurons)
            {
                results.Add(neuron.GetResult(inputs));
                index++;
            }

            // 4. Applies the activation function
            functionHandler.ApplyFunction(results, this.Function);

            // 5. Return the results
            return results;
        }

        /// ----------------------------------------------------------------------------------------------
        /// METHODS FOR A LAYER OF KERNELS
        /// ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Generate a feature map for every kernel of this layer. Only work if the layer is a conv layer.
        /// </summary>
        /// <param name="rawGrid"></param>
        /// <returns></returns>
        public List<double[,]> MakeFeatureMaps(double[,] rawGrid)
        {
            // 1. Check the layer's type
            if (this.Type != layerType.convolutive)
            {
                throw new InvalidOperationException("Seule les couches convolutives peuvent générer des feature maps. Cette couche est de type " + this.Type.ToString());
            }

            // 2. Set up variables
            List<double[,]> featureMaps = new List<double[,]>();

            // 3. Add the feature map of every kernel
            foreach (Kernel kernel in this.Kernels)
            {
                featureMaps.Add(kernel.MakeFeatureMap(rawGrid));
            }

            // 4. Applies the function
            for (int i = 0; i < featureMaps.Count; i++)
            {
                featureMaps[i] = functionHandler.ApplyFunction(featureMaps[i], this.Function);
            }

            // 5. Return the results
            return featureMaps;
        }
        /// <summary>
        /// Generate a feature map for every kernel of this layer based on multiple previous maps. Only work if the layer is a conv layer.
        /// </summary>
        /// <param name="rawGrid"></param>
        /// <returns>A list of maps (double [,])</returns>
        public List<double[,]> MakeFeatureMaps(List<double[,]> rawGrid)
        {
            // 1. Check the layer's type
            if (this.Type != layerType.convolutive)
            {
                throw new InvalidOperationException("Seule les couches convolutives peuvent générer des feature maps. Cette couche est de type " + this.Type.ToString());
            }

            // 2. Set up variables
            List<double[,]> featureMaps = new List<double[,]>();

            // 3. Add the feature map of every kernel
            foreach (Kernel kernel in this.Kernels)
            {
                featureMaps.Add(kernel.MakeFeatureMap(rawGrid));
            }

            // 4. Applies the function
            for (int i = 0; i < featureMaps.Count; i++)
            {
                featureMaps[i] = functionHandler.ApplyFunction(featureMaps[i], this.Function);
            }

            // 5. Return the results
            return featureMaps;
        }

        /// ----------------------------------------------------------------------------------------------
        /// OTHERS METHODS
        /// ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Correct the layer in the code (no writting in files)
        /// </summary>
        /// <param name="expected_results"></param>
        /// <param name="real_outputs"></param>
        /// <param name="processed_values"></param>
        public void CorrectLayer(double[] expected_results, List<double> real_outputs, List<double> processed_values)
        {
            if (this.Type == layerType.full)
            {
                for (int i = 0; i < this.Neurons.Count; i++)
                {
                    this.Neurons[i].CorrectNeuron(expected_results[i], real_outputs[i], processed_values, this.Function);
                }
            }
            else if (this.Type == layerType.convolutive)
            {
                throw new NotImplementedException();
            }
        }
    }
}
