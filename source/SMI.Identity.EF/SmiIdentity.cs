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


        public ImpersonationIdentity GetImpersonationById(string identityId)
        {
            var identity = _db.Identity.FirstOrDefault(i => i.IdentityID == new Guid(identityId));
            if (identity == null || identity.ImpersonationId == null)
                return null;

            var impersonatedIdentity = _db.Identity.FirstOrDefault(i => i.IdentityID == identity.ImpersonationId);
            if (impersonatedIdentity == null) return null;

            return new ImpersonationIdentity
            {
                impersonationId = identity.ImpersonationId.ToString(),
                emailAddress = impersonatedIdentity.EmailAddress,
                emailVerified = impersonatedIdentity.IsApproved,
                isLockedOut = impersonatedIdentity.IsLockedOut,
                nickname = impersonatedIdentity.Nickname,
                roles = impersonatedIdentity.Roles == null ? new List<string>() : impersonatedIdentity.Roles.Select(x => x.RoleName).ToList()
            };
        }
    }
}
