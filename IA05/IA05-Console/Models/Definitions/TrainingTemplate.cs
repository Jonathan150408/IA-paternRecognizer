using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IA05_Console.Models
{
    /// <summary>
    /// TrainingTemplate : A template like a flashcard with question - response.
    /// It is used to train the network by itself.
    /// </summary>
    public partial class TrainingTemplate
    {
        /// <summary>
        /// FakeUserInput : An array [,] of double that simulate the user's input.
        /// This what the AI will recieve to train.
        /// </summary>
        [JsonIgnore]
        public double[,] FakeUserInput { get; set; }

        [JsonPropertyName("FakeUserInput")]
        public double[][] JaggedFakeUserInput { get; set; }

        /// <summary>
        /// ExpectedResult : An array [,] of double that simulate the user's response.
        /// This what the AI will recieve to train.
        /// </summary>
        [JsonPropertyName("ExpectedResult")]
        public double[] ExpectedResult { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TrainingTemplate() { }
        /// <summary>
        /// "Manual" contructor
        /// </summary>
        public TrainingTemplate(double[,] fakeInput, double[] expectedResults)
        {
            this.FakeUserInput = fakeInput;
            this.ExpectedResult = expectedResults;
        }

        public void SetupFakeUserInput()
        {
            this.FakeUserInput = To2D(this.JaggedFakeUserInput);
        }
        public void SetSaveFakeUserInput()
        {
            this.JaggedFakeUserInput = ToJagged(FakeUserInput);
        }
        /// <summary> 
        /// Convert from double[,] to double[][].
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private double[][] ToJagged(double[,] array)
        {
            // 1. Set up the variables
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);

            // 2. Convert the array into [][]
            var result = new double[rows][];
            for (int i = 0; i < rows; i++)
            {
                result[i] = new double[cols];
                for (int j = 0; j < cols; j++)
                {
                    result[i][j] = array[i, j];
                }
            }

            // 3. Return the result
            return result;
        }

        /// <summary> 
        /// Convert from double[][] to double[,].
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private double[,] To2D(double[][] jagged)
        {
            // 1. Check the array
            if (jagged == null || jagged.Length == 0)
                throw new ArgumentException("Jagged array is null or empty.");

            // 2. Set up the variables
            int rows = jagged.Length;
            int cols = jagged[0].Length;

            // 3. Check if the array is fine
            for (int i = 1; i < rows; i++)
            {
                if (jagged[i].Length != cols)
                    throw new ArgumentException("Invalid matrix: rows must all have the same length.");
            }

            // 4. Convert the array into [,]
            var result = new double[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = jagged[i][j];
                }
            }

            // 5. Return the result
            return result;
        }
    }
}
