using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Playground.Web.Providers
{
    public class UsersDbContext : IdentityDbContext
    {
        public UsersDbContext(string connectionString)
            : base(connectionString)
        {
            Database.SetInitializer(new UsersDBInitializer());
        }
    }

    public class UsersDBInitializer : CreateDatabaseIfNotExists<UsersDbContext>
    {
        protected override void Seed(UsersDbContext context)
        {
            base.Seed(context);
            
            context.Roles.Add(new IdentityRole("admin"));

            context.Roles.Add(new IdentityRole("user"));

            context.SaveChanges();
        }
    }
}