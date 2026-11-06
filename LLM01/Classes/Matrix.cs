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
            var m = new Matrix(values.GetLength(0), values.GetLength(1));
            for (int r = 0; r < m.Rows; r++)
                for (int c = 0; c < m.Cols; c++)
                    m[r, c] = values[r, c];
            return m;
        }

        public static Matrix MatMul(Matrix a, Matrix b)
        {
            // version test
            List<int> ints = [];
            // À vous : (n×k) · (k×m) → (n×m)
            // Triple boucle : result[i,j] = somme sur t de a[i,t] * b[t,j]
            throw new NotImplementedException();

            // Test
            if (!(a.Rows == b.Rows && a.Cols == b.Cols))
            {
                throw new Exception("Les matrices ne correspondent pas");
            }

            //// Variables
            //float[] result = new(a.Rows, a.Cols);

            //// Calcul
            //for (int aRows = 0; aRows < a.Rows; aRows++)
            //{
            //    for (int aCols = 0; aCols < a.Cols; aCols++)
            //    {
            //        result[aRows, aCols] =
            //    }
            //}

            // Retour

        }

        public Matrix Transpose()
        {
            // À vous : result[j,i] = this[i,j]
            throw new NotImplementedException();
        }

        public Matrix SoftmaxRows()
        {
            // À vous : softmax appliqué à chaque ligne séparément
            throw new NotImplementedException();
        }
    }
}
