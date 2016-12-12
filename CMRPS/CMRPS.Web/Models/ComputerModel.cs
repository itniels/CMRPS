using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CMRPS.Web.Models
{
    [Table(name: "Computers")]
    public class ComputerModel
    {
        public int Id { get; set; }

        [Display(Name = "Friendly Name")]
        public string Name { get; set; }

        [Display(Name = "IPAddress")]
        public string IP { get; set; }

        [Display(Name = "MAC Address")]
        public string MAC { get; set; }

        [Display(Name = "Last Seen")]
        public DateTime LastSeen { get; set; }

        [Required]
        [Display(Name = "Computername")]
        public string Hostname { get; set; }

        public ComputerTypeModel Type { get; set; }

        public ColorModel Color { get; set; }

        public LocationModel Location { get; set; }

        [Display(Name = "Online:")]
        public bool IsOnline { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Purchase Date")]
        public DateTime PurchaseDate { get; set; }

        // Info (optional)
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Manufacturer")]
        public string Manufacturer { get; set; }

        [Display(Name = "Model")]
        public string Model { get; set; }

        [Display(Name = "CPU")]
        public string CPU { get; set; }

        [Display(Name = "CPU Cores")]
        public string CPUCores { get; set; }

        [Display(Name = "RAM")]
        public string RAM { get; set; }

        [Display(Name = "RAM Size")]
        public string RAMSize { get; set; }

        [Display(Name = "Disk(s)")]
        public string Disk { get; set; }

        [Display(Name = "Disk Size")]
        public string DiskSize { get; set; }

        [Display(Name = "Ethernet Cable")]
        public string EthernetCable { get; set; }

        [Display(Name = "Ethernet WIFI")]
        public string EthernetWifi { get; set; }

        [Display(Name = "Operating System")]
        public string OS { get; set; }

        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        public double Price { get; set; }
    }
}