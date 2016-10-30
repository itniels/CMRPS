using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CMRPS.Web.Models
{
    [Table(name: "ComputerTypes")]
    public class ComputerTypeModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Image")]
        public string ImagePath { get; set; }

        [Display(Name = "Filename")]
        public string Filename { get; set; }

        public virtual List<ComputerModel> Computers { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}