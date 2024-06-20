using EventReceiverApi.DataStorage.Models;
using Microsoft.EntityFrameworkCore;

namespace EventReceiverApi.DataStorage.Repositories
{
    public class EventRepository : IRepository, IDisposable
    {
        private readonly AppDBContext _context;
        private readonly DbSet<EventModel> _dbSet;

        public EventRepository(AppDBContext context)
        {
            _context = context;
            _dbSet = _context.Set<EventModel>();
        }

        public async Task AddAsync(EventModel eventModel)
        {
            _dbSet.Add(eventModel);
            await _context.SaveChangesAsync();
        }

        public virtual async Task<List<EventModel>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
