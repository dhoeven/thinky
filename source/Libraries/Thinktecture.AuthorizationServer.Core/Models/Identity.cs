using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thinktecture.AuthorizationServer.Models
{
    public class Identity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid   IdentityID      { get; set; }
        public Guid?  ImpersonationId { get; set; }
        public string EmailAddress    { get; set; }
        public string Nickname        { get; set; }
        public bool   IsApproved      { get; set; }
        public bool   IsLockedOut     { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
    }
}
