using System;
using Greenergy.Energinet;
using Xunit;

namespace greenergy.test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            DateTime now = DateTime.Now;
            DateTime earlier = now.AddHours(-1);

            System.Console.WriteLine(now.CompareTo(earlier));

            int result = now.CompareTo(earlier);

            Assert.True(now.CompareTo(earlier) == 1);
        }
    }
}
