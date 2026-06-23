using IA05.Models;
using IA05_Console.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IA05_Console.Services
{
    /// <summary>
    /// TrainingTemplatesService : A service that loads the templates.
    /// </summary>
    public class TrainingTemplatesService
    {
        /// <summary>
        /// TemplatePath : A string that leads to the templates file.
        /// </summary>
        private string TemplatePath { get; set; }

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="fileName"></param>
        public TrainingTemplatesService(string fileName)
        {
            TemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Ressources", "TrainingData", fileName);
        }

        /// <summary>
        /// Load the template.
        /// </summary>
        /// <returns>
        /// A list of templates ready to be used.
        /// </returns>
        public List<TrainingTemplate> LoadTemplates()
        {
            // 1. Set up variables
            string jsonString = File.ReadAllText(TemplatePath);
            List<TrainingTemplate> templates = new List<TrainingTemplate>();

            // 2. Parse json to data
            templates = JsonSerializer.Deserialize<List<TrainingTemplate>>(jsonString);

            // 3. Set up the templates
            foreach (TrainingTemplate template in templates)
            {
                template.SetupFakeUserInput();
            }

            // 4. Return the result
            return templates;
        }

        public void SaveTemplates(List<TrainingTemplate> templates)
        {
            // 1. Set up the kernels for saving
            foreach (TrainingTemplate template in templates)
            {
                template.SetSaveFakeUserInput();
            }

            // 2. Save the network into json file
            File.WriteAllText(TemplatePath, JsonSerializer.Serialize<List<TrainingTemplate>>(templates));
        }
    }
}
