using System.Collections;
using System.Collections.Generic;

namespace Thinktecture.AuthorizationServer.Models
{
    public class ImpersonationIdentity
    {
        public string impersonationId { get; set; }
        public bool emailVerified { get; set; }
        public bool isLockedOut { get; set; }
        public string nickname { get; set; }
        public string emailAddress { get; set; }
        public IList<string> roles { get; set; }
    }
}