using MediatR;

namespace BackgroundTaskQueue.Handlers.Commands;

internal sealed class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Guid>
{
    private readonly ILogger<CreateCustomerCommandHandler> _logger;

    public CreateCustomerCommandHandler(ILogger<CreateCustomerCommandHandler> logger)
    {
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customerId = Guid.NewGuid();

        await Task.Delay(1000, cancellationToken);
        _logger.LogInformation($"Customer Created {customerId}");

        return customerId;
    }
}

public sealed class CreateCustomerCommand : IRequest<Guid>
{
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}