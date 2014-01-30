using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Playground.WebAPI.App_Start
{
    public class Startup
    {
    }
}


#region old implementation 

/*
 using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using WebApplication2.Providers;
using System.Data.Entity;

namespace WebApplication2
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(string connectionString) : base(connectionString) 
        {
            Database.SetInitializer(new MyDbContextInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUser>().HasKey<string>(l => l.Id);
            modelBuilder.Entity<IdentityUserLogin>().HasKey<string>(l => l.UserId);
            modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id);
            modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });
        }

        

        //public DbSet<IdentityUser> Users { get; set; }
        
        //public DbSet<IdentityUserClaim> Claims { get; set; }
        //public DbSet<IdentityUserLogin> Logins { get; set; }
        // public DbSet<IdentityUserRole> UserRoles { get; set; }
        public DbSet<IdentityRole> Roles { get; set; }
    }

    public class MyDbContextInitializer :
        // DropCreateDatabaseAlways<PlaygroundDbContext>
        //DropCreateDatabaseIfModelChanges<MyDbContext>
        CreateDatabaseIfNotExists<MyDbContext>
    {
        protected override void Seed(MyDbContext context)
        {
            base.Seed(context);

            context.Roles.Add(new IdentityRole()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Admin"
            });

            context.Roles.Add(new IdentityRole()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "User"
            });

            context.SaveChanges();
        }
    }

    public partial class Startup
    {
        static Startup()
        {
            PublicClientId = "self";

            UserManagerFactory = () => new UserManager<IdentityUser>(
                new UserStore<IdentityUser>(
                    new MyDbContext("TestUsersDB")));


            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId, UserManagerFactory),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                AllowInsecureHttp = true
            };
        }

        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static Func<UserManager<IdentityUser>> UserManagerFactory { get; set; }

        public static string PublicClientId { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // app.UseCookieAuthentication(new CookieAuthenticationOptions());
            
            // app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalBearer);
            
            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);
            

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //    consumerKey: "",
            //    consumerSecret: "");

            app.UseFacebookAuthentication(
                appId: "409949269107872",
                appSecret: "0c093299348ccd1208f834435327ec8d");

            //app.UseGoogleAuthentication();
        }
    }
}

*/

#endregion