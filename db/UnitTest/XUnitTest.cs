using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    public class XUnitTest
    {
        public static TheoryData<int[]> GetData()
        {
            return new TheoryData<int[]>
            {
                new int[] {1, 2, 3},
            };
        }

        [Theory]
        [MemberData(nameof(GetData))]
        public void Test1(int[] arr)
        {
            Assert.Equal(3, arr.Length);
        }
    }
}
