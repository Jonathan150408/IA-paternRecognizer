using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA04.Models
{
    public partial class Network
    {
        public void MakePrediction(double[,] input)
        {
            // 1. Set up the history
            history.Add(input);

            // 2. Run the calculations
            foreach (Layer layer in this.Layers)
            {
                if (layer.Type == Layer.layerType.convolutive)
                {
                    history.Add(layer.MakeFeatureMaps((double[,])history.Last()));
                }
                else if (layer.Type == Layer.layerType.full)
                {
                    history.Add(layer.GetLayerResults((List<double>)history.Last()));
                }
            }
        }
    }
}
