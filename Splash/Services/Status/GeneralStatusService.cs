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
    [Route("/Status")]
    public class GeneralStatus { }

    public class GeneralStatusService : Service
    {
        public object Get(GeneralStatus request)
        {
            return new HttpError(HttpStatusCode.OK, "All services are functional.");
        }
    }
}