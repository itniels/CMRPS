using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CMRPS.Web.Models
{
    public class LocationModel
    {
        public int Id { get; set; }

        [Display(Name = "Location Name")]
        public string Location { get; set; }

        public virtual List<ComputerModel> Computers { get; set; }
    }
}