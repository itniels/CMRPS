using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Action = CMRPS.Web.Enums.Action;

namespace CMRPS.Web.Models
{
    [Table(name:"Schedules")]
    public class ScheduledModel
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Active")]
        public bool Active { get; set; }

        [Display(Name = "Action")]
        public Enums.ScheduledAction Action { get; set; }

        [Display(Name = "Select")]
        public Enums.ScheduledType Type { get; set; }

        public DateTime LastRun { get; set; }

        // By ID
        public string JsonComputerList { get; set; }

        // By Color
        public int ColorId { get; set; }

        // By Location
        public int LocationId { get; set; }

        // By Type
        public int TypeId { get; set; }

        // WeekDays
        public bool DayMonday { get; set; }
        public bool DayTuesday { get; set; }
        public bool DayWednsday { get; set; }
        public bool DayThursday { get; set; }
        public bool DayFriday { get; set; }
        public bool DaySaturday { get; set; }
        public bool DaySunday { get; set; }

        // Time
        public int Hour { get; set; }
        public int Minute { get; set; }
    }
}