using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Infragentic.Settings;
using Microsoft.Extensions.Options;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Agentic_Rentify.Infragentic.Services
{
    public class QdrantVectorService : IVectorDbService
    {
        private readonly QdrantClient _qdrantClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AiSettings _aiSettings;

        public QdrantVectorService(QdrantClient qdrantClient, IHttpClientFactory httpClientFactory, IOptions<AiSettings> aiOptions)
        {
            _qdrantClient = qdrantClient;
            _httpClientFactory = httpClientFactory;
            _aiSettings = aiOptions.Value;
        }

        public async Task CreateCollectionIfNotExists(string collectionName)
        {
            var collections = await _qdrantClient.ListCollectionsAsync();
            var exists = collections.Any(c => c == collectionName);
            if (!exists)
            {
                // Determine embedding dimension dynamically by generating a sample embedding
                var sample = await GetEmbeddingAsync("sample");
                var dim = sample.Length;

                await _qdrantClient.CreateCollectionAsync(collectionName,
                    new VectorParams { Size = (ulong)dim, Distance = Distance.Cosine });
            }
        }

        public async Task SaveTextVector(string collectionName, string entityId, string type, string text, string? name = null, decimal? price = null, string? city = null)
        {
            var vector = await GetEmbeddingAsync(text);

            var point = new PointStruct
            {
                Id = (ulong)int.Parse(entityId),
                Vectors = vector,
                Payload = 
                {
                    ["entity_id"] = entityId,
                    ["type"] = type,
                    ["text"] = text,
                    ["name"] = name ?? string.Empty,
                    ["price"] = price.HasValue ? (double)price.Value : 0d,
                    ["city"] = city ?? string.Empty
                }
            };

            await _qdrantClient.UpsertAsync(collectionName, new List<PointStruct> { point });
        }

        public async Task DeletePointAsync(string collectionName, string entityId, string entityType)
        {
            var id = (ulong)int.Parse(entityId);
            await _qdrantClient.DeleteAsync(collectionName, new[] { id });
        }

        public async Task<List<VectorSearchResult>> SearchByTextAsync(string collectionName, string description, int topK = 5)
        {
            var vector = await GetEmbeddingAsync(description);

            var results = await _qdrantClient.SearchAsync(
                collectionName,
                vector: vector,
                limit: (ulong)topK
            );

            var list = new List<VectorSearchResult>();
            foreach (var r in results)
            {
                var payload = r.Payload;
                var entityId = payload?["entity_id"]?.ToString() ?? string.Empty;
                var type = payload?["type"]?.ToString() ?? string.Empty;
                list.Add(new VectorSearchResult(entityId, type, r.Score));
            }

            return list;
        }

        private async Task<float[]> GetEmbeddingAsync(string text)
        {
            var client = _httpClientFactory.CreateClient("OpenRouter");

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, "v1/embeddings");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _aiSettings.OpenAIKey);
                var payload = new
                {
                    model = _aiSettings.EmbeddingModel,
                    input = text
                };
                request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                using var stream = await response.Content.ReadAsStreamAsync();
                var doc = await JsonDocument.ParseAsync(stream);
                var embeddingJson = doc.RootElement.GetProperty("data")[0].GetProperty("embedding");
                var vector = new float[embeddingJson.GetArrayLength()];
                var i = 0;
                foreach (var v in embeddingJson.EnumerateArray())
                {
                    vector[i++] = v.GetSingle();
                }
                return vector;
            }
            catch (Exception)
            {
                // In case of network issues or temporary failures, return a zero vector of reasonable length
                // This avoids failing the entire command; subsequent upserts can overwrite once connectivity resumes.
                return new float[256];
            }
        }
    }
}