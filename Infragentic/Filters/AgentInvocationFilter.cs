using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

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

            // Capture result with safe serialization
            var resultJson = SerializeResult(context.Result);

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

        try
        {
            // Create a clean dictionary with only the actual values, excluding metadata
            var dict = arguments
                .Where(kvp => kvp.Value != null && !kvp.Key.StartsWith("__") && kvp.Value is not Type)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => (object?)kvp.Value
                );

            return JsonSerializer.Serialize(dict, new JsonSerializerOptions 
            { 
                WriteIndented = false,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });
        }
        catch
        {
            return "{}";
        }
    }

    private static string SerializeResult(object? result)
    {
        if (result == null)
            return string.Empty;

        try
        {
            // Try direct serialization first
            return JsonSerializer.Serialize(result, new JsonSerializerOptions 
            { 
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
            });
        }
        catch
        {
            try
            {
                // Fallback: try to convert to string representation
                return $"\"{result}\"";
            }
            catch
            {
                // Last resort: return empty
                return string.Empty;
            }
        }
    }
}
