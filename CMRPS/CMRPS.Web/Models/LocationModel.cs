using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMRPS.Web.Models
{
    public class LocationModel
    {
        public int Id { get; set; }

        public string Location { get; set; }

        public virtual List<ComputerModel> Computers { get; set; }
    }
}