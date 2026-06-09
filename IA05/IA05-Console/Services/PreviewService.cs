using IA05.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IA05.Services
{
    /// <summary>
    /// PreviewService : A service that loads the previews.
    /// </summary>
    public class PreviewService
    {
        /// <summary>
        /// Path : A string that specifies the folder path to take to find the network's file.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Contructor
        /// </summary>
        public PreviewService()
        {
            Path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Ressources", "config.json");
        }

        /// <summary>
        /// Permet de créer et charger une IA à partir de JSON
        /// </summary>
        /// <returns>Le réseau neuronal</returns>
        public List<NetworkPreview> LoadPreviews()
        {
            string jsonString = File.ReadAllText(Path);
            List<NetworkPreview> previews = JsonSerializer.Deserialize<List<NetworkPreview>>(jsonString);
            return previews;
        }

        /// <summary>
        /// Gives the used path
        /// </summary>
        /// <returns></returns>
        public string GetPath()
        {
            return this.Path;
        }
    }
}
