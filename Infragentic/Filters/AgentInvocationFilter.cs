using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using System.Diagnostics;
using System.Text.Json;

namespace Agentic_Rentify.Infragentic.Filters;

public class AgentInvocationFilter(IServiceScopeFactory serviceScopeFactory) : IFunctionInvocationFilter
{
    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        var stopwatch = Stopwatch.StartNew();
        var userId = context.Arguments != null && context.Arguments.TryGetValue("UserId", out var userIdObj)
            ? userIdObj?.ToString()
            : null;

        try
        {
            await next(context);
            stopwatch.Stop();

            // Capture result
            var resultJson = context.Result != null 
                ? JsonSerializer.Serialize(context.Result, new JsonSerializerOptions { WriteIndented = false })
                : string.Empty;

            // Log execution (fire and forget)
            _ = Task.Run(() => LogExecutionAsync(
                userId: userId,
                pluginName: context.Function.PluginName,
                functionName: context.Function.Name,
                argumentsJson: context.Arguments != null ? SerializeArguments(context.Arguments) : "{}",
                resultJson: resultJson,
                isError: false,
                errorMessage: null,
                executionDurationMs: stopwatch.ElapsedMilliseconds
            ));
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            // Log error
            _ = Task.Run(() => LogExecutionAsync(
                userId: userId,
                pluginName: context.Function.PluginName,
                functionName: context.Function.Name,
                argumentsJson: context.Arguments != null ? SerializeArguments(context.Arguments) : "{}",
                resultJson: string.Empty,
                isError: true,
                errorMessage: ex.Message,
                executionDurationMs: stopwatch.ElapsedMilliseconds
            ));

            throw;
        }
    }

    private async Task LogExecutionAsync(
        string? userId,
        string? pluginName,
        string? functionName,
        string argumentsJson,
        string resultJson,
        bool isError,
        string? errorMessage,
        long executionDurationMs)
    {
        try
        {
            using var scope = serviceScopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IAgentLogRepository>();

            var log = new AgentExecutionLog
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PluginName = pluginName ?? "Unknown",
                FunctionName = functionName ?? "Unknown",
                ArgumentsJson = argumentsJson,
                ResultJson = resultJson,
                IsError = isError,
                ErrorMessage = errorMessage,
                ExecutionDurationMs = executionDurationMs,
                Timestamp = DateTime.UtcNow
            };

            await repository.SaveLogAsync(log);
        }
        catch
        {
            // Silently fail to not interrupt the main flow
        }
    }

    private static string SerializeArguments(KernelArguments? arguments)
    {
        if (arguments == null)
            return "{}";

        var dict = arguments.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value
        );

        try
        {
            return JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = false });
        }
        catch
        {
            return "{}";
        }
    }
}
