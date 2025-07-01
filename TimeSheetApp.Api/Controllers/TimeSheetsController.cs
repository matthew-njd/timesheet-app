using Microsoft.AspNetCore.Mvc;
using TimeSheetApp.Api.Data;

namespace TimeSheetApp.Api.Controllers
{
    public class TimeSheetsController(ApplicationDbContext context, ILogger<TimeSheetsController> logger) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ILogger<TimeSheetsController> _logger = logger;

        
    }
}