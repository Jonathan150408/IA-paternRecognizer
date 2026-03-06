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
		public double[,] Filter
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
							current_result += this.Filter[iFromFilter, jFromFilter] * rawGrid[i + iFromFilter, j + jFromFilter];
						}
					}
					featureMap[i, j] = current_result;
				}
			}

            return FeatureMapsWithTanH(featureMap);
        }
        /// <summary>
        /// Creates new feature maps based on the olds, simultate a 3D kernel (the filter is 2-dimensional but applied in a 3rd dimension)
        /// </summary>
        /// <param name="previousMaps">A list of old feature maps</param>
        /// <returns>A list of 2d array -> feature maps</returns>
        public double[,] RegenerateFeatureMap(List<double[,]> previousMaps)
        {
			//in this part, we generate multiple feature maps, next step we'll merge theses maps together
			List<double[,]> new_maps = new List<double[,]>();
			// for every maps that are given, we generate a new one based on it
			for (int oldMapsCounter = 0; oldMapsCounter < previousMaps.Count; oldMapsCounter++)
			{
                double[,] temp_featureMap = new double[previousMaps[
					oldMapsCounter].GetLength(0) - this.Filter.GetLength(0) + 1,
					previousMaps[oldMapsCounter].GetLength(1) - this.Filter.GetLength(1) + 1
					];

                double current_result;
                //browses the raw grid and calculate if the pattern matches the filter
                for (int i = 0; i < temp_featureMap.GetLength(0); i++)
                {
                    for (int j = 0; j < temp_featureMap.GetLength(1); j++)
                    {
                        // try to match the pattern
                        current_result = 0;
                        for (int iFromFilter = 0; iFromFilter < this.Filter.GetLength(0); iFromFilter++)
                        {
                            for (int jFromFilter = 0; jFromFilter < this.Filter.GetLength(1); jFromFilter++)
                            {
                                current_result += this.Filter[iFromFilter, jFromFilter] * previousMaps[oldMapsCounter][i + iFromFilter, j + jFromFilter];
                            }
                        }
                        temp_featureMap[i, j] = current_result;
                    }
                }
                new_maps.Add(temp_featureMap);
            }

			// Here we merge the maps to finish with on only map
			double[,] result = new double[previousMaps[
                    0].GetLength(0) - this.Filter.GetLength(0) + 1,
                    previousMaps[0].GetLength(1) - this.Filter.GetLength(1) + 1
					];

			//every value from every row from every maps is added to the correct cell of the result
			for (int i = 0; i < result.GetLength(0) - 1; i++)
			{
				for (int j = 0;j < result.GetLength(1) - 1; j++)
				{
					foreach (double[,] map in new_maps)
					{
						result[i, j] += map[i, j];
					}
				}
			}

			return FeatureMapsWithTanH(result);
        }

		private double[,] FeatureMapsWithTanH(double[,] old_map)
		{
            double[,] featureMap = new double[old_map.GetLength(0), old_map.GetLength(1)];

			for (int i = 0;i < old_map.GetLength(0); i++)
			{
				for(int j = 0; j < old_map.GetLength(1); j++)
				{
					featureMap[i, j] = Math.Tanh(old_map[i, j]);
				}
			}

            return featureMap;
		}
    }
}
