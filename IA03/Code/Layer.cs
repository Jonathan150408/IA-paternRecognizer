using IA03;
using System;
using System.Collections.Generic;
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
			abs_sum
		}
		public enum Type
		{
			kernel,
			full_layer
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
		/// Represents the usage of the layer (filter/analyse)
		/// </summary>
		private Type _type;
		public Type type
		{
			get { return _type; }
			set { _type = value; }
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
		public Layer(List<Neuron> neurons, Function func, Type usage)
		{
			Neurons = neurons;
			function = func;
			type = usage;
		}
		/// <summary>
		/// Calculate the result of the layer
		/// </summary>
		/// <param name="inputs">The inputs of the layer</param>
		/// <returns>An array where each neuron has a place</returns>
		public double[] GetLayerResults(List<double> inputs)
		{
			double[] results = new double[this.Neurons.Count];
			int index = 0;
			foreach (Neuron neuron in this.Neurons)
			{
				results[index] = neuron.GetResult(inputs, this.function);
				index++;
			}
			return results;
		}

	}
}
