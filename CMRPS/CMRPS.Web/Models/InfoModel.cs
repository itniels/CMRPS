using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMRPS.Web.Models
{
    public class InfoModel
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public DateTime PurchaseDate { get; set; }

        public string Manufacturer { get; set; }

        public string Model { get; set; }

        public string CPU { get; set; }

        public int CPUCores { get; set; }

        public string RAM { get; set; }

        public int RAMSize { get; set; }

        public string Disk { get; set; }

        public int DiskSize { get; set; }

        public string EthernetCable { get; set; }

        public string EthernetWifi { get; set; }

        public string OS { get; set; }

    }
}