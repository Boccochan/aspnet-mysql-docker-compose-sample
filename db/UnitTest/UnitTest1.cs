using db.Commands;
using db.Controllers;
using db.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Testcontainers.MySql;
using Xunit.Abstractions;
using Xunit.Sdk;


namespace UnitTest
{
    /**
     * DBの照合順序を考慮するテストでは、実際に使用するDBを使用する。
     * ※DBごとに照合順序のルールが違う可能性があったり、そもそもDBごとにサポートしている
     * 照合順序が違うため。
     * 
     * IClassFixtureによって、クラスごとに一度だけDBのセットアップを実行する。
     * 関数ごとにマイグレーションを実行するのは実行コストの面から避けたほうがよい。
     * しかし、フィクスチャをテストクラスごとに共有してしまうと、テストデータまで共有される
     * こととなり制約も大きくなる。
     * 
     * これらを総合すると、照合順序を考慮するテストでは、クラスの最初でDBのセットアップを行い、
     * 関数ごとにデータを初期化して、かつ並列で制御しないようにする。
     * 
     * xUnit v2ではクラス内ではデフォルトで並列にテストを実行しないとのこと。
     * https://xunit.net/docs/running-tests-in-parallel.html
     * Test Collections参照
     */
    public class UnitTest1(DatabaseFixture dbFixture) : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            // データは関数ごとにクリアする。
            // ※テストが並列で実行されないことを前提とする。
            var users = await dbFixture.dbContext!.Users.ToListAsync();

            if(users.Any())
            {
                dbFixture.dbContext.RemoveRange(users);
                await dbFixture.dbContext.SaveChangesAsync();
            }
        }

        [Fact]
        public async void A_Test1()
        {
            var logger = new Mock<ILogger<Target>>().Object;
            var target= new Target(logger, dbFixture.dbContext!);

            var user = new Users
            {
                Id = 1,
                Name = "user",
                Email = "password"
            };

            dbFixture.dbContext.Add(user);
            dbFixture.dbContext.SaveChanges();

            var users = await target.GetUsers();
            Assert.Single(users);
            var result = users.FirstOrDefault();

            Assert.NotNull(result);
            Assert.Equal("user", result.Name);
            Assert.Equal("password", result.Email);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async void B_Test2()
        {
            dbFixture.dbContext.ChangeTracker.Clear();
            var logger = new Mock<ILogger<Target>>().Object;
            var target = new Target(logger, dbFixture.dbContext!);
            var users = await target.GetUsers();

            Assert.Single(users); // データベースがクリアされているのでエラーが出るのが正解
            var result = users.FirstOrDefault();

            Assert.NotNull(result);
            Assert.Equal("user", result.Name);
            Assert.Equal("password", result.Email);
            Assert.Equal(1, result.Id);
        }
    }
}