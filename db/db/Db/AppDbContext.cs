using Microsoft.EntityFrameworkCore;

namespace db.Db
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // DbSet の定義（テーブルを表す）
        public DbSet<Users> Users { get; set; }
    }
}

public class Users
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
}
