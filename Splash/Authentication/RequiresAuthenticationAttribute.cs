using System.Net;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.Text;
using ServiceStack.WebHost.Endpoints.Extensions;
using Splash.Model.Entities;

namespace Splash.Authentication
{
    /// <summary>
    /// Indicates that the request dto, which is associated with this attribute,
    /// requires authentication.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method /*MVC Actions*/, Inherited = false, AllowMultiple = false)]
    public class RequiresAuthenticationAttribute : RequestFilterAttribute
    {
        public RequiresAuthenticationAttribute(ApplyTo applyTo)
            : base(applyTo)
        {
            this.Priority = (int)RequestFilterPriority.Authenticate;
        }

        public RequiresAuthenticationAttribute() 
            : this(ApplyTo.All) 
        { 
        }

        public override void Execute(IHttpRequest req, IHttpResponse res, object requestDto)
        {
            AuthenticateBasicAuth(req, res);
        }

        //Also shared by RequiredRoleAttribute and RequiredPermissionAttribute
        public static User AuthenticateBasicAuth(IHttpRequest req, IHttpResponse res)
        {
            var userCredentialsPair = req.GetBasicAuthUserAndPassword();
            var email = userCredentialsPair.HasValue ? userCredentialsPair.Value.Key : String.Empty;
            var password = userCredentialsPair.HasValue ? userCredentialsPair.Value.Value : String.Empty;

            User user = null;
            bool isValid = false;

            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var userQuery = session.QueryOver<User>()
                        .Where(table => table.Email == email)
                        .And(table => table.Password == password);

                    user = userQuery.SingleOrDefault();

                    transaction.Commit();

                    isValid = (user != null);
                }
            }

            if (!isValid)
            {
                res.StatusCode = (int)HttpStatusCode.Unauthorized;
                res.EndServiceStackRequest();
            }

            return user;
        }    
    }
}