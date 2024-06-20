using EventReceiverApi.BusinessLogic.Services;
using EventReceiverApi.DataStorage.Models;
using EventReceiverApi.DataStorage.Repositories;
using Moq;

namespace EventReceiverApiTest
{
    public class EventServiceTest
    {
        [Fact]
        public async Task AddAsync_ShouldCallAddAsyncOnRepository()
        {
            // Arrange
            var mockRepository = new Mock<IRepository>();
            var eventService = new EventService(mockRepository.Object);
            var eventModel = new EventModel();

            // Act
            await eventService.AddAsync(eventModel);

            // Assert
            mockRepository.Verify(r => r.AddAsync(eventModel), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ShouldThrowArgumentNullException_WhenEventModelIsNull()
        {
            // Arrange
            var mockRepository = new Mock<IRepository>();
            var eventService = new EventService(mockRepository.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => eventService.AddAsync(null));
        }

        [Fact]
        public async Task GetEventForPeriodAsync_ShouldReturnCorrectEventCountPerMinute()
        {
            // Arrange
            var mockRepository = new Mock<IRepository>();
            var eventService = new EventService(mockRepository.Object);

            var startTime = new DateTime(2023, 6, 1, 0, 0, 0);
            var endTime = new DateTime(2023, 6, 1, 0, 5, 0);
            var offset = 0;

            var events = new List<EventModel>
            {
                new() { Timestamp = new DateTime(2023, 6, 1, 0, 1, 0), Value = 1 },
                new() { Timestamp = new DateTime(2023, 6, 1, 0, 1, 30), Value = 1 },
                new() { Timestamp = new DateTime(2023, 6, 1, 0, 2, 0), Value = 1 }
            };

            mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(events);

            // Act
            var result = await eventService.GetEventForPeriodAsync(startTime, endTime, offset);

            // Assert
            Assert.Equal(2, result[new DateTime(2023, 6, 1, 0, 1, 0)]);
            Assert.Equal(1, result[new DateTime(2023, 6, 1, 0, 2, 0)]);
        }

        [Fact]
        public async Task GetEventForPeriodAsync_ShouldReturnCorrectEventSumOfValuePerMinute()
        {
            // Arrange
            var mockRepository = new Mock<IRepository>();
            var eventService = new EventService(mockRepository.Object);

            var startTime = new DateTime(2023, 6, 1, 0, 0, 0);
            var endTime = new DateTime(2023, 6, 1, 0, 5, 0);
            var offset = 0;

            var events = new List<EventModel>
            {
                new() { Timestamp = new DateTime(2023, 6, 1, 0, 1, 0), Value = 10 },
                new() { Timestamp = new DateTime(2023, 6, 1, 0, 1, 30), Value = 20 },
                new() { Timestamp = new DateTime(2023, 6, 1, 0, 2, 0), Value = 30 }
            };

            mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(events);

            // Act
            var result = await eventService.GetEventForPeriodAsync(startTime, endTime, offset);

            // Assert
            Assert.Equal(30, result[new DateTime(2023, 6, 1, 0, 1, 0)]);
            Assert.Equal(30, result[new DateTime(2023, 6, 1, 0, 2, 0)]);
        }

        [Fact]
        public async Task GetEventForPeriodAsync_ShouldReturnEmptyDictionary_WhenNoEventsInPeriod()
        {
            // Arrange
            var mockRepository = new Mock<IRepository>();
            var eventService = new EventService(mockRepository.Object);

            var startTime = new DateTime(2023, 6, 1, 0, 0, 0);
            var endTime = new DateTime(2023, 6, 1, 0, 5, 0);
            var offset = 0;

            var events = new List<EventModel>(); // No events

            mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(events);

            // Act
            var result = await eventService.GetEventForPeriodAsync(startTime, endTime, offset);

            // Assert
            Assert.Empty(result);
        }
    }
}