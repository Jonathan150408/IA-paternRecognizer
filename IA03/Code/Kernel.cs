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
		private double[,,] _filter;
		public double[,,] Filter
		{
			get { return _filter; }
			set { _filter = value; }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public Kernel(double[,,] values)
		{
			_filter = values;
		}

		/// <summary>
		/// Creates the feature map of the given grid according to the filter = overline the corresponding pattern
		/// </summary>
		/// <param name="rawGrid"></param>
		/// <returns></returns>
		public double[,] GenerateFeatureMap(double[,] rawGrid, int kernel_index)
		{
			double[,] featureMap = new double[rawGrid.GetLength(0) - this.Filter.GetLength(0) + 1, rawGrid.GetLength(1) - this.Filter.GetLength(1) + 1];
			double current_result;
			//browses the raw grid and calculate if the pattern matches the filter
			for (int i = 0; i < featureMap.GetLength(0); i++)
			{
				for (int j = 0; j < featureMap.GetLength(1); j++)
				{
					// try to match the pattern
					current_result = 0;
					for (int iFromFilter = 0; iFromFilter < this.Filter.GetLength(0); iFromFilter++)
					{
						for (int jFromFilter = 0; jFromFilter < this.Filter.GetLength(1); jFromFilter++)
						{
							current_result += this.Filter[iFromFilter, jFromFilter, kernel_index] * rawGrid[i + iFromFilter, j + jFromFilter];
						}
					}
					featureMap[i, j] = current_result;
				}
			}


			return featureMap;
		}
		public double[,] Generate3DFeatureMap(List<double[,]> previous_feature_maps)
		{
			List<double[,]> generated_maps = new List<double[,]>();

            // for each previous maps, we generate the corresponding map with the 3d kernel and store it
            for (int i = 0; i < this._filter.GetLength(2) - 1; i++)
			{
				generated_maps.Add(this.GenerateFeatureMap(previous_feature_maps[i], i));
			}

            double[,] result = new double[generated_maps[0].GetLength(0) - 1, generated_maps[0].GetLength(1) - 1];

            // here we add the results at same XY coordinates to only get 1 map
            for (int i = 0; i < generated_maps[0].GetLength(0) - 1; i++) // width
            {
                for (int j = 0; j < generated_maps[0].GetLength(1) - 1; j++) // height
                {
					double temp_cell_result = 0;
					// adding the cells
					foreach (double[,] map in generated_maps)
					{
						temp_cell_result += map[i, j];
					}
					result[i, j] = temp_cell_result;
                }
            }

            return result;
		}
	}
}
