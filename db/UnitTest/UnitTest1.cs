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
     * DB�̏ƍ��������l������e�X�g�ł́A���ۂɎg�p����DB���g�p����B
     * ��DB���Ƃɏƍ������̃��[�����Ⴄ�\������������A��������DB���ƂɃT�|�[�g���Ă���
     * �ƍ��������Ⴄ���߁B
     * 
     * IClassFixture�ɂ���āA�N���X���ƂɈ�x����DB�̃Z�b�g�A�b�v�����s����B
     * �֐����ƂɃ}�C�O���[�V���������s����͎̂��s�R�X�g�̖ʂ���������ق����悢�B
     * �������A�t�B�N�X�`�����e�X�g�N���X���Ƃɋ��L���Ă��܂��ƁA�e�X�g�f�[�^�܂ŋ��L�����
     * ���ƂƂȂ萧����傫���Ȃ�B
     * 
     * �����𑍍�����ƁA�ƍ��������l������e�X�g�ł́A�N���X�̍ŏ���DB�̃Z�b�g�A�b�v���s���A
     * �֐����ƂɃf�[�^�����������āA������Ő��䂵�Ȃ��悤�ɂ���B
     * 
     * xUnit v2�ł̓N���X���ł̓f�t�H���g�ŕ���Ƀe�X�g�����s���Ȃ��Ƃ̂��ƁB
     * https://xunit.net/docs/running-tests-in-parallel.html
     * Test Collections�Q��
     */
    public class UnitTest1(DatabaseFixture dbFixture) : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            // �f�[�^�͊֐����ƂɃN���A����B
            // ���e�X�g������Ŏ��s����Ȃ����Ƃ�O��Ƃ���B
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

            Assert.Single(users); // �f�[�^�x�[�X���N���A����Ă���̂ŃG���[���o��̂�����
            var result = users.FirstOrDefault();

            Assert.NotNull(result);
            Assert.Equal("user", result.Name);
            Assert.Equal("password", result.Email);
            Assert.Equal(1, result.Id);
        }
    }
}