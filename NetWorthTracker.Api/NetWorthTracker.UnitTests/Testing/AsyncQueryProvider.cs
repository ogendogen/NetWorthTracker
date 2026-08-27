using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace NetWorthTracker.Tests.Testing;

internal sealed class AsyncQueryProvider : IAsyncQueryProvider
{
    private readonly IQueryProvider _inner;

    public AsyncQueryProvider(IQueryProvider inner)
    {
        _inner = inner;
    }

    public IQueryable CreateQuery(Expression expression)
    {
        return _inner.CreateQuery(expression);
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return _inner.CreateQuery<TElement>(expression);
    }

    public object? Execute(Expression expression)
    {
        return _inner.Execute(expression);
    }

    public TResult Execute<TResult>(Expression expression)
    {
        return _inner.Execute<TResult>(expression);
    }

    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var taskType = typeof(TResult);
        if (!taskType.IsGenericType || taskType.GetGenericTypeDefinition() != typeof(Task<>))
        {
            throw new InvalidOperationException($"Unsupported asynchronous result type: {taskType}.");
        }

        var resultType = taskType.GetGenericArguments()[0];
        var result = _inner.Execute(expression);
        var task = typeof(Task)
            .GetMethod(nameof(Task.FromResult))!
            .MakeGenericMethod(resultType)
            .Invoke(null, [result]);

        return (TResult)task!;
    }
}