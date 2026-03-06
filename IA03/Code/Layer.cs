using IA03;
using IA03.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA03
{
    internal class Layer
    {
		/// <summary>
		/// Enums constructor
		/// </summary>
		public enum Function
		{
			sigmoid,
			tanh,
			abs_sum,			//previously used by me to sum the absolute values
			kernel,				//used to make the programm know, that this is a kernel and not a layer
			softmax,
			none
		}
		/// <summary>
		/// Represents the function of the layer
		/// </summary>
		private Function _function;
		public Function function
		{
			get { return _function; }
			private set { _function = value; }
		}

		/// <summary>
		/// The neurons of the layer
		/// </summary>
		private List<Neuron> _neurons;
		public List<Neuron> Neurons
		{
			get { return _neurons; }
			private set { _neurons = value; }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="neurons"></param>
		public Layer(List<Neuron> neurons, Function func)
		{
			Neurons = neurons;
			function = func;
		}
		/// <summary>
		/// Calculate the result of the layer
		/// </summary>
		/// <param name="inputs">The inputs of the layer</param>
		/// <returns>An array where each neuron has a place</returns>
		public List<double> GetLayerResults(List<double> inputs)
		{
			List<double> results = new List<double>(this.Neurons.Count);
			int index = 0;
			foreach (Neuron neuron in this.Neurons)
			{
				results.Add(neuron.GetResult(inputs, this.function));
				index++;
			}

			//softmax needs all the raw results to operate, we can't place it in neuron.cs
			if (this.function == Function.softmax)
			{
				return	SoftMax(results);
			}
			//every other case has been calculated previously in neuron.cs
			else
			{
                return results;
            }
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
				finals_outputs.Add(exponentials[i]/exponentials_sum);
			}

			return finals_outputs;
		}

		public void CorrectLayer(double[] expected_results, List<double> real_outputs, List<double> processed_values)
        {
			string path = Path.Combine(AppContext.BaseDirectory, "Ressources", "layers", "layer1.txt");
			StreamWriter sw = new StreamWriter(path);
            sw.WriteLine("softmax+");
            //rewrite the file
            int counter = 0;
			foreach (Neuron neuron in this.Neurons)
			{
				(double[] temp_weights, double temp_adjustement) = neuron.CorrectNeuron(expected_results[counter], real_outputs[counter], processed_values, this.function);
				counter++;
				foreach (double dbl in temp_weights)
				{
					sw.Write(dbl + " ");
				}
				sw.Write(temp_adjustement);
				if (counter != 3)
				{
                    sw.WriteLine(";");
                }
            }
			sw.Close();
		}

	}
}
