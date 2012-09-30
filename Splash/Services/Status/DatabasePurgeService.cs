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

namespace Splash.Services.Splash
{
    [Route("/Database", "DELETE")]
    public class PurgeDatabase { }

    public class DatabasePurgeService : Service
    {
        public object Delete(PurgeDatabase request)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var rawDropSqlCommand = session.Connection.CreateCommand();
                rawDropSqlCommand.CommandText = "DROP DATABASE splash";
                rawDropSqlCommand.ExecuteNonQuery();

                var rawCreateSqlCommand = session.Connection.CreateCommand();
                rawCreateSqlCommand.CommandText = "CREATE DATABASE splash";
                rawCreateSqlCommand.ExecuteNonQuery();
            }

            return new ResponseStatus() { Message = "The database was purged successfully." };
        }
    }
}