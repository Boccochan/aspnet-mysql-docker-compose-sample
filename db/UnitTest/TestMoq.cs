using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Moq;

namespace UnitTest
{
    public class TestMoq
    {
        public class HogeDto
        {
            public string? Name { get; set; }
        }

        public interface IHoge
        {
            bool DoSomething(string arg);

            string Run(HogeDto hoge);

            bool Range(int arg);

            string Range2(int arg);
        }

        [Fact]
        public void TestRange() 
        {
            var mock = new Mock<IHoge>();
            mock.Setup(x => x.Range(It.IsInRange<int>(0, 100, Moq.Range.Inclusive))).Throws(new Exception("Hello"));

            var result = Assert.Throws<Exception>(() => mock.Object.Range(50));

            Assert.Equal("Hello", result.Message);
        }

        [Fact]
        public void TestRange2()
        {
            var mock = new Mock<IHoge>();
            mock.Setup(x => x.Range2(It.IsInRange<int>(0, 100, Moq.Range.Inclusive)))
                .Returns("hoge");

            var result = mock.Object.Range2(501);

            Assert.Null(result);
        }

        [Fact]
        public void Test1() 
        {
            var mock = new Mock<IHoge>();
            mock.Setup(x => x.DoSomething("abc"))
                .Returns(true);
            
            Assert.True(mock.Object.DoSomething("abc"));
        }

        [Fact]
        public void Test2() {
            var mock = new Mock<IHoge>();
            mock.Setup(x => x.DoSomething("abc"))
                .Returns(true);

            Assert.False(mock.Object.DoSomething("def"));
        }

        [Fact]
        public void Test3() {
            var mock = new Mock<IHoge>();
            mock.SetupSequence(x => x.DoSomething("abc"))
                .Returns(true)
                .Returns(false);

            Assert.True(mock.Object.DoSomething("abc"));
            Assert.False(mock.Object.DoSomething("abc"));
        }

        [Fact]
        public void Test4()
        {
            var mock = new Mock<IHoge>();
            bool result = false;

            mock.Setup(x => x.DoSomething(It.IsAny<string>()))
                .Callback<string>(x => { result = true; })
                .Returns(() => result);

            var hoge = mock.Object.DoSomething("abc");

            Assert.True(hoge);
            Assert.True(result);
        }

        [Fact]
        public void Test5()
        {
            var mock = new Mock<IHoge>();

            mock.Object.Run(new HogeDto { Name = "user" });

            mock.Verify(s => s.Run(It.IsAny<HogeDto>()), Times.Once());
        }

        [Fact]
        public void Test6()
        {
            var mock = new Mock<IHoge>();
            mock.Setup(x => x.Run(It.IsAny<HogeDto>()))
                .Returns((HogeDto hoge) => hoge.Name == "user" ? "ohohoho" : "aaaa");

            Assert.Equal("ohohoho", mock.Object.Run(new HogeDto { Name= "user" }));
            Assert.Equal("aaaa", mock.Object.Run(new HogeDto { Name= "uuu" }));
        }
    }
}
