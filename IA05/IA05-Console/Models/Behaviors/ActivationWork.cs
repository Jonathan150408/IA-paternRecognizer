using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA05.Models
{
    public partial class Activation
    {
        /// ---------------------------------------------------------------------------------------------
        /// WITH LIST OF DOUBLE FOR A NEURON
        /// ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Applies the activation function to the list of results
        /// </summary>
        /// <param name="inputs">These are the raw outputs from the layer</param>
        /// <returns>A list of double that represents the layers's final results</returns>
        public List<double> ApplyFunction(List<double> inputs, Layer.ActivationFunction function)
        {
            // 1. Set up variables
            List<double> outputs;

            // 2. Calculates the result with the function
            switch (function)
            {
                case Layer.ActivationFunction.softmax:
                    outputs = SoftMax(inputs);
                    break;
                case Layer.ActivationFunction.tanh:
                    outputs = Tanh(inputs);
                    break;
                case Layer.ActivationFunction.sigmoid:
                    outputs = Sigmoid(inputs);
                    break;
                case Layer.ActivationFunction.none:
                    outputs = inputs;
                    break;
                default:
                    throw new EntryPointNotFoundException();
            }

            // 3. Return the result
            return outputs;
        }
        /// <summary>
        /// Applies the softmax function to the list
        /// </summary>
        /// <param name="raw_outputs"></param>
        /// <returns>A list of double where each value represents a percentage of probability</returns>
        private List<double> SoftMax(List<double> raw_outputs)
        {
            // 1. Get the greatest input
            double max_value = raw_outputs[0];
            for (int i = 1; i < raw_outputs.Count; i++)
            {
                if (raw_outputs[i] > max_value)
                {
                    max_value = raw_outputs[i];
                }
            }

            // 2. Calculate the exponentials
            List<double> exponentials = new List<double>(raw_outputs.Count);
            foreach (double raw_output in raw_outputs)
            {
                exponentials.Add(Math.Exp(raw_output - max_value)); //subtract the max so softmax becomes more accurate
            }

            // 3. Calculate the sum
            double exponentials_sum = 0;
            foreach (double exponential in exponentials)
            {
                exponentials_sum += exponential;
            }

            // 4. Calculate the probability
            List<double> finals_outputs = new List<double>(exponentials.Count);
            for (int i = 0; i < exponentials.Count; i++)
            {
                finals_outputs.Add(exponentials[i] / exponentials_sum);
            }

            // 5. Returns the results
            return finals_outputs;
        }

        /// <summary>
        /// Applies the Tanh function to each output.
        /// </summary>
        /// <param name="raw_outputs"></param>
        /// <returns>A list of double that represents the layers's final results</returns>
        private List<double> Tanh(List<double> raw_outputs)
        {
            // 1. Set up variables
            List<double> final_outputs = new List<double>(raw_outputs.Count);

            // 2. Browse the list and add the result
            foreach (double raw_output in raw_outputs)
            {
                final_outputs.Add(Math.Tanh(raw_output));
            }

            // 3. Return the result
            return final_outputs;
        }

        /// <summary>
        /// Applies the Sigmoid function to each output.
        /// </summary>
        /// <param name="raw_outputs"></param>
        /// <returns>A list of double that represents the layers's final results</returns>
        private List<double> Sigmoid(List<double> raw_outputs)
        {
            // 1. Set up variables
            List<double> final_outputs = new List<double>(raw_outputs.Count);

            // 2. Browse the list and add the result
            foreach (double raw_output in raw_outputs)
            {
                final_outputs.Add(1.0 / (1.0 + Math.Exp(-raw_output)));
            }

            // 3. Return the result
            return final_outputs;
        }

        /// ---------------------------------------------------------------------------------------------
        /// WITH AN ARRY OF DOUBLE FOR A KERNEL
        /// ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Applies the activation function to the list of results
        /// </summary>
        /// <param name="inputs">These are the raw outputs from the layer</param>
        /// <returns>A list of double that represents the layers's final results</returns>
        public double[,] ApplyFunction(double[,] inputs, Layer.ActivationFunction function)
        {
            // 1. Set up variables
            double[,] outputs;

            // 2. Calculates the result with the function
            switch (function)
            {
                case Layer.ActivationFunction.softmax:
                    throw new InvalidProgramException("Impossible d'appliquer SoftMax à une feature map");
                case Layer.ActivationFunction.tanh:
                    outputs = Tanh(inputs);
                    break;
                case Layer.ActivationFunction.sigmoid:
                    outputs = Sigmoid(inputs);
                    break;
                case Layer.ActivationFunction.none:
                    outputs = inputs;
                    break;
                default:
                    throw new EntryPointNotFoundException();
            }

            // 3. Return the result
            return outputs;
        }

        /// <summary>
        /// Applies the Tanh function to each output.
        /// </summary>
        /// <param name="raw_outputs"></param>
        /// <returns>An array of double that represents the layers's final results</returns>
        private double[,] Tanh(double[,] raw_outputs)
        {
            // 1. Set up variables
            double[,] result = new double[raw_outputs.GetLength(0), raw_outputs.GetLength(1)];

            // 2. Browse the list and add the result
            for (int i = 0; i < raw_outputs.GetLength(0); i++)
            {
                for (int j = 0; j < raw_outputs.GetLength(1); j++)
                {
                    result[i, j] = Math.Tanh(raw_outputs[i, j]);
                }
            }

            // 3. Return the result
            return result;
        }

        /// <summary>
        /// Applies the Sigmoid function to each output.
        /// </summary>
        /// <param name="raw_outputs"></param>
        /// <returns>An array of double that represents the layers's final results</returns>
        private double[,] Sigmoid(double[,] raw_outputs)
        {
            // 1. Set up variables
            double[,] result = new double[raw_outputs.GetLength(0), raw_outputs.GetLength(1)];

            // 2. Browse the list and add the result
            for (int i = 0; i < raw_outputs.GetLength(0); i++)
            {
                for (int j = 0; j < raw_outputs.GetLength(1); j++)
                {
                    result[i, j] = Math.Tanh(1.0 / (1.0 + Math.Exp(-raw_outputs[i, j])));
                }
            }

            // 3. Return the result
            return result;
        }
    }
}
