using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA05.Models
{
    public partial class Network
    {
        /// <summary>
        /// Make the whole thinking process
        /// </summary>
        /// <param name="input"></param>
        public Dictionary<string, double> MakePrediction(List<double[,]> input)
        {
            // 1. Set up variables
            this.History.Clear();
            this.History.Add(input);

            // 2. Calculate the result for every step
            foreach (string step in this.Schema)
            {
                switch (step)
                {
                    case "MaxPooling":
                        this.History.Add(GeneralMaxPooling(this.History.Last()));
                        break;
                    default:
                        int.TryParse(step, out int layerId);
                        Layer currentLayer = this.Layers.Find(l  => l.Id == layerId);
                        if (currentLayer.Type == Layer.layerType.full)
                        {
                            this.History.Add(currentLayer.GetLayerResults(History.Last()));
                        }
                        else if (currentLayer.Type == Layer.layerType.convolutive)
                        {
                            this.History.Add(currentLayer.MakeFeatureMaps(History.Last()));
                        }
                        break;
                        
                }
                
            }

            // 3. Get the biggest result
            double[,] lastResults = this.History.Last().Last();
            Dictionary<string, double> resultChances = new Dictionary<string, double>(); // name - probability (for example : triangle - 64.64841)

            for (int i = 0; i < lastResults.GetLength(0); i++)
            {
                for (int j = 0; j < lastResults.GetLength(1); j++)
                {
                    resultChances.Add(this.Layers.Last().Results[i * lastResults.GetLength(0) + j], lastResults[i, j]);
                }
            }

            

            // 4. Return the result
            return resultChances;
        }

        /// <summary>
        /// Takes all the feature maps and proceed to a 2x2 max pooling -> we only take the max value and the map become 4 times smaller
        /// </summary>
        /// <param name="feature_maps">A list of feature maps</param>
        /// <returns>A list of pooled feature maps</returns>
        private List<double[,]> GeneralMaxPooling(List<double[,]> feature_maps)
        {
            List<double[,]> pooled_maps = new List<double[,]>();

            foreach (double[,] map in feature_maps)
            {
                // temporary map -> 4x smaller as the original map
                double[,] temp_pooled_map = new double[(int)Math.Ceiling((double)(map.GetLength(0) / 2)), (int)Math.Ceiling((double)(map.GetLength(1) / 2))];
                for (int i = 0; i < map.GetLength(0) - 1; i += 2)
                {
                    for (int j = 0; j < map.GetLength(1) - 1; j += 2)
                    {
                        List<double> temp_values_to_pool = new List<double>() { map[i, j] };
                        try
                        {
                            temp_values_to_pool.Add(map[i + 1, j]);
                        }
                        catch { }
                        try
                        {
                            temp_values_to_pool.Add(map[i, j + 1]);
                        }
                        catch { }
                        try
                        {
                            temp_values_to_pool.Add(map[i + 1, j + 1]);
                        }
                        catch { }
                        temp_pooled_map[(int)Math.Ceiling((decimal)i / 2), (int)Math.Ceiling((decimal)j / 2)] = MaxPooling(temp_values_to_pool);
                    }
                }
                pooled_maps.Add(temp_pooled_map);
            }

            return pooled_maps;
        }

        /// <summary>
        /// Return the greatest value in the frame.
        /// </summary>
        /// <param name="doubles"></param>
        /// <returns></returns>
        private double MaxPooling(List<double> doubles)
        {
            double max_value = 0;

            foreach (double value in doubles)
            {
                max_value = Math.Max(max_value, value);
            }

            return max_value;
        }
    }
}
