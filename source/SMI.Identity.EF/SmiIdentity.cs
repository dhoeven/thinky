using System;
using System.Collections.Generic;
using System.Linq;
using Thinktecture.AuthorizationServer.EF;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;

namespace SMI.Identity.EF
{
    public class SmiIdentity : ISmiIdentity
    {
        private SmiContext _db;

        public SmiIdentity(SmiContext context)
        {
            _db = context;
        }

        public List<IdentityMembership> FindIdentityMemberships(string identityId)
        {
            return _db.Memberships.Where(
                m => identityId != null && m.IdentityID == new Guid(identityId)).ToList();
        }
    }
}
