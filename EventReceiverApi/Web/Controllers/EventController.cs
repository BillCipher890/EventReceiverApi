using EventReceiverApi.BusinessLogic.Services;
using EventReceiverApi.DataStorage.Models;
using EventReceiverApi.Web.Filters;
using Microsoft.AspNetCore.Mvc;

namespace EventReceiverApi.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController(IEventService eventService) : Controller
    {
        private readonly IEventService _eventService = eventService;

        [HttpPost]
        [Route("AddEvent")]
        public async Task<IActionResult> AddEvent(EventModel newEvent)
        {
            await _eventService.AddAsync(newEvent);
            return Ok();
        }

        [HttpGet]
        [Route("GetEvents")]
        [ValidateEventParameters]
        public async Task<ActionResult<Dictionary<DateTime, int>>> GetEventsForPeriod(DateTime startTime, DateTime endTime, int offset)
        {
            return Ok(await _eventService.GetEventForPeriodAsync(startTime, endTime, offset));
        }
    }
}
