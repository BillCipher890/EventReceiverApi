using EventReceiverApi.DataStorage.Models;
using Microsoft.EntityFrameworkCore;

namespace EventReceiverApi.DataStorage
{
    public class AppDBContext : DbContext
    {
        public DbSet<EventModel> EventModels { get; set; }

        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
