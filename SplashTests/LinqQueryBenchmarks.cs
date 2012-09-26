using Microsoft.VisualStudio.TestTools.UnitTesting;
using Splash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Splash.Extensions;

using LocationModel = Splash.Model.Entities.Location;
using UserModel = Splash.Model.Entities.User;
using System.Diagnostics;

namespace SplashTests
{
    [TestClass]
    public class LinqQueryBenchmarks
    {
        [TestMethod]
        public void TestAAWarmCache()
        {
            int? MinTimestamp = 0;
            int? MaxTimestamp = 999999999;

            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            for (int i = 0; i < 10000; i++)
            {
                using (var session = NHibernateHelper.OpenSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        var user = session.Get<UserModel>(1);

                        var locations = user.Locations.Where(location =>
                            location.Timestamp >= (MinTimestamp.HasValue ? MinTimestamp.Value : 0) &&
                            location.Timestamp <= (MaxTimestamp.HasValue ? MaxTimestamp.Value : DateTime.Now.ToTimestamp()));
                    }
                }
            }

            stopwatch.Stop();

            Trace.WriteLine("**************************");
            Trace.WriteLine("**************************");
            Trace.WriteLine("**************************");
            Trace.WriteLine("Milliseconds for TestAAWarmCache: " + stopwatch.ElapsedMilliseconds);
            Trace.WriteLine("**************************");
            Trace.WriteLine("**************************");
            Trace.WriteLine("**************************");
        }

        [TestMethod]
        public void TestABAddMockData()
        {
            int? MinTimestamp = 0;
            int? MaxTimestamp = 999999999;
            long nowTime = DateTime.Now.ToTimestamp();

            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            for (int i = 0; i < 50000; i++)
            {
                using (var session = NHibernateHelper.OpenSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        var user = session.Get<UserModel>(1);

                        var locations = user.Locations.Where(location =>
                            location.Timestamp >= (MinTimestamp.HasValue ? MinTimestamp.Value : 0) &&
                            location.Timestamp <= (MaxTimestamp.HasValue ? MaxTimestamp.Value : nowTime));
                    }
                }
            }

            stopwatch.Stop();

            Trace.WriteLine("**************************");
            Trace.WriteLine("**************************");
            Trace.WriteLine("**************************");
            Trace.WriteLine("Milliseconds for TestABAddMockData: " + stopwatch.ElapsedMilliseconds);
            Trace.WriteLine("**************************");
            Trace.WriteLine("**************************");
            Trace.WriteLine("**************************");
        }

        [TestMethod]
        public void TestACAddMockData2()
        {
            string test = "";
            int? MinTimestamp = 0;
            int? MaxTimestamp = 999999999;
            long nowTime = DateTime.Now.ToTimestamp();

            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            for (int i = 0; i < 50000; i++)
            {
                using (var session = NHibernateHelper.OpenSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        var locationsQuery = session.QueryOver<LocationModel>()
                            .Where(table => table.User.Id == 1)
                            .And(table => table.Timestamp >= (MinTimestamp.HasValue ? MinTimestamp.Value : 0))
                            .And(table => table.Timestamp <= (MaxTimestamp.HasValue ? MaxTimestamp.Value : nowTime));
                    }
                }
            }

            stopwatch.Stop();

            Trace.WriteLine("**************************");
            Trace.WriteLine("**************************");
            Trace.WriteLine("**************************");
            Trace.WriteLine("Milliseconds for TestACAddMockData: " + stopwatch.ElapsedMilliseconds);
            Trace.WriteLine("**************************");
            Trace.WriteLine("**************************");
            Trace.WriteLine("**************************");
        }
    }
}
