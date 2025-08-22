using Grpc.Core;
using GrpcExample;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GrpcServer;

// User service implementation
public class UserServiceImpl : UserService.UserServiceBase
{
    private readonly ILogger<UserServiceImpl> _logger;
    private static readonly List<User> _users = new();
    private static int _nextId = 1;

    public UserServiceImpl(ILogger<UserServiceImpl> logger)
    {
        _logger = logger;
        
        // Seed with some initial data
        if (!_users.Any())
        {
            _users.AddRange(new[]
            {
                new User { Id = _nextId++, Name = "John Doe", Email = "john@example.com", Age = 30, CreatedAt = DateTime.UtcNow.ToString() },
                new User { Id = _nextId++, Name = "Jane Smith", Email = "jane@example.com", Age = 25, CreatedAt = DateTime.UtcNow.ToString() }
            });
        }
    }

    // Unary RPC - GetUser
    public override Task<User> GetUser(GetUserRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"GetUser called with ID: {request.Id}");
        
        var user = _users.FirstOrDefault(u => u.Id == request.Id);
        if (user == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"User with ID {request.Id} not found"));
        }

        return Task.FromResult(user);
    }

    // Unary RPC - CreateUser
    public override Task<User> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"CreateUser called with name: {request.Name}");

        var user = new User
        {
            Id = _nextId++,
            Name = request.Name,
            Email = request.Email,
            Age = request.Age,
            CreatedAt = DateTime.UtcNow.ToString()
        };

        _users.Add(user);
        return Task.FromResult(user);
    }

    // Server Streaming RPC - GetAllUsers
    public override async Task GetAllUsers(Empty request, IServerStreamWriter<User> responseStream, ServerCallContext context)
    {
        _logger.LogInformation("GetAllUsers called - streaming all users");

        foreach (var user in _users)
        {
            if (context.CancellationToken.IsCancellationRequested)
                break;

            await responseStream.WriteAsync(user);
            
            // Add a small delay to demonstrate streaming
            await Task.Delay(100);
        }
    }

    // Client Streaming RPC - BatchCreateUsers
    public override async Task<BatchCreateResponse> BatchCreateUsers(IAsyncStreamReader<CreateUserRequest> requestStream, ServerCallContext context)
    {
        _logger.LogInformation("BatchCreateUsers called - receiving user stream");

        var createdUsers = new List<User>();
        
        await foreach (var request in requestStream.ReadAllAsync())
        {
            var user = new User
            {
                Id = _nextId++,
                Name = request.Name,
                Email = request.Email,
                Age = request.Age,
                CreatedAt = DateTime.UtcNow.ToString()
            };

            _users.Add(user);
            createdUsers.Add(user);
            
            _logger.LogInformation($"Batch created user: {user.Name}");
        }

        return new BatchCreateResponse
        {
            CreatedCount = createdUsers.Count,
            Users = { createdUsers }
        };
    }
}

// Server startup
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure Kestrel to support HTTP/2 without TLS for development
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenLocalhost(5000, listenOptions =>
            {
                listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
            });
        });

        // Add services
        builder.Services.AddGrpc();
        builder.Services.AddLogging();

        var app = builder.Build();

        // Configure the HTTP request pipeline
        app.MapGrpcService<UserServiceImpl>();
        app.MapGet("/", () => "gRPC Server is running. Use a gRPC client to communicate.");

        Console.WriteLine("gRPC Server starting on http://localhost:5000 (HTTP/2 only)");
        app.Run();
    }
}