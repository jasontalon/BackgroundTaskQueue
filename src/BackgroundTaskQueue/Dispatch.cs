using MediatR;

namespace BackgroundTaskQueue;

public delegate ValueTask Dispatch(params INotification[] notifications);