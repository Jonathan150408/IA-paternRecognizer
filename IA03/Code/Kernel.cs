using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA03.Code
{
    internal class Kernel
    {
		/// <summary>
		/// The doubles that will filter the area
		/// </summary>
		private double[,] _filter;
		public double[,] filter
		{
			get { return _filter; }
			set { _filter = value; }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public Kernel(double[,] values)
		{
			_filter = values;
		}

		/// <summary>
		/// Creates the feature map of the given grid according to the filter = overline the corresponding pattern
		/// </summary>
		/// <param name="rawGrid"></param>
		/// <returns></returns>
		public double[,] GenerateFeatureMap(double[,] rawGrid)
		{
			double[,] featureMap = new double[rawGrid.GetLength(0) - filter.GetLength(0) + 1, rawGrid.GetLength(1) - filter.GetLength(1) + 1];
			double current_result = 0;
			//browses the raw grid and calculate if the pattern matches the filter
			for (int i = 0; i < featureMap.GetLength(0); i++)
			{
				for (int j = 0; j < featureMap.GetLength(1); j++)
				{
					current_result = 0;
					for (int iFromFilter = 0; iFromFilter < this.filter.GetLength(0); iFromFilter++)
					{
						for (int jFromFilter = 0; jFromFilter < this.filter.GetLength(1); jFromFilter++)
						{
							current_result += this.filter[iFromFilter, jFromFilter] * rawGrid[i + iFromFilter, j + jFromFilter];
						}
					}
					featureMap[i, j] = current_result;
				}
			}


			return featureMap;
		}
	}
}
