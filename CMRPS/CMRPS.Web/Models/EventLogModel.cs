using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMRPS.Web.Enums;
using Action = CMRPS.Web.Enums.Action;

namespace CMRPS.Web.Models
{
    public class EventLogModel
    {
        public int Id { get; set; }

        public ApplicationUser User { get; set; }

        public Action Action { get; set; }

        public ActionStatus Status { get; set; }

        public DateTime Timestamp { get; set; }

        public string Description { get; set; }
    }
}