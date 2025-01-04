using db.Db;
using Microsoft.EntityFrameworkCore;

namespace db.Commands
{
    public class Target
    {
        ILogger _logger;
        private readonly AppDbContext _context;

        public Target(ILogger logger, AppDbContext context) { 
            _logger = logger;
            _context = context;
        }

        public async Task<List<Users>> GetUsers() {
            var users = await _context.Users.ToListAsync();

            if (users != null)
            {
                return users;
            }

            throw new Exception("Error");

        }

        public int Hoge()
        {
            return 1;
        }
    }
}
