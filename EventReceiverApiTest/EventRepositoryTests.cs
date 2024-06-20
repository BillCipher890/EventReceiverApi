using EventReceiverApi.DataStorage.Models;
using EventReceiverApi.DataStorage.Repositories;
using EventReceiverApi.DataStorage;
using Microsoft.EntityFrameworkCore;

namespace EventReceiverApiTest
{
    public class EventRepositoryTests : IDisposable
    {
        private readonly DbContextOptions<AppDBContext> _options;
        private readonly AppDBContext _context;
        private readonly EventRepository _repository;

        public EventRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(databaseName: "EventDatabase")
                .Options;
            _context = new AppDBContext(_options);
            _repository = new EventRepository(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task AddAsync_ShouldAddEventToDatabase()
        {
            // Arrange
            var eventModel = new EventModel { Id = 1, Timestamp = DateTime.UtcNow, Name = "Event 1", Value = 10 };

            // Act
            await _repository.AddAsync(eventModel);

            // Assert
            var storedEvent = await _context.EventModels.FindAsync(eventModel.Id);
            Assert.NotNull(storedEvent);
            Assert.Equal(eventModel.Id, storedEvent.Id);
        }

        [Fact]
        public async Task AddAsync_ShouldSaveMultipleEvents()
        {
            // Arrange
            var eventModel1 = new EventModel { Id = 1, Timestamp = DateTime.UtcNow, Name = "Event 1", Value = 10 };
            var eventModel2 = new EventModel { Id = 2, Timestamp = DateTime.UtcNow, Name = "Event 2", Value = 20 };

            // Act
            await _repository.AddAsync(eventModel1);
            await _repository.AddAsync(eventModel2);

            // Assert
            var events = await _context.EventModels.ToListAsync();
            Assert.Equal(2, events.Count);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllEvents()
        {
            // Arrange
            var eventModel1 = new EventModel { Id = 1, Timestamp = DateTime.UtcNow, Name = "Event 1", Value = 10 };
            var eventModel2 = new EventModel { Id = 2, Timestamp = DateTime.UtcNow, Name = "Event 2", Value = 20 };

            _context.EventModels.Add(eventModel1);
            _context.EventModels.Add(eventModel2);
            await _context.SaveChangesAsync();

            // Act
            var events = await _repository.GetAllAsync();

            // Assert
            Assert.Equal(2, events.Count);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyListWhenNoEvents()
        {
            // Act
            var events = await _repository.GetAllAsync();

            // Assert
            Assert.Empty(events);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnCorrectEventDetails()
        {
            // Arrange
            var eventModel = new EventModel { Id = 1, Timestamp = DateTime.UtcNow, Name = "Event 1", Value = 10 };

            _context.EventModels.Add(eventModel);
            await _context.SaveChangesAsync();

            // Act
            var events = await _repository.GetAllAsync();

            // Assert
            Assert.Single(events);
            Assert.Equal(eventModel.Id, events[0].Id);
            Assert.Equal(eventModel.Value, events[0].Value);
        }
    }
}
