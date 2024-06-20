using EventReceiverApi.DataStorage.Models;

namespace EventReceiverApi.DataStorage.Repositories
{
    public interface IRepository
    {
        Task AddAsync(EventModel eventModel);
        Task<List<EventModel>> GetAllAsync();
    }
}
