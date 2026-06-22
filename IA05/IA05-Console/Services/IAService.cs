using IA05.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace IA05.Services
{
    /// <summary>
    /// Le service qui permet de lire et écrire du JSON
    /// </summary>
    public class IAService
    {
        /// <summary>
        /// Path représente le chemin de fichier qui mène au JSON
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Le contructeur
        /// </summary>
        public IAService(string endPath)
        {
            Path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Ressources", endPath);
        }

        /// <summary>
        /// Permet de créer et charger une IA à partir de JSON
        /// </summary>
        /// <returns>Le réseau neuronal</returns>
        public Network LoadNetwork()
        {
            // 1. Get the content of the json file
            string jsonString = File.ReadAllText(Path);

            // 2. Convert into data
            Network network = JsonSerializer.Deserialize<Network>(jsonString);

            // 3. Set up the kernels
            foreach (Layer layer in network.Layers)
            {
                if (layer.Type == Layer.layerType.convolutive)
                {
                    foreach (Kernel kernel in layer.Kernels)
                    {
                        kernel.SetupFilter();
                    }
                }
            }

            // 4. Return the result
            return network;
        }

        /// <summary>
        /// Overwrites the json file with the current data to save the network
        /// </summary>
        /// <param name="network"></param>
        public void SaveNetwork(Network network)
        {
            // 1. Set up the kernels for saving
            foreach (Layer layer in network.Layers)
            {
                if (layer.Type == Layer.layerType.convolutive)
                {
                    foreach (Kernel kernel in layer.Kernels)
                    {
                        kernel.SetSaveFilter();
                    }
                }
            }

            // 2. Save the network into json file
            File.WriteAllText(Path, JsonSerializer.Serialize<Network>(network));
        }
    }
}
