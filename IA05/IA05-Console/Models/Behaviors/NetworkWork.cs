using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA04.Models
{
    public partial class Network
    {
        public void MakePrediction(List<double[,]> input)
        {
            // 1. Set up variables
            feedForwardHistory.Clear();
            mapHistory.Clear();
            mapHistory.Add(input);

            // 2. Calculate the result for every layer
            foreach (Layer layer in this.Layers)
            {
                if (layer.Type == Layer.layerType.full)
                {
                    if (feedForwardHistory.Count == 0)
                    {
                        List<double> flatMap = Flatten(mapHistory.Last());
                        feedForwardHistory.Add(layer.GetLayerResults(flatMap));
                    }
                    else
                    {
                        feedForwardHistory.Add(feedForwardHistory[feedForwardHistory.Count - 1]);
                    }
                }
                else if (layer.Type == Layer.layerType.convolutive)
                {
                    mapHistory.Add(layer.MakeFeatureMaps(mapHistory[mapHistory.Count - 1]));
                }
            }
        }

        private List<double> Flatten(List<double[,]> maps)
        { 
            // 1. Set up the variable
            List<double> flat = new List<double>();

            // 2. Add each double from the array to the list
            foreach (double[,] map in maps)
            {
                foreach (double value in map)
                {
                    flat.Add(value);
                }
            }

            // 3. Return the result
            return flat;
        }
    }
}
