using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;

namespace Tests.Data
{
    class TestSmiIdentityConfiguration : ISmiIdentity
    {
        public List<IdentityMembership> memberships {get; set;}

        public TestSmiIdentityConfiguration()
        {
            memberships = new List<IdentityMembership>();
        }
        public List<IdentityMembership> FindIdentityMemberships(string identityId)
        {
            return memberships;
        }
    }
}
