using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GrpcService.Services
{
    /// <summary>
    /// The Multi-thread Multiplication Service. Uses the divide and conquer algorithm. Handles square matrices of any power of 2 size.
    /// </summary>
    public class MultiThreadMultiplicationService : MultiThreadMultiplication.MultiThreadMultiplicationBase
    {
        private readonly ILogger<MultiThreadMultiplicationService> logger;

        public MultiThreadMultiplicationService(ILogger<MultiThreadMultiplicationService> logger)
        {
            this.logger = logger;
        }

        public override Task<Matrix> Multiply(Matrices request, ServerCallContext context)
        {
            var matrixA = request.MatrixA
                .AsParallel()
                .Select(row => row.Item.ToArray())
                .ToArray();
            var matrixB = request.MatrixB
                .AsParallel()
                .Select(row => row.Item.ToArray())
                .ToArray();
            var result = MultiplyMatrix(matrixA, matrixB, 0, 0, 0, 0, matrixA.Length);
            var matrix = new Matrix
            {
                Result = {result.Select(row => new Row {Item = {row}})}
            };
            return Task.FromResult(matrix);
        }

        private static int[][] MultiplyMatrix(IReadOnlyList<int[]> matrixA, IReadOnlyList<int[]> matrixB,
            int rowA, int colA, int rowB, int colB, int size)
        {
            var result = new int[size][];
            if (size <= 128)
            {
                result = Helpers.Matrix.MultiplyMatrix(matrixA, matrixB, rowA, colA, rowB, colB, size);
            }
            else
            {
                size /= 2;
                var subResultOne = new int[size][];
                var subResultTwo = new int[size][];
                var subResultThree = new int[size][];
                var subResultFour = new int[size][];
                Parallel.For(0, 4, index =>
                {
                    switch (index)
                    {
                        case 0:
                            subResultOne = Helpers.Matrix.AddMatricesMultiThread(
                                MultiplyMatrix(matrixA, matrixB, rowA, colA, rowB, colB, size),
                                MultiplyMatrix(matrixA, matrixB, rowA, colA + size, rowB + size, colB, size));
                            break;
                        case 1:
                            subResultTwo = Helpers.Matrix.AddMatricesMultiThread(
                                MultiplyMatrix(matrixA, matrixB, rowA, colA, rowB, colB + size, size),
                                MultiplyMatrix(matrixA, matrixB, rowA, colA + size, rowB + size, colB + size, size));
                            break;
                        case 2:
                            subResultThree = Helpers.Matrix.AddMatricesMultiThread(
                                MultiplyMatrix(matrixA, matrixB, rowA + size, colA, rowB, colB, size),
                                MultiplyMatrix(matrixA, matrixB, rowA + size, colA + size, rowB + size, colB, size));
                            break;
                        case 3:
                            subResultFour = Helpers.Matrix.AddMatricesMultiThread(
                                MultiplyMatrix(matrixA, matrixB, rowA + size, colA, rowB, colB + size, size),
                                MultiplyMatrix(matrixA, matrixB, rowA + size, colA + size, rowB + size, colB + size,
                                    size));
                            break;
                    }
                });

                Parallel.For(0, size, index =>
                {
                    result[index] = new int[size * 2];
                    result[index + size] = new int[size * 2];
                    var upperMiddle = new int[size * 2];
                    Array.Copy(subResultOne[index], upperMiddle, size);
                    Array.Copy(subResultTwo[index], 0, upperMiddle, size, size);

                    var lowerMiddle = new int[size * 2];
                    Array.Copy(subResultThree[index], lowerMiddle, size);
                    Array.Copy(subResultFour[index], 0, lowerMiddle, size, size);

                    Array.Copy(upperMiddle, result[index], size * 2);
                    Array.Copy(lowerMiddle, result[index + size], size * 2);
                });
            }

            return result;
        }
    }
}