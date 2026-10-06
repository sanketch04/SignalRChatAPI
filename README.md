# Real-Time Chat System — ASP.NET Core Web API & SignalR

## 📌 Project Overview

This project is a real-time chat system built using **ASP.NET Core Web API** and **SignalR**. It enables clients to send and receive messages instantly through a SignalR Hub.

A separate **.NET Console Application** acts as a SignalR client to connect to the backend and exchange messages in real time.

The backend can also be integrated with an Angular frontend to provide a web-based chat interface.

## 🚀 Features

- Real-time, bidirectional communication using SignalR.
- ASP.NET Core Web API backend.
- Dedicated SignalR Hub for chat communication.
- Console application for testing real-time messaging.
- Send and receive chat messages without refreshing the application.
- CORS configuration for frontend integration.
- Asynchronous communication using `async` and `await`.
- Connection management and error handling.
- Support for multiple connected clients.

## 🛠️ Technologies Used

| Technology | Purpose |
|---|---|
| C# | Backend programming language |
| ASP.NET Core Web API | Backend API development |
| SignalR | Real-time communication |
| .NET Console Application | SignalR client testing |
| Angular | Optional web frontend |
| HTTP / WebSockets | Client-server communication |
| Visual Studio / VS Code | Development environment |

## 🏗️ Project Architecture

```text
RealTimeChatSystem/
│
├── ChatApi/
│   ├── Controllers/
│   ├── Hubs/
│   │   └── ChatHub.cs
│   ├── Program.cs
│   ├── appsettings.json
│   └── ChatApi.csproj
│
├── SignalClient/
│   ├── Program.cs
│   └── SignalClient.csproj
│
└── README.md
```

> Note: This is a suggested structure. Adjust the folder and file names to match your actual solution.

## 🔄 How the System Works

1. The ASP.NET Core Web API starts and hosts the SignalR Hub.
2. The SignalR Hub exposes a connection endpoint, such as `/chatHub`.
3. The console client establishes a connection with the Hub.
4. The client sends a message through the Hub.
5. The Hub broadcasts the message to connected clients.
6. All clients subscribed to the broadcast receive the message in real time.

### Communication Flow

```text
Console Client A ──┐
                   │
Angular Frontend ──┼──> ASP.NET Core Web API
                   │          │
Console Client B ──┘       SignalR Hub
                              │
                         Broadcast Message
                              │
                    All Connected Clients
```

## 📡 SignalR Hub

The `ChatHub` handles client connections and real-time messaging.

Example implementation:

```csharp
using Microsoft.AspNetCore.SignalR;

namespace ChatApi.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(
            string user,
            string message)
        {
            await Clients.All.SendAsync(
                "ReceiveMessage",
                user,
                message);
        }
    }
}
```

### Explanation

- `Hub`: Base class used to create a SignalR Hub.
- `SendMessage`: Method invoked by connected clients.
- `Clients.All`: Targets all currently connected clients.
- `SendAsync`: Sends the event named `ReceiveMessage` with the username and message.

## ⚙️ Backend Configuration

### 1. Register SignalR and CORS

Configure SignalR and the frontend's permitted origin in `Program.cs`.

```csharp
using ChatApi.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowAngular");

app.MapControllers();

app.MapHub<ChatHub>("/chatHub");

app.Run();
```

**Important:** Use the exact Angular origin and backend port configured in your environment. When using SignalR with credentials, specify allowed origins explicitly instead of using `AllowAnyOrigin()`.

### 2. Configure the Hub Endpoint

```csharp
app.MapHub<ChatHub>("/chatHub");
```

The endpoint above makes the Hub available at:

```text
https://localhost:7218/chatHub
```

Replace the port with the HTTPS port assigned to your API.

### 3. Install SignalR Packages

For the backend, SignalR server support is included in ASP.NET Core's shared framework in typical modern projects.

For the console client, install the SignalR client package:

```bash
dotnet add package Microsoft.AspNetCore.SignalR.Client
```

Use a package version compatible with your target .NET framework.

## 💻 Console SignalR Client

The console application connects to the backend and sends messages through the SignalR Hub.

Example `Program.cs`:

```csharp
using Microsoft.AspNetCore.SignalR.Client;

Console.Write("Enter your username: ");
var user = Console.ReadLine() ?? "User";

var connection = new HubConnectionBuilder()
    .WithUrl("https://localhost:7218/chatHub")
    .WithAutomaticReconnect()
    .Build();

connection.On<string, string>(
    "ReceiveMessage",
    (sender, message) =>
    {
        Console.WriteLine(
            $"{sender}: {message}");
    });

try
{
    await connection.StartAsync();

    Console.WriteLine("Connected to chat server.");
    Console.WriteLine("Type a message or /exit to quit.");

    while (true)
    {
        var message = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(message))
            continue;

        if (message.Equals(
            "/exit",
            StringComparison.OrdinalIgnoreCase))
        {
            break;
        }

        await connection.InvokeAsync(
            "SendMessage",
            user,
            message);
    }
}
catch (Exception ex)
{
    Console.WriteLine(
        $"Connection error: {ex.Message}");
}
finally
{
    await connection.DisposeAsync();
}
```

> If the backend uses a development HTTPS certificate that the console client does not trust, trust the development certificate rather than disabling certificate validation.

## ▶️ How to Run the Project

### Step 1: Start the backend

Open a terminal in the backend project directory:

```bash
dotnet restore
dotnet build
dotnet run
```

Check the terminal output for the actual HTTPS URL and port.

### Step 2: Start the console client

Open a second terminal in the console client directory:

```bash
dotnet restore
dotnet run
```

Enter a username and start sending messages.

### Step 3: Test multiple clients

Run the console client in two separate terminals.

- Enter a different username in each terminal.
- Send a message from the first client.
- Verify that the second client receives it.
- Reply from the second client and verify that the first client receives the response.

### Step 4: Connect the Angular frontend (optional)

Install the SignalR client in the Angular project:

```bash
npm install @microsoft/signalr
```

Connect using the configured backend URL:

```typescript
import * as signalR from '@microsoft/signalr';

const connection = new signalR.HubConnectionBuilder()
  .withUrl('https://localhost:7218/chatHub')
  .withAutomaticReconnect()
  .build();
```

Subscribe to incoming messages and invoke `SendMessage` using the same event and method names defined in the backend Hub.

## 🧪 Testing Checklist

- [ ] Backend starts successfully.
- [ ] SignalR Hub endpoint is accessible.
- [ ] Console client connects successfully.
- [ ] Messages are broadcast to connected clients.
- [ ] Multiple clients can exchange messages.
- [ ] Angular can connect from its configured origin.
- [ ] CORS preflight requests succeed.
- [ ] HTTPS certificate is trusted.
- [ ] Disconnected clients can reconnect where supported.

## 🛠️ Troubleshooting

### CORS Error

**Problem:** The browser reports that the response has no `Access-Control-Allow-Origin` header.

**Solution:**
- Add the exact frontend origin to the CORS policy.
- Enable `AllowCredentials()` when required by the SignalR connection.
- Place `UseCors()` correctly in the middleware pipeline.
- Ensure the frontend uses the correct backend URL.

### Unable to Connect

**Problem:** The console or Angular client cannot connect to the Hub.

**Solution:**
- Confirm that the backend is running.
- Check the HTTPS port in the backend launch settings.
- Verify that `/chatHub` matches the mapped Hub endpoint.
- Check HTTPS certificate trust and browser console errors.

### Messages Are Not Received

**Problem:** The client connects, but incoming messages do not appear.

**Solution:**
- Ensure the client subscribes to `ReceiveMessage`.
- Confirm that the Hub broadcasts using the same event name.
- Verify that the client invokes `SendMessage` with the correct arguments.
- Check the backend logs for exceptions.

## 🔐 Security Considerations

- Configure authentication and authorization before exposing the chat service publicly.
- Validate usernames and message content on the server.
- Apply message-length limits and rate limiting where appropriate.
- Use HTTPS in production.
- Do not expose secrets or credentials in source control.
- Avoid trusting client-supplied usernames when authenticated user identity is available.

## 🔮 Future Enhancements

- User authentication with JWT.
- Private messaging between users.
- Chat rooms and groups.
- Persistent message storage using Entity Framework Core and SQL Server.
- Online/offline presence indicators.
- Typing indicators and read receipts.
- Message history and pagination.
- File and image sharing.
- Deployment to a cloud hosting platform.

## 📚 Learning Outcomes

Through this project, you can learn:

- How ASP.NET Core Web APIs are configured.
- How SignalR enables real-time, bidirectional communication.
- How to create and configure a SignalR Hub.
- How to connect a .NET Console Application to SignalR.
- How to broadcast messages to multiple clients.
- How CORS affects communication between Angular and ASP.NET Core.
- How to troubleshoot connection and HTTPS issues.

## 👨‍💻 Author

Developed as a practical project to learn ASP.NET Core Web API, SignalR, and real-time client-server communication.
