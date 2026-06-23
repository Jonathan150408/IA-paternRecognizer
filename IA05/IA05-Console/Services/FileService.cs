using IA05.Models;
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
    /// A service that set the files up
    /// </summary>
    public class FileService
    {
        /// <summary>
        /// WorkingDirectory : A string that leads to the "working" folder, the folder in which the programm will work
        /// </summary>
        private string WorkingDirectory { get; set; }

        /// <summary>
        /// RootDirectory : A string that leads to the father "Ressources" directory
        /// </summary>
        private string RootDirectory { get; set; }
        /// <summary>
        /// RootDirectoryRepoScoped : A string that leads to the father "Ressources" directory from the App_IA folder
        /// </summary>
        private string RootDirectoryRepoScoped { get; set; }


        /// <summary>
        /// The constructor
        /// </summary>
        public FileService()
        {
            WorkingDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Ressources");
            RootDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\IA05-Console\\Ressources");
            RootDirectoryRepoScoped = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\IA05\\IA05-Console\\Ressources");
        }

        /// <summary>
        /// Check for updates and copy files if needed
        /// </summary>
        public void Setup()
        {
            // Case 1 : launch program for the first time
            if (!Directory.Exists(WorkingDirectory) && Directory.Exists(RootDirectory))
            {
                // 1. The working dir
                Directory.CreateDirectory(WorkingDirectory);

                // 2. The files
                foreach (string file in Directory.GetFiles(RootDirectory))
                {
                    string fileName = Path.GetFileName(file);
                    File.Copy(file, Path.Combine(WorkingDirectory, fileName), false);
                }

                // 3. Training dir
                Directory.CreateDirectory(Path.Combine(WorkingDirectory, "TrainingData"));

                // 4. Training files
                foreach (string file in Directory.GetFiles(Path.Combine(RootDirectory, "TrainingData")))
                {
                    string fileName = Path.GetFileName(file);
                    File.Copy(file, Path.Combine(WorkingDirectory, "TrainingData", fileName), false);
                }

                // 5. Stop here
                return;
            }

            // Case 2 : the "root" ressources recieved an update
            if (Directory.Exists(RootDirectory) && Directory.Exists(WorkingDirectory))
            {
                // 1. Networks files
                if (Directory.GetFiles(RootDirectory).GetLength(0) > Directory.GetFiles(WorkingDirectory).GetLength(0))
                {
                    foreach (string file in Directory.GetFiles(RootDirectory))
                    {
                        string fileName = Path.GetFileName(file);
                        File.Copy(file, Path.Combine(WorkingDirectory, fileName), true);
                    }
                }

                // 2. Training files
                if (
                    Directory.GetFiles(Path.Combine(RootDirectory, "TrainingData")).GetLength(0) >
                    Directory.GetFiles(Path.Combine(WorkingDirectory, "TrainingData")).GetLength(0)
                    )
                {
                    foreach (string file in Directory.GetFiles(Path.Combine(RootDirectory, "TrainingData")))
                    {
                        string fileName = Path.GetFileName(file);
                        File.Copy(file, Path.Combine(WorkingDirectory, "TrainingData", fileName), false);
                    }
                }

                // 3. Stop here
                return;
            }

            // Case 3 : launch the exec for the first time
            if (!Directory.Exists(WorkingDirectory) && Directory.Exists(RootDirectoryRepoScoped))
            {
                // 1. Create working dir
                Directory.CreateDirectory(WorkingDirectory);

                // 2. Copy working files
                foreach (string file in Directory.GetFiles(RootDirectoryRepoScoped))
                {
                    string fileName = Path.GetFileName(file);
                    File.Copy(file, Path.Combine(WorkingDirectory, fileName), true);
                }

                // 3. Create training dir
                Directory.CreateDirectory(Path.Combine(WorkingDirectory, "TrainingData"));

                // 4. Copy training files
                foreach (string file in Directory.GetFiles(Path.Combine(RootDirectoryRepoScoped, "TrainingData")))
                {
                    string fileName = Path.GetFileName(file);
                    File.Copy(file, Path.Combine(WorkingDirectory, "TrainingData", fileName), false);
                }

                // 5. Stop here
                return;
            }
        }

        /// <summary>
        /// Commit the file into the root folder running the program from the dev folder (not the .exe)
        /// </summary>
        public void Commit()
        {
            // 1. Check if rootdir exists -> if no, that means either we launched the .exe or we have a problem
            if (Directory.Exists(RootDirectory))
            {
                // 2. Overwrite the working files
                foreach (string file in Directory.GetFiles(WorkingDirectory))
                {
                    string fileName = Path.GetFileName(file);
                    File.Copy(file, Path.Combine(RootDirectory, fileName), true);
                }

                // 3. Overwrite the training files
                foreach (string file in Directory.GetFiles(Path.Combine(WorkingDirectory, "TrainingData")))
                {
                    string fileName = Path.GetFileName(file);
                    File.Copy(file, Path.Combine(RootDirectory, "TrainingData", fileName), true);
                }
            }
        }
    }
}
