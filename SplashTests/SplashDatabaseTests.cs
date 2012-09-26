using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Splash;
using NHibernate;
using Splash.Model.Entities;
using System.Collections.Generic;
using Splash.Extensions;

namespace SplashTests
{
    [TestClass]
    public class SplashDatabaseTests
    {
        [TestMethod]
        public void TestAddMockData()
        {
            var jason = new User()
            {
                Created = DateTime.Now.ToTimestamp(),
                FirstName = "Jason",
                LastName = "Pang",
                Nickname = "jp2011",
                Email = "jasonpang2011@gmail.com",
                PhoneNumber = "14088329201",
                Password = "123456",           
            };

            var sebastian = new User()
            {
                Created = DateTime.Now.AddHours(3).ToTimestamp(),
                FirstName = "Sebastian",
                LastName = "Liu",
                Nickname = "cbassliu10",
                Email = "sebastian.liu@gmail.com",
                PhoneNumber = "14084394728",
                Password = "123456",
            };

            var gerald = new User()
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

            jason.Locations.Add(jasonsCollege);
            jason.Locations.Add(sebastiansCollege);
            jason.Locations.Add(geraldsCollege);

            jason.Friends.Add(new Friend()
                {
                    Friender = jason,
                    Friendee = sebastian,
                    FrienderPermissions = FrienderPermissions.CanFollowFriendee,
                });

            jason.Friends.Add(new Friend()
                {
                    Friender = jason,
                    Friendee = gerald,
                    FrienderPermissions = FrienderPermissions.None,
                });

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
                    // No cascade effect
                    //session.Delete(sebastiansCollege);

                    // Yes cascade effect
                    //session.Delete(jason);

                    transaction.Commit();
                }
            }
        }
    }
}
