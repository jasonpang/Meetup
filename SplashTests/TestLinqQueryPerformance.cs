using Microsoft.VisualStudio.TestTools.UnitTesting;
using Splash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Splash.Extensions;
using System.Diagnostics;

using LocationModel = Splash.Model.Entities.Location;
using UserModel = Splash.Model.Entities.User;


namespace SplashTests
{
    // ServiceStack Request DTO, returning a List<LocationModel>
    public class GetLocations
    {
        // The user id owning these Locations 
        public int UserId { get; set; }
        // If specified, narrows the search by a minimum timestamp 
        public long? MinTimestamp { get; set; }
        // If specified, narrows the search by a maximum timestamp
        public long? MaxTimestamp { get; set; }
        // If specified, returns the last N Location objects stored by the user (filters in conjunction with Min/MaxTimestamp)
        public int? Last { get; set; }
    }

    // Model object
    public class LocationModelExample
    {
        public virtual int Id { get; protected set; }

        public virtual UserModel User { get; set; }
        public virtual long Timestamp { get; set; }

        public virtual decimal Latitude { get; set; }
        public virtual decimal Longitude { get; set; }

        public LocationModelExample()
        {
            Timestamp = default(long);

            Latitude = default(decimal);
            Longitude = default(decimal);
        }

        public override string ToString()
        {
            return String.Format("{Timestamp={0}, Latitude={0}, Longitude={1}", Timestamp, Latitude, Longitude);
        }
    }

    [TestClass]
    public class TestLinqQueryPerformance
    {
        private Stopwatch stopwatch;
        private GetLocations request;

        // Results of the queries are stored here
        private IEnumerable<LocationModel> locationsList = null;

        public TestLinqQueryPerformance()
        {
            stopwatch = new Stopwatch();

            request = new GetLocations()
            {
                UserId = 1,
                MinTimestamp = 0,
                MaxTimestamp = DateTime.Now.ToTimestamp(),
                Last = 5,
            };
        }

        [TestMethod]
        public void TestWarmup()
        {
            stopwatch.Start();

            // Warmup method one
            for (int i = 0; i < 100; i++)
            {
                using (var session = NHibernateHelper.OpenSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        var user = session.Get<UserModel>(request.UserId);

                        var locations = user.Locations.Where(location =>
                            location.Timestamp >= (request.MinTimestamp.HasValue ? request.MinTimestamp.Value : 0) &&
                            location.Timestamp <= (request.MaxTimestamp.HasValue ? request.MaxTimestamp.Value : DateTime.Now.ToTimestamp()));

                        if (request.Last.HasValue)
                        {
                            locationsList = locations.TakeLastN(request.Last.Value);
                        }
                        else
                        {
                            locationsList = locations;
                        }
                    }
                }
            }

            stopwatch.Stop();

            Trace.WriteLine(String.Format("Method 1 Warmup took {0} seconds over 100 iterations.", stopwatch.ElapsedMilliseconds / 1000.0));
            Trace.WriteLine(String.Empty);
            Trace.WriteLine("Results of the last query:");
            Trace.WriteLine(String.Join(Environment.NewLine, locationsList.Select(x => x))); // Print the list of Locations
            Trace.WriteLine(String.Empty);

            stopwatch.Reset();
            stopwatch.Start();

            // Warmup method two
            for (int i = 0; i < 100; i++)
            {
                using (var session = NHibernateHelper.OpenSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        var locationsQuery = session.QueryOver<LocationModel>()
                                    .Where(table => table.User.Id == request.UserId)
                                    .And(table => table.Timestamp >= (request.MinTimestamp.HasValue ? request.MinTimestamp.Value : 0))
                                    .And(table => table.Timestamp <= (request.MaxTimestamp.HasValue ? request.MaxTimestamp.Value : DateTime.Now.ToTimestamp()));

                        if (request.Last.HasValue)
                        {
                            locationsList = locationsQuery.List().TakeLastN(request.Last.Value);
                        }
                        else
                        {
                            locationsList = locationsQuery.List();
                        }
                    }
                }
            }

            stopwatch.Stop();

            Trace.WriteLine(String.Format("Method 2 Warmup took {0} seconds over 100 iterations.", stopwatch.ElapsedMilliseconds / 1000.0));
            Trace.WriteLine(String.Empty);
            Trace.WriteLine("Results of the last query:");
            Trace.WriteLine(String.Join(Environment.NewLine, locationsList.Select(x => x))); // Print the list of Locations
            Trace.WriteLine(String.Empty);
        }

        [TestMethod]
        public void TestMethod1()
        {
            stopwatch.Reset();
            stopwatch.Start();

            var timeStampNow = DateTime.Now.ToTimestamp();

            // Warmup method one
            for (int i = 0; i < 5000; i++)
            {
                using (var session = NHibernateHelper.OpenSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        var user = session.Get<UserModel>(request.UserId);

                        var locations = user.Locations.Where(location =>
                            location.Timestamp >= (request.MinTimestamp.HasValue ? request.MinTimestamp.Value : 0) &&
                            location.Timestamp <= (request.MaxTimestamp.HasValue ? request.MaxTimestamp.Value : timeStampNow));

                        if (request.Last.HasValue)
                        {
                            locationsList = locations.TakeLastN(request.Last.Value);
                        }
                        else
                        {
                            locationsList = locations;
                        }
                    }
                }
            }

            stopwatch.Stop();

            Trace.WriteLine(String.Format("Method 1 took {0} seconds over 100 iterations.", stopwatch.ElapsedMilliseconds / 1000.0));
            Trace.WriteLine(String.Empty);
            Trace.WriteLine("Results of the last query:");
            Trace.WriteLine(String.Join(Environment.NewLine, locationsList.Select(x => x))); // Print the list of Locations
            Trace.WriteLine(String.Empty);
        }

        [TestMethod]
        public void TestMethod2()
        {
            stopwatch.Reset();
            stopwatch.Start();

            var timeStampNow = DateTime.Now.ToTimestamp();

            // Warmup method two
            for (int i = 0; i < 5000; i++)
            {
                using (var session = NHibernateHelper.OpenSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        var locationsQuery = session.QueryOver<LocationModel>()
                                    .Where(table => table.User.Id == request.UserId)
                                    .And(table => table.Timestamp >= (request.MinTimestamp.HasValue ? request.MinTimestamp.Value : 0))
                                    .And(table => table.Timestamp <= (request.MaxTimestamp.HasValue ? request.MaxTimestamp.Value : timeStampNow));

                        if (request.Last.HasValue)
                        {
                            locationsList = locationsQuery.List().TakeLastN(request.Last.Value);
                        }
                        else
                        {
                            locationsList = locationsQuery.List();
                        }
                    }
                }
            }

            stopwatch.Stop();

            Trace.WriteLine(String.Format("Method 2 took {0} seconds over 5000 iterations.", stopwatch.ElapsedMilliseconds / 1000.0));
            Trace.WriteLine(String.Empty);
            Trace.WriteLine("Results of the last query:");
            Trace.WriteLine(String.Join(Environment.NewLine, locationsList.Select(x => x))); // Print the list of Locations
            Trace.WriteLine(String.Empty);
        }
    }
}
