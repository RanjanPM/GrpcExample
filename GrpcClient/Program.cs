using Grpc.Net.Client;
using GrpcExample;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GrpcClient;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Enable HTTP/2 support for unencrypted connections
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        
        // Create gRPC channel configured for HTTP/2 over HTTP
        var httpHandler = new SocketsHttpHandler();
        
        var httpClient = new HttpClient(httpHandler)
        {
            DefaultRequestVersion = HttpVersion.Version20,
            DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact
        };
        
        using var channel = GrpcChannel.ForAddress("http://localhost:5000", new GrpcChannelOptions
        {
            HttpClient = httpClient
        });
        
        var client = new UserService.UserServiceClient(channel);

        Console.WriteLine("gRPC Client started. Connecting to server...\n");

        try
        {
            // 1. Unary RPC - Get existing user
            await GetUserExample(client);
            
            // 2. Unary RPC - Create a new user
            await CreateUserExample(client);
            
            // 3. Server Streaming RPC - Get all users
            await GetAllUsersExample(client);
            
            // 4. Client Streaming RPC - Batch create users
            await BatchCreateUsersExample(client);

            Console.WriteLine("\nAll examples completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    // Example 1: Unary RPC - GetUser
    private static async Task GetUserExample(UserService.UserServiceClient client)
    {
        Console.WriteLine("=== GetUser Example ===");
        
        try
        {
            var response = await client.GetUserAsync(new GetUserRequest { Id = 1 });
            Console.WriteLine($"Found user: {response.Name} ({response.Email}) - Age: {response.Age}");
        }
        catch (Grpc.Core.RpcException ex)
        {
            Console.WriteLine($"gRPC Error: {ex.Status}");
        }
        
        Console.WriteLine();
    }

    // Example 2: Unary RPC - CreateUser
    private static async Task CreateUserExample(UserService.UserServiceClient client)
    {
        Console.WriteLine("=== CreateUser Example ===");
        
        var response = await client.CreateUserAsync(new CreateUserRequest
        {
            Name = "Alice Johnson",
            Email = "alice@example.com",
            Age = 28
        });
        
        Console.WriteLine($"Created user: {response.Name} with ID: {response.Id}");
        Console.WriteLine();
    }

    // Example 3: Server Streaming RPC - GetAllUsers
    private static async Task GetAllUsersExample(UserService.UserServiceClient client)
    {
        Console.WriteLine("=== GetAllUsers (Server Streaming) Example ===");
        
        using var streamingCall = client.GetAllUsers(new Empty());
        
        while (await streamingCall.ResponseStream.MoveNext(CancellationToken.None))
        {
            var user = streamingCall.ResponseStream.Current;
            Console.WriteLine($"Received user: {user.Name} (ID: {user.Id})");
        }
        
        Console.WriteLine("Finished receiving all users\n");
    }

    // Example 4: Client Streaming RPC - BatchCreateUsers
    private static async Task BatchCreateUsersExample(UserService.UserServiceClient client)
    {
        Console.WriteLine("=== BatchCreateUsers (Client Streaming) Example ===");
        
        using var streamingCall = client.BatchCreateUsers();
        
        // Send multiple user creation requests
        var usersToCreate = new[]
        {
            new CreateUserRequest { Name = "Bob Wilson", Email = "bob@example.com", Age = 35 },
            new CreateUserRequest { Name = "Carol Brown", Email = "carol@example.com", Age = 42 },
            new CreateUserRequest { Name = "David Lee", Email = "david@example.com", Age = 29 }
        };

        foreach (var userRequest in usersToCreate)
        {
            await streamingCall.RequestStream.WriteAsync(userRequest);
            Console.WriteLine($"Sent create request for: {userRequest.Name}");
            
            // Add delay to simulate real-world scenario
            await Task.Delay(500);
        }

        // Signal completion
        await streamingCall.RequestStream.CompleteAsync();

        // Get the response
        var response = await streamingCall;
        Console.WriteLine($"Batch operation completed. Created {response.CreatedCount} users.");
        
        foreach (var user in response.Users)
        {
            Console.WriteLine($"  - {user.Name} (ID: {user.Id})");
        }
        
        Console.WriteLine();
    }
}