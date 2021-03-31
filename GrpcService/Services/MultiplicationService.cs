using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GrpcService.Services
{
    /// <summary>
    /// The Multiplication Service. Uses the divide and conquer algorithm. Handles square matrices of any size.
    /// </summary>
    public class MultiplicationService : Multiplication.MultiplicationBase
    {
        private readonly ILogger<MultiplicationService> logger;

        public MultiplicationService(ILogger<MultiplicationService> logger)
        {
            this.logger = logger;
        }

        public override Task<Matrix> Multiply(Matrices request, ServerCallContext context)
        {
            var matrixA = request.MatrixA
                .Select(row => row.Item.ToArray())
                .ToArray();
            var matrixB = request.MatrixB
                .Select(row => row.Item.ToArray())
                .ToArray();
            var result = this.MultiplyMatrix(matrixA, matrixB, 0, 0, 0, 0, matrixA.Length);
            var matrix = new Matrix
            {
                Result = {result.Select(row => new Row {Item = {row}})}
            };
            
            return Task.FromResult(matrix);
        }

        private int[][] MultiplyMatrix(IReadOnlyList<int[]> matrixA, IReadOnlyList<int[]> matrixB, 
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

        private static int[][] AddMatrices(IReadOnlyList<int[]> matrixA, IReadOnlyList<int[]> matrixB)
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
    }
}