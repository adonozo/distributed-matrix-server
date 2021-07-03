# REST gRPC Server

This is the server for a distributed matrix multiplication service. Matrices are multiplied using the "divide and conquer" algorithm.
This is only a gRPC Server; the REST part of the name is kept for consistency purposes.

Multiplication services are stateless, allowing us to deploy multiple instances to improve efficiency. Operations
are asynchronous and multithreading is also available. 

---

This project was built using .NET Core 5

To run the project:

```bash
dotnet GrpcServer.dll
```

Make sure the port is declared in `appsettings.json` under __Port__

---

## Services:

This gRPC server has three services: add matrices, multiply matrices, and multiply matrices with multi-threading enabled.
Each service require two square matrices with a dimension of a power of 2.
See the `calculator.proto` for more details on the expected format.
