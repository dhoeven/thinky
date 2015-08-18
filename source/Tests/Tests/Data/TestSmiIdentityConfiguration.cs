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
        public string ImpersonationId = null;

        public TestSmiIdentityConfiguration()
        {
            memberships = new List<IdentityMembership>();

        }
        public List<IdentityMembership> FindIdentityMemberships(string identityId)
        {
            return memberships;
        }

        public string GetImpersonationId(string identityId)
        {
            return ImpersonationId;
        }


        public ImpersonationIdentity GetImpersonationById(string identityId)
        {
            return new ImpersonationIdentity();
        }
    }
}
