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
    }
}
