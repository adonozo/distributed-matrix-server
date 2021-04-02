using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GrpcService.Services
{
    /// <summary>
    /// The Add Service. Handles square matrices of any size.
    /// </summary>
    public class AddService : Add.AddBase
    {
        private readonly ILogger<AddService> logger;

        public AddService(ILogger<AddService> logger)
        {
            this.logger = logger;
        }

        public override Task<Matrix> Add(Matrices request, ServerCallContext context)
        {
            var matrixA = request.MatrixA
                .Select(row => row.Item.ToArray())
                .ToArray();
            var matrixB = request.MatrixB
                .Select(row => row.Item.ToArray())
                .ToArray();
            var result = Helpers.Matrix.AddMatrices(matrixA, matrixB);
            var matrix = new Matrix
            {
                Result = {result.Select(row => new Row {Item = {row}})}
            };
            
            return Task.FromResult(matrix);
        }
    }
}