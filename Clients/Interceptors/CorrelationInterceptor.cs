using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Clients.Interceptors;

public class CorrelationInterceptor : Interceptor
{
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        var (key, token) = GenerateCorrelationToken();
        context.Options.Headers?.Add(key, token);
        // todo: проверить не null ли Headers если не создавать новый экземпляр Metadata?
        
        return continuation(request, context);
    }
    
    private (string key, string token) GenerateCorrelationToken()
    {
        const string correlationIdKey = "X-Correlation-Id";

        return (correlationIdKey, Guid.NewGuid().ToString());
    }
}