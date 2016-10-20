using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CMRPS.Web.Models
{
    public class ComputerModel
    {
        public int Id { get; set; }

        public ComputerTypeModel Type { get; set; }

        [Display(Name = "Friendly Name")]
        public string Name { get; set; }

        [Display(Name = "IPAddress")]
        public string IP { get; set; }

        [Display(Name = "MAC Address")]
        public string MAC { get; set; }

        [Display(Name = "Computername")]
        public string Hostname { get; set; }

        public ColorModel Color { get; set; }

        public InfoModel info { get; set; }

        public LocationModel Location { get; set; }

        public bool Status { get; set; }
    }
}