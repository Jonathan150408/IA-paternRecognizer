using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA04.Models
{
    public partial class Kernel
    {
        /// <summary>
        /// Creates the feature map of the given grid according to the filter => overline the corresponding pattern
        /// </summary>
        /// <param name="rawGrid"></param>
        /// <returns></returns>
        public double[,] MakeFeatureMap(double[,] rawGrid)
        {
            // 1. Set up the variables
            double[,] featureMap = new double[rawGrid.GetLength(0) - this.Filter.GetLength(0) + 1, rawGrid.GetLength(1) - this.Filter.GetLength(1) + 1];

            // 2. Browse the raw grid and calculate if the pattern matches the filter
            for (int i = 0; i < featureMap.GetLength(0); i++)
            {
                for (int j = 0; j < featureMap.GetLength(1); j++)
                {
                    // 3. Try to match the pattern               
                    featureMap[i, j] = MatchGrid(rawGrid, i, j);
                }
            }

            // 3. Return the results
            return featureMap;
        }

        /// <summary>
        /// Match a part of the grid with the filter and add everything to get a single value.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="coordinateX">The X coordinate of the frame of the grid</param>
        /// <param name="coordinateY">The Y coordinate of the frame of the grid</param>
        /// <returns>A double determining wether the pattern figures in this frame of the grid</returns>
        private double MatchGrid(double[,] grid, int coordinateX, int coordinateY)
        {
            // 1. Set up variables
            double result = 0;

            // 2. Match the grid with the filter
            for (int i = 0; i < this.Filter.GetLength(0); i++)
            {
                for (int j = 0; j < this.Filter.GetLength(1); j++)
                {
                    result += this.Filter[i][j] * grid[coordinateX + i, coordinateY + j];
                }
            }

            // 3. Return the result
            return result;
        }

        /// <summary>
        /// Creates new feature maps based on more feature maps, simultate a 3D kernel (the filter is 2-dimensional but applied in a 3rd dimension)
        /// </summary>
        /// <param name="previousMaps">A list of old feature maps</param>
        /// <returns>One and only one feature map</returns>
        public double[,] MakeFeatureMap(List<double[,]> previousMaps)
        {
            // 1. Set up variables
            List<double[,]> new_maps = new List<double[,]>();

            // 2. Generate a new map for every map
            for (int oldMapsCounter = 0; oldMapsCounter < previousMaps.Count; oldMapsCounter++)
            {
                double[,] temp_featureMap = new double[
                    previousMaps[oldMapsCounter].GetLength(0) - this.Filter.GetLength(0) + 1,
                    previousMaps[oldMapsCounter].GetLength(1) - this.Filter.GetLength(1) + 1];

                // 3. Browses the grid and calculate if the pattern matches the filter
                for (int i = 0; i < temp_featureMap.GetLength(0); i++)
                {
                    for (int j = 0; j < temp_featureMap.GetLength(1); j++)
                    {
                        // 4. Try to match the pattern
                        temp_featureMap[i, j] = MatchGrid(previousMaps[oldMapsCounter], i, j);
                    }
                }
                new_maps.Add(temp_featureMap);
            }

            // 5. Merge the maps to end with one map
            double[,] result = new double[
                previousMaps[0].GetLength(0) - this.Filter.GetLength(0) + 1,
                previousMaps[0].GetLength(1) - this.Filter.GetLength(1) + 1];

            // 6. Add every value from every maps to the correct cell
            for (int i = 0; i < result.GetLength(0) - 1; i++)
            {
                for (int j = 0; j < result.GetLength(1) - 1; j++)
                {
                    foreach (double[,] map in new_maps)
                    {
                        result[i, j] += map[i, j];
                    }
                }
            }

            // 7. Return the results
            return result;
        }
    }
}
