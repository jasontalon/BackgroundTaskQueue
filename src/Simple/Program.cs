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
builder.Services.AddSingleton<ITaskQueue, TaskQueue>();

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