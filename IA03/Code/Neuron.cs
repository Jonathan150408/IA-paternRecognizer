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

            if (function == Layer.Function.abs_sum)
            {
                //adds the inpunt*weight result to result 16 times (1 for each input)
                for (int i = 0; i < this.Weights.Length; i++)
                    result += Math.Abs(this._weights[i] * inputs[i]);
                return result + Adjustment;
            }
            else
            {
                //adds input * weight to the result (1 for each input)
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
                    case Layer.Function.softmax:
                    case Layer.Function.none:
                    default:
                        return result;
                }
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
    /// Eachakes a litte correction on the values
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="output"></param>
    /// <param name="processed_values"></param>
        public (double[], double) CorrectNeuron(double expected, double output, List<double> processed_values, Layer.Function function)
        {
            //learning rate
            const double LEARNING_RATE = 0.01;

            //delta
            double delta;
            switch (function)
            {
                case Layer.Function.tanh:
                    delta = (1 - output * output) * (output - expected);
                    break;
                case Layer.Function.softmax:
                    delta = output - expected;
                    break;
                default:
                    throw new NotImplementedException();
            }

            //updates the weights
            double[] new_weights = new double[this.Weights.Length];
            for (int i = 0; i < this.Weights.Length; i++)
            {
                new_weights[i] = this.Weights[i] - LEARNING_RATE * processed_values[i] * delta;
            }

            //updates the adjustment
            double new_adjustment = this.Adjustment - LEARNING_RATE * delta;

            return (new_weights, new_adjustment);
        }

    }
}
