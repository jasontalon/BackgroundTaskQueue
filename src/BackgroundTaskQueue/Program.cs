using BackgroundTaskQueue;
using BackgroundTaskQueue.Controllers;
using BackgroundTaskQueue.Handlers.Commands;
using BackgroundTaskQueue.Handlers.Notifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(typeof(CreateCustomerCommand).Assembly);

builder.Services.AddHostedService<QueuedHostedService>();
builder.Services.AddSingleton<ITaskQueue>(services =>
{
    var queueCapacity = 100;
    var logger = services.GetService<ILogger<TaskQueue>>();
    var parallelizedMediator = new ParallelizedMediator(services.GetRequiredService<ServiceFactory>());
    return new TaskQueue(queueCapacity, parallelizedMediator, logger);
});

var app = builder.Build();


app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.MapGet("/ping", async ([FromServices] ILogger<CustomersController> logger) =>
{
    await Task.Delay(250);
    logger.LogInformation("Ping! Pong!");
    return "Pong";
});

app.Run();