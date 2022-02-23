using BackgroundTaskQueue;
using BackgroundTaskQueue.Handlers.Commands;
using MediatR;


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

builder.Services.AddScoped<Dispatch>(services =>
{
    var taskQueue = services.GetRequiredService<ITaskQueue>();
    var logger = services.GetRequiredService<ILogger<Dispatch>>();

    return async notifications =>
    {
        foreach (var notification in notifications)
        {
            logger.LogInformation($"{notification.GetType().Name} Logged!");

            await taskQueue.EnqueueAsync(notification);
        }
    };
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

app.MapGet("/ping", () => "Pong!");

app.Run();