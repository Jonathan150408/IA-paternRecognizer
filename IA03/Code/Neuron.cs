using IA03;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA03
{
    class Neuron
    {
        /// <summary>
        /// Weights
        /// </summary>
        private double[] _weights;
        public double[] Weights
        {
            get { return _weights; }
            private set { _weights = value; }
        }

        /// <summary>
        /// Adjustment
        /// </summary>
        private double _adjustment;
        public double Adjustment
        {
            get { return _adjustment; }
            private set { _adjustment = value; }
        }


        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="adjustment"></param>
        public Neuron(double[] weights, double adjustment)
        {
            this._weights = weights;
            this._adjustment = adjustment;
        }

        /// <summary>
        /// makes the calculation and get the result for the neuron
        /// </summary>
        /// <returns></returns>
        public double GetResult(List<double> inputs, Layer.Function function)
        {
            double result = 0;

            if (function == Layer.Function.sigmoid || function == Layer.Function.tanh)
            {
                //adds the inpunt*weight result to result 16 times (1 for each input)
                for (int i = 0; i < this.Weights.Length; i++)
                    result += this._weights[i] * inputs[i];

                //adds the adjustement
                result += this._adjustment;

                //return the "activated" result
                switch (function)
                {
                    case Layer.Function.sigmoid:
                        return Sigmoid(result);
                    case Layer.Function.tanh:
                        return Math.Tanh(result);
                    default:
                        return result;
                }
            }
            else
            {
                //adds the inpunt*weight result to result 16 times (1 for each input)
                for (int i = 0; i < this.Weights.Length; i++)
                    result += Math.Abs(this._weights[i] * inputs[i]);
                return result + Adjustment;
            }

        }
        /// <summary>
        /// The sigmoid function
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private double Sigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }
        /// <summary>
        /// This method adjusts the weights and adjustment for a better result
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="given_values"></param>
        /// <param name="function"></param>
        public void Rework(double expected, List<double> given_values, Layer.Function function)
        {
            //output of the neuron
            double output = this.GetResult(given_values, function);
            //learning rate
            double learning_rate = 0.001;

            //updates the weights
            for (int i = 0; i < this.Weights.Length; i++)
            {
                this.Weights[i] += learning_rate * (expected - output) * given_values[i] * (1 - output * output);
            }

            //updates the adjustment
            this.Adjustment += learning_rate * (expected - output) * (1 - output * output);
        }

    }
}
