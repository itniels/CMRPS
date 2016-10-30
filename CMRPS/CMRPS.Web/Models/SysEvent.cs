using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using CMRPS.Web.Enums;
using Action = CMRPS.Web.Enums.Action;

namespace CMRPS.Web.Models
{
    [Table(name: "Events")]
    public class SysEvent
    {
        public int ID { get; set; }
        public DateTime Timestamp { get; set; }

        public Action Action { get; set; }
        public ActionStatus ActionStatus { get; set; }
        public ApplicationUser User { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Description { get; set; }
        public string Exception { get; set; }
    }
}