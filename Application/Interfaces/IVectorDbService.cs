using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agentic_Rentify.Application.Interfaces
{
    public record VectorSearchResult(string EntityId, string Type, float Score);

    public interface IVectorDbService
    {
        Task CreateCollectionIfNotExists(string collectionName);
        Task SaveTextVector(string collectionName, string entityId, string type, string text, string? name = null, decimal? price = null, string? city = null);
        Task DeletePointAsync(string collectionName, string entityId, string entityType);
        Task<List<VectorSearchResult>> SearchByTextAsync(string collectionName, string description, int topK = 5);
    }
}