using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA04.Models
{
    /// <summary>
    /// Activation : diminutif de "fonction d'activation", fonction mathématique qui permet d'interpréter les sorties des neurones/kernels en limitant leur envergure
    /// </summary>
    public partial class Activation
    {
        public enum ActivationFunction
        {
            sigmoid,
            tanh,
            softmax,
            none
        }
        /// <summary>
        /// Définit la fonction appliquée
        /// </summary>
        public ActivationFunction Function { get; set; }

        /// <summary>
        /// Contructeur par défaut
        /// </summary>
        public Activation() { }

        /// <summary>
        /// Applique la fonction
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public List<double> ApplyFunction(List<double> inputs)
        {
            switch (this.Function)
            {
                case ActivationFunction.softmax:
                    return SoftMax(inputs);
                case ActivationFunction.tanh:
                    return Tanh(inputs);
                case ActivationFunction.sigmoid:
                    return Sigmoid(inputs);
                case ActivationFunction.none:
                    return inputs;
                default:
                    throw new EntryPointNotFoundException();
            }   
        }

        /// <summary>
        /// Applique la fonction SoftMax à la liste de valeurs donnée
        /// </summary>
        /// <param name="raw_outputs"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Applique la fonction Tanh à chaque output individuellement, puis les rend ensemble
        /// </summary>
        /// <param name="raw_outputs"></param>
        /// <returns></returns>
        private List<double> Tanh(List<double> raw_outputs)
        {
            List<double> final_outputs = new List<double>(raw_outputs.Count);

            foreach (double raw_output in raw_outputs)
            {
                final_outputs.Add(Math.Tanh(raw_output));
            }

            return final_outputs;
        }


        /// <summary>
        /// Applique la fonction Sigmoid à chaque output individuellement, puis les rend ensemble
        /// </summary>
        /// <param name="raw_outputs"></param>
        /// <returns></returns>
        private List<double> Sigmoid(List<double> raw_outputs)
        {
            List<double> final_outputs = new List<double>(raw_outputs.Count);

            foreach (double raw_output in raw_outputs)
            {
                final_outputs.Add(1.0 / (1.0 + Math.Exp(-raw_output)));
            }

            return final_outputs;
        }
    }
}
