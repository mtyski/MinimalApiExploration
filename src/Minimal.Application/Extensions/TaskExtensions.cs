namespace Minimal.Application.Extensions;

public static class TaskExtensions
{
    public static Task<TResponse> Bind<TRequest, TResponse>(
        this Task<TRequest> requestTask,
        Func<TRequest, Task<TResponse>> bindingFunction,
        CancellationToken cancellationToken = default) =>
        requestTask.ContinueWith(task => bindingFunction(task.Result), cancellationToken)
            .Unwrap();
}