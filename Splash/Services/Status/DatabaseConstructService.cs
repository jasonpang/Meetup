using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Splash.Extensions;
using ServiceStack.Common.Web;
using System.Net;
using Splash.Model.Entities;

using UserModel = Splash.Model.Entities.User;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using System.Reflection;

namespace Splash.Services.Splash
{
    [Route("/Database", "PUT")]
    public class ConstructDatabase { }

    public class DatabaseConstructService : Service
    {
        private void ReconstructDatabase(Configuration config)
        {
            var schema = new SchemaExport(config);
            schema.Create(false, true);
        }

        public object Put(ConstructDatabase request)
        {
            var _sessionFactory = Fluently.Configure()
                //.Database(MySQLConfiguration.Standard.ConnectionString(c => c.FromConnectionStringWithKey("MySqlConnectionString")))
                .Database(MySQLConfiguration.Standard.ConnectionString("Server=localhost; Port=3306; Database=splash; Uid=jasonhpang; Pwd=jasonhpang;"))
                .Mappings(m => m.FluentMappings
                    .AddFromAssembly(Assembly.GetExecutingAssembly())
                    .Conventions.Add(FluentNHibernateExtensions.AllConventions()))
                .ExposeConfiguration(ReconstructDatabase)
                .BuildSessionFactory();

            var jason = new UserModel()
            {
                Created = DateTime.Now.ToTimestamp(),
                FirstName = "Jason",
                LastName = "Pang",
                Nickname = "jp2011",
                Email = "jasonpang2011@gmail.com",
                PhoneNumber = "14088329201",
                Password = "123456",
            };

            var sebastian = new UserModel()
            {
                Created = DateTime.Now.AddHours(3).ToTimestamp(),
                FirstName = "Sebastian",
                LastName = "Liu",
                Nickname = "cbassliu10",
                Email = "sebastian.liu@gmail.com",
                PhoneNumber = "14084394728",
                Password = "123456",
            };

            var gerald = new UserModel()
            {
                Created = DateTime.Now.AddHours(3).ToTimestamp(),
                FirstName = "Gerald",
                LastName = "Fong",
                Nickname = "geraldgfong",
                Email = "geraldgfong@gmail.com",
                PhoneNumber = "14085178821",
                Password = "123456",
            };

            var jasonsCollege = new Location()
            {
                Timestamp = DateTime.Now.ToTimestamp(),
                Latitude = 38.538429m,
                Longitude = -121.75886m,
                User = jason,
            };

            var sebastiansCollege = new Location()
            {
                Timestamp = DateTime.Now.AddHours(3).ToTimestamp(),
                Latitude = 40.729239m,
                Longitude = -73.995871m,
                User = jason,
            };

            var geraldsCollege = new Location()
            {
                Timestamp = DateTime.Now.AddHours(-3).ToTimestamp(),
                Latitude = 37.875404m,
                Longitude = -122.246161m,
                User = jason,
            };

            // Add a bunch of mock locations

            var locations = new List<Location>();

            var timeStampNow = DateTime.Now.ToTimestamp();
            var latitudeNow = 30.000m;
            var longitudeNow = 60.000m;

            for (int i = 0; i < 10; i++)
            {
                locations.Add(new Location()
                {
                    Timestamp = ++timeStampNow,
                    Latitude = latitudeNow += 0.15m,
                    Longitude = longitudeNow += 0.15m,
                    User = jason,
                });
            }

            jason.Locations.Add(jasonsCollege);
            jason.Locations.Add(sebastiansCollege);
            jason.Locations.Add(geraldsCollege);

            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.SaveOrUpdate(jason);
                    session.SaveOrUpdate(sebastian);
                    session.SaveOrUpdate(gerald);

                    transaction.Commit();
                }
            }

            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    locations.ForEach(location => session.Save(location));

                    transaction.Commit();
                }
            }

            return new ResponseStatus() { Message = "The database was reconstructed successfully." };
        }
    }
}