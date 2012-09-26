using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Splash.Extensions;
using Splash.Model.Entities;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System.Reflection;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Helpers;
using System.Web.Configuration;

namespace Splash
{
    public static class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;

        private static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                    InitializeSessionFactory();
                return _sessionFactory;
            }
        }

        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }

        public static void InitializeSessionFactory()
        {
            _sessionFactory = Fluently.Configure()
                //.Database(MySQLConfiguration.Standard.ConnectionString(c => c.FromConnectionStringWithKey("MySqlConnectionString")))
                .Database(MySQLConfiguration.Standard.ConnectionString("Server=localhost; Port=3306; Database=splash; Uid=jasonhpang; Pwd=jasonhpang;"))
                .Mappings(m => m.FluentMappings
                    .AddFromAssembly(Assembly.GetExecutingAssembly())
                    .Conventions.Add(FluentNHibernateExtensions.ForLowercaseSystem(String.Empty, true)))
                .ExposeConfiguration(ReconstructDatabase)
                .BuildSessionFactory();
        }

        private static void ReconstructDatabase(Configuration config)
        {
//            if (Boolean.Parse(WebConfigurationManager.AppSettings["DoReconstructDatabase"]))
            {
                //var schema = new SchemaExport(config);
                //schema.Drop(false, true);
                //schema.Create(false, true);
            }
        }
    }
}