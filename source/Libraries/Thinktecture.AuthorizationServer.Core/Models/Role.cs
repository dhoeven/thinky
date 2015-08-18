using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thinktecture.AuthorizationServer.Models
{
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid RoleID { get; set; }
        [MaxLength(50)]
        public string RoleName { get; set; }

        public ICollection<Identity> Identities { get; set; }
    }
}
