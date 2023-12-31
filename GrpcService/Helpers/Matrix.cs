using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrpcService.Helpers
{
    /// <summary>
    /// The Matrix class is a helper for matrices addition and multiplication.
    /// </summary>
    public static class Matrix
    {
        /// <summary>
        /// Add operation between two matrices of the same size.
        /// </summary>
        /// <param name="matrixA">An <see cref="int"/> matrix with a power of 2 size.</param>
        /// <param name="matrixB">An <see cref="int"/> matrix with a power of 2 size.</param>
        /// <returns>The resulting <see cref="int"/> matrix</returns>
        public static int[][] AddMatrices(IReadOnlyList<int[]> matrixA, IReadOnlyList<int[]> matrixB)
        {
            var size = matrixA.Count;
            var matrixC = new int[size][];
            for (var i = 0; i < size; i++)
            {
                matrixC[i] = new int[size];
                for (var j = 0; j < size; j++)
                {
                    matrixC[i][j] = matrixA[i][j] + matrixB[i][j];
                }
            }

            return matrixC;
        }

        /// <summary>
        /// Adds two matrices of the same size using a <see cref="Parallel"/> method for multithreading.
        /// </summary>
        /// <param name="matrixA">An <see cref="int"/> matrix with a power of 2 size.</param>
        /// <param name="matrixB">An <see cref="int"/> matrix with a power of 2 size.</param>
        /// <returns>The resulting <see cref="int"/> matrix</returns>
        public static int[][] AddMatricesMultiThread(IReadOnlyList<int[]> matrixA, IReadOnlyList<int[]> matrixB)
        {
            var size = matrixA.Count;
            var matrixC = new int[size][];
            Parallel.For(0, size, i =>
            {
                matrixC[i] = new int[size];
                for (var j = 0; j < size; j++)
                {
                    matrixC[i][j] = matrixA[i][j] + matrixB[i][j];
                }
            });

            return matrixC;
        }
        
        /// <summary>
        /// Matrix multiplication using a divide and conquer algorithm.
        /// </summary>
        public static int[][] MultiplyMatrix(IReadOnlyList<int[]> matrixA, IReadOnlyList<int[]> matrixB, 
            int rowA, int colA, int rowB, int colB, int size)
        {
            var result = new int[size][];
            if (size == 1)
            {
                result[0] = new int[1];
                result[0][0] = matrixA[rowA][colA] * matrixB[rowB][colB];
            }
            else
            {
                size /= 2;
                var subResultOne = AddMatrices(
                    MultiplyMatrix(matrixA, matrixB, rowA, colA, rowB, colB, size),
                    MultiplyMatrix(matrixA, matrixB, rowA, colA + size, rowB + size, colB, size));
                var subResultTwo = AddMatrices(
                    MultiplyMatrix(matrixA, matrixB, rowA, colA, rowB, colB + size, size),
                    MultiplyMatrix(matrixA, matrixB, rowA, colA + size, rowB + size, colB + size, size));
                var subResultThree = AddMatrices(
                    MultiplyMatrix(matrixA, matrixB, rowA + size, colA, rowB, colB, size),
                    MultiplyMatrix(matrixA, matrixB, rowA + size, colA + size, rowB + size, colB, size));
                var subResultFour = AddMatrices(
                    MultiplyMatrix(matrixA, matrixB, rowA + size, colA, rowB, colB + size, size),
                    MultiplyMatrix(matrixA, matrixB, rowA + size, colA + size, rowB + size, colB + size, size));

                for (var i = 0; i < size; i++)
                {
                    result[i] = new int[size * 2];
                    result[i + size] = new int[size * 2];
                    var upperMiddle = new int[size * 2];
                    Array.Copy(subResultOne[i], upperMiddle, size);
                    Array.Copy(subResultTwo[i], 0, upperMiddle, size, size);
                    
                    var lowerMiddle = new int[size * 2];
                    Array.Copy(subResultThree[i], lowerMiddle, size);
                    Array.Copy(subResultFour[i], 0, lowerMiddle, size, size);
                    
                    Array.Copy(upperMiddle, result[i], size * 2);
                    Array.Copy(lowerMiddle, result[i + size], size * 2);
                }
            }

            return result;
        }
    }
}