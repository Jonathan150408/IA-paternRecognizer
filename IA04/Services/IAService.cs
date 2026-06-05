using IA04.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace IA04.Services
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
        public IAService()
        {
            Path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Ressources", "network.json");
        }

        /// <summary>
        /// Permet de créer et charger une IA à partir de JSON
        /// </summary>
        /// <returns>Le réseau neuronal</returns>
        public Network LoadNetwork()
        {
            string jsonString = File.ReadAllText(Path);
            Network network = JsonSerializer.Deserialize<Network>(jsonString);
            return network;
        }


        /// <summary>
        /// Donne le chemin de fichier utilisé
        /// </summary>
        /// <returns></returns>
        public string GetPath()
        {
            return this.Path;
        }

    }
}
