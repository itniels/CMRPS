using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CMRPS.Web.Models
{
    [Table(name:"Logins")]
    public class SysLogin
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }

        public ApplicationUser User { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public bool Success { get; set; }
    }
}