using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thinktecture.AuthorizationServer.Models
{
    public class IdentityMembership
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid IdentiyMembershipID { get; set; }
        public Guid IdentityID { get; set; }
        public int MembershipID { get; set; }
        public bool CanAccessNeeds { get; set; }
        public bool CanAccessSentShares { get; set; }
        public bool CanAccessReceivedShares { get; set; }
        public bool CanAccessMembership { get; set; }
        public bool IsPrimaryMember { get; set; }
    }
}
