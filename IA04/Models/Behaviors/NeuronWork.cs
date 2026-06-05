using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA04.Models
{
    public partial class Neuron
    {
        //learning rate
        const double LEARNING_RATE = 0.002;


        /// <summary>
        /// Calculate the result for a single neuron. Also applies the activation function.
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        public double GetResult(List<double> inputs)
        {
            double result = 0;

            // 1. Add input * weight to the result
            for (int i = 0; i < this.Weights.Length; i++)
                result += this.Weights[i] * inputs[i];

            // 2. Add the adjustement
            result += this.Adjustment;

            return result;
        }

        /// <summary>
        /// Correct the neuron based on the result and the expected answer.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="output"></param>
        /// <param name="processed_values"></param>
        /// <param name="function"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void CorrectNeuron(double expected, double output, List<double> processed_values, Layer.ActivationFunction function)
        {

            // 1. Calculate delta
            double delta;
            switch (function)
            {
                case Layer.ActivationFunction.tanh:
                    delta = (1 - output * output) * (output - expected);
                    break;
                case Layer.ActivationFunction.softmax:
                    delta = output - expected;
                    break;
                default:
                    throw new NotImplementedException();
            }

            // 2. Update the weights
            for (int i = 0; i < this.Weights.Length; i++)
            {
                this.Weights[i] -= LEARNING_RATE * processed_values[i] * delta;
            }

            // 3. Update the adjustment
            this.Adjustment -= LEARNING_RATE * delta;
        }
    }
}
