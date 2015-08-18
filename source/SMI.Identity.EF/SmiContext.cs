using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.EF
{
    public class SmiContext : DbContext
    {
        public SmiContext() : base ("SmiContext") { }
        public DbSet<IdentityMembership> Memberships { get; set; }
        public DbSet<Identity> Identity { get; set; }
        public DbSet<Role> Roles { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Identity>().
          HasMany(c => c.Roles).
          WithMany(p => p.Identities).
          Map(
           m =>
           {
               m.MapLeftKey("Identity_IdentityID");
               m.MapRightKey("Role_RoleID");
               m.ToTable("IdentityRoles");
           });


            base.OnModelCreating(modelBuilder);
        }
    }

}
