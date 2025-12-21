using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Application.Specifications;
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
            var attractions = await _unitOfWork.Repository<Attraction>().ListAsync(new ActiveEntitySpecification<Attraction>());
            foreach (var a in attractions)
            {
                var text = string.Join(" ", new[] { a.Name, a.Description, a.Overview });
                await _vectorDbService.SaveTextVector(collectionName, a.Id.ToString(), "Attraction", text, a.Name, a.Price, a.City);
            }

            // Sync Trips
            var trips = await _unitOfWork.Repository<Trip>().ListAsync(new ActiveEntitySpecification<Trip>());
            foreach (var t in trips)
            {
                var text = string.Join(" ", new[] { t.Title, t.Description, t.City ?? string.Empty, t.StartDate?.ToShortDateString() ?? string.Empty });
                await _vectorDbService.SaveTextVector(collectionName, t.Id.ToString(), "Trip", text, t.Title, t.Price, t.City);
            }

            // Sync Hotels
            var hotels = await _unitOfWork.Repository<Hotel>().ListAsync(new ActiveEntitySpecification<Hotel>());
            foreach (var h in hotels)
            {
                var text = string.Join(" ", new[] { h.Name, h.Description });
                await _vectorDbService.SaveTextVector(collectionName, h.Id.ToString(), "Hotel", text, h.Name, h.BasePrice, h.City);
            }

            // Sync Cars
            var cars = await _unitOfWork.Repository<Car>().ListAsync(new ActiveEntitySpecification<Car>());
            foreach (var c in cars)
            {
                var text = string.Join(" ", new[] { c.Name, c.Description, c.Overview });
                await _vectorDbService.SaveTextVector(collectionName, c.Id.ToString(), "Car", text, c.Name, c.Price, null);
            }
        }
    }
}