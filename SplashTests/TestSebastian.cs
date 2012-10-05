using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Diagnostics;

namespace SplashTests
{
    [TestClass]
    public class TestSebastianMethods
    {
        private long revNumber = 0;
        private long tempNumber = 0;

        [TestMethod]
        public void TestSebastian()
        {
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 100000; i++)
            {
                reverse(123456789123456789);
            }
            stopwatch.Stop();

            Trace.WriteLine("Time: " + stopwatch.ElapsedMilliseconds * 1000.0);
        }

        [TestMethod]
        public void TestJason()
        {
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 100000; i++)
            {
                reverse(123456789123456789);
            }
            stopwatch.Stop();   
            reverse(123456789123456789);
            Trace.WriteLine("Time: " + stopwatch.ElapsedMilliseconds * 1000.0);
        }

        public long reverse2(long number)
        {
            var numberAsString = number.ToString();

            var numberAsStringReversed = new StringBuilder(String.Empty);

            for (int i = numberAsString.Length; i > 0; i--)
            {
                numberAsStringReversed.Append(numberAsString[i]);
            }

            var newString = numberAsStringReversed.ToString();

            return Int64.Parse(newString);
        }

        public long reverse(long number)
        {
            revNumber = 0;
            tempNumber = 0;

            while (number > 0)
            {
                tempNumber = number % 10;

                revNumber = (revNumber * 10) + tempNumber;
                number = number / 10;
            }
            return revNumber;
        }
    }
}
