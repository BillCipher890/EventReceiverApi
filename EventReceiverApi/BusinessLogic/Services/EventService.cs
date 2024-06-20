using EventReceiverApi.DataStorage.Models;
using EventReceiverApi.DataStorage.Repositories;

namespace EventReceiverApi.BusinessLogic.Services
{
    public class EventService(IRepository eventRepository) : IEventService
    {
        private readonly IRepository _eventRepository = eventRepository;

        public async Task AddAsync(EventModel eventModel)
        {
            ArgumentNullException.ThrowIfNull(eventModel);
            await _eventRepository.AddAsync(eventModel);
        }

        public async Task<Dictionary<DateTime, int>> GetEventForPeriodAsync(DateTime startTime, DateTime endTime, int offset)
        {
            startTime = startTime.AddHours(-offset);
            endTime = endTime.AddHours(-offset);
            var oneMinuteSpan = new TimeSpan(0, 1, 0);
            var allEvents = await _eventRepository.GetAllAsync();
            return allEvents
                .Where(e => e.Timestamp >= startTime && e.Timestamp <= endTime)
                .GroupBy(e => new DateTime(e.Timestamp.Ticks / oneMinuteSpan.Ticks * oneMinuteSpan.Ticks))
                .ToDictionary(g => g.Key, g => g.Sum(e => e.Value));
        }
    }
}
