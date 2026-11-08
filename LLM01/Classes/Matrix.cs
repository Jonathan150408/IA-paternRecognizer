using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLM01
{
    public class Matrix
    {
        public int Rows { get; }
        public int Cols { get; }
        public float[] Data { get; }

        public Matrix(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Data = new float[rows * cols];
        }

        public float this[int r, int c]
        {
            get => Data[r * Cols + c];
            set => Data[r * Cols + c] = value;
        }

        public static Matrix FromArray(float[,] values)
        {
            var matrix = new Matrix(values.GetLength(0), values.GetLength(1));
            for (int r = 0; r < matrix.Rows; r++)
                for (int c = 0; c < matrix.Cols; c++)
                    matrix[r, c] = values[r, c];
            return matrix;
        }

        public static Matrix MatMul(Matrix a, Matrix b)
        {
            // Validation
            if (a.Cols != b.Rows)
                throw new Exception($"Impossible : a est {a.Rows}x{a.Cols}, b est {b.Rows}x{b.Cols}");

            // Variables
            var result = new Matrix(a.Rows, b.Cols);

            // Calculations
            for (int i = 0; i < a.Rows; i++)
                for (int j = 0; j < b.Cols; j++)
                {
                    float somme = 0;
                    for (int t = 0; t < a.Cols; t++)
                        somme += a[i,t] * b[t,j];
                    result[i, j] = somme;
                }

            // Return
            return result;
        }

        public Matrix Transpose()
        {
            // À vous : result[j,i] = this[i,j]
            throw new NotImplementedException();
            // ??
        }

        public Matrix SoftmaxRows()
        {
            // À vous : softmax appliqué à chaque ligne séparément

            // Variable
            Matrix matrix = new Matrix(this.Rows, this.Cols);
            List<float[]> splitedComputedRows = new();

            // Calculation
            for (int r = 0; r < this.Rows; r++)
            {
                splitedComputedRows.Add(this.Data.Where(v => this.Data.IndexOf(v) > r * this.Cols && this.Data.IndexOf(v) < r * this.Cols + this.Rows).ToArray());
            }

            // Transform
            return Matrix.FromArray(splitedComputedRows.Select(r => r.Select(v => v)).ToArray());
        }
    }
    public static partial class DSLExtensions
    {
        public static float[] SoftMax(this float[] array)
        {
            // 1. Get the greatest input
            float maxValue = array[0];
            for (int i = 1; i < array.GetLength(0); i++)
            {
                if (array[i] > maxValue)
                {
                    maxValue = array[i];
                }
            }

            // 2. Calculate the exponentials
             List<float> exponentials = new List<float>(array.GetLength(0));
            foreach (float raw_output in array)
            {
                exponentials.Add((float)Math.Exp((double)raw_output - (double)maxValue)); //subtract the max so softmax becomes more accurate
            }

            // 3. Calculate the sum
            float exponentials_sum = 0;
            foreach (float exponential in exponentials)
            {
                exponentials_sum += exponential;
            }

            // 4. Calculate the probability
            List<float> finals_outputs = new List<float>(exponentials.Count);
            for (int i = 0; i < exponentials.Count; i++)
            {
                finals_outputs.Add(exponentials[i] / exponentials_sum);
            }

            // 5. Returns the results
            return finals_outputs.ToArray();
        }
    }
}
