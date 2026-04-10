using System;
using System.Collections.Generic;
using System.Text;

namespace IACore.Model
{
    /// <summary>
    /// Activation : diminutif de "fonction d'activation", fonction mathématique qui permet d'interpréter les sorties des neurones/kernels en limitant leur envergure
    /// </summary>
    public class Activation
    {
        public enum activationFunction
        {
            sigmoid,
            tanh,
            softmax,
            none
        }
        /// <summary>
        /// Définit la fonction appliquée
        /// </summary>
        private activationFunction _function;
        public activationFunction Function
        {
            get { return _function; }
            private set { _function = value; }
        }

        public List<double> ApplyFunction(List<double> inputs)
        {
            switch (this.Function)
            {

            }

            return null;
        }

        private List<double> SoftMax(List<double> raw_outputs)
        {
            //1. get the greatest input
            double max_value = raw_outputs[0];
            for (int i = 1; i < raw_outputs.Count; i++)
            {
                if (raw_outputs[i] > max_value)
                {
                    max_value = raw_outputs[i];
                }
            }
            //2. calculate the exponentials
            List<double> exponentials = new List<double>(raw_outputs.Count);
            foreach (double raw_output in raw_outputs)
            {
                exponentials.Add(Math.Exp(raw_output - max_value)); //subtract the max so softmax becomes more accurate
            }
            //3. calculate the sum
            double exponentials_sum = 0;
            foreach (double exponential in exponentials)
            {
                exponentials_sum += exponential;
            }
            //4. calculate the probability
            List<double> finals_outputs = new List<double>(exponentials.Count);
            for (int i = 0; i < exponentials.Count; i++)
            {
                finals_outputs.Add(exponentials[i] / exponentials_sum);
            }

            return finals_outputs;
        }
    }
}
