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
            var result = Helpers.Matrix.MultiplyMatrix(matrixA, matrixB, 0, 0, 0, 0, matrixA.Length);
            var matrix = new Matrix
            {
                Result = {result.Select(row => new Row {Item = {row}})}
            };
            
            return Task.FromResult(matrix);
        }
    }
}