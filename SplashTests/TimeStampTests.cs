using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Splash;
using NHibernate;
using Splash.Model.Entities;
using System.Collections.Generic;
using Splash.Extensions;
using System.Diagnostics;

namespace SplashTests
{
    [TestClass]
    public class TimeStampTests
    {
        [TestMethod]
        public void TestTimeStamp()
        {
            for (int i = 0; i < 1; i++)
            {
                DateTime now = DateTime.Now;
                long timeStamp = now.ToTimestamp();
                DateTime nowAndAgain = timeStamp.ToDateTime();

                Trace.WriteLine(nowAndAgain.ToString() + "    " + nowAndAgain.Millisecond);                
            }
        }
    }
}
