# gRPC Example - C# Implementation

A comprehensive example demonstrating gRPC implementation in C# with both server and client applications, showcasing different RPC patterns including unary, server streaming, and client streaming.

## 🔍 Keywords
`grpc` `csharp` `dotnet` `protobuf` `microservices` `rpc` `tutorial` `example` `asp-net-core` `http2` `client-server` `streaming` `protocol-buffers` `distributed-systems`

## 🚀 Features

- **Unary RPCs**: Traditional request-response pattern (GetUser, CreateUser)
- **Server Streaming**: Server sends multiple responses (GetAllUsers)
- **Client Streaming**: Client sends multiple requests (BatchCreateUsers)
- **Protocol Buffers**: Strongly-typed message definitions
- **Error Handling**: Proper gRPC status codes and exception handling
- **In-Memory Storage**: Simple user management with seed data
- **Logging**: Server-side request logging for observability

## 📁 Project Structure

```
GrpcExample/
├── Protos/
│   └── user.proto              # Protocol Buffer service definition
├── GrpcServer/
│   ├── GrpcServer.csproj       # Server project file
│   └── Program.cs              # Server implementation
├── GrpcClient/
│   ├── GrpcClient.csproj       # Client project file
│   └── Program.cs              # Client examples
├── .gitignore                  # Git ignore rules
├── GrpcExample.sln             # Solution file
└── README.md                   # This file
```

## 🛠️ Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/) for version control

## ⚡ Quick Start

### 1. Clone the Repository
```bash
git clone <repository-url>
cd GrpcExample
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Build the Solution
```bash
dotnet build
```

### 4. Run the Server
Open a terminal and start the gRPC server:
```bash
cd GrpcServer
dotnet run
```
You should see: `gRPC Server starting on http://localhost:5000 (HTTP/2 only)`

### 5. Run the Client
Open another terminal and run the client examples:
```bash
cd GrpcClient
dotnet run
```

## 📋 API Reference

### Service Definition
The `UserService` provides the following RPC methods:

| Method | Type | Description |
|--------|------|-------------|
| `GetUser` | Unary | Retrieve a user by ID |
| `CreateUser` | Unary | Create a new user |
| `GetAllUsers` | Server Streaming | Stream all users |
| `BatchCreateUsers` | Client Streaming | Create multiple users |

### Message Types
- **User**: Contains id, name, email, age, and created_at
- **GetUserRequest**: Contains user id
- **CreateUserRequest**: Contains name, email, and age
- **BatchCreateResponse**: Contains created count and user list

## 🔧 Configuration

### Server Configuration
The server is configured to:
- Listen on `http://localhost:5000`
- Use HTTP/2 protocol
- Enable detailed error messages for development
- Provide in-memory user storage with seed data

### Client Configuration
The client is configured to:
- Connect to `http://localhost:5000`
- Enable HTTP/2 over unencrypted connections
- Handle all four RPC patterns with examples

## 📚 Examples

### Unary RPC Example
```csharp
// Get user by ID
var response = await client.GetUserAsync(new GetUserRequest { Id = 1 });
Console.WriteLine($"User: {response.Name} ({response.Email})");
```

### Server Streaming Example
```csharp
// Stream all users
using var streamingCall = client.GetAllUsers(new Empty());
while (await streamingCall.ResponseStream.MoveNext(CancellationToken.None))
{
    var user = streamingCall.ResponseStream.Current;
    Console.WriteLine($"Received: {user.Name}");
}
```

### Client Streaming Example
```csharp
// Batch create users
using var streamingCall = client.BatchCreateUsers();
await streamingCall.RequestStream.WriteAsync(new CreateUserRequest 
{ 
    Name = "John Doe", 
    Email = "john@example.com", 
    Age = 30 
});
await streamingCall.RequestStream.CompleteAsync();
var response = await streamingCall;
```

## 🐛 Troubleshooting

### Common Issues

**HTTP/2 Connection Errors**
If you see `HTTP_1_1_REQUIRED` errors:
- Ensure the server is running on HTTP/2
- Check that `AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true)` is set in the client

**Build Errors with Generated Code**
If you see errors about missing `GrpcExample` namespace:
- Clean and rebuild the solution: `dotnet clean && dotnet build`
- Ensure the `.proto` file is in the correct location
- Verify NuGet packages are restored

**Port Already in Use**
If port 5000 is busy:
- Change the port in `GrpcServer/Program.cs`
- Update the client connection string accordingly
- Or stop other services using port 5000

## 🧪 Testing

### Manual Testing
Run the client application to test all RPC patterns:
```bash
cd GrpcClient
dotnet run
```

### Custom Testing
Modify the client code to test specific scenarios:
- Add new user creation requests
- Test error handling with invalid IDs
- Experiment with streaming patterns

## 🔐 Security Considerations

This example is designed for **development and learning purposes**. For production use, consider:

- **TLS/SSL**: Use HTTPS with proper certificates
- **Authentication**: Implement JWT or other auth mechanisms
- **Authorization**: Add role-based access control
- **Input Validation**: Validate all incoming data
- **Rate Limiting**: Prevent abuse and DoS attacks
- **Logging & Monitoring**: Implement comprehensive observability

## 📖 Learning Resources

- [gRPC Official Documentation](https://grpc.io/docs/)
- [gRPC in .NET](https://docs.microsoft.com/en-us/aspnet/core/grpc/)
- [Protocol Buffers Guide](https://developers.google.com/protocol-buffers)
- [ASP.NET Core gRPC Services](https://docs.microsoft.com/en-us/aspnet/core/grpc/aspnetcore)

## 🎯 Next Steps

Enhance this example by adding:

- **Bidirectional Streaming**: Two-way communication patterns
- **Authentication & Authorization**: Secure your services
- **Unit Testing**: Test your gRPC services
- **Docker Containerization**: Package for deployment
- **Load Balancing**: Scale across multiple instances
- **Interceptors**: Add cross-cutting concerns
- **Health Checks**: Monitor service availability
- **Metrics & Tracing**: Observability with OpenTelemetry

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/awesome-feature`)
3. Commit your changes (`git commit -m 'Add awesome feature'`)
4. Push to the branch (`git push origin feature/awesome-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙋‍♂️ Support

If you have questions or run into issues:

- Check the [Troubleshooting](#-troubleshooting) section
- Review the [gRPC documentation](https://grpc.io/docs/)
- Open an issue in this repository
- Ask on [Stack Overflow](https://stackoverflow.com/questions/tagged/grpc) with the `grpc` tag

---

**Happy gRPC coding! 🚀**