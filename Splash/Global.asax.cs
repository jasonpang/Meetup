using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Funq;
using NHibernate;
using ServiceStack.Common.Web;
using ServiceStack.Logging;
using ServiceStack.Logging.Support.Logging;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.ServiceInterface.Validation;
using ServiceStack.WebHost.Endpoints;
using Splash.Authentication;
using Splash.Services.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.SessionState;

// if (Boolean.Parse(WebConfigurationManager.AppSettings["ValidationIsEnabled"]))

namespace Splash
{
    public class Global : System.Web.HttpApplication
    {
        public class SplashAppHost : AppHostBase
        {
            public SplashAppHost() :
                base("Splash", typeof (UserService).Assembly)
            {
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

                // Register privileges filter
                this.RequestFilters.Add((httpReq, httpResp, requestDto) =>
                                            {
                                            });

                // Register validators
                container.RegisterValidators(typeof (CreateUserValidator).Assembly);
            }
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            NHibernateHelper.InitializeSessionFactory();
            new SplashAppHost().Init();
        }
    }
}