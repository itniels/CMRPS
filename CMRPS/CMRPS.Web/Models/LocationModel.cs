using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CMRPS.Web.Models
{
    [Table(name: "Locations")]
    public class LocationModel
    {
        public int Id { get; set; }

        [Display(Name = "Location Name")]
        public string Location { get; set; }

        public virtual List<ComputerModel> Computers { get; set; }

        public override string ToString()
        {
            return this.Location;
        }
    }
}