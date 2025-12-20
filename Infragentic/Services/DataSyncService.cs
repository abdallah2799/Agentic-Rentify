using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Core.Entities;

namespace Agentic_Rentify.Infragentic.Services
{
    public class DataSyncService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVectorDbService _vectorDbService;

        public DataSyncService(IUnitOfWork unitOfWork, IVectorDbService vectorDbService)
        {
            _unitOfWork = unitOfWork;
            _vectorDbService = vectorDbService;
        }

        public async Task SyncAsync(string collectionName = "rentify_memory")
        {
            await _vectorDbService.CreateCollectionIfNotExists(collectionName);

            // Sync Attractions
            var attractions = await _unitOfWork.Repository<Attraction>().ListAllAsync();
            foreach (var a in attractions)
            {
                var text = string.Join(" ", new[] { a.Name, a.Description, a.Overview });
                await _vectorDbService.SaveTextVector(collectionName, a.Id.ToString(), "Attraction", text);
            }

            // Sync Trips
            var trips = await _unitOfWork.Repository<Trip>().ListAllAsync();
            foreach (var t in trips)
            {
                var text = string.Join(" ", new[] { t.Title, t.Description });
                await _vectorDbService.SaveTextVector(collectionName, t.Id.ToString(), "Trip", text);
            }
        }
    }
}