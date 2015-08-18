using System;
using System.Collections.Generic;
using Thinktecture.AuthorizationServer.Models;

namespace Thinktecture.AuthorizationServer.Interfaces
{
    public interface ISmiIdentity
    {
        List<IdentityMembership> FindIdentityMemberships(string identityId);
        ImpersonationIdentity GetImpersonationById(string identityId);
    }
}
