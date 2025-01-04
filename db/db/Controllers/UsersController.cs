using db.Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace db.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly AppDbContext _context;
        public UsersController(ILogger<WeatherForecastController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet(Name = "Users")]
        public async Task<List<Users>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();

            if (users.Any())
            {
                return users;
            }

            throw new Exception("Error");
        }
    }
}
