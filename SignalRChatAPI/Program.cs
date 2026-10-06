
using Microsoft.AspNetCore.SignalR;
using SignalRChatAPI.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Register CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Register SignalR
builder.Services.AddSignalR();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// IMPORTANT: Enable CORS before mapping the SignalR hub
app.UseCors("AngularPolicy");

app.UseAuthorization();

app.MapControllers();

// Your existing SignalR hub endpoint
app.MapHub<ChatHub>("/chatHub");

app.Run();
