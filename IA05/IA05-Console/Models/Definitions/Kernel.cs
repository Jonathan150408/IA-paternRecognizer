using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IA04.Models
{
    /// <summary>
    /// Kernel : filtre permettant de générer une feature map
    /// </summary>
    public partial class Kernel
    {
        /// <summary>
        /// Permet d'ordonner les neurones de la couche
        /// </summary>
        [JsonPropertyName("Id")]
        public int Id { get; set; }

        /// <summary>
        /// Filter : un tableau de valeurs permettant d'extraire les features
        /// </summary>
        [JsonPropertyName("Filter")]
        public double[][] JaggedFilter{ get; set; }
        [JsonIgnore]
        public double[,] Filter { get; set; }

        /// <summary>
        /// Contructeur par défaut
        /// </summary>
        public Kernel()
        {
            Filter = To2D(JaggedFilter);
        }

        // Convertit double[,] en double[][]
        private static double[][] ToJagged(double[,] array)
        {
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);

            var result = new double[rows][];
            for (int i = 0; i < rows; i++)
            {
                result[i] = new double[cols];
                for (int j = 0; j < cols; j++)
                {
                    result[i][j] = array[i, j];
                }
            }
            return result;
        }

        // Convertit double[][] en double[,]
        private static double[,] To2D(double[][] jagged)
        {
            if (jagged == null || jagged.Length == 0)
                throw new ArgumentException("Jagged array is null or empty.");

            int rows = jagged.Length;
            int cols = jagged[0].Length;

            // Validation
            for (int i = 1; i < rows; i++)
            {
                if (jagged[i].Length != cols)
                    throw new ArgumentException("Invalid matrix: rows must all have the same length.");
            }

            var result = new double[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = jagged[i][j];
                }
            }
            return result;
        }
    }
}
