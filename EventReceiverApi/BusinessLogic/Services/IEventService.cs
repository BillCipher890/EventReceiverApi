using EventReceiverApi.DataStorage.Models;

namespace EventReceiverApi.BusinessLogic.Services
{
    public interface IEventService
    {
        Task AddAsync(EventModel eventModel);
        Task<Dictionary<DateTime, int>> GetEventForPeriodAsync(DateTime startTime, DateTime endTime, int offset);
    }
}
