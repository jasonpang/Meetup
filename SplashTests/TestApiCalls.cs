using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceStack.ServiceClient.Web;
using ServiceStack.Service;
using Splash.Model.Entities;
using System.Collections.Generic;
using Splash.Services.User;

namespace SplashTests
{
    [TestClass]
    public class TestApiCalls : JsonServiceClient
    {
        private IRestClient client;

        public TestApiCalls()
        {
            client = new JsonServiceClient("http://localhost/");
        }

        [TestMethod]
        public void TestRetrieveUser()
        {
            var user = client.Get(new RetrieveUser() { UserId = 1 });

            var jason = new User()
            {
                Created = user.Created,
                FirstName = "Jason",
                LastName = "Pang",
                Nickname = "jp2011",
                Email = "jasonpang2011@gmail.com",
                PhoneNumber = "14088329201",
                Password = "123456",
            };

            Assert.AreEqual(user, jason);
        }
    }
}
