using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMRPS.Web.Models
{
    public class ComputerModel
    {
        public int Id { get; set; }

        public ComputerTypeModel Type { get; set; }

        public string Name { get; set; }

        public string IP { get; set; }

        public string MAC { get; set; }

        public string Hostname { get; set; }

        public ColorModel Color { get; set; }

        public InfoModel info { get; set; }

        public LocationModel Location { get; set; }
    }
}