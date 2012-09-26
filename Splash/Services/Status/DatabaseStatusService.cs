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

using UserModel = Splash.Model.Entities.User;

namespace Splash.Services.Splash
{
    [Route("/Status/Database")]
    public class DatabaseStatus { }

    public class DatabaseStatusService : Service
    {
        public object Get(DatabaseStatus request)
        {
            try
            {
                using (var session = NHibernateHelper.OpenSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        session.CreateCriteria<UserModel>().List<UserModel>();
                    }
                }

                return new HttpError(HttpStatusCode.OK, "Retrieving from the database is functional.");
            }
            catch (Exception ex)
            {
                return new HttpError(HttpStatusCode.InternalServerError, ex);
            }
        }

        public object Delete(DatabaseStatus request)
        {
            var user = new UserModel() { LastLoggedIn = DateTime.Now.ToTimestamp() };

            try
            {
                using (var session = NHibernateHelper.OpenSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        session.SaveOrUpdate(user);
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                return new HttpError(HttpStatusCode.InternalServerError, "Creating in the database failed with: " + ex);
            }

            try
            {
                using (var session = NHibernateHelper.OpenSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        session.Delete(user);
                        transaction.Commit();
                    }
                }

                return new HttpError(HttpStatusCode.OK, "Creating in and deleting from the database is functional.");
            }
            catch (Exception ex)
            {
                return new HttpError(HttpStatusCode.InternalServerError, "Deleting from the database failed with: " + ex);
            }
        }
    }
}