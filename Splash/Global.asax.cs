using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Funq;
using NHibernate;
using ServiceStack.Common.Web;
using ServiceStack.Logging;
using ServiceStack.Logging.Support.Logging;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Validation;
using ServiceStack.WebHost.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.SessionState;

namespace Splash
{
    public class UserService : Service { }

    public class Global : System.Web.HttpApplication
    {
        

        public class SplashAppHost : AppHostBase    
        {
            public SplashAppHost() :
                base("Splash", typeof(UserService).Assembly)
            {
                if (Boolean.Parse(WebConfigurationManager.AppSettings["ValidationIsEnabled"]))
                    Plugins.Add(new ValidationFeature());
            }

            public override void Configure(Container container)
            {
                SetConfig(new EndpointHostConfig() 
                {
                    DefaultContentType = ContentType.Json,
                    DebugMode = true,
                    EnableFeatures = Feature.All,
                });

                // container.RegisterValidators(typeof(CreateUserValidator).Assembly);
            }
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            NHibernateHelper.InitializeSessionFactory();
            new SplashAppHost().Init();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
                
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}