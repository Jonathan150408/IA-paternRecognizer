using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

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
            Path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Ressources", "config.json");
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
