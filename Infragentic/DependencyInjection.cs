using Agentic_Rentify.Infragentic.Settings;
using Agentic_Rentify.Infragentic.Plugins;
using Agentic_Rentify.Infragentic.Filters;
using Agentic_Rentify.Infragentic.Services;
using Agentic_Rentify.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Qdrant.Client;

namespace Agentic_Rentify.Infragentic;

public static class InfragenticExtensions
{
    public static IServiceCollection AddInfragenticServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind AI settings
        services.Configure<AiSettings>(configuration.GetSection("AI"));
        var aiSettings = configuration.GetSection("AI").Get<AiSettings>()!;

        // Register custom HttpClient for OpenRouter with required headers
        services.AddHttpClient("OpenRouter", client =>
        {
            client.BaseAddress = new Uri(aiSettings.OpenAIEndpoint);
            client.DefaultRequestHeaders.Add("HTTP-Referer", "https://agentic-rentify.com");
            client.DefaultRequestHeaders.Add("X-Title", "Agentic Rentify AI");
        });

        // Register Semantic Kernel with OpenRouter configuration
        services.AddSingleton<Kernel>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var openRouterClient = httpClientFactory.CreateClient("OpenRouter");
            var mediator = sp.GetRequiredService<MediatR.IMediator>();
            var serviceScopeFactory = sp.GetRequiredService<IServiceScopeFactory>();

            var kernelBuilder = Kernel.CreateBuilder();

            // Add OpenAI Chat Completion pointing to OpenRouter
            kernelBuilder.AddOpenAIChatCompletion(
                modelId: aiSettings.ChatModel,
                apiKey: aiSettings.OpenAIKey,
                httpClient: openRouterClient
            );

            // Embeddings are accessed via OpenRouter HTTP client directly in QdrantVectorService

            // Register agent invocation filter for observability
            kernelBuilder.Services.AddSingleton<IFunctionInvocationFilter>(
                new AgentInvocationFilter(serviceScopeFactory)
            );

            // Create plugin instances with resolved dependencies and add them
            var discoveryPlugin = new DiscoveryPlugin(serviceScopeFactory);
            var bookingPlugin = new BookingPlugin(serviceScopeFactory);

            kernelBuilder.Plugins.AddFromObject(discoveryPlugin, "Discovery");
            kernelBuilder.Plugins.AddFromObject(bookingPlugin, "Booking");

            return kernelBuilder.Build();
        });

        // Register Qdrant Native Client
        services.AddSingleton<QdrantClient>(sp =>
        {
            return new QdrantClient(
                host: aiSettings.QdrantHost,
                port: aiSettings.QdrantPort,
                https: false
            );
        });

        // Register Vector DB service
        services.AddScoped<IVectorDbService, QdrantVectorService>();
        services.AddScoped<DataSyncService>();

        // Register AI Services
        services.AddScoped<IChatAiService, ChatAiService>();

        return services;
    }
}
