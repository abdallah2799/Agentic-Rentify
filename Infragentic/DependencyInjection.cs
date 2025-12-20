using Agentic_Rentify.Infragentic.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
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

            var kernelBuilder = Kernel.CreateBuilder();

            // Add OpenAI Chat Completion pointing to OpenRouter
            kernelBuilder.AddOpenAIChatCompletion(
                modelId: aiSettings.ChatModel,
                apiKey: aiSettings.OpenAIKey,
                httpClient: openRouterClient
            );

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

        return services;
    }
}
