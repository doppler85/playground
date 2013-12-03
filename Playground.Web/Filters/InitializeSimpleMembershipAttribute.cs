using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;

using WebMatrix.WebData;
using Playground.Web.Models;
using System.Web.Http.Filters;

namespace Playground.Web.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class InitializeSimpleMembershipAttribute : ActionFilterAttribute
    {
        private static SimpleMembershipInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    // Ensure ASP.NET Simple Membership is initialized only once per app start
        //    LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        //}

        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        }

        private class SimpleMembershipInitializer
        {
            public SimpleMembershipInitializer()
            {
                Database.SetInitializer<UsersContext>(null);

                try
                {
                    using (var context = new UsersContext())
                    {
                        if (!context.Database.Exists())
                        {
                            // Create the SimpleMembership database without Entity Framework migration schema
                            ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
                        }
                    }

                    WebSecurity.InitializeDatabaseConnection("UsersDB", "UserProfile", "UserId", "UserName", autoCreateTables: true);

                    // add roles 
                    var roles = (SimpleRoleProvider)System.Web.Security.Roles.Provider;
                    if (!roles.RoleExists(Playgorund.Web.Util.Constants.RoleNames.Admin))
                    {
                        roles.CreateRole(Playgorund.Web.Util.Constants.RoleNames.Admin);
                    }
                    if (!roles.RoleExists(Playgorund.Web.Util.Constants.RoleNames.User))
                    {
                        roles.CreateRole(Playgorund.Web.Util.Constants.RoleNames.User);
                    }
                    if (!WebSecurity.UserExists(Playgorund.Web.Util.Constants.AdminUser.AdminUserName))
                    {
                        string user = WebSecurity.CreateUserAndAccount(Playgorund.Web.Util.Constants.AdminUser.AdminUserName, Playgorund.Web.Util.Constants.AdminUser.AdminPass);
                        roles.AddUsersToRoles(new string[] { Playgorund.Web.Util.Constants.AdminUser.AdminUserName }, new string[] { Playgorund.Web.Util.Constants.RoleNames.Admin });
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
                }
            }
        }
    }
}
