using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CMRPS.Web.Models
{
    [Table(name:"WorkerQueue")]
    public class WorkerQueue
    {
        // Named queue.
        public string Name { get; set; }

        public bool isEnqueued { get; set; }

        public List<int> Computers { get; set; }
    }
}