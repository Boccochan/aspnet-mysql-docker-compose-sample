using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using db.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Testcontainers.MySql;
using Xunit.Abstractions;

namespace UnitTest
{
    public class DatabaseFixture : IAsyncLifetime
    {
        private readonly MySqlContainer? _mySqlContainer = new MySqlBuilder()
                    .WithDatabase("db")
                    .WithUsername("user")
                    .WithPassword("password")
                    .Build();
        public AppDbContext dbContext { get; private set; } = null!;

        public async Task InitializeAsync()
        {
            await _mySqlContainer!.StartAsync();
            var connectionString = _mySqlContainer.GetConnectionString();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 27)))
                .Options;

            dbContext = new AppDbContext(options);
            await dbContext.Database.MigrateAsync();
        }

        public async Task DisposeAsync()
        {
            await _mySqlContainer!.StopAsync();
        }
    }

    // 複数クラスで共有する方法もあるけど、データベースも共有されるので
    // テストデータまで共有される。混乱のもとなのでやめておいた方がよい。
    //[CollectionDefinition("Shared MySQL Collection")]
    //public class SharedMySQLCollection : ICollectionFixture<DatabaseFixture> { }
}
