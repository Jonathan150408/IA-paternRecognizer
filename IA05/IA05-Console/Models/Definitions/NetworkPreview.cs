using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IA05_Console.Models.Definitions
{
    /// <summary>
    /// NetworkPreview : A light object that resumes the infos of a neural network without having to load it.
    /// </summary>
    public class NetworkPreview
    {
        /// <summary>
        /// Id : An unique positve integer that referes to an object.
        /// </summary>
        [JsonPropertyName("Id")]
        public uint Id { get; set; }
        /// <summary>
        /// Name : A string that is the name of the network.
        /// </summary>
        [JsonPropertyName("Name")]
        public string Name { get; set; }
        /// <summary>
        /// Description : A string that describes what the IA does.
        /// </summary>
        [JsonPropertyName("Description")]
        public string Description { get; set; }
        /// <summary>
        /// Path : A string that specifies the path of the file starting from the current folder (/Ressources).
        /// </summary>
        [JsonPropertyName("Path")]
        public string Path { get; set; }
        /// <summary>
        /// NeedForm : A boolean used to know if a form is needed.
        /// </summary>
        [JsonPropertyName("NeedForm")]
        public bool NeedForm { get; set; }
        /// <summary>
        /// GridDimensions : 2 positives intergers that gives the drawing grid's dimensions. FACULTATIVE
        /// </summary>
        [JsonPropertyName("Form")]
        public uint[] GridDimensions { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        public NetworkPreview(uint id, string name, string description, string path, bool needForm, uint[] gridDimensions)
        {
            Id = id;
            Name = name;
            Description = description;
            Path = path;
            NeedForm = needForm;
            GridDimensions = gridDimensions ?? new uint[0];
        }
    }
}
