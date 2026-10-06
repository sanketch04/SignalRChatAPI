
using Microsoft.AspNetCore.SignalR.Client;

var connection = new HubConnectionBuilder()
    .WithUrl("https://localhost:7218/chatHub")
    .WithAutomaticReconnect()
    .Build();

// Receive messages from the SignalR Hub
connection.On<string, string>(
    "ReceiveMessage",
    (user, message) =>
    {
        Console.WriteLine($"{user}: {message}");
    });

try
{
    await connection.StartAsync();

    Console.WriteLine("Connected to SignalR!");

    Console.Write("Enter username: ");
    string user = Console.ReadLine() ?? "Guest";

    Console.WriteLine("Type a message, or type exit to stop.");

    while (true)
    {
        string? message = Console.ReadLine();

        if (message == null ||
            message.Equals("exit", StringComparison.OrdinalIgnoreCase))
        {
            break;
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            continue;
        }

        await connection.InvokeAsync(
            "SendMessage",
            user,
            message);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
finally
{
    await connection.DisposeAsync();
}
